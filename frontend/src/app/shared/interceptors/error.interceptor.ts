import { type HttpErrorResponse, type HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { inject } from '@angular/core';
import { ErrorService } from '../services/error.service';
import { type ProblemDetails } from '../models/problem-details';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(ErrorService);

  return next(req).pipe(
    catchError((errorResponse: HttpErrorResponse) => {
      let problemDetails: ProblemDetails;

      if (errorResponse.error && typeof errorResponse.error === 'object') {
        problemDetails = errorResponse.error as ProblemDetails;
      } else {
        problemDetails = {
          title: 'Network Error',
          status: errorResponse.status,
          detail: 'Unable to connect to the server',
        };
      }

      if (errorResponse.status === 400 && problemDetails.errors) {
        return throwError(() => problemDetails);
      }

      errorService.handleError(problemDetails);

      return throwError(() => problemDetails);
    }),
  );
};
