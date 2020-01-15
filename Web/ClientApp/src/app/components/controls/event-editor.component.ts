import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { EventService } from "../../services/event.service";
import { Utilities } from '../../helpers/utilities';
import * as generated from '../../services/endpoint.services';
import { ViewModelStates } from '../../models/enum.models';


@Component({
  selector: 'event-editor',
  templateUrl: './event-editor.component.html',
  styleUrls: ['./event-editor.component.scss']
})

export class EventEditorComponent implements OnInit {
  protected isChildEditMode: boolean;
  protected isSaving = false;
  protected uniqueId: string = Utilities.uniqueId();
  protected event: generated.EventViewModel = new generated.EventViewModel();
  protected formResetToggle = true;
  protected actionDevices: generated.ActionDeviceViewModel[];
  protected dataPoints: generated.DataPointViewModel[];
  protected viewModelstates = ViewModelStates;
  protected viewModelState: ViewModelStates = ViewModelStates.View;
  protected states = generated.ActionDeviceStates;
  protected stateKeys: number[];

  public changesSavedCallback: (event: generated.EventViewModel) => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @Input()
  public isViewOnly: boolean;

  @ViewChild('f', { static: false })
  protected form;

  // ViewChilds Required because ngIf hides template variables from global scope
  @ViewChild('name', { static: false })
  protected name;

  @ViewChild('actionDevice', { static: false })
  protected actionDevice;

  @ViewChild('state', { static: false })
  protected state;

  @ViewChild('eventConditions', { static: false })
  protected eventConditions;

  constructor(private alertService: AlertService,
    private eventService: EventService) {
    this.stateKeys = Object.keys(this.states).filter(k => !isNaN(Number(k))).map(Number);
  }

  ngOnInit() {
    this.loadData();
  }

  private loadData() {
    this.alertService.startLoadingMessage();
    this.eventService.getEditorData().subscribe(results => this.onLoadDataSuccessful(results[0], results[1]),
      error => this.onLoadDataFailed(error));
  }

  private onLoadDataSuccessful(actionDevices: generated.ActionDeviceViewModel[], dataPoints: generated.DataPointViewModel[]) {
    this.alertService.stopLoadingMessage();
    this.actionDevices = actionDevices;
    this.dataPoints = dataPoints;
  }

  private onLoadDataFailed(error: any) {
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage('Load Error', `Unable to retrieve user data from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
      MessageSeverity.error, error);
    this.actionDevices = new generated.ActionDeviceViewModel[0];
    this.dataPoints = new generated.DataPointViewModel[0];
  }

  protected onEventConditionsChanged(eventConditions: generated.EventConditionViewModel[]) {
    this.event.eventConditions = [...eventConditions];
  }

  protected onChildEditModeChanged(childEditMode: boolean) {
    this.isChildEditMode = childEditMode;
  }

  protected save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage('Saving changes...');
    this.event.actionDevice = this.actionDevices.find(ad => ad.actionDeviceId == this.event.actionDeviceId);
    this.event.stateDescription = this.states[this.event.state];
    if (this.viewModelState == ViewModelStates.New) {
      this.eventService.addEvent(this.event).subscribe(event => this.saveSuccessHelper(event), error => this.saveFailedHelper(error));
    } else if (this.viewModelState == ViewModelStates.Edit) {
      this.eventService.updateEvent(this.event).subscribe(event => this.saveSuccessHelper(event), error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(event?: generated.EventViewModel) {
    
   /* this.event.eventId = event.eventId;
    this.event.createdBy = event.createdBy;
    this.event.createdDate = event.createdDate;
    this.event.updatedBy = event.updatedBy;
    this.event.updatedDate = event.updatedDate;
    this.event.rowVersion = event.rowVersion.valueOf();*/
    Object.assign(this.event, event);
    this.event.eventConditions = [...event.eventConditions]

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    var eventName: string = this.event.name; 

    if (this.viewModelState == ViewModelStates.New) {
      this.alertService.showMessage('Success', `Event \"${eventName}\" was created successfully`, MessageSeverity.success);
    } else if (this.viewModelState == ViewModelStates.Edit) {
      this.alertService.showMessage('Success', `Changes to event \"${eventName}\" was saved successfully`, MessageSeverity.success);
    }

    this.viewModelState = ViewModelStates.View;

    if (this.changesSavedCallback) {
      this.changesSavedCallback(this.event);
    }
    this.resetForm();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage('Save Error', 'The below errors occurred while saving your changes:', MessageSeverity.error, error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
  }

  protected cancel() {
    this.alertService.showMessage('Canceled', 'Operation canceled by user', MessageSeverity.default);
    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback) {
      this.changesCancelledCallback();
    }
  }

  protected close() {
    if (this.changesSavedCallback) {
      this.changesSavedCallback(this.event);
    }
  }

  public resetForm(replace = false) {
    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  public newEvent() {
    this.viewModelState = ViewModelStates.New;
    this.event = new generated.EventViewModel();
    this.event.eventConditions = generated.EventConditionViewModel[0];
 
    return this.event;
  }

  public editEvent(event: generated.EventViewModel) {
    if (event) {
      this.viewModelState = ViewModelStates.Edit;
      this.event = new generated.EventViewModel();
      Object.assign(this.event, event);

      return this.event;
    } else {
      return this.newEvent();
    }
  }

  protected get canManageEvents() {
    return this.eventService.userHasPermission(generated.PermissionValues.ManageEvents);
  }

  protected get isEditMode() {
    return this.viewModelState == ViewModelStates.Edit || this.viewModelState == ViewModelStates.New;
  }

}
