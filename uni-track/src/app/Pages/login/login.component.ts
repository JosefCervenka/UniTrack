import {Component, computed, inject, signal} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { LoginUser } from '../../Models/login-user';
import { UserServiceService } from '../../Services/user-service.service';
import {ApiValidationError} from "../../Models/error-model";


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private userService = inject(UserServiceService);

  model: LoginUser = {
    email: '',
    password: ''
  };

  errors = signal<ApiValidationError[]>([]);

  generalError = computed(() =>
    this.errors().find(
      e => e.field.toLowerCase() !== 'email' && e.field.toLowerCase() !== 'password'
    )?.message
  );

  async onSubmit(): Promise<void> {
    this.errors.set([]);

    const result = await this.userService.login(this.model);

    if (result) {
      this.errors.set(result);
    }
  }

  fieldError(fieldName: string): string | undefined {
    return this.errors().find(e => e.field.toLowerCase() === fieldName.toLowerCase())?.message;
  }
}
