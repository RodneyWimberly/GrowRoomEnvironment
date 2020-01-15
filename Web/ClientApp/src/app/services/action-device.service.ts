import { Injectable } from '@angular/core';
import * as generated from './endpoint.services';
import { Observable } from 'rxjs';

@Injectable()
export class ActionDeviceService {
  constructor(private actionDeviceEndpointService: generated.ActionDeviceEndpointService) {

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

  setActionDeviceState(actionDeviceId: number, state: generated.ActionDeviceStates): Observable<void> {
    return this.actionDeviceEndpointService.putState(actionDeviceId, state);
  }

  addActionDevice(actionDevice: generated.ActionDeviceViewModel): Observable<generated.ActionDeviceViewModel> {
    return this.actionDeviceEndpointService.post(actionDevice);
  }

  updateActionDevice(actionDevice: generated.ActionDeviceViewModel): Observable<void> {
    return this.actionDeviceEndpointService.put(actionDevice.actionDeviceId, actionDevice);
  }

  patchActionDevice(actionDeviceId: number, actionDevice: generated.Operation[]): Observable<void> {
    return this.actionDeviceEndpointService.patch(actionDeviceId, actionDevice);
  }

  deleteActionDevice(actionDeviceId: number): Observable<generated.ActionDeviceViewModel> {
    return this.actionDeviceEndpointService.delete(actionDeviceId);
  }
}
