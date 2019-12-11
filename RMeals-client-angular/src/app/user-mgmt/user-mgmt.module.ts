import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SpinnerModule } from './../_spinner/spinner.module';

import { UserMgmtComponent } from './user-mgmt.component';
import { UsersService } from '../_services/users-service';

import { MessageModule } from './../_message-component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { UserRolesModalComponent } from './user-roles/user-roles.modal.component';

const routes: Routes = [
  {
    path: '',
    children: [
      { path: '**', component: UserMgmtComponent }
    ]
  }
];

@NgModule({
  declarations: [
    UserMgmtComponent,
    UserRolesModalComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,

    SpinnerModule,
    MessageModule,

    RouterModule.forChild(routes),

    NgbModule
  ],
  providers: [
    UsersService
  ],
  entryComponents: [
    UserRolesModalComponent
  ]
})
export class UserMgmtModule { }
