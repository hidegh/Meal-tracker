import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SpinnerModule } from './../_spinner/spinner.module';

import { UserProfileComponent } from './user-profile.component';
import { UsersService } from '../_services/users-service';
import { MessageModule } from './../_message-component';
import { NumberRangeNativeValidatorModule } from '../_directives/number-range-native-validator.module';

const routes: Routes = [
  {
    path: '',
    children: [
      { path: ':id', component: UserProfileComponent },
      { path: '**', component: UserProfileComponent }
    ]
  }
];

@NgModule({
  declarations: [
    UserProfileComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,

    SpinnerModule,
    MessageModule,

    NumberRangeNativeValidatorModule,

    RouterModule.forChild(routes)
  ],
  providers: [
    UsersService
  ]
})
export class UserProfileModule { }
