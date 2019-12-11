import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './../../_auth/auth.service';

@Component({
  selector: 'ks-header',
  templateUrl: './header.component.html'
})
export class HeaderComponent {
  title: string = "R-Meals";

  constructor(
    public router: Router,
    public auth: AuthService
    ) {

  }

  login() {
    this.router.navigate(['/login']);
  }

  logout() {
    this.router.navigate(['/logout']);
  }

}
