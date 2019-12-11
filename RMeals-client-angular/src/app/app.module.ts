import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule, ErrorHandler } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ToasterModule } from 'angular2-toaster';

import { GlobalErrorHandler } from './app-error-handler.component';

import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpClientModule } from '@ngx-progressbar/http-client';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AppWelcomeComponent } from './app.welcome.component';

import { environment } from '../environments/environment';

import { AUTHENTICATION_INTERCEPTOR_CONFIGURATION_TOKEN, IHttpAuthenticationInterceptorConfiguration, HttpAuthenticationInterceptor, MyHttpClientFactory, Http401RedirectInterceptor, Http403RedirectInterceptor } from './_http';
import { AuthService } from './_auth/auth.service';

import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { LogoutComponent } from './logout/logout.component';
import { MessageModule } from './_message-component';

@NgModule({
  declarations: [
    AppComponent,
    AppWelcomeComponent,

    LoginComponent,
    RegisterComponent,
    LogoutComponent
  ],
  imports: [
    RouterModule,
    BrowserModule,
    BrowserAnimationsModule,

    ToasterModule.forRoot(),

    HttpClientModule,

    AppRoutingModule,

    HttpClientModule,

    NgProgressModule.forRoot(),
    NgProgressHttpClientModule,

    FormsModule,
    ReactiveFormsModule,

    MessageModule
  ],
  providers: [
    { provide: ErrorHandler, useClass: GlobalErrorHandler },

    AuthService,

    {
      provide: AUTHENTICATION_INTERCEPTOR_CONFIGURATION_TOKEN,
      useValue: {
        tokenGetter: () => localStorage.access_token
      } as IHttpAuthenticationInterceptorConfiguration
    },
    HttpAuthenticationInterceptor,
    Http401RedirectInterceptor,
    Http403RedirectInterceptor,

    MyHttpClientFactory
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
