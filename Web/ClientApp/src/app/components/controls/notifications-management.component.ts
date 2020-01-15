import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input } from '@angular/core';
import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { NotificationService } from '../../services/notification.service';
import { AccountService } from "../../services/account.service";
import * as generated from '../../services/endpoint.services';
import { Utilities } from '../../helpers/utilities';


@Component({
    selector: 'notifications-management',
    templateUrl: './notifications-management.component.html',
    styleUrls: ['./notifications-management.component.scss']
})
export class NotificationsManagementComponent implements OnInit, OnDestroy {
    columns: any[] = [];
    rows: generated.NotificationViewModel[] = [];
    loadingIndicator: boolean;

    dataLoadingConsecutiveFailurs = 0;
    dataLoadingSubscription: any;


    @Input()
    isViewOnly: boolean;

    @Input()
    verticalScrollbar = false;


    @ViewChild('statusHeaderTemplate', { static: true })
    statusHeaderTemplate: TemplateRef<any>;

    @ViewChild('statusTemplate', { static: true })
    statusTemplate: TemplateRef<any>;

    @ViewChild('dateTemplate', { static: true })
    dateTemplate: TemplateRef<any>;

    @ViewChild('contentHeaderTemplate', { static: true })
    contentHeaderTemplate: TemplateRef<any>;

    @ViewChild('contenBodytTemplate', { static: true })
    contenBodytTemplate: TemplateRef<any>;

    @ViewChild('actionsTemplate', { static: true })
    actionsTemplate: TemplateRef<any>;

    constructor(private alertService: AlertService, private translationService: AppTranslationService, private accountClient: AccountService, private notificationService: NotificationService) {
    }


    ngOnInit() {

        if (this.isViewOnly) {
            this.columns = [
                { prop: 'date', cellTemplate: this.dateTemplate, width: 100, resizeable: false, canAutoResize: false, sortable: false, draggable: false },
                { prop: 'header', cellTemplate: this.contentHeaderTemplate, width: 200, resizeable: false, sortable: false, draggable: false },
            ];
        } else {
            const gT = (key: string) => this.translationService.getTranslation(key);

            this.columns = [
                { prop: '', name: '', width: 10, headerTemplate: this.statusHeaderTemplate, cellTemplate: this.statusTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false },
                { prop: 'date', name: gT('notifications.Date'), cellTemplate: this.dateTemplate, width: 30 },
                { prop: 'body', name: gT('notifications.Notification'), cellTemplate: this.contenBodytTemplate, width: 500 },
                { name: '', width: 80, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
            ];
        }


        this.initDataLoading();
    }


    ngOnDestroy() {
        if (this.dataLoadingSubscription) {
            this.dataLoadingSubscription.unsubscribe();
        }
    }



    initDataLoading() {

        if (this.isViewOnly && this.notificationService.recentNotifications) {
            this.rows = this.processResults(this.notificationService.recentNotifications);
            return;
        }

        this.loadingIndicator = true;

        const dataLoadTask = this.isViewOnly ? this.notificationService.getNewNotifications() : this.notificationService.getNewNotificationsPeriodically();

        this.dataLoadingSubscription = dataLoadTask
            .subscribe(notifications => {
                this.loadingIndicator = false;
                this.dataLoadingConsecutiveFailurs = 0;

                this.rows = this.processResults(notifications);
            },
            error => {
                this.loadingIndicator = false;

                this.alertService.showMessage('Load Error', 'Loading new notifications from the server failed!', MessageSeverity.warn);
                this.alertService.logError(error);

                if (this.dataLoadingConsecutiveFailurs++ < 5) {
                    setTimeout(() => this.initDataLoading(), 5000);
                } else {
                    this.alertService.showStickyMessage('Load Error', 'Loading new notifications from the server failed!', MessageSeverity.error);
                }

            });


        if (this.isViewOnly) {
            this.dataLoadingSubscription = null;
        }
    }


    private processResults(notifications: generated.NotificationViewModel[]) {

        if (this.isViewOnly) {
            notifications.sort((a, b) => {
                return b.date.valueOf() - a.date.valueOf();
            });
        }

        return notifications;
    }



    getPrintedDate(value: Date) {
        if (value) {
            return Utilities.printTimeOnly(value) + ' on ' + Utilities.printDateOnly(value);
        }
    }


    deleteNotification(row: generated.NotificationViewModel) {
        this.alertService.showDialog('Are you sure you want to delete the notification \"' + row.header + '\"?', DialogType.confirm, () => this.deleteNotificationHelper(row));
    }


    deleteNotificationHelper(row: generated.NotificationViewModel) {

        this.alertService.startLoadingMessage('Deleting...');
        this.loadingIndicator = true;

        this.notificationService.deleteNotification(row)
            .subscribe(results => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.rows = this.rows.filter(item => item.notificationId != row.notificationId);
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage('Delete Error', `An error occured whilst deleting the notification.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }


    togglePin(row: generated.NotificationViewModel) {

        const pin = !row.isPinned;
        const opText = pin ? 'Pinning' : 'Unpinning';

        this.alertService.startLoadingMessage(opText + '...');
        this.loadingIndicator = true;

        this.notificationService.pinUnpinNotification(row, pin)
            .subscribe(results => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                row.isPinned = pin;
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage(opText + ' Error', `An error occured whilst ${opText} the notification.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }


    get canManageNotifications() {
        return this.accountClient.userHasPermission(generated.PermissionValues.ManageRoles); // Todo: Consider creating separate permission for notifications
    }

}
