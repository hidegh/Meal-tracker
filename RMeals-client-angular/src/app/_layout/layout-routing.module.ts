import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from './layout.component';
import { AuthGuard } from './../_auth/auth.guard';

// NOTE:
// Children[] should contain just the routes to the main (lazy loaded) component modules here!
// Each of those components is then itself responsible for defining it's own sub-routes!
const routes: Routes = [

  /**
   * This is the default layout, lazy-loaded components should be defined as children inside (to keep layout)!
   * For multi layout sample see: https://stackblitz.com/edit/angular-multi-layout-example
   */
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', redirectTo: 'welcome' },
      { path: 'welcome', loadChildren: () => import('../welcome/welcome.module').then(m => m.WelcomeModule) },
      { path: 'user-profile', loadChildren: () => import('../user-profile/user-profile.module').then(m => m.UserProfileModule) },
      { path: 'user-meals', loadChildren: () => import('../user-meals/user-meals.module').then(m => m.UserMealsModule) },
      { path: 'user-mgmt', loadChildren: () => import('../user-mgmt/user-mgmt.module').then(m => m.UserMgmtModule) },
    ]
  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule {
}
