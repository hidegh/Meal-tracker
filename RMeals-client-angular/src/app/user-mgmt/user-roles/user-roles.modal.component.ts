import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UsersService, UserDetailsDto } from '../../_services/users-service';
import { Observable } from 'rxjs';
import { RoleConsts } from '../../role-consts';
import { tap } from 'rxjs/operators';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'ks-user-roles-modal',
  templateUrl: 'user-roles.modal.component.html'
})
export class UserRolesModalComponent implements OnInit {

  @Input() user: UserDetailsDto;

  public loading: boolean = false;
  public updating: boolean  = false;

  public roleConsts = RoleConsts;
  public userRolesList$: Observable<string[]>;

  @ViewChild("f", { static: false }) form: NgForm;

  constructor(
    public modal: NgbActiveModal,
    public usersService: UsersService
    ) {

  }

  ngOnInit() {
    this.loading = true;
    this.userRolesList$ = this.usersService.getUserRoles(this.user.id).pipe(tap(_ => this.loading = false ));
  }

  containsRole(roles: string[], role: string): boolean {
    roles = roles || [];
    return roles.some(r => r === role);
  }

  update() {
    const roles = [];
    if (this.form.value.roleManager) roles.push(RoleConsts.Manager);
    if (this.form.value.roleAdmin) roles.push(RoleConsts.Admin);

    this.updating = true;
    this.usersService.updateUserRoles(this.user.id, roles).pipe(tap(_ => this.updating = false )).subscribe(success => this.modal.close());
  }
}
