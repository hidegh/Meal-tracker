import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { map, first } from 'rxjs/operators';

import { environment } from './../../environments/environment';
import { UrlConsts } from './../../environments/url-consts';

import { MyHttpClientFactory } from './../_http';

@Injectable()
export class MealsService {

  http: HttpClient;

  constructor(httpClientFactory: MyHttpClientFactory) {
    this.http = httpClientFactory.createHttpClient(environment.urls[UrlConsts.API], true);
  }

  public loadUserMeals(userId: number, filter: LoadUserMealsFilter = {}): Observable<MealDailySummaryDto[]> {

    const myParams: any = {};
    if (filter.dateFrom !== undefined) myParams.dateFrom = filter.dateFrom.toISOString();
    if (filter.dateTo !== undefined) myParams.dateTo = filter.dateTo.toISOString();
    if (filter.timeFrom) myParams.timeFrom = filter.timeFrom;
    if (filter.timeTo) myParams.timeTo = filter.timeTo;

    return this.http.get(`/users/${userId}/meals`, { params: myParams })
      .pipe(
        first(),
        map(data => data as MealDailySummaryDto[])
      );
  }

  public deleteUserMeal(userId: number, mealId: number): Observable<any> {
    return this.http.delete(`/users/${userId}/meals/${mealId}`)
      .pipe(first());
  }

  public updateUserMeal(userId: number, mealId: number, mealDto: MealDto): Observable<any> {
    return this.http.put(`/users/${userId}/meals/${mealId}`, mealDto)
      .pipe(first());
  }

  public createUserMeal(userId: number, mealDto: MealDto): Observable<number> {
    return this.http.post(`/users/${userId}/meals`, mealDto)
      .pipe(
        first(),
        map(id => id as number)
      );
  }

}

export interface LoadUserMealsFilter {
   dateFrom?: Date;
   dateTo?: Date;
   timeFrom?: string;
   timeTo?: string;
}

export class MealDailySummaryDto {
  day: Date;
  mealsCount: number;
  dailyCaloriesConsumed: number;
  dailyCaloriesExceeded: boolean;

  meals: MealDailyItemDto[];
}

export class MealDailyItemDto {
  id: number;
  date: Date;
  calories: number;
  description: string;
}

export class MealDto {
  date: Date;
  calories: number;
  description: string;
}
