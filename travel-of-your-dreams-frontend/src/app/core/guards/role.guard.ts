import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export const roleGuard = (roles: string[]): CanActivateFn => () => {
  const auth = inject(AuthService);
  if (roles.some((role) => auth.hasRole(role))) return true;
  return inject(Router).parseUrl('/');
};
