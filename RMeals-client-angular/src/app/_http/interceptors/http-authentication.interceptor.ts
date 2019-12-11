import { Optional, InjectionToken, Inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';

export const AUTHENTICATION_INTERCEPTOR_CONFIGURATION_TOKEN = new InjectionToken<IHttpAuthenticationInterceptorConfiguration>('AUTHENTICATION_INTERCEPTOR_CONFIGURATION_TOKEN');

export interface IHttpAuthenticationInterceptorConfiguration {
  tokenGetter: () => string;
}

type Nil = undefined | null;
const isNil = (x: any): x is Nil => x === null || x === undefined;

@Injectable()
export class HttpAuthenticationInterceptor implements HttpInterceptor {

  constructor(
    @Optional() @Inject(AUTHENTICATION_INTERCEPTOR_CONFIGURATION_TOKEN) private config: IHttpAuthenticationInterceptorConfiguration
    ) {

      this.config = this.config || {} as IHttpAuthenticationInterceptorConfiguration;
      if (isNil(this.config.tokenGetter)) this.config.tokenGetter = () => localStorage.access_token;
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${this.config.tokenGetter()}`
      }
    });

    // TODO: ensure logout on 401
    // check if authentication token is valid; if not, the auth-service should emit a loggedIn change, which should listen to login page redirection.

    return next.handle(req);
  }

}
