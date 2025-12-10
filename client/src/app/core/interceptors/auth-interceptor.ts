import { HttpInterceptorFn } from '@angular/common/http';

// Note that we will need to add the interceptor to the interceptors array in the app.config.ts file
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // The provided request (req) is immutable so we need to clone it. We can add to the cloned object additional properties like the withCredentials that is applied here
  const clonedRequest = req.clone({
    withCredentials: true,
  });
  return next(clonedRequest);
};
