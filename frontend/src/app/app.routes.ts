import { type Routes } from '@angular/router';
import { unleashGuard } from '../shared/guards/unleash.guard';

export const routes: Routes = [
  {
    path: 'landing',
    loadComponent: () =>
      import('./pages/landing/landing.component').then((m) => m.LandingComponent),
  },
  {
    path: 'auth',
    canActivate: [unleashGuard('authentication', '/')],
    loadComponent: () => import('./pages/auth/auth.component').then((m) => m.AuthComponent),
  },
  {
    path: '**',
    redirectTo: 'landing',
  },
];
