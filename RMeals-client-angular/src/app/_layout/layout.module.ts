// Angular
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Custom - layout
import { LayoutRoutingModule } from './layout-routing.module';
import { LayoutComponent } from './layout.component';

// Custom - menu
import { MenuComponent } from './menu/menu.component';

import { HeaderComponent } from './header/header.component';

// Custom - header

// Custom - footer

@NgModule({
  declarations: [
    // Layout
    LayoutComponent,

    // Header
    HeaderComponent,

    // Menu
    MenuComponent,

    // Footer

    //
    // Here we add new modules

  ],
  imports: [

    //
    // Basic angular related imports
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,

    //
    // Main things (as routing) first
    LayoutRoutingModule,

    //
    // Other modules (from node_modules)

  ],
  providers: []
})
export class AppLayoutModule { }
