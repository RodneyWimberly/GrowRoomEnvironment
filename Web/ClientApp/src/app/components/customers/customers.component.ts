import { Component } from '@angular/core';
import { fadeInOut } from '../../helpers/animations';


@Component({
    selector: 'customers',
    templateUrl: './customers.component.html',
    styleUrls: ['./customers.component.scss'],
    animations: [fadeInOut]
})
export class CustomersComponent {

}
