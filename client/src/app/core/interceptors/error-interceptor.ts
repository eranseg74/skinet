import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar-service';

// Note that the interceptor gets the request as a parameter and forwards it to the relevant component using the next() function. Everything before the return statement will be executed before forwarding the request to the server, and everything after the return statement will be executed on the returned response before it reaches the component.
// Since this is not a regular class we declare variables using let or const
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  // Injecting the Router so we can navigate or send the user to another URL
  const router = inject(Router);
  const snackbar = inject(SnackbarService);
  // The next function returns an Observable. We do not want to subscribe to the Observable but we do want to manipulate or do something with it so we use the pipe() function that can receive rxjs functions such as the catchError and throwError
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 400) {
        // Checking if the error has an errors propery which is an array of errors (such as in the validation error) or is just an error object that contains some properties such as the status, title, etc. If there is an errors array then we put all the errors in the modelStateErrors array so we will be able to display them in a dedicated component. otherwise we will toast it as usual.
        if (err.error.errors) {
          const modelStateErrors = [];
          for (const key in err.error.errors) {
            if (err.error.errors[key]) {
              modelStateErrors.push(err.error.errors[key]);
            }
          }
          // Throwing the modelStateErrors back to the component. Also flatening the array so we will have a single flat array of strings. This is important because some values can be an array on their own.
          throw modelStateErrors.flat();
        } else {
          snackbar.error(err.error.title || err.error);
        }
      }
      if (err.status === 401) {
        snackbar.error(err.error.title || err.error);
      }
      if (err.status === 404) {
        router.navigateByUrl('/not-found');
      }
      if (err.status === 500) {
        // If we want to pass some data along with the route to the component that is the activated route we can define navigation extras as follows:
        const navigationExtras: NavigationExtras = { state: { error: err.error } };
        // And then pass the path and the navigation extra
        // NOTE that the component will be able to access this data only from the constructor!!!
        router.navigateByUrl('/server-error', navigationExtras);
      }
      return throwError(() => err);
    })
  );
};
