import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';

export interface CanDeactivateComponent {
  canDeactivate: () => boolean;
}

@Injectable()
export class CanDeactivateComponentGuardService implements CanDeactivate<CanDeactivateComponent> {

  constructor() { }

canDeactivate(component: CanDeactivateComponent): boolean {
    return component.canDeactivate ? component.canDeactivate() : true;
  }
}
