import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error-interceptor';
import { loadingInterceptor } from './core/interceptors/loading-interceptor';
import { InitService } from './core/services/init-service';
import { lastValueFrom } from 'rxjs';
import { MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';

// This function will run on app initialization (see the app.config.ts file). It will run the init function which gets the cart id from the local storage and returns an observable with the cart. We are using the finally option to indicate that when we get a response from the promise we can display the cart so we will remove the splach class. Since we don't want to subscribe to the observable but just execute it once, we use the lastValueFrom (firstValueFrom is also Ok) because we are getting one value from the service anyway

export const appConfig: ApplicationConfig = {
  // Note that if implementing interceptors they should be added to the provideHttpClient function. In this case this function will get a function - 'withInterceptors()' that receives an array with all the implemented interceptors
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor, loadingInterceptor])),
    // The provided function is injected at application startup and executed during app initialization. If the function returns a Promise or an Observable, initialization does not complete until the Promise is resolved or the Observable is completed.
    provideAppInitializer(async () => {
      const initService = inject(InitService);
      return lastValueFrom(initService.init()).finally(() => {
        const splash = document.getElementById('initial-splash');
        if (splash) {
          splash.remove();
        }
      });
    }),
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { autoFocus: 'dialog', restoreFocus: true },
    },
  ],
};
