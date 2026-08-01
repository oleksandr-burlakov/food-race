import { type CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { UnleashService } from '../../services/unleash.service';

export const unleashGuard = (flagName: string, fallbackUrl = '/'): CanActivateFn => {
  return () => {
    const unleash = inject(UnleashService);
    const router = inject(Router);

    if (unleash.isEnabled(flagName)) {
      return true;
    }

    return router.parseUrl(fallbackUrl);
  };
};
