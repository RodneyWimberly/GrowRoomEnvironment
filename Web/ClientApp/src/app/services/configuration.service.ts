import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { AppTranslationService } from './app-translation.service';
import { AppThemeService } from './app-theme.service';
import { LocalStorageService } from './local-storage.service';
import { DbKeys } from '../helpers/db-keys';
import { Utilities } from '../helpers/utilities';
import { environment } from '../../environments/environment';
import { UserConfigurationModel } from '../models/user-configuration.model';

@Injectable()
export class ConfigurationService {

    constructor(
        private localStorageService: LocalStorageService,
        private appTranslationService: AppTranslationService,
        private appThemeService: AppThemeService) {

        this.loadLocalChanges();
    }

    set language(value: string) {
        this._language = value;
        this.saveToLocalStore(value, DbKeys.LANGUAGE);
        this.appTranslationService.changeLanguage(value);
    }
    get language() {
        return this._language || ConfigurationService.defaultLanguage;
    }

    set themeId(value: number) {
        value = +value;
        this._themeId = value;
        this.saveToLocalStore(value, DbKeys.THEME_ID);
        this.appThemeService.installTheme(this.appThemeService.getThemeByID(value));
    }
    get themeId() {
        return this._themeId || ConfigurationService.defaultThemeId;
    }

    set homeUrl(value: string) {
        this._homeUrl = value;
        this.saveToLocalStore(value, DbKeys.HOME_URL);
    }
    get homeUrl() {
        return this._homeUrl || ConfigurationService.defaultHomeUrl;
    }

    set showDashboardStatistics(value: boolean) {
        this._showDashboardStatistics = value;
        this.saveToLocalStore(value, DbKeys.SHOW_DASHBOARD_STATISTICS);
    }
    get showDashboardStatistics() {
        return this._showDashboardStatistics != null ? this._showDashboardStatistics : ConfigurationService.defaultShowDashboardStatistics;
    }

    set showDashboardNotifications(value: boolean) {
        this._showDashboardNotifications = value;
        this.saveToLocalStore(value, DbKeys.SHOW_DASHBOARD_NOTIFICATIONS);
    }
    get showDashboardNotifications() {
        return this._showDashboardNotifications != null ? this._showDashboardNotifications : ConfigurationService.defaultShowDashboardNotifications;
    }

    set showDashboardTodo(value: boolean) {
        this._showDashboardTodo = value;
        this.saveToLocalStore(value, DbKeys.SHOW_DASHBOARD_TODO);
    }
    get showDashboardTodo() {
        return this._showDashboardTodo != null ? this._showDashboardTodo : ConfigurationService.defaultShowDashboardTodo;
    }

    set showDashboardBanner(value: boolean) {
        this._showDashboardBanner = value;
        this.saveToLocalStore(value, DbKeys.SHOW_DASHBOARD_BANNER);
    }
    get showDashboardBanner() {
        return this._showDashboardBanner != null ? this._showDashboardBanner : ConfigurationService.defaultShowDashboardBanner;
    }

    public static readonly appVersion: string = '3.0.0';

    // ***Specify default configurations here***
    public static readonly defaultLanguage: string = 'en';
    public static readonly defaultHomeUrl: string = '/';
    public static readonly defaultThemeId: number = 1;
    public static readonly defaultShowDashboardStatistics: boolean = true;
    public static readonly defaultShowDashboardNotifications: boolean = true;
    public static readonly defaultShowDashboardTodo: boolean = false;
    public static readonly defaultShowDashboardBanner: boolean = true;

    public baseUrl = environment.baseUrl || Utilities.baseUrl();
    public tokenUrl = environment.tokenUrl || environment.baseUrl || Utilities.baseUrl();
    public loginUrl = environment.loginUrl;
    public fallbackBaseUrl = 'http://www.wimberlytech.com';
    // ***End of defaults***

    private _language: string = null;
    private _homeUrl: string = null;
    private _themeId: number = null;
    private _showDashboardStatistics: boolean = null;
    private _showDashboardNotifications: boolean = null;
    private _showDashboardTodo: boolean = null;
    private _showDashboardBanner: boolean = null;

    private onConfigurationImported: Subject<boolean> = new Subject<boolean>();
    configurationImported$ = this.onConfigurationImported.asObservable();

    private loadLocalChanges() {

        if (this.localStorageService.exists(DbKeys.LANGUAGE)) {
            this._language = this.localStorageService.getDataObject<string>(DbKeys.LANGUAGE);
            this.appTranslationService.changeLanguage(this._language);
        } else {
            this.resetLanguage();
        }

        if (this.localStorageService.exists(DbKeys.THEME_ID)) {
            this._themeId = this.localStorageService.getDataObject<number>(DbKeys.THEME_ID);
            this.appThemeService.installTheme(this.appThemeService.getThemeByID(this._themeId));
        } else {
            this.resetTheme();
        }

        if (this.localStorageService.exists(DbKeys.HOME_URL)) {
            this._homeUrl = this.localStorageService.getDataObject<string>(DbKeys.HOME_URL);
        }

        if (this.localStorageService.exists(DbKeys.SHOW_DASHBOARD_STATISTICS)) {
            this._showDashboardStatistics = this.localStorageService.getDataObject<boolean>(DbKeys.SHOW_DASHBOARD_STATISTICS);
        }

        if (this.localStorageService.exists(DbKeys.SHOW_DASHBOARD_NOTIFICATIONS)) {
            this._showDashboardNotifications = this.localStorageService.getDataObject<boolean>(DbKeys.SHOW_DASHBOARD_NOTIFICATIONS);
        }

        if (this.localStorageService.exists(DbKeys.SHOW_DASHBOARD_TODO)) {
            this._showDashboardTodo = this.localStorageService.getDataObject<boolean>(DbKeys.SHOW_DASHBOARD_TODO);
        }

        if (this.localStorageService.exists(DbKeys.SHOW_DASHBOARD_BANNER)) {
            this._showDashboardBanner = this.localStorageService.getDataObject<boolean>(DbKeys.SHOW_DASHBOARD_BANNER);
        }
    }

    private saveToLocalStore(data: any, key: string) {
        setTimeout(() => this.localStorageService.savePermanentData(data, key));
    }

    public import(jsonValue: string) {

        this.clearLocalChanges();

        if (jsonValue) {
            const importValue: UserConfigurationModel = Utilities.JsonTryParse(jsonValue);

            if (importValue.language != null) {
                this.language = importValue.language;
            }

            if (importValue.themeId != null) {
                this.themeId = +importValue.themeId;
            }

            if (importValue.homeUrl != null) {
                this.homeUrl = importValue.homeUrl;
            }

            if (importValue.showDashboardStatistics != null) {
                this.showDashboardStatistics = importValue.showDashboardStatistics;
            }

            if (importValue.showDashboardNotifications != null) {
                this.showDashboardNotifications = importValue.showDashboardNotifications;
            }

            if (importValue.showDashboardTodo != null) {
                this.showDashboardTodo = importValue.showDashboardTodo;
            }

            if (importValue.showDashboardBanner != null) {
                this.showDashboardBanner = importValue.showDashboardBanner;
            }
        }

        this.onConfigurationImported.next();
    }

    public export(changesOnly = true): string {

        const exportValue: UserConfigurationModel = {
            language: changesOnly ? this._language : this.language,
            themeId: changesOnly ? this._themeId : this.themeId,
            homeUrl: changesOnly ? this._homeUrl : this.homeUrl,
            showDashboardStatistics: changesOnly ? this._showDashboardStatistics : this.showDashboardStatistics,
            showDashboardNotifications: changesOnly ? this._showDashboardNotifications : this.showDashboardNotifications,
            showDashboardTodo: changesOnly ? this._showDashboardTodo : this.showDashboardTodo,
            showDashboardBanner: changesOnly ? this._showDashboardBanner : this.showDashboardBanner
        };

        return JSON.stringify(exportValue);
    }

    public clearLocalChanges() {
        this._language = null;
        this._themeId = null;
        this._homeUrl = null;
        this._showDashboardStatistics = null;
        this._showDashboardNotifications = null;
        this._showDashboardTodo = null;
        this._showDashboardBanner = null;

        this.localStorageService.deleteData(DbKeys.LANGUAGE);
        this.localStorageService.deleteData(DbKeys.THEME_ID);
        this.localStorageService.deleteData(DbKeys.HOME_URL);
        this.localStorageService.deleteData(DbKeys.SHOW_DASHBOARD_STATISTICS);
        this.localStorageService.deleteData(DbKeys.SHOW_DASHBOARD_NOTIFICATIONS);
        this.localStorageService.deleteData(DbKeys.SHOW_DASHBOARD_TODO);
        this.localStorageService.deleteData(DbKeys.SHOW_DASHBOARD_BANNER);

        this.resetLanguage();
        this.resetTheme();
    }

    private resetLanguage() {
        const language = this.appTranslationService.useBrowserLanguage();

        if (language) {
            this._language = language;
        } else {
            this._language = this.appTranslationService.useDefaultLangage();
        }
    }

    private resetTheme() {
        this.appThemeService.installTheme();
        this._themeId = null;
    }
}
