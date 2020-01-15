import { Component } from '@angular/core';
import { fadeInOut } from '../../helpers/animations';

@Component({
    selector: 'events',
    templateUrl: './events.component.html',
    styleUrls: ['./events.component.scss'],
    animations: [fadeInOut]
})
/** events component*/
export class EventsComponent {
    /** events ctor */
    constructor() {

    }
}
