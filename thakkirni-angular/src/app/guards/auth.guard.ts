import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn) {
    return true;
  }

  return router.createUrlTree(['/login']);
};

export const adminGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn && authService.isAdmin()) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};
<<<<<<< HEAD

export const managerGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn && authService.isAdminOrManager()) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
