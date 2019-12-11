import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AuthService } from 'src/app/_auth/auth.service';

@Component({
  template: `
  `
})
export class LogoutComponent {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
