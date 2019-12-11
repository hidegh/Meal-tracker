import { Component } from '@angular/core';
import { AuthService } from 'src/app/_auth/auth.service';
import { RoleConsts } from 'src/app/role-consts';

@Component({
  selector: 'ks-menu',
  templateUrl: 'menu.component.html',
  styles: [
    `
    ul {
      padding:0
    }

    ul li.active a {
      color: white;
    }

    ul li a i {
      margin-right: 0.5rem;
    }
    `
  ]
})
export class MenuComponent {

  // NOTE: previously I had the expressions inside HTML
  roleConsts = RoleConsts;

  get isUser() {
    return !this.auth.isUserInRole(RoleConsts.Admin) && !this.auth.isUserInRole(RoleConsts.Manager);
  }

  get canManage() {
    return this.auth.isUserInRole(RoleConsts.Admin) || this.auth.isUserInRole(RoleConsts.Manager);
  }

  constructor(
    public auth: AuthService
  ) {

  }

}
