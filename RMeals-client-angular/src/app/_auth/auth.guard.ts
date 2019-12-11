import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

import { AuthService } from './auth.service';
import { isatty } from 'tty';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authService: AuthService
    ) {

    }

    canActivate(
      route: ActivatedRouteSnapshot,
      state: RouterStateSnapshot
      ): boolean | UrlTree | Promise<boolean | UrlTree> | Observable<boolean | UrlTree> {
        const isAuth = this.authService.isAuthenticated();

        if (!isAuth)
          this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });

        return isAuth;
    }
}


