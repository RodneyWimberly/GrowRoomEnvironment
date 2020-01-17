import { Injectable } from '@angular/core';
import * as generated from './endpoint.services';
import { Observable, forkJoin } from 'rxjs';

@Injectable()
export class EventService {
  constructor(private eventEndpointService: generated.EventEndpointService,
    private eventConditionEndpointService: generated.EventConditionEndpointService,
    private actionDeviceEndpointService: generated.ActionDeviceEndpointService,
    private dataPointEndpointService: generated.DataPointEndpointService,
    private authEndpointService: generated.AuthEndpointService) {

  }

  getEditorData(): Observable<[generated.ActionDeviceViewModel[], generated.DataPointViewModel[]]> {
    return forkJoin(this.getActionDevices(), this.getDataPoints());
  }

  public getOperatorSymbol(operator: generated.Operators): string {
    var symbol: string;
    switch (operator) {
      case generated.Operators.Equal:
        {
          symbol = "==";
          break;
        }
      case generated.Operators.GreaterThan:
        {
          symbol = ">";
          break;
        }
      case generated.Operators.LessThan:
        {
          symbol = "<";
          break;
        }
      case generated.Operators.NotEqual:
        {
          symbol = "!=";
          break;
        }
    }
    return symbol;
  }

  public get canManageEvents(): boolean {
    return this.authEndpointService.userPermissions.some(p => p == generated.PermissionValues.ManageEvents);
  }

  public get canViewEvents(): boolean {
    return this.authEndpointService.userPermissions.some(p => p == generated.PermissionValues.ViewEvents);
  }

  public get canExecuteEvents(): boolean {
    return this.authEndpointService.userPermissions.some(p => p == generated.PermissionValues.ExecuteEvents);
  }

  public getActionDevices(getDisabled?: boolean, pageNumber?: number, pageSize?: number): Observable<generated.ActionDeviceViewModel[]> {
    if (getDisabled == null)
      getDisabled = false;
    return pageNumber && pageSize ? this.actionDeviceEndpointService.getAllPaged(pageNumber, pageSize, getDisabled) :
      this.actionDeviceEndpointService.getAll(getDisabled);
  }

  public getActionDevice(actionDeviceId: number): Observable<generated.ActionDeviceViewModel> {
    return this.actionDeviceEndpointService.get(actionDeviceId);
  }

  public getDataPoints(getDisabled?: boolean, pageNumber?: number, pageSize?: number): Observable<generated.DataPointViewModel[]> {
    if (getDisabled == null)
      getDisabled = false;
    return pageNumber && pageSize ? this.dataPointEndpointService.getAllPaged(pageNumber, pageSize, getDisabled) :
      this.dataPointEndpointService.getAll(getDisabled);
  }

  public getDataPoint(dataPointId: number): Observable<generated.DataPointViewModel> {
    return this.dataPointEndpointService.get(dataPointId);
  }

  public getEventConditionsByEventId(eventId: number, pageNumber?: number, pageSize?: number): Observable<generated.EventConditionViewModel[]> {
    return pageNumber && pageSize ? this.eventConditionEndpointService.getByEventIdPaged(eventId, pageNumber, pageSize) :
      this.eventConditionEndpointService.getByEventId(eventId);
  }

  public addEventCondition(eventCondition: generated.EventConditionViewModel): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.post(eventCondition);
  }

  public updateEventCondition(eventCondition: generated.EventConditionViewModel): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.put(eventCondition.eventConditionId, eventCondition);
  }

  public patchEventCondition(eventConditionId: number, eventCondition: generated.Operation[]): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.patch(eventConditionId, eventCondition);
  }

  public deleteEventCondition(eventConditionId: number): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.delete(eventConditionId);
  }

  public getEvents(getDisabled?: boolean, pageNumber?: number, pageSize?: number): Observable<generated.EventViewModel[]> {
    if (getDisabled == null)
      getDisabled = false;
    return pageNumber && pageSize ? this.eventEndpointService.getAllPaged(pageNumber, pageSize, getDisabled) :
      this.eventEndpointService.getAll(getDisabled);
  }

  public getEvent(eventId: number): Observable<generated.EventViewModel> {
    return this.eventEndpointService.get(eventId);
  }

  public addEvent(event: generated.EventViewModel): Observable<generated.EventViewModel> {
    return this.eventEndpointService.post(event);
  }

  public updateEvent(event: generated.EventViewModel): Observable<generated.EventViewModel> {
    return this.eventEndpointService.put(event.eventId, event);
  }

  public patchEvent(eventId: number, event: generated.Operation[]): Observable<generated.EventViewModel> {
    return this.eventEndpointService.patch(eventId, event);
  }

  public deleteEvent(eventId: number): Observable<generated.EventViewModel> {
    return this.eventEndpointService.delete(eventId);
  }
}
