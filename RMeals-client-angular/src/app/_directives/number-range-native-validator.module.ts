import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NumberRangeNativeValidator } from './number-range-native-validator.directive';

@NgModule({
  imports: [
      CommonModule
  ],
  declarations: [
    NumberRangeNativeValidator
  ],
  exports: [
    NumberRangeNativeValidator
  ]
})
export class NumberRangeNativeValidatorModule {
}
