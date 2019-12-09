import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { ExtendedLogService } from "../../services/extended-log.service";
import { Utilities } from '../../helpers/utilities';
import { ExtendedLogEditorComponent } from './extended-log-editor.component';
import * as generated from '../../services/endpoint.services';

@Component({
    selector: 'extended-log-management',
    templateUrl: './extended-log-management.component.html',
    styleUrls: ['./extended-log-management.component.scss']
})

export class ExtendedLogManagementComponent implements OnInit, AfterViewInit {
    columns: any[] = [];
    rows: generated.ExtendedLogViewModel[] = [];
    rowsCache: generated.ExtendedLogViewModel[] = [];
    editedLog: generated.ExtendedLogViewModel;
    sourceLog: generated.ExtendedLogViewModel;
    editingLogId: { id: number };
    loadingIndicator: boolean;
       
    @ViewChild('indexTemplate', { static: true })
    indexTemplate: TemplateRef<any>;

    @ViewChild('actionsTemplate', { static: true })
    actionsTemplate: TemplateRef<any>;

    @ViewChild('editorModal', { static: true })
    editorModal: ModalDirective;

    @ViewChild('logEditor', { static: true })
    logEditor: ExtendedLogEditorComponent;

    constructor(private alertService: AlertService, private translationService: AppTranslationService, private extendedLogService: ExtendedLogService) {
    }

    ngOnInit() {

        const gT = (key: string) => this.translationService.getTranslation(key);

        this.columns = [
            { prop: 'id', name: gT('logs.management.Id'), width: 60, cellTemplate: this.indexTemplate, canAutoResize: false },
            { prop: 'eventId', name: gT('logs.management.EventId'), width: 70 },
            { prop: 'level', name: gT('logs.management.Level'), width: 60 },
            { prop: 'name', name: gT('logs.management.Name'), width: 150 },
            { prop: 'message', name: gT('logs.management.Message'), width: 300 },
            { prop: 'browser', name: gT('logs.management.Browser'), width: 100 },
            { prop: 'host', name: gT('logs.management.Host'), width: 100 },
            { prop: 'path', name: gT('logs.management.Path'), width: 100 },
            { prop: 'user', name: gT('logs.management.User'), width: 100 },
            { prop: 'timeStamp', name: gT('logs.management.TimeStamp'), width: 180 }
        ];

        this.loadData();
    }

    ngAfterViewInit() {
       this.logEditor.changesCancelledCallback = () => {
            this.editorModal.hide();
        };
    }

    onActivate(event) {
        if (event.type == 'click') {
            this.editLog(event.row);
        }
    }

    loadData() {
        this.alertService.startLoadingMessage();
        this.loadingIndicator = true;

        this.extendedLogService.getExtendedLogs()
            .subscribe(logs => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.rowsCache = [...logs];
                this.rows = logs;
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage('Load Error', `Unable to retrieve logs from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }

    onSearchChanged(value: string) {
        this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.message, r.browser, r.host, r.path, r.user));
    }

    onEditorModalHidden() {
        this.editingLogId = null;
        this.logEditor.resetForm(true);
    }

    editLog(row: generated.ExtendedLogViewModel) {
        this.editingLogId = { id: row.id };
        this.sourceLog = row;
        this.editedLog = this.logEditor.editLog(row);
        this.editorModal.show();
    }

    clearLog() {
        this.alertService.showDialog('Are you sure you want to clear the log?', DialogType.confirm, () => this.clearLogHelper());
    }

    clearLogHelper() {
        this.alertService.startLoadingMessage('Deleting...');
        this.loadingIndicator = true;

        this.extendedLogService.clearExtendedLogs()
            .subscribe(results => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.rowsCache = [];
                this.rows = [];
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage('Delete Error', `An error occurred while clearing the log.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }
}
