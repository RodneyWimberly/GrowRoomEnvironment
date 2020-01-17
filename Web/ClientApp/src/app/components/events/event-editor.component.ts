import { Component, OnInit, ViewChild } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { EventService } from "../../services/event.service";
import { Utilities } from '../../helpers/utilities';
import * as generated from '../../services/endpoint.services';
import { ViewModelStates } from '../../models/enum.models';
import { EventConditionsManagementComponent } from './event-conditions-management.component';

@Component({
  selector: 'event-editor',
  templateUrl: './event-editor.component.html',
  styleUrls: ['./event-editor.component.scss']
})

export class EventEditorComponent implements OnInit {
  public changesSavedCallback: (event: generated.EventViewModel) => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  protected isSaving = false;
  protected uniqueId: string = Utilities.uniqueId();
  protected formResetToggle = true;
  protected actionDevices: generated.ActionDeviceViewModel[];
  protected dataPoints: generated.DataPointViewModel[];
  protected viewModelState: ViewModelStates = ViewModelStates.Edit;
  protected states = generated.ActionDeviceStates;
  protected stateKeys: number[];

 
  @ViewChild('form', { static: false })
  public form;

  @ViewChild('isEnabled', { static: false })
  public isEnabled;

  @ViewChild('name', { static: false })
  public name;

  @ViewChild('actionDevice', { static: false })
  public actionDevice;

  @ViewChild('state', { static: false })
  public state;

  @ViewChild('eventConditions', { static: false })
  public eventConditions: EventConditionsManagementComponent;

  constructor(private alertService: AlertService,
    private eventService: EventService) {
    this.stateKeys = Object.keys(this.states).filter(k => !isNaN(Number(k))).map(Number);
  }

  public ngOnInit(): void {
    this.loadData();
  }

  public resetForm(replace = false): void {
    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;
      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  public newEvent(): generated.EventViewModel {
    this.viewModelState = ViewModelStates.New;
    return this.event = undefined;
  }

  public editEvent(event: generated.EventViewModel): generated.EventViewModel {
    this.viewModelState = ViewModelStates.Edit;
    return this.event = event;
  }

  public viewEvent(event: generated.EventViewModel): generated.EventViewModel {
    this.viewModelState = ViewModelStates.View;
    return this.event = event;
  }

  public get event(): generated.EventViewModel {
    if (this.eventConditions && this.eventConditions.event) {
      return this.eventConditions.event;
    } else {
      return new generated.EventViewModel();
    }
  }

  public set event(event: generated.EventViewModel) {
    if (!event) {
      event = new generated.EventViewModel();
      event.eventConditions = [];
    }
    this.eventConditions.event = new generated.EventViewModel();
    Object.assign(this.eventConditions.event, event);
    this.eventConditions.loadData();
  }

  protected get canManageEvents(): boolean {
    return this.eventService.canManageEvents;
  }

  protected get isEditMode(): boolean {
    return this.viewModelState == ViewModelStates.Edit || this.viewModelState == ViewModelStates.New;
  }

  protected cancel(): void {
    this.alertService.showMessage('Canceled', 'Operation canceled by user', MessageSeverity.default);
    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback) {
      this.changesCancelledCallback();
    }
  }

  protected close(): void {
    if (this.changesCancelledCallback) {
      this.changesCancelledCallback();
    }
  }

  protected save(): void {
    this.isSaving = true;
    this.alertService.startLoadingMessage('Saving changes...');
    this.event.actionDevice = this.actionDevices.find(ad => ad.actionDeviceId == this.event.actionDeviceId);
    this.event.stateDescription = this.states[this.event.state];
    this.event.eventConditions.forEach((value: generated.EventConditionViewModel, index: number, array: generated.EventConditionViewModel[]) => {
      value.operatorDescription = generated.Operators[value.operator].toString();
      value.dataPoint = this.dataPoints.find(dp => dp.dataPointId == value.dataPointId);
    });
    if (this.viewModelState == ViewModelStates.New) {
      this.eventService.addEvent(this.event).subscribe(event => this.saveSuccessHelper(event), error => this.saveFailedHelper(error));
    } else if (this.viewModelState == ViewModelStates.Edit) {
      this.eventService.updateEvent(this.event).subscribe(event => this.saveSuccessHelper(event), error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(event?: generated.EventViewModel): void {
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

    this.viewModelState = ViewModelStates.Edit;

    if (this.changesSavedCallback) {
      this.changesSavedCallback(this.event);
    }
    this.resetForm();
  }

  private saveFailedHelper(error: any): void {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage('Save Error', 'The below errors occurred while saving your changes:', MessageSeverity.error, error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
  }

  private loadData(): void {
    this.alertService.startLoadingMessage();
    this.eventService.getEditorData().subscribe(
      results => this.onLoadDataSuccessful(results[0], results[1]),
      error => this.onLoadDataFailed(error));
  }

  private onLoadDataSuccessful(actionDevices: generated.ActionDeviceViewModel[], dataPoints: generated.DataPointViewModel[]): void {
    this.alertService.stopLoadingMessage();
    this.actionDevices = actionDevices;
    this.dataPoints = dataPoints;
  }

  private onLoadDataFailed(error: any): void {
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage('Load Error', `Unable to retrieve user data from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`, MessageSeverity.error, error);
    this.actionDevices = new generated.ActionDeviceViewModel[0];
    this.dataPoints = new generated.DataPointViewModel[0];
  }
}
  

