import {Component} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {UserServiceService} from '../../Services/user-service.service';

function passwordStrength(control: AbstractControl): ValidationErrors | null {
  const value = control.value as string;
  if (!value) return null;
  const errors: ValidationErrors = {};
  if (!/[A-Z]/.test(value)) errors['uppercase'] = true;
  if (!/[a-z]/.test(value)) errors['lowercase'] = true;
  if (!/[0-9]/.test(value)) errors['digit'] = true;
  return Object.keys(errors).length ? errors : null;
}

@Component({
  selector: 'app-create-account',
  standalone: true,
  imports: [ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule],
  templateUrl: './create-account.component.html',
  styleUrl: './create-account.component.scss'
})
export class CreateAccountComponent {
  submitting = false;
  success = false;
  error = false;
  serverErrors: string[] = [];

  readonly form = this.formBuilder.nonNullable.group({
    userName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50), Validators.pattern(/^[a-zA-Z0-9._-]+$/)]],
    givenName: ['', [Validators.required, Validators.pattern(/\S/), Validators.maxLength(100)]],
    surname: ['', [Validators.required, Validators.pattern(/\S/), Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.pattern(/^[^@]+@[^@]+$/), Validators.maxLength(256)]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9\s\-]{7,20}$/)]],
    password: ['', [Validators.required, Validators.pattern(/\S/), Validators.minLength(8), Validators.maxLength(128), passwordStrength]],
    passwordAgain: ['', Validators.required]
  });

  constructor(private formBuilder: FormBuilder, private userService: UserServiceService) {
  }

  submit(): void {
    if (this.submitting) return;
    this.success = false;
    this.error = false;
    this.serverErrors = [];
    this.form.markAllAsTouched();
    if (this.form.invalid || this.form.controls.password.value !== this.form.controls.passwordAgain.value) return;

    this.submitting = true;
    this.userService.createUser(this.form.getRawValue()).subscribe({
      next: () => {
        this.submitting = false;
        this.success = true;
        this.form.reset();
      },
      error: (response: unknown) => {
        this.submitting = false;
        this.error = true;
        if (response instanceof HttpErrorResponse && response.status === 400) {
          let body: unknown = response.error;
          if (typeof body === 'string') {
            try {
              body = JSON.parse(body);
            } catch {
              body = null;
            }
          }
          if (Array.isArray(body)) {
            this.serverErrors = body.filter((message): message is string => typeof message === 'string' && message.trim().length > 0);
          }
        }
      }
    });
  }
}
