import { Component, Input } from '@angular/core';

@Component({
  selector: 'ks-spinner',
  template: `
    <div *ngIf="loading" class="spinner-backdrop h-100 w-100 d-flex">
      <div class="my-auto mx-auto spinner-content">
          <span class=" spinner-icon"><i class="fas fa-circle-notch fa-spin"></i></span>
          <span class="spinner-text">{{text}}</span>
      </div>
    </div>
  `,
  styles: [
    `
    :host {
      height: 100% !important;
      width: 100% !important;
    }

    .spinner-backdrop {
      position: relative;
      top: 0;
      right: 0;
      bottom: 0;
      left: 0;
      z-index: 99999;
    }

    .spinner-backdrop {
      background-image: radial-gradient(ellipse closest-side, rgba(0, 0, 0, .15), rgba(0, 0, 0, .03));
      box-shadow: 0 0  1rem 1rem rgba(0, 0, 0, .03);
    }

    .spinner-content {
      color: midnightblue;
      color: mediumvioletred;
      text-shadow: .1rem .1rem white;
    }

    .spinner-content > * {
      display: block;
      text-align: center;
    }

    .spinner-icon {
      font-size: 6rem;
      padding-right: 2rem;
    }

    .spinner-text
    {
      font-size: 4rem;
      display: block;
    }

    `
  ]
})
export class SpinnerComponent {

  @Input() loading = false;
  @Input() text = "Loading...";

}
