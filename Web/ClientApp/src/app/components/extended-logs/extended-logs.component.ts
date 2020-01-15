import { Component } from '@angular/core';
import { fadeInOut } from '../../helpers/animations';

@Component({
    selector: 'extended-logs',
    templateUrl: './extended-logs.component.html',
    styleUrls: ['./extended-logs.component.scss'],
    animations: [fadeInOut]
})
export class ExtendedLogsComponent {
    constructor() {

    }
}
