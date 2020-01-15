import { Injectable } from '@angular/core';
import * as generated from './endpoint.services';
import { Observable } from 'rxjs';

@Injectable()
export class DataPointService {
    constructor(private dataPointEndpointService: generated.DataPointEndpointService) {

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

    addDataPoint(dataPoint: generated.DataPointViewModel): Observable<generated.DataPointViewModel> {
        return this.dataPointEndpointService.post(dataPoint);
    }

    updateDataPoint(dataPoint: generated.DataPointViewModel): Observable<void>  {
        return this.dataPointEndpointService.put(dataPoint.dataPointId, dataPoint);
    }

    patchDataPoint(dataPointId: number, dataPoint: generated.Operation[]): Observable<void> {
        return this.dataPointEndpointService.patch(dataPointId, dataPoint);
    }

    deleteDataPoint(dataPointId: number): Observable<generated.DataPointViewModel> {
        return this.dataPointEndpointService.delete(dataPointId);
    }
}
