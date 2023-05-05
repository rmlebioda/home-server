using Api.Startup.Extensions;
using Api.Startup.Models.Upscaling;
using BrunoZell.ModelBinding;
using Cli.RealEsrgan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Startup.Controllers;

[ApiController]
[Route("upscaling")]
public class UpscalingController : ControllerBase
{
    #if DEBUG
    private static readonly TimeSpan FileMaxCacheLifeSpan = new(0, 0, 5, 0);
    #else
    private static readonly TimeSpan FileMaxCacheLifeSpan = new(1, 12, 0, 0);
    #endif
    private readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly RealEsrganService _realEsrganService;

    public UpscalingController(ILogger<UpscalingController> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _realEsrganService = new RealEsrganService(logger);
    }

    [HttpGet("real-esrgan/help", Name = "Real-ESRGAN help")]
    public async Task<ActionResult<string>> RealEsrganHelp()
    {
        return Ok(await _realEsrganService.GetHelp());
    }

    [HttpPost("real-esrgan/image", Name = "Real-ESRGAN upscaling")]
    public async Task<ActionResult<RealEsrganResponse>> RealEsrgan(
        [ModelBinder(BinderType = typeof(JsonModelBinder))]
        RealEsrganSettings settings,
        IFormFile image)
    {
        var fileExtension = Path.GetExtension(image.FileName);
        var uploadedFilePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}{fileExtension}");
        await System.IO.File.WriteAllBytesAsync(uploadedFilePath, await image.GetBytes());

        var taskGuid = Guid.NewGuid().ToString();
        var upscaleTask =
            _realEsrganService.UpscaleImage(uploadedFilePath, image.FileName, settings.ToRealEsrganUpscaleSettings());

        _ = _memoryCache.Set(taskGuid, upscaleTask, CreateCacheOptions());
        _logger.LogDebug("Registered new task {TaskId} and set cache to expire in {ExpiryTimeSpan}", taskGuid,
            FileMaxCacheLifeSpan.ToString());

        return Accepted(new RealEsrganResponse
        {
            Status = RealEsrganStatus.Started,
            ErrorMessage = null,
            ImageIdentifier = taskGuid
        });
    }

    private MemoryCacheEntryOptions CreateCacheOptions()
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = FileMaxCacheLifeSpan
        };
        options.RegisterPostEvictionCallback(OnPostEvictionCallback);
        return options;
    }

    private void OnPostEvictionCallback(object key, object? value, EvictionReason reason, object? state)
    {
        if (value is Task<RealEsrganUpscaleResult> task && task.IsCompleted)
        {
            var result = task.Result;
            _logger.LogDebug(
                "Clearing cache, deleting source image {SourceImagePath} and output image {OutputImagePath}",
                result.SourceFilePath, result.OutputFilePath);
            System.IO.File.Delete(result.SourceFilePath);
            System.IO.File.Delete(result.OutputFilePath);
        }
    }

    [HttpGet("real-esrgan/image/{taskId}", Name = "Real-ESRGAN upscaled image")]
    public ActionResult RealEsrganHelp(string taskId)
    {
        if (_memoryCache.TryGetValue(taskId, out Task<RealEsrganUpscaleResult>? task) && task is not null)
        {
            if (!task.IsCompleted)
            {
                HttpContext.Response.Headers.Add("Retry-After", "10");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Image not ready yet");
            }

            var result = task.Result;
            var newSuggestedFileName = Path.GetFileNameWithoutExtension(result.OriginalFileName) + "_Upscaled" +
                                       Path.GetExtension(result.OriginalFileName);
            return PhysicalFile(result.OutputFilePath, "application/json", newSuggestedFileName);
        }

        return NotFound();
    }
}