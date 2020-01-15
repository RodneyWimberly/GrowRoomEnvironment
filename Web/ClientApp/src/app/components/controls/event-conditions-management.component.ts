import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input, Output, EventEmitter, ViewChildren, QueryList, ElementRef } from '@angular/core';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { EventService } from "../../services/event.service";
import { ViewModelStates } from "../../models/enum.models";
import { Utilities } from '../../helpers/utilities';
import * as generated from '../../services/endpoint.services';
import { DatatableComponent } from '@swimlane/ngx-datatable';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'event-conditions-management',
  templateUrl: './event-conditions-management.component.html',
  styleUrls: ['./event-conditions-management.component.scss']
})

export class EventConditionsManagementComponent implements OnInit {
  private rowsCache: generated.EventConditionViewModel[] = [];
  private rowCache: generated.EventConditionViewModel;
  protected columns: any[] = [];
  protected rows: generated.EventConditionViewModel[] = [];
  protected loadingIndicator: boolean;
  protected operators = generated.Operators;
  protected operatorKeys: number[];
  protected editId: number = 0;
  protected states = ViewModelStates;
  private _viewModelState: ViewModelStates = ViewModelStates.View;

  @Input()
  public event: generated.EventViewModel;

  @Input()
  public dataPoints: generated.DataPointViewModel[];

  @Output()
  public eventConditionsChanged: EventEmitter<generated.EventConditionViewModel[]> = new EventEmitter<generated.EventConditionViewModel[]>();

  @Output()
  public editModeChanged: EventEmitter<boolean> = new EventEmitter<boolean>();

  @ViewChild('dataTable', { static: true })
  private ngxDatatable: DatatableComponent;

  @ViewChild('idTemplate', { static: true })
  private idTemplate: TemplateRef<any>;

  @ViewChild('dataPointTemplate', { static: true })
  private dataPointTemplate: TemplateRef<any>;

  @ViewChild('operatorTemplate', { static: true })
  private operatorTemplate: TemplateRef<any>;

  @ViewChild('valueTemplate', { static: true })
  private valueTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate', { static: true })
 private actionsTemplate: TemplateRef<any>;

  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private eventService: EventService) {
    this.operatorKeys = Object.keys(this.operators).filter(k => !isNaN(Number(k))).map(Number);
  }

  public ngOnInit() {

    const gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: 'eventConditionId', name: gT('eventConditions.management.EventConditionId'), width: 30, cellTemplate: this.idTemplate, canAutoResize: false },
      { prop: 'dataPointId', name: gT('eventConditions.management.DataPoint'), width: 230, cellTemplate: this.dataPointTemplate },
      { prop: 'operator', name: gT('eventConditions.management.Operator'), width: 160, cellTemplate: this.operatorTemplate },
      { prop: 'value', name: gT('eventConditions.management.Value'), width: 160, cellTemplate: this.valueTemplate },
    ];

    if (this.canManageEvents) {
      this.columns.push({ name: '', width: 180, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false });
    }
    this.loadData();
  }

  protected get viewModelState(): ViewModelStates {
    return this._viewModelState;
  }

  protected set viewModelState(value: ViewModelStates) {
    this._viewModelState = value;
    this.editModeChanged.emit(this.viewModelState == ViewModelStates.Edit || this.viewModelState == ViewModelStates.New);
  }
  
  protected get canManageEvents() {
    return this.eventService.userHasPermission(generated.PermissionValues.ManageEvents);
  }

  protected getDataPointName(dataPointId: number): string {
    var dataPoint: generated.DataPointViewModel = this.dataPoints.find(d => d.dataPointId == dataPointId);
    return dataPoint ? dataPoint.name : "";
  }

  protected onSearchChanged(value: string) {
    if (this.viewModelState == ViewModelStates.View) {
      this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.eventId, r.eventConditionId, r.dataPoint.name, r.operatorDescription, r.value));
    }
  }
 
  protected newEventCondition() {
    this.viewModelState = ViewModelStates.New;


    this.rowCache = null;
    var eventCondition: generated.EventConditionViewModel = this.createEventCondition();

    this.rows.splice(this.rows.length, 0, eventCondition);
    this.rows = [...this.rows];
    this.ngxDatatable.offset = Math.round(this.rows.length / this.ngxDatatable.pageSize);
  }

  protected editEventCondition(row: generated.EventConditionViewModel) {
    this.viewModelState = ViewModelStates.Edit;
    this.editId = row.eventConditionId;
    this.rowCache = new generated.EventConditionViewModel();
    Object.assign(this.rowCache, row);
  }

  protected deleteEventCondition(row: generated.EventConditionViewModel) {
    this.alertService.showDialog(`Are you sure you want to delete condition \"${row.dataPoint.name} ${this.eventService.getOperatorSymbol(row.operator)} \'${row.value}\'\"?`, DialogType.confirm,
      () => {
        this.viewModelState = ViewModelStates.Delete;
        this.saveEventCondition(row);
      });
  }

  protected cancelEventCondition(row: generated.EventConditionViewModel) {
    if (this.viewModelState == ViewModelStates.Edit) {
      Object.assign(row, this.rowCache);
    }
    else if (this.viewModelState == ViewModelStates.New) {
      this.rows = [...this.rowsCache];

    }
    this.viewModelState = ViewModelStates.View;
    this.editId = 0;
    this.rowCache = null;
  }

  protected saveEventCondition(row: generated.EventConditionViewModel) {
    if (this.viewModelState == ViewModelStates.View) {
      return;
    }
    this.loadingIndicator = true;

    if (this.viewModelState == ViewModelStates.Delete) {
      this.alertService.startLoadingMessage('Deleting...');
      this.eventService.deleteEventCondition(row.eventConditionId).subscribe(response => this.onSaveEventConditionSucccess(row), error => this.onSaveEventConditionFailed(error));
    }
    else {
      row.operatorDescription = this.operators[row.operator];
      row.dataPoint = this.dataPoints.find(d => d.dataPointId == row.dataPointId);
      if (this.viewModelState == ViewModelStates.New) {
        this.alertService.startLoadingMessage("Add current event condition!");
        this.eventService.addEventCondition(row).subscribe(eventCondition => this.onSaveEventConditionSucccess(row, eventCondition), error => this.onSaveEventConditionFailed(error));
      }
      else if (this.viewModelState == ViewModelStates.Edit) {
        this.alertService.startLoadingMessage("Saving current event condition!");
        this.eventService.updateEventCondition(row).subscribe(eventCondition => this.onSaveEventConditionSucccess(row, eventCondition), error => this.onSaveEventConditionFailed(error));
      }
    }
  }

  private onSaveEventConditionSucccess(row: generated.EventConditionViewModel, eventCondition?: generated.EventConditionViewModel) {
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;

    if (this.viewModelState == ViewModelStates.New) {
      row.eventConditionId = eventCondition.eventConditionId;
      row.updatedBy = eventCondition.updatedBy;
      row.updatedDate = eventCondition.updatedDate;
      row.rowVersion = eventCondition.rowVersion.valueOf();
      this.rowsCache = [...this.rows];
      this.event.eventConditions = [...this.rows];
      this.alertService.showMessage('Success', `Event Condition was created successfully`, MessageSeverity.success);
    }
    else if (this.viewModelState == ViewModelStates.Edit) {
      row.updatedBy = eventCondition.updatedBy;
      row.updatedDate = eventCondition.updatedDate;
      row.rowVersion = eventCondition.rowVersion.valueOf();
      this.alertService.showMessage('Success', `Event Condition was saved successfully`, MessageSeverity.success);
    }
    else if (this.viewModelState == ViewModelStates.Delete) {
      this.rowsCache = this.rowsCache.filter(item => item !== row);
      this.rows = this.rows.filter(item => item !== row);
      this.event.eventConditions = this.event.eventConditions.filter(item => item !== row);
      this.alertService.showMessage('Success', `Event Condition was deleted successfully`, MessageSeverity.success);
    }
    this.viewModelState = ViewModelStates.View;
    this.rowCache = null;
    this.editId = 0;
    this.eventConditionsChanged.emit(this.event.eventConditions);
  }

  private onSaveEventConditionFailed(error: any) {
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;
    this.alertService.showStickyMessage(`Error`, 'The below errors occurred while saving your changes: ', MessageSeverity.error, error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);
    this.viewModelState = ViewModelStates.View;
    this.rowCache = null;
    this.editId = 0;
  }

  private loadData() {
    if (this.event && this.event.eventConditions && this.event.eventConditions.length > 0) {
      this.rowsCache = [...this.event.eventConditions];
      this.rows = this.event.eventConditions;
    }
  }

  private createEventCondition(): generated.EventConditionViewModel {
    var eventCondition: generated.EventConditionViewModel = new generated.EventConditionViewModel();
    eventCondition.eventConditionId = 0;
    eventCondition.dataPointId = this.dataPoints[0].dataPointId;
    eventCondition.operator = generated.Operators.Equal;
    eventCondition.eventId = this.event.eventId;
    return eventCondition;
  }
}
