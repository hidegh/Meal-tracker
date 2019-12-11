import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, distinctUntilChanged, catchError, first } from 'rxjs/operators';

import { JwtHelperService } from '@auth0/angular-jwt';

import { MyHttpClientFactory } from './../_http';

import { environment } from './../../environments/environment';
import { UrlConsts } from './../../environments/url-consts';

@Injectable({ providedIn: 'root' })
export class AuthService {

  public loggedIn = false;

  public userData: any = {};

  protected loggedInStatusChangedSubject$ = new BehaviorSubject<boolean>(this.loggedIn);
  public loggedInStatusChanged$: Observable<boolean> = this.loggedInStatusChangedSubject$.pipe(distinctUntilChanged());

  protected emitLoggedInStatus(loggedIn: boolean) {
    // NOTE: must set value here, as there's no guarantee that the loggedInStatusChanged$ will be subscribed!
    this.loggedIn = loggedIn;
    this.loggedInStatusChangedSubject$.next(loggedIn);
  }

  protected http: HttpClient;
  protected jwtHelper = new JwtHelperService();

  constructor(httpClientFactory: MyHttpClientFactory) {

    this.http = httpClientFactory.createHttpClient(environment.urls[UrlConsts.API], false);

    // if already auth, fetch base user data
    if (this.isAuthenticated && localStorage.user_data)
      this.userData = JSON.parse(localStorage.user_data);
  }

  public login(userName: string, password: string): Observable<any> {

    const payload = new HttpParams()
      .set('userName', userName)
      .set('password', password);

    return this.http
      .post('authentication/login', payload)
      .pipe(
        first(),
        tap((jwt: any) => {
          const token = jwt.token;
          this.setData(token);
          this.emitLoggedInStatus(true);
        })
      );
  }

  public isAuthenticated() {
    const token = localStorage.getItem("access_token");
    const isLoggedIn = token && !this.jwtHelper.isTokenExpired(token);
    this.emitLoggedInStatus(isLoggedIn);
    return isLoggedIn;
  }

  public logout() {
    this.clearData();
    this.emitLoggedInStatus(false);
  }

  public isUserInRole(roleName: string) {
    if (this.userData && this.userData.roles) {
      return this.userData.roles.some(r => r === roleName);
    }

    return false;
  }

  protected setData(token: any) {
    localStorage.setItem("access_token", token);

    const decodedToken = this.jwtHelper.decodeToken(token);

    this.userData.userId =  decodedToken.sub;
    this.userData.userName = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

    const roleDetail = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']

    if (Array.isArray(roleDetail))
      this.userData.roles  = roleDetail;
    else if (roleDetail)
      this.userData.roles = [roleDetail];
    else
      this.userData.roles = [];

    localStorage.setItem("user_data", JSON.stringify(this.userData));
  }

  protected clearData() {
    delete localStorage.access_token;
    delete localStorage.user_data;
    this.userData = {};
  }

  public register(userName: string, password: string): Observable<any> {

    const payload = new HttpParams()
      .set('userName', userName)
      .set('password', password);

    return this.http
      .post('authentication/register', payload)
      .pipe(
        first(),
        tap((jwt: any) => {
          // do also login
          const token = jwt.token;
          this.setData(token);
          this.emitLoggedInStatus(true);
        })
      );

  }

}
