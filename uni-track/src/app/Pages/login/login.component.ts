import {Component} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {UserServiceService} from '../../Services/user-service.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  submitting = false;
  success = false;
  error = false;
  serverErrors: string[] = [];

  readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.pattern(/^[^@]+@[^@]+$/), Validators.maxLength(256)]],
    password: ['', [Validators.required, Validators.maxLength(128)]]
  });

  constructor(private formBuilder: FormBuilder, private userService: UserServiceService) {
  }

  submit(): void {
    if (this.submitting) return;
    this.success = false;
    this.error = false;
    this.serverErrors = [];
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.submitting = true;
    this.userService.login(this.form.getRawValue()).subscribe({
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
              body = [body];
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
