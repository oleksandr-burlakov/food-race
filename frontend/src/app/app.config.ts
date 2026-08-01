import {
  type ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { UnleashService } from '../shared/services/unleash.service';

export function initializeUnleash() {
  const unleashService = inject(UnleashService);
  return new Promise<void>((resolve) => {
    const timeout = setTimeout(() => resolve(), 2000);

    if (unleashService.isReady()) {
      clearTimeout(timeout);
      resolve();
    } else {
      const checkReady = setInterval(() => {
        if (unleashService.isReady()) {
          clearInterval(checkReady);
          clearTimeout(timeout);
          resolve();
        }
      }, 50);
    }
  });
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideAppInitializer(initializeUnleash),
    provideRouter(routes),
  ],
};
