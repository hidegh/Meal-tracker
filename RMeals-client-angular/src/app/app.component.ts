import { Component, ViewEncapsulation } from '@angular/core';

import {ToasterConfig, BodyOutputType} from "angular2-toaster";

import { environment } from '../environments/environment';

const variable = require('./_json/json-date-ex');

@Component({
  selector: 'ks-root',
  templateUrl: './app.component.html',
  styleUrls: ['./../scss/styles.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent {

  title = 'kitchen-sink';
  env = environment;

  toasterConfig: ToasterConfig = new ToasterConfig({
    positionClass: 'toast-bottom-right',
    limit: 0,
    preventDuplicates: false,
    timeout: { success: 4000, info: 2000, warning: 4000, error: 10000 },
    animation: 'fade',
    tapToDismiss: false,
    showCloseButton: true,
    closeHtml: '<button class="btn btn-sm p-0">x</button>',
    // issue-feature ench.: progressBar: true,
    bodyOutputType: BodyOutputType.Default
  });

  constructor() {
    (<any>JSON).useDateStringifyMode = 2;
  }

}
