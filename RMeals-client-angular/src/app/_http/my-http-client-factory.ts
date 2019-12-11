import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { MyHttpHandler } from './my-http-extensions';
import { HttpBaseUrlAppenderInterceptor, HttpAuthenticationInterceptor, Http401RedirectInterceptor, Http403RedirectInterceptor } from './interceptors';

export enum ResponseCasingEnum {
  Default,
  CamelCase,
  PascalCase
}

@Injectable()
export class MyHttpClientFactory {

  constructor(
    private backend: HttpBackend,
    private interceptor401: Http401RedirectInterceptor,
    private interceptor403: Http403RedirectInterceptor,
    private authenticationInterceptor: HttpAuthenticationInterceptor
    ) {
  }

  createHttpClient(baseUrl: string, auth: boolean = true) {

    const interceptors = [];

    if (baseUrl)
      interceptors.push(new HttpBaseUrlAppenderInterceptor(baseUrl));

    if (auth)
      interceptors.push(this.authenticationInterceptor);

    interceptors.push(this.interceptor401);
    interceptors.push(this.interceptor403);

    const handler = new MyHttpHandler(this.backend, interceptors);
    const client = new HttpClient(handler);
    return client;
  }

}
