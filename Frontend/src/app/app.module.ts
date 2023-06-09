import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {ImageUpscalingComponent} from './modules/components/image-upscaling/image-upscaling.component';
import {MatSidenavModule} from '@angular/material/sidenav';
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {MatButtonModule} from "@angular/material/button";
import {MatIconModule} from "@angular/material/icon";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatListModule} from "@angular/material/list";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatExpansionModule} from "@angular/material/expansion";
import {MatSlideToggleModule} from "@angular/material/slide-toggle";
import {MatCardModule} from "@angular/material/card";
import {ColorSchemeService} from "./modules/services/color-scheme.service";
import {HttpClientModule} from "@angular/common/http";
import {MatBadgeModule} from "@angular/material/badge";
import {InboxService} from "./modules/services/inbox.service";
import {MatMenuModule} from "@angular/material/menu";
import {InboxComponent} from './modules/components/inbox/inbox.component';
import {ReactiveFormsModule} from "@angular/forms";
import {MatCheckboxModule} from "@angular/material/checkbox";
import {MatInputModule} from "@angular/material/input";
import {MatGridListModule} from "@angular/material/grid-list";
import {MatTooltipModule} from "@angular/material/tooltip";
import {MatTableModule} from "@angular/material/table";
import {TimeagoModule} from "ngx-timeago";
import {TorrentComponent} from './modules/components/torrent/torrent.component';
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {MatSnackBarModule} from "@angular/material/snack-bar";
import { TorrentAddDialogComponent } from './modules/components/torrent-add-dialog/torrent-add-dialog.component';
import {MatDialogModule} from "@angular/material/dialog";
import {NgxFilesizeModule} from "ngx-filesize";
import { TorrentDeleteDialogComponent } from './modules/components/torrent-delete-dialog/torrent-delete-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    ImageUpscalingComponent,
    InboxComponent,
    TorrentComponent,
    TorrentAddDialogComponent,
    TorrentDeleteDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatSidenavModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatListModule,
    MatFormFieldModule,
    MatExpansionModule,
    MatSlideToggleModule,
    MatCardModule,
    HttpClientModule,
    MatBadgeModule,
    MatMenuModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatInputModule,
    MatGridListModule,
    MatTooltipModule,
    MatTableModule,
    TimeagoModule.forRoot(),
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    NgxFilesizeModule
  ],
  providers: [
    ColorSchemeService,
    InboxService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
