import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  if (isPublicRequest(request.method, request.url)) return next(request);

  const token = inject(AuthService).token;
  if (!token) return next(request);
  return next(request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};

function isPublicRequest(method: string, url: string) {
  const path = parsePath(url);
  const verb = method.toUpperCase();

  if (verb === 'GET' && (
    path.startsWith('/api/v1/atracciones') ||
    path.startsWith('/api/v1/tickets') ||
    path.startsWith('/api/v1/resenias') ||
    path.startsWith('/api/v2/atracciones') ||
    path.startsWith('/api/v2/reservas')
  )) {
    return true;
  }

  if (verb === 'POST' && (
    path === '/api/v1/auth/login' ||
    path === '/api/v1/auth/register' ||
    path === '/api/v1/admin/auth/login' ||
    path === '/api/v1/integrations/booking/token' ||
    path === '/api/v2/reservas' ||
    (path.startsWith('/api/v2/reservas/') && path.endsWith('/pagos/confirmacion'))
  )) {
    return true;
  }

  return false;
}

function parsePath(url: string) {
  try {
    return new URL(url).pathname;
  } catch {
    return url.startsWith('/') ? url : `/${url}`;
  }
}
