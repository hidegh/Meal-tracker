//
// Based on: http://www.learn-angular.fr/how-can-we-have-multiple-instances-of-httpclient-instance-with-angular/
//

import { HttpHandler, HttpInterceptor, HttpRequest, HttpEvent, HttpBackend } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injector, InjectionToken } from '@angular/core';

export class MyHttpInterceptorHandler implements HttpHandler {

  constructor(private next: HttpHandler, private interceptor: HttpInterceptor) {}

  handle(req: HttpRequest<any>): Observable<HttpEvent<any>> {
    return this.interceptor.intercept(req, this.next);
  }
}

/**
 * Special HttpBackend (handler) which accepts a list of HttpInterceptor (interfaces) - so injection is currently not supported for it.
 */
export class MyHttpHandler implements HttpHandler {

  private chain: HttpHandler | null = null;

  constructor(private backend: HttpBackend, private interceptors: HttpInterceptor[]) { }

  handle(req: HttpRequest<any>): Observable<HttpEvent<any>> {
    if (this.chain === null) {
      this.chain = this.interceptors.reduceRight(
        (next, interceptor) => new MyHttpInterceptorHandler(next, interceptor), this.backend);
    }
    return this.chain.handle(req);
  }
}

/**
 * USAGE SAMPLE:
 *
 * export const MY_HTTP_INTERCEPTORS = new InjectionToken<HttpInterceptor[]>('MY_HTTP_INTERCEPTORS');
 *
 * @Injectable()
 * export class Http1Service extends HttpClient {
 *   constructor(backend: HttpBackend, private injector: Injector) {
 *     super(new MyHandlerService(backend, injector, HTTP_INTERCEPTORS));
 *   }
 * }
 */
export class MyHttpTokenInjectionHandler implements HttpHandler {

  private chain: HttpHandler | null = null;

  constructor(private backend: HttpBackend, private injector: Injector, private interceptors: InjectionToken<HttpInterceptor[]>) { }

  handle(req: HttpRequest<any>): Observable<HttpEvent<any>> {
    if (this.chain === null) {
      const interceptors = this.injector.get(this.interceptors, []);
      this.chain = interceptors.reduceRight(
        (next, interceptor) => new MyHttpInterceptorHandler(next, interceptor), this.backend);
    }
    return this.chain.handle(req);
  }
}
