import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Routes, RouterModule } from '@angular/router';

import { WelcomeComponent } from './welcome.component';

const routes: Routes = [
      {
        path: '',
        children: [
          { path: '', component: WelcomeComponent }
        ]
      }

];

@NgModule({
  declarations: [
    WelcomeComponent
  ],
  imports: [
    // NOTE: inline route definition without an extra module (also keep it as last or at least below CommonModule)
    RouterModule.forChild(routes),

    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: []
})
export class WelcomeModule { }
