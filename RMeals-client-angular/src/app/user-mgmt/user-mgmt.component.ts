import { Component } from '@angular/core';

import { Observable } from 'rxjs';

import { UserDetailsDto, UsersService } from '../_services/users-service';
import { AuthService } from '../_auth/auth.service';
import { RoleConsts } from '../role-consts';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserRolesModalComponent } from './user-roles/user-roles.modal.component';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: 'user-mgmt.component.html'
})
export class UserMgmtComponent {

  loading: boolean = false;
  userList$: Observable<UserDetailsDto[]>;

  get isAdmin(): boolean { return this.auth.isUserInRole(RoleConsts.Admin); }

  constructor(
    public auth: AuthService,
    public usersService: UsersService,
    public modalService: NgbModal
    ) {

    this.loading = true;
    this.userList$ = this.usersService.getUsers().pipe(finalize(() => this.loading = false));
  }

  openRoleManagementModal(user: UserDetailsDto) {
    const modalRef = this.modalService.open(UserRolesModalComponent);
    modalRef.componentInstance.user = user;
    modalRef.result.then(
      (closeReason) => {
      },
      (dismissReason) => {
      }
    );
  }

}
