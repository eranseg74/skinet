import { Directive, effect, inject, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account-service';

// The purpose of this directive is to display an element according to a certain condition. In the project we want to display the ADMIN link in the header only if the user has an admin role.
@Directive({
  selector: '[appIsAdmin]', // We write on the element that we want to apply the directive on -> *appIsAdmin
})
export class IsAdmin {
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  constructor() {
    effect(() => {
      if (this.accountService.isAdmin()) {
        // The createEmbeddedView allows the element to be displayed in the user's browser. The templateRef is the template that is going to be displayed and this is the element that we will put the directive on.
        // We use the view container to display the template, and we use the directive on the element in the HTML when we want this to be checked
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainerRef.clear(); // If the element was displayed, remove it
      }
    });
  }

  // This code will be executed when the directive is initialized but we will not get notified of changes. For that we would need to use an observable or a signal with a computed value. The isAdmin in the account service is a computed signal that will notify us of changes in the user roles but it will nor work when we log in and out. Just after refreshing the browser. To overcome this we can use the effect() function in the account service constructor to notify us of changes in the isAdmin computed signal. We will implement it this way for simplicity.
  /* DOES NOT WORK PROPERLY
  ngOnInit(): void {
    if (this.accountService.isAdmin()) {
      // The createEmbeddedView allows the element to be displayed in the user's browser. The templateRef is the template that is going to be displayed and this is the element that we will put the directive on.
      // We use the view container to display the template, and we use the directive on the element in the HTML when we want this to be checked
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear(); // If the element was displayed, remove it
    }
  }
  */
}
