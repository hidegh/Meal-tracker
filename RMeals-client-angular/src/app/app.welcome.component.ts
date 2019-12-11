import { Component, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './_auth/auth.service';

@Component({
  selector: 'ks-app-welcome',
  template: `
    <div class="container h-100 d-flex justify-content-center">
      <div class="jumbotron my-auto">
        <h1 class="display-3">Welcome to R-Meals</h1>
      </div>
    </div>
  `
})
export class AppWelcomeComponent {

  constructor(
    private router: Router,
    private auth: AuthService
    ) {
      // NOTE:
      // Maybe some autoLogin on the AuthService side if token is in the localStore and still vaid?
      // Otherwise we could initially route to login!
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 1000);
  }

}
