import { Component, ViewChild } from '@angular/core';

import * as generated from '../../services/endpoint.services';

@Component({
    selector: 'extended-log-editor',
    templateUrl: './extended-log-editor.component.html',
    styleUrls: ['./extended-log-editor.component.scss']
})
export class ExtendedLogEditorComponent {
    private editingLogId: number;
    public logEdit: generated.ExtendedLogViewModel = new generated.ExtendedLogViewModel();
    public selectedValues: { [key: string]: boolean; } = {};
    public formResetToggle = true;
    public changesCancelledCallback: () => void;

    @ViewChild('f', { static: false })
    private form;

    constructor() {
    }

    cancel() {
        this.logEdit = new generated.ExtendedLogViewModel();
        this.resetForm();
        if (this.changesCancelledCallback) {
            this.changesCancelledCallback();
        }
    }

    resetForm(replace = false) {

        if (!replace) {
            this.form.reset();
        } else {
            this.formResetToggle = false;

            setTimeout(() => {
                this.formResetToggle = true;
            });
        }
    }

    editLog(log: generated.ExtendedLogViewModel) {
        if (log) {
            this.editingLogId = log.id;
            this.selectedValues = {};
            this.logEdit = new generated.ExtendedLogViewModel();
            Object.assign(this.logEdit, log);

            return this.logEdit;
        }
    }

    get errorLevel(): string {
      return this.logEdit.level + " - " + this.logEdit.levelDescription;
    }

    set errorLevel(value: string) {

    }
}
