import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MealDailyItemDto, MealsService } from '../../_services/meals-service';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'ks-meal-detail-modal',
  templateUrl: 'meal-detail.modal.component.html'
})
export class MealDetailModalComponent implements OnInit {

  @Input() userId: number;
  @Input() item: MealDailyItemDto = {} as MealDailyItemDto;

  public get isNewItem(): boolean { return this.item && !this.item.id; }

  public deleting: boolean  = false;
  public updating: boolean  = false;
  public creating: boolean  = false;

  public get loading(): boolean { return this.deleting || this.updating || this.creating; }

  public formGroup: FormGroup;

  constructor(
    public modal: NgbActiveModal,
    public formBuilder: FormBuilder,
    public mealsService: MealsService
    ) {

  }

  ngOnInit() {
    this.setMealDto(this.item);
  }

  setMealDto(meal: MealDailyItemDto) {
    const dateTimeString = moment(meal.date).format("YYYY-MM-DDTHH:mm");
    this.formGroup = this.formBuilder.group({
      dateTimeString: [dateTimeString, Validators.required],
      calories: [meal.calories, [Validators.required, Validators.min(0), Validators.max(5000)]],
      description: meal.description
    });
  }

  fetchMealDto(): MealDailyItemDto {
    const value = this.formGroup.value;

    const meal = {
      date: moment(value.dateTimeString).toDate(),
      calories: value.calories,
      description: value.description
    } as MealDailyItemDto;

    return meal;
  }

  create() {
    const meal = this.fetchMealDto();

    this.creating = true;

    this.mealsService
      .createUserMeal(this.userId, meal)
      .pipe(
        finalize(() => this.creating = false)
      )
      .subscribe(succ => this.modal.close());
  }

  update() {
    const meal = this.fetchMealDto();
    const mealId = this.item.id;

    this.updating = true;

    this.mealsService
      .updateUserMeal(this.userId, mealId, meal)
      .pipe(
        finalize(() => this.updating = false)
      )
      .subscribe(succ => this.modal.close());
  }

  delete() {
    const mealId = this.item.id;

    this.deleting = true;

    this.mealsService
      .deleteUserMeal(this.userId, mealId)
      .pipe(
        finalize(() => this.deleting = false)
      )
      .subscribe(succ => this.modal.close());
  }

}
