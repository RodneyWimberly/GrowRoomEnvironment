import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { EventService } from "../../services/event.service";
import { Utilities } from '../../helpers/utilities';
import { EventEditorComponent } from './event-editor.component';
import * as generated from '../../services/endpoint.services';
import { DatatableComponent } from '@swimlane/ngx-datatable';

@Component({
  selector: 'events-management',
  templateUrl: './events-management.component.html',
  styleUrls: ['./events-management.component.scss']
})

export class EventsManagementComponent implements OnInit, AfterViewInit {
  protected editingEventName: { name: string };
  protected loadingIndicator: boolean;
  protected columns: any[] = [];
  protected rows: generated.EventViewModel[] = [];
  protected editedEvent: generated.EventViewModel;
  private cachedEvent: generated.EventViewModel;
  private cachedRows: generated.EventViewModel[] = [];


  @ViewChild('dataTable', { static: true })
  private ngxDatatable: DatatableComponent;
  
  @ViewChild('isEnabledTemplate', { static: true })
  private isEnabled: TemplateRef<any>;

  @ViewChild('eventConditionsTemplate', { static: true })
  private eventConditionsTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate', { static: true })
  private actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal', { static: true })
  private editorModal: ModalDirective;

  @ViewChild('eventEditor', { static: true })
  private eventEditor: EventEditorComponent;

  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    protected eventService: EventService) {
  }

  public ngOnInit() {

    const gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: 'isEnabled', name: gT('events.management.IsEnabled'), width: 70, cellTemplate: this.isEnabled, canAutoResize: false },
      { prop: 'name', name: gT('events.management.Name'), width: 50 },
      { prop: 'eventConditions', name: gT('events.management.EventConditions'), width: 350, cellTemplate: this.eventConditionsTemplate },
      { prop: 'actionDevice.name', name: gT('events.management.ActionDevice'), width: 50 },
      { prop: 'stateDescription', name: gT('events.management.State'), width: 50 }
    ];

    if (this.canManageEvents) {
      this.columns.push({ name: '', width: 200, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false });
    }
    this.loadData();
  }

  public ngAfterViewInit() {
    this.eventEditor.changesSavedCallback = (event: generated.EventViewModel) => {
      this.onEditorModalSaved(event);
      this.editorModal.hide();
    };

    this.eventEditor.changesCancelledCallback = () => {
      this.editedEvent = null;
      this.cachedEvent = null;
      this.editorModal.hide();
    };
  }

  private loadData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.eventService.getEvents(true).subscribe(events => this.onLoadDataSuccessful(events), error => this.onLoadDataFailed(error));

  }

  private onLoadDataSuccessful(events: generated.EventViewModel[]) {
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;

    this.cachedRows = [...events];
    this.rows = events;
  }

  private onLoadDataFailed(error: any) {
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;

    this.alertService.showStickyMessage('Load Error', `Unable to retrieve events from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
      MessageSeverity.error, error);
  }

  protected onSearchChanged(value: string) {
    this.rows = this.cachedRows.filter(r => Utilities.searchArray(value, false, r.eventId, r.name, r.state, r.actionDevice.name));
  }

  private onEditorModalSaved(newEvent: generated.EventViewModel) {

    if (this.cachedEvent) {
      let sourceIndex = this.cachedRows.indexOf(this.cachedEvent, 0);
      if (sourceIndex > -1) {
        Object.assign(this.cachedEvent, newEvent);
        Object.assign(this.cachedRows[sourceIndex], this.cachedEvent);
        Object.assign(this.rows[sourceIndex], this.cachedEvent);
      }
    } else {
      const event = new generated.EventViewModel();
      Object.assign(event, newEvent);
      this.cachedRows.splice(this.cachedRows.length, 0, event);
      this.rows.splice(this.rows.length, 0, event);
      this.rows = [...this.rows];
      this.ngxDatatable.offset = Math.round(this.rows.length / this.ngxDatatable.pageSize);
    }
    this.editedEvent = null;
    this.cachedEvent = null;
  }

  protected onEditorModalHidden() {
    this.editingEventName = null;
    this.eventEditor.resetForm(true);
  }

  protected newEvent() {
    this.editingEventName = null;
    this.cachedEvent = null;
    this.editedEvent = this.eventEditor.newEvent();
    this.editorModal.show();
  }

  protected editEvent(row: generated.EventViewModel) {
    this.editingEventName = { name: row.name };
    this.cachedEvent = row;
    this.editedEvent = this.eventEditor.editEvent(row);
    this.editorModal.show();
  }

  protected deleteEvent(row: generated.EventViewModel) {
    this.alertService.showDialog('Are you sure you want to delete \"' + row.name + '\"?', DialogType.confirm, () => this.deleteEventHelper(row));
  }

  private deleteEventHelper(row: generated.EventViewModel) {

    this.alertService.startLoadingMessage('Deleting...');
    this.loadingIndicator = true;

    this.eventService.deleteEvent(row.eventId)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.cachedRows = this.cachedRows.filter(item => item !== row);
        this.rows = this.rows.filter(item => item !== row);
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage('Delete Error', `An error occurred whilst deleting the event.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
            MessageSeverity.error, error);
        });
  }

  protected get canManageEvents() {
    return this.eventService.canManageEvents;
  }
}
