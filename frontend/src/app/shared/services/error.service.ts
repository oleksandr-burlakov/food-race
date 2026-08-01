import { Injectable } from '@angular/core';
import { type ProblemDetails } from '../models/problem-details';
import { type AbstractControl, type FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  handleError(error: ProblemDetails): void {
    console.error(`[${error.title || 'Error'}]: ${error.detail}`);

    // TODO: in future this.toastr.error(error.detail, error.title);
    alert(`${error.title ?? 'Помилка'}: ${error.detail ?? 'Щось пішло не так'}`);
  }

  applyValidationErrors(form: FormGroup, problemDetails: ProblemDetails): void {
    if (!problemDetails.errors) return;

    Object.keys(problemDetails.errors).forEach((propertyName) => {
      const controlName = propertyName.charAt(0).toLowerCase() + propertyName.slice(1);
      const control: AbstractControl | null = form.get(controlName);

      if (control && problemDetails.errors) {
        const customErrors = problemDetails.errors[propertyName];
        control.setErrors({ serverError: customErrors.join(' ') });
      }
    });
  }
}
