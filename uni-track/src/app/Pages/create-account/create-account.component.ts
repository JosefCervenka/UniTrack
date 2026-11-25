import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CreateUser } from '../../Models/create-user';
import { ApiValidationError } from '../../Models/error-model';
import { UserServiceService } from '../../Services/user-service.service';

const FIELDS: (keyof CreateUser)[] = [
  'userName', 'givenName', 'surname', 'email', 'phoneNumber', 'password', 'passwordAgain'
];

@Component({
  selector: 'app-create-account',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './create-account.component.html',
  styleUrl: './create-account.component.scss'
})
export class CreateAccountComponent {
  private userService = inject(UserServiceService);

  model: CreateUser = {
    userName: '',
    givenName: '',
    surname: '',
    email: '',
    phoneNumber: '',
    password: '',
    passwordAgain: ''
  };

  errors = signal<ApiValidationError[]>([]);
  success = signal(false);

  generalError = computed(() =>
    this.errors().find(
      e => !FIELDS.some(field => field.toLowerCase() === e.field.toLowerCase())
    )?.message
  );

  async onSubmit(): Promise<void> {
    this.errors.set([]);
    this.success.set(false);

    const result = await this.userService.createUser(this.model);

    if (result) {
      this.errors.set(result);
      return;
    }

    this.success.set(true);
  }

  fieldError(fieldName: string): string | undefined {
    return this.errors().find(e => e.field.toLowerCase() === fieldName.toLowerCase())?.message;
  }
}
