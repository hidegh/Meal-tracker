import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MealDailyItemDto } from '../../_services/meals-service';

@Component({
  selector: 'ks-meal-daily-meals',
  templateUrl: 'meal-daily-meals.component.html'
})
export class MealDailyMealsComponent {

  @Input() dailyMeals: MealDailyItemDto[] = [];
  @Input() dailyCaloriesExceeded: boolean = false;

  @Output() mealSelected = new EventEmitter<MealDailyItemDto>();

  selectMeal(meal: MealDailyItemDto) {
    this.mealSelected.emit(meal);
  }
}
