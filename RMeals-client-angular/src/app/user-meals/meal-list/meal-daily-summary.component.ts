import { Component, Input } from '@angular/core';
import { MealDailySummaryDto } from '../../_services/meals-service';

@Component({
  selector: 'ks-meal-daily-summary',
  templateUrl: 'meal-daily-summary.component.html'
})
export class MealDailySummaryComponent {

  @Input() dailySummary: MealDailySummaryDto = {} as MealDailySummaryDto;

}
