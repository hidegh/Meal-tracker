import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AppWelcomeComponent } from './app.welcome.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './_auth/auth.guard';
import { LogoutComponent } from './logout/logout.component';

/**
 * NOTE:
 *
 * Auth0 login can be set up in 2 major ways:
 *
 * Either you (version 1, welcome page without layout):
 * - set up an empty (default) route to point to the app.welcome.component
 * - add a canActivate guard to the entire layout-module route
 *
 *   { path: 'callback', component: AppCallbackComponent },
 *   { path: '', loadChildren: () => import('./_layout/layout.module').then(m => m.AppLayoutModule), canActivate: [AuthGuard] }
 *
 * Or you (version 2, default page with layout):
 * - keep the layout-module without canActivate guard
 * - configure the empty (default) route inside the layout-routing
 *   - you can set up to FORCE AUTO LOGIN here, by checking if user is logged in and executing login if not
 *   - or simply leave the redirection to login screen up to the AuthGuard, when hitting a secured route
 * - configure rest of the routes with canActivate: [AuthGuard]
 * - also due using the main layout you will probably end up altering the layout to use *ngIf="auth.loggedIn" on most of the menus
 */
const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'logout',
    component: LogoutComponent
  },
  {
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () => import('./_layout/layout.module').then(m => m.AppLayoutModule)
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: false }) ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
