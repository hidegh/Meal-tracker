import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MealListFilterDto } from './meal-list-filter.dto.ts';

@Component({
  selector: 'ks-meal-list-filter',
  templateUrl: 'meal-list-filter.component.html'
})
export class MealListFilterComponent {

  @Input() filter = new MealListFilterDto();
  @Output() filterChange = new EventEmitter<MealListFilterDto>();

  setFilter() {
    this.filterChange.emit(this.filter);
  }

  clearFilter() {
    this.filter.dateStringFrom = "";
    this.filter.dateStringTo = "";
    this.filter.timeStringFrom = "";
    this.filter.timeStringTo = "";
    this.filterChange.emit(this.filter);
  }
}
