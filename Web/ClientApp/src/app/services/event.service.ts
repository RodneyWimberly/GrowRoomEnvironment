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

  userHasPermission(permissionValue: generated.PermissionValue): boolean {
    return this.authEndpointService.userPermissions.some(p => p == permissionValue);
  }

  getActionDevices(getDisabled?: boolean, pageNumber?: number, pageSize?: number): Observable<generated.ActionDeviceViewModel[]> {
    if (getDisabled == null)
      getDisabled = false;
    return pageNumber && pageSize ? this.actionDeviceEndpointService.getAllPaged(pageNumber, pageSize, getDisabled) :
      this.actionDeviceEndpointService.getAll(getDisabled);
  }

  getActionDevice(actionDeviceId: number): Observable<generated.ActionDeviceViewModel> {
    return this.actionDeviceEndpointService.get(actionDeviceId);
  }

  getDataPoints(getDisabled?: boolean, pageNumber?: number, pageSize?: number): Observable<generated.DataPointViewModel[]> {
    if (getDisabled == null)
      getDisabled = false;
    return pageNumber && pageSize ? this.dataPointEndpointService.getAllPaged(pageNumber, pageSize, getDisabled) :
      this.dataPointEndpointService.getAll(getDisabled);
  }

  getDataPoint(dataPointId: number): Observable<generated.DataPointViewModel> {
    return this.dataPointEndpointService.get(dataPointId);
  }

  getEventConditionsByEventId(eventId: number, pageNumber?: number, pageSize?: number): Observable<generated.EventConditionViewModel[]> {
    return pageNumber && pageSize ? this.eventConditionEndpointService.getByEventIdPaged(eventId, pageNumber, pageSize) :
      this.eventConditionEndpointService.getByEventId(eventId);
  }

  addEventCondition(eventCondition: generated.EventConditionViewModel): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.post(eventCondition);
  }

  updateEventCondition(eventCondition: generated.EventConditionViewModel): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.put(eventCondition.eventConditionId, eventCondition);
  }

  patchEventCondition(eventConditionId: number, eventCondition: generated.Operation[]): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.patch(eventConditionId, eventCondition);
  }

  deleteEventCondition(eventConditionId: number): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.delete(eventConditionId);
  }

  getEvents(getDisabled?: boolean, pageNumber?: number, pageSize?: number): Observable<generated.EventViewModel[]> {
    if (getDisabled == null)
      getDisabled = false;
    return pageNumber && pageSize ? this.eventEndpointService.getAllPaged(pageNumber, pageSize, getDisabled) :
      this.eventEndpointService.getAll(getDisabled);
  }

  getEvent(eventId: number): Observable<generated.EventViewModel> {
    return this.eventEndpointService.get(eventId);
  }

  addEvent(event: generated.EventViewModel): Observable<generated.EventViewModel> {
    return this.eventEndpointService.post(event);
  }

  updateEvent(event: generated.EventViewModel): Observable<generated.EventViewModel> {
    return this.eventEndpointService.put(event.eventId, event);
  }

  patchEvent(eventId: number, event: generated.Operation[]): Observable<generated.EventViewModel> {
    return this.eventEndpointService.patch(eventId, event);
  }

  deleteEvent(eventId: number): Observable<generated.EventViewModel> {
    return this.eventEndpointService.delete(eventId);
  }
}
