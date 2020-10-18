import { Component, Input, ContentChild, OnInit } from '@angular/core';
import { InputRefDirective } from '../../directives/input-ref.directive';
import { debug } from 'util';

@Component({
  selector: 'custom-input',
  templateUrl: './custom-input.component.html',
  styleUrls: ['./custom-input.component.scss']
})
export class CustomInputComponent implements OnInit {
  @Input() label: string;
  @Input() validations: { [index: string]: string };
  @Input() info: string;

  @ContentChild(InputRefDirective, { static: true }) input: InputRefDirective;

  get isError() {
    return this.input.hasError;
  }

  get errorMessages() {
    const errors = this.input.errors;
    const messages = [];
    const keys = Object.keys(this.validations);

    keys.forEach(key => {
      if (errors[key]) {
        messages.push(this.validations[key]);
      }
    });
    return messages;
  }

  ngOnInit() { }

  constructor() { }
}
