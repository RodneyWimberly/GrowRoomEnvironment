/// <reference path="../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { ExtendedLogEditorComponent } from './extended-log-editor.component';

let component: ExtendedLogEditorComponent;
let fixture: ComponentFixture<ExtendedLogEditorComponent>;

describe('extended-log-editor component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ ExtendedLogEditorComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(ExtendedLogEditorComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});