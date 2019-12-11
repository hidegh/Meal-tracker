import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from './../_auth/auth.service';

declare var $: any;

@Component({
  selector: 'ks-layout',
  templateUrl: './layout.component.html',
  styleUrls: [
    // kendo and custom variables
    './../../scss/variables.scss',
    // main layout (no branding)
    './../../scss/layout/main.scss',
    './../../scss/layout/header.main.scss',
    './../../scss/layout/sidebar.main.scss',
    // sidebar customization (branding)
    './../../scss/layout/sidebar.bs.scss',
    // keep this for tests...
    './layout.component.scss',
  ],
  encapsulation: ViewEncapsulation.None
})
export class LayoutComponent implements OnInit {

  autoLogin: boolean = false;

  routes: any[];

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private auth: AuthService
    ) {

  }

  ngOnInit(): void {
    // Auto remove statically assigned open state if viewport is too small (e.g. when we start on a mobile)
    const vw = document.body.clientWidth;

    if (vw < 720) {
      $('.sidebar-left, .sidebar-right').removeClass('open');
    }

    if (this.autoLogin) {
      if (this.auth.loggedIn === false)
        this.router.navigate(['/login']);
    }
  }

  /** If there are no lazy-loaded modules or if the routes are configured on one place, we can fetch routes: this.routes = this.fetchRoutes(router.config); */
  fetchRoutes(routes: any[], path?: string): any[] {
    return routes.map(item => {
        const result: any = {
            text: item.text,
            path: (path ? `${ path }/` : '') + item.path
        };

        if (item.children) {
            result.items = this.fetchRoutes(item.children, item.path);
        }

        return result;
    });
}

}
