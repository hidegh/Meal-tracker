import { Component } from '@angular/core';

@Component({
  selector: 'ks-welcome-main',
  template: `
  <div class="container h-100 d-flex justify-content-center">
    <div class="jumbotron my-auto">
      <h1 class="display-3">{{title}}</h1>
    </div>
  </div>
  `
})
export class WelcomeComponent {
  title = 'Welcome to R-Meals!';
}
