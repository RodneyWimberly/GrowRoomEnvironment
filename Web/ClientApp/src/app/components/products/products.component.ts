import { Component } from '@angular/core';
import { fadeInOut } from '../../helpers/animations';

@Component({
    selector: 'products',
    templateUrl: './products.component.html',
    styleUrls: ['./products.component.scss'],
    animations: [fadeInOut]
})
export class ProductsComponent {
}
