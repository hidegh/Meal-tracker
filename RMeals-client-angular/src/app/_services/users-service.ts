import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { map, first } from 'rxjs/operators';

import { environment } from './../../environments/environment';
import { UrlConsts } from './../../environments/url-consts';

import { MyHttpClientFactory } from './../_http';

@Injectable()
export class UsersService {

  http: HttpClient;

  constructor(httpClientFactory: MyHttpClientFactory) {
    this.http = httpClientFactory.createHttpClient(environment.urls[UrlConsts.API], true);
  }

  public loadUserProfile(userId: number): Observable<UserProfileDto> {
    return this.http.get(`/users/${userId}/profile`)
      .pipe(
        first(),
        map(data => {
          return data as UserProfileDto;
        })
      );
  }

  public saveUserProfile(userId: number, profile: UserProfileDto): Observable<UserProfileDto> {
    return this.http.put(`/users/${userId}/profile`, profile)
      .pipe(
        first(),
        map(data => {
          return data as UserProfileDto;
        })
      );
  }

  public getUsers(userId?: number): Observable<UserDetailsDto[]> {

    const myParams: any = {};
    if (userId) myParams.userId = userId;

    return this.http.get(`/users`, { params: myParams })
      .pipe(
        first(),
        map(data => {
          return data as UserDetailsDto[];
        })
      );
  }

  public getUserRoles(userId: number): Observable<string[]> {

    return this.http.get(`/users/${userId}/roles`)
      .pipe(
        first(),
        map(data => {
          return data as string[];
        })
      );
  }

  public updateUserRoles(userId: number, roles: string[]): Observable<any> {
    return this.http.put(`/users/${userId}/roles`, roles).pipe(first());
  }

}

export class UserProfileDto {
  allowedCalories: number;
}

export class UserDetailsDto {
  id: number;
  name: string;
  allowedCalories: number;
}
