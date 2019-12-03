/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { ExtendedLogComponent } from './extended-log.component';

let component: ExtendedLogComponent;
let fixture: ComponentFixture<ExtendedLogComponent>;

describe('extended-log component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ ExtendedLogComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(ExtendedLogComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});