import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { first, finalize, map } from 'rxjs/operators';

import { AuthService } from './../_auth/auth.service';
import { Subscription, Observable, of } from 'rxjs';

import { MealsService, LoadUserMealsFilter, MealDailySummaryDto, MealDailyItemDto } from '../_services/meals-service';

import { MealListFilterDto } from './meal-list-filter/meal-list-filter.dto.ts';

import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import { MealDetailModalComponent } from './meal-detail/meal-detail.modal.component';
import { UsersService, UserDetailsDto } from '../_services/users-service';

/*
There's an auto unsubscribe directive in NPM!
*/
@Component({
  templateUrl: 'user-meals.component.html'
})
export class UserMealsComponent implements OnDestroy {

  loading: boolean = false;

  paramsSubscription: Subscription;

  userId: any;
  userMeals$: Observable<MealDailySummaryDto[]>;

  userDetails$: Observable<any | UserDetailsDto>;

  filter: LoadUserMealsFilter = {} as LoadUserMealsFilter;

  constructor(
    private route: ActivatedRoute,
    private auth: AuthService,
    private mealsService: MealsService,
    private usersService: UsersService,
    private modalService: NgbModal
  ) {

    this.paramsSubscription = this.route.params.subscribe(params => {
      this.loadUserMeals(params.id);
    });
  }

  ngOnDestroy() {
    if (this.paramsSubscription)
      this.paramsSubscription.unsubscribe();
  }

  filterChange(uiFilter: MealListFilterDto) {
    const filter = {
      dateFrom: uiFilter.dateFrom,
      dateTo: uiFilter.dateTo,
      timeFrom: uiFilter.timeFrom,
      timeTo: uiFilter.timeTo
    } as  LoadUserMealsFilter;

    this.filter = filter;

    this.loadUserMeals(this.userId, this.filter);
  }

  loadUserMeals(userId: number, filter?: LoadUserMealsFilter) {
    this.userId = userId || this.auth.userData.userId;

    this.userDetails$ = this.userId == this.auth.userData.userId
      ? of({ name: this.auth.userData.userName /* NOTE: no calories info here */ })
      : this.usersService.getUsers(this.userId).pipe(map(i => i[0] as UserDetailsDto));

    this.loading = true;

    this.userMeals$ =
      this.mealsService
        .loadUserMeals(this.userId, filter)
        .pipe(
          first(),
          finalize(() => { this.loading = false; /* we use ngIf-else, no need for a separate loading flag */ })
        );
  }

  reload() {
    this.loadUserMeals(this.userId, this.filter);
  }

  addMeal() {
    const meal = {
      date: new Date()
    } as MealDailyItemDto;

    this.openMealModal(meal);
  }

  editMeal(meal: MealDailyItemDto) {
    this.openMealModal(meal);
  }

  openMealModal(meal: MealDailyItemDto) {
    const modalRef = this.modalService.open(MealDetailModalComponent);
    modalRef.componentInstance.userId = this.userId;
    modalRef.componentInstance.item = meal;
    modalRef.result.then(
      (closeReason) => {
        this.reload();
      },
      (dismissReason) => {
      }
    );
  }

}
