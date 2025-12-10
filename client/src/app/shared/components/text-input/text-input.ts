import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { MatInput } from '@angular/material/input';
import { MatError, MatFormField, MatLabel } from '@angular/material/select';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule, MatFormField, MatInput, MatLabel, MatError],
  templateUrl: './text-input.html',
  styleUrl: './text-input.scss',
})
export class TextInput implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';

  // When injecting anything to an Angular component, Angular will try to reuse it if it was already used. Since we do not want that because we want each FormControl to be independent and not reused from a previous FormControl element we use the @Self decorator which prevents it. Note that all FormControl elements inherit from the NgControl class
  constructor(@Self() public controlDir: NgControl) {
    this.controlDir.valueAccessor = this;
  }
  writeValue(obj: any): void {
    // throw new Error('Method not implemented.'); // Do not want to throw an error
  }
  registerOnChange(fn: any): void {
    // throw new Error('Method not implemented.'); // Do not want to throw an error
  }
  registerOnTouched(fn: any): void {
    // throw new Error('Method not implemented.'); // Do not want to throw an error
  }
  // setDisabledState?(isDisabled: boolean): void { // Do not need that method. Removing it since it is optional
  //   throw new Error('Method not implemented.');
  // }

  // Adding a getter for TypeScript satisfactioning. The controlDir.control is of type AbstractControl so we are returning it as a FormControl because the template does not recognize the AbstractControl type
  get control() {
    return this.controlDir.control as FormControl;
  }
}
