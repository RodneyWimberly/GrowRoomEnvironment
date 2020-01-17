import { Component, OnInit, TemplateRef, ViewChild, Input } from '@angular/core';

import { AlertService, DialogType } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { EventService } from "../../services/event.service";
import { Utilities } from '../../helpers/utilities';
import * as generated from '../../services/endpoint.services';
import { DatatableComponent } from '@swimlane/ngx-datatable';

@Component({
  selector: 'event-conditions-management',
  templateUrl: './event-conditions-management.component.html',
  styleUrls: ['./event-conditions-management.component.scss']
})

export class EventConditionsManagementComponent implements OnInit {
  public event: generated.EventViewModel;
  protected columns: any[] = [];
  protected rows: generated.EventConditionViewModel[] = [];
  protected loadingIndicator: boolean;
  protected operators = generated.Operators;
  protected operatorKeys: number[];

  @Input()
  public dataPoints: generated.DataPointViewModel[];

  @ViewChild('dataTable', { static: true })
  private ngxDatatable: DatatableComponent;

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

  public ngOnInit(): void {
    const translate = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: 'dataPointId', name: translate('eventConditions.management.DataPoint'), width: 230, cellTemplate: this.dataPointTemplate },
      { prop: 'operator', name: translate('eventConditions.management.Operator'), width: 160, cellTemplate: this.operatorTemplate },
      { prop: 'value', name: translate('eventConditions.management.Value'), width: 240, cellTemplate: this.valueTemplate },
      { name: '', width: 120, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadData();
  }

  protected getDataPointName(dataPointId: number): string {
    var dataPoint: generated.DataPointViewModel = this.dataPoints.find(d => d.dataPointId == dataPointId);
    return dataPoint ? dataPoint.name : "";
  }

  protected onSearchChanged(value: string): void {
    this.rows = this.event.eventConditions.filter(r => Utilities.searchArray(value, false, r.eventId, r.eventConditionId, r.dataPoint.name, r.operatorDescription, r.value));
  }
 
  protected newEventCondition(): void{
    var eventCondition: generated.EventConditionViewModel = new generated.EventConditionViewModel();
    eventCondition.eventConditionId = 0;
    eventCondition.dataPointId = this.dataPoints[0].dataPointId;
    eventCondition.operator = generated.Operators.Equal;
    eventCondition.eventId = this.event.eventId;

    this.rows.splice(this.rows.length, 0, eventCondition);
    this.rows = [...this.rows];
    this.event.eventConditions = [...this.rows];
    this.ngxDatatable.offset = Math.round(this.rows.length / this.ngxDatatable.pageSize);
  }

  protected deleteEventCondition(row: generated.EventConditionViewModel): void {
    this.alertService.showDialog(`Are you sure you want to delete condition \"${this.getDataPointName(row.dataPointId)} ${this.eventService.getOperatorSymbol(row.operator)} \'${row.value}\'\"?`, DialogType.confirm,
      () => {
        this.rows = this.rows.filter(item => item !== row);
        this.event.eventConditions = this.event.eventConditions.filter(item => item !== row);
      });
  }

  public loadData(): void {
    if (this.event && this.event.eventConditions && this.event.eventConditions.length > 0) {
      this.rows = this.event.eventConditions;
    } else {
      this.rows = [];
    }

  }
}
