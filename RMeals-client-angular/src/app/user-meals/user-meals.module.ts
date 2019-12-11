import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SpinnerModule } from './../_spinner/spinner.module';
import { MessageModule } from './../_message-component';

import { MealsService } from '../_services/meals-service';

import { UserMealsComponent } from './user-meals.component';

import { MealListFilterComponent } from './meal-list-filter/meal-list-filter.component';

import { MealListComponent } from './meal-list/meal-list.component';
import { MealDailySummaryComponent } from './meal-list/meal-daily-summary.component';
import { MealDailyMealsComponent } from './meal-list/meal-daily-meals.component';

import { MealDetailModalComponent } from './meal-detail/meal-detail.modal.component';

import { UsersService } from '../_services/users-service';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

const routes: Routes = [
  {
    path: '',
    children: [
      { path: ':id', component: UserMealsComponent },
      { path: '**', component: UserMealsComponent }
    ]
  }
];

@NgModule({
  declarations: [
    UserMealsComponent,

    MealListFilterComponent,

    MealListComponent,
    MealDailySummaryComponent,
    MealDailyMealsComponent,

    MealDetailModalComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,

    SpinnerModule,
    MessageModule,

    RouterModule.forChild(routes),

    NgbModule
  ],
  providers: [
    MealsService,
    UsersService
  ],
  entryComponents: [
    MealDetailModalComponent
  ]
})
export class UserMealsModule { }
