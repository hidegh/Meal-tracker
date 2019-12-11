import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MealDailySummaryDto, MealDailyItemDto } from '../../_services/meals-service';

@Component({
  selector: 'ks-meal-list',
  templateUrl: 'meal-list.component.html'
})
export class MealListComponent {

  @Input() meals: MealDailySummaryDto[] = [];

  @Output() mealSelected = new EventEmitter<MealDailyItemDto>();

  selectMeal(meal: MealDailyItemDto) {
    this.mealSelected.emit(meal);
  }

}
