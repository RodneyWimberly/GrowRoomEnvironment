/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { ExtendedLogManagementComponent } from './extended-log-management.component';

let component: ExtendedLogManagementComponent;
let fixture: ComponentFixture<ExtendedLogManagementComponent>;

describe('extended-log-management component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ ExtendedLogManagementComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(ExtendedLogManagementComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});