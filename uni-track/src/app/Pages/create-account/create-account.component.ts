import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { firstValueFrom } from 'rxjs';
import { CreateUser } from '../../Models/create-user';
import { UserServiceService } from '../../Services/user-service.service';

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

  async onSubmit(): Promise<void> {
    await firstValueFrom(this.userService.createUser(this.model));
  }
}
