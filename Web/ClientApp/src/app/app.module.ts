import { NgModule, ErrorHandler } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { OAuthModule, OAuthStorage } from 'angular-oauth2-oidc';
import { ToastaModule } from 'ngx-toasta';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { ChartsModule } from 'ng2-charts';

import { AppRoutingModule } from './app-routing.module';
import { AppErrorHandler } from './app-error.handler';
import { AppTitleService } from './services/app-title.service';
import { AppTranslationService, TranslateLanguageLoader } from './services/app-translation.service';
import { ConfigurationService } from './services/configuration.service';
import { AlertService } from './services/alert.service';
import { AppThemeService } from './services/app-theme.service';
import { LocalStorageService } from './services/local-storage.service';
import { AuthStorageService } from './services/auth-storage.service';
import { NotificationService } from './services/notification.service';
import { NotificationMockService } from './services/notification-mock.service';
import { AccountService } from "./services/account.service";
import { EventService } from "./services/event.service";
import { EventConditionService } from "./services/event-condition.service";
import { ActionDeviceService } from "./services/action-device.service";
import { DataPointService } from "./services/data-point.service";
import { ExtendedLogService } from "./services/extended-log.service";
import * as generated from './services/endpoint.services';

import { EqualValidatorDirective } from './directives/equal-validator.directive';
import { LastElementDirective } from './directives/last-element.directive';
import { AutofocusDirective } from './directives/autofocus.directive';
import { BootstrapTabDirective } from './directives/bootstrap-tab.directive';
import { BootstrapToggleDirective } from './directives/bootstrap-toggle.directive';
import { BootstrapSelectDirective } from './directives/bootstrap-select.directive';
import { BootstrapDatepickerDirective } from './directives/bootstrap-datepicker.directive';
import { GroupByPipe } from './pipes/group-by.pipe';
import { EnumToArrayPipe } from './pipes/enum-to-array.pipe';

import { AppComponent } from './components/app.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { AboutComponent } from './components/about/about.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { NotificationsManagementComponent } from './components/controls/notifications-management.component';
import { SearchBoxComponent } from './components/controls/search-box.component';
 
import { SettingsComponent } from './components/settings/settings.component';
import { UserEditorComponent } from './components/settings/user-editor.component';
import { UserPreferencesComponent } from './components/settings/user-preferences.component';
import { UsersManagementComponent } from './components/settings/users-management.component';
import { RolesManagementComponent } from './components/settings/roles-management.component';
import { RoleEditorComponent } from './components/settings/role-editor.component';

import { ExtendedLogsComponent } from './components/extended-logs/extended-logs.component';
import { ExtendedLogsManagementComponent } from './components/extended-logs/extended-logs-management.component';
import { ExtendedLogEditorComponent } from './components/extended-logs/extended-log-editor.component';

import { EventsComponent } from './components/events/events.component';
import { EventsManagementComponent } from './components/events/events-management.component';
import { EventEditorComponent } from './components/events/event-editor.component';
import { EventConditionsManagementComponent } from './components/events/event-conditions-management.component';

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        FormsModule,
        AppRoutingModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useClass: TranslateLanguageLoader
            }
        }),
    NgxDatatableModule,
        OAuthModule.forRoot(),
        ToastaModule.forRoot(),
        TooltipModule.forRoot(),
        PopoverModule.forRoot(),
        BsDropdownModule.forRoot(),
        CarouselModule.forRoot(),
        ModalModule.forRoot(),
        ChartsModule
    ],
    declarations: [
        AppComponent,
        LoginComponent,
        HomeComponent,
        EventsComponent, EventsManagementComponent, EventEditorComponent, EventConditionsManagementComponent,
        ExtendedLogsComponent, ExtendedLogsManagementComponent, ExtendedLogEditorComponent,
        SettingsComponent,
        UsersManagementComponent, UserEditorComponent, UserPreferencesComponent,
        RolesManagementComponent, RoleEditorComponent,
        AboutComponent,
        NotFoundComponent,
        NotificationsManagementComponent,
        SearchBoxComponent,
        EqualValidatorDirective,
        LastElementDirective,
        AutofocusDirective,
        BootstrapTabDirective,
        BootstrapToggleDirective,
        BootstrapSelectDirective,
        BootstrapDatepickerDirective,
        GroupByPipe, EnumToArrayPipe
    ],
    providers: [
        { provide: ErrorHandler, useClass: AppErrorHandler },
        { provide: OAuthStorage, useClass: AuthStorageService },
        AlertService,
        AppThemeService,
        ConfigurationService,
        AppTitleService,
        AppTranslationService,
        NotificationService,
        AccountService,
        ExtendedLogService,
        EventService,
        EventConditionService,
        ActionDeviceService,
        DataPointService,
        NotificationMockService,
        generated.AccountEndpointService,
        generated.ExtendedLogEndpointService,
        generated.EventEndpointService,
        generated.EventConditionEndpointService,
        generated.ActionDeviceEndpointService,
        generated.DataPointEndpointService,
        LocalStorageService
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
