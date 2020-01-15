import { Injectable } from '@angular/core';
import * as generated from './endpoint.services';
import { Observable } from 'rxjs';

@Injectable()
export class EventConditionService {
  constructor(private eventConditionEndpointService: generated.EventConditionEndpointService) {

  }

  getEventConditions(pageNumber?: number, pageSize?: number): Observable<generated.EventConditionViewModel[]> {
    return pageNumber && pageSize ? this.eventConditionEndpointService.getAllPaged(pageNumber, pageSize) :
      this.eventConditionEndpointService.getAll();
  }

  getEventCondition(eventConditionId: number): Observable<generated.EventConditionViewModel> {
    return this.eventConditionEndpointService.get(eventConditionId);
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
}
