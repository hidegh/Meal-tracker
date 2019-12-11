import { HttpInterceptor, HttpEvent, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';

export class HttpBaseUrlAppenderInterceptor implements HttpInterceptor {

  constructor(private baseUrl: string) {
    this.baseUrl = this.baseUrl.replace(/\/$/, ""); // trim end slash of base url
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const urlPath = req.url.replace(/^\/+/, ''); // trim starting slashes of the partial url
    req = req.clone({
      // `http://i.imgur.com/ETO2miA.jpg`
      // `${this.baseUrl}/${urlPath}`
      url: `${this.baseUrl}/${urlPath}`
    });
    return next.handle(req);
  }
}
