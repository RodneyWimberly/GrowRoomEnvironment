import { Injectable, Injector, ErrorHandler } from '@angular/core';
import { AlertService, MessageSeverity } from './services/alert.service';
import * as generated from './services/endpoint.services';
import { ExtendedLogService } from './services/extended-log.service';

@Injectable()
export class AppErrorHandler extends ErrorHandler {

    // private alertService: AlertService;
    private extendedLogService: ExtendedLogService
    constructor(private injector: Injector) {
        super();
    }


    handleError(error: Error) {
        if (this.extendedLogService == null)
            this.extendedLogService = this.injector.get(ExtendedLogService);
        // if (this.alertService == null) {
        //    this.alertService = this.injector.get(AlertService);
        // }

        // this.alertService.showStickyMessage("Fatal Error!", "An unresolved error has occured. Please reload the page to correct this error", MessageSeverity.warn);
        // this.alertService.showStickyMessage("Unhandled Error", error.message || error, MessageSeverity.error, error);
        
        var extendedLog = new generated.ExtendedLogViewModel();
        extendedLog.eventId = 0;
        extendedLog.message = error.message + '\r\n\r\nStack:\r\n' + error.stack;
        extendedLog.level = 4;
        extendedLog.name = error.name;
        extendedLog.timeStamp = new Date(Date.now());
        this.extendedLogService.addExtendedLog(extendedLog);

        if (confirm('Fatal Error!\nAn unresolved error has occured. Do you want to reload the page to correct this?\n\nError: ' + error.message)) {
            window.location.reload(true);
        }
        
        super.handleError(error);
    }
}
