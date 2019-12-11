import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AuthService } from 'src/app/_auth/auth.service';
import { MessageComponent } from '../_message-component';

@Component({
  templateUrl: 'register.component.html'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  submitted = false;

  @ViewChild(MessageComponent, { static: false }) mc: MessageComponent;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authServ: AuthService
  ) {
    // redirect to home if already logged in
    /*
    if (this.authService.isAuthenticated) {
        this.router.navigate(['/']);
    }
    */
  }

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    },
      {
        validator: this.passwordMatchValidator
      });
  }

  get f() { return this.registerForm.controls; }

  passwordMatchValidator(control: AbstractControl) {
    const password = control.get('password').value;
    const confirmPassword = control.get('confirmPassword').value;

    if (password !== confirmPassword)
      //control.get('confirmPassword').setErrors({ confirmPassword: true });
      return { confirmPassword: true };
    else
      return null;
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.registerForm.invalid) {
      return;
    }

    this.loading = true;
    this.authServ.register(this.registerForm.value.username, this.registerForm.value.password)
      .subscribe(
        data => {
          this.router.navigate(['']);
        },
        error => {
          this.loading = false;
          this.mc.error(error.error || "Some error occured", 5000);
          // NOTE: no need to throw from now... throw error;
        });
  }
}
