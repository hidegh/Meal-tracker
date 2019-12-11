import { Component, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { first, finalize, map } from 'rxjs/operators';

import { AuthService } from './../_auth/auth.service';
import { Subscription, pipe, Observable, of } from 'rxjs';

import { MessageComponent } from './../_message-component';

import { UsersService, UserProfileDto, UserDetailsDto } from '../_services/users-service';

/*
There's an auto unsubscribe directive in NPM!
*/
@Component({
  templateUrl: 'user-profile.component.html'
})
export class UserProfileComponent implements OnDestroy {

  loading = true;
  updating = false;

  userId: any;
  profile: UserProfileDto = { } as UserProfileDto;

  paramsSubscription: Subscription;

  userDetails$: Observable<any | UserDetailsDto>;

  @ViewChild(MessageComponent, { static: false }) mc: MessageComponent;

  constructor(
    private route: ActivatedRoute,
    private auth: AuthService,
    private usersService: UsersService
  ) {
    this.paramsSubscription = this.route.params.subscribe(params => this.loadUserProfile(params.id));
  }

  ngOnDestroy() {
    if (this.paramsSubscription)
      this.paramsSubscription.unsubscribe();
  }

  loadUserProfile(userId?: number) {
    this.userId = userId || this.auth.userData.userId;

    this.userDetails$ = this.userId == this.auth.userData.userId
      ? of({ name: this.auth.userData.userName /* NOTE: no calories info here */ })
      : this.usersService.getUsers(this.userId).pipe(map(i => i[0] as UserDetailsDto));

    this.usersService
      .loadUserProfile(this.userId)
      .pipe(
        finalize(() => this.loading = false)
      )
      .subscribe(data => { this.profile = data; });
  }

  saveUserProfile() {
    this.updating = true;

    this.usersService
      .saveUserProfile(this.userId, this.profile)
      .pipe(
        finalize(() => this.updating = false)
      )
      .subscribe(
        success => this.mc.success("Update was succesful.", 2500),
        httpError =>  this.mc.error(`Update failed with errror: ${httpError.error.title}, errors: ${JSON.stringify(httpError.error.errors)}`, 2500)
      );
  }

  onSubmit() {
   this.saveUserProfile();
  }

}
