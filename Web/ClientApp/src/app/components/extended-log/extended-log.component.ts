import { Component } from '@angular/core';
import { fadeInOut } from '../../helpers/animations';

@Component({
    selector: 'extended-log',
    templateUrl: './extended-log.component.html',
    styleUrls: ['./extended-log.component.scss'],
    animations: [fadeInOut]
})
/** extended-log component*/
export class ExtendedLogComponent {
    /** extended-log ctor */
    constructor() {

    }
}
