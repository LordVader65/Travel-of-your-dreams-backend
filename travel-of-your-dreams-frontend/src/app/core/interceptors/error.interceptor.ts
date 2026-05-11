import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiErrorResponse } from '../../shared/models/api-response.model';

export const errorInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const body = error.error as ApiErrorResponse | undefined;
      const message = body?.details?.[0] ?? body?.error ?? error.message;
      return throwError(() => new Error(message));
    })
  );
