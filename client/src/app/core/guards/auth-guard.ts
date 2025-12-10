import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account-service';
import { map, of } from 'rxjs';

// Because we chose the CanActivate choice in the guard creation we see that this class implements the CanActivateFn function. It gets a route which is the activated route snapshot, and the state which is the router snapshot.
export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  // Here we are checking for the currentUser which is a signal. A signal has synchroneous functionality which in this case creates a timing issue because when I am logging in, the current user is set initially to null so we are being redirected back to the login page. Once we have the current user it is too late and we are already redirected to the login component. The solution is using Observables which are anynchronous functions so the call will wait for the currentUser() to finish and only then decide whether to continue to the application or return the user to the login component
  if (accountService.currentUser()) {
    // We don't need to subscribe to the of observable because it is a guard and the guard is responsible for subscribing and unsubscribing automatically. We junt need to make sure we return an Observable and the guard will wait for these observables to complete before continuing
    return of(true);
  } else {
    return accountService.getAuthState().pipe(
      map((auth) => {
        if (auth.isAuthenticated) {
          return true; // This will allow the user to continue to the requested route. This is an Observable because we are inside a pipe
        } else {
          // Adding additional parametrers to the query params so once a user is logged in they will have the returnUrl property which they can return to if they are blocked by the Auth guard
          router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
          return false;
        }
      })
    );
  }
};
