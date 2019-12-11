import { NG_VALIDATORS, Validator, AbstractControl, ValidationErrors } from '@angular/forms';
import { Directive, forwardRef, ElementRef } from '@angular/core';

@Directive({
  selector: "input[type=number][ngModel]",
  providers: [
      { provide: NG_VALIDATORS, useExisting: forwardRef(() => NumberRangeNativeValidator), multi: true }
  ]
})
export class NumberRangeNativeValidator implements Validator {

  constructor(
      private element: ElementRef
  ) {
  }

  validate(c: AbstractControl): ValidationErrors {

    const result = {} as any;

    if (this.element.nativeElement.validity.rangeOverflow)
      result.max = true;

    if (this.element.nativeElement.validity.rangeUnderflow)
      result.min = true;

    if (Object.keys(result).length)
      return result;

    return null;
  }
}
