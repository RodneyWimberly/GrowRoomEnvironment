import { Injectable } from '@angular/core';
import * as generated from './endpoint.services';

@Injectable()
export class ExtendedLogService {
    constructor(private extendedLogEndpointService: generated.ExtendedLogEndpointService) {

    }

    addExtendedLog(extendedLog: generated.ExtendedLogViewModel) {
        return this.extendedLogEndpointService.post(extendedLog);
    }

    getExtendedLogs(pageNumber?: number, pageSize?: number) {
        return pageNumber && pageSize ? this.extendedLogEndpointService.getAllPaged(pageNumber, pageSize) :
            this.extendedLogEndpointService.getAll();
    }

    getExtendedLogsByLevel(level: number, pageNumber?: number, pageSize?: number) {
        return pageNumber && pageSize ? this.extendedLogEndpointService.getByLevelPaged(level, pageNumber, pageSize) :
            this.extendedLogEndpointService.getByLevel(level);
    }

    clearExtendedLogs() {
        return this.extendedLogEndpointService.deleteAll();
    }
}
