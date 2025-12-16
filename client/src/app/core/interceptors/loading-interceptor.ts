import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { BusyService } from '../services/busy-service';
import { environment } from '../../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);
  busyService.busy();
  // When we are in production we do not want the delay so we are checking if the environment is production. If so we use the identity function which does nothing
  return next(req).pipe(
    environment.production ? identity : delay(500),
    finalize(() => busyService.idle())
  );
};
