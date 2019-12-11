import { ErrorHandler, Injectable, Injector, ApplicationRef, NgZone } from '@angular/core';

import {ToasterService, Toast} from "angular2-toaster";

@Injectable()
export class GlobalErrorHandler extends ErrorHandler {

  private counter: number = 0;

  constructor(
    public toaster: ToasterService,
    public injector: Injector,
    public zone: NgZone
  ) {
    super();
  }

  handleError(error) {

    console.log('GLOBAL ERROR OCCURED!')

    let counter = ++this.counter;

    const title = 'Oops! An error occured. (#' + counter + ')';
    const message = error.message;

    // Error obj.:
    // - error.message
    // - error.stack()
    const toast: Toast = {
      type: 'error',
      title: title,
      body: message,
      showCloseButton: true,
      onShowCallback: (toast) => {
        // console log...
        console.log("error (#" + counter + "): ", message);
        // ...or log via logger (where we log the error object)
        // this.logger.error(`Error #: ${counter}`, error);
      }
    };

    super.handleError(error);

    this.zone.run(() => this.toaster.pop(toast));

    /*
     Rethrowing the error in the GlobalErrorHandler leads to an uncaught exception where the browser stops execution. That's why it works only once.
     The solution is simply to not rethrow in the ErrorHandler.

     More detail: In this case there is no onError-function provided in subscribe(). That's why rxjs/Subscriber.js runs into the catch-part of __tryOrUnsub. It tries to execute the onError-function but fails (because there is none) and therefore unsubscribes from the Observable and throws the error. The GlobalErrorHandler picks it up, does logging and notification and then rethrows it.
     At this point the error becomes an uncaught exception outside of Angular. (the default ErrorHandler does not rethrow an error)
     */
  }

}
