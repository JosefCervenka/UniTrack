import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { LoginUser } from '../../Models/login-user';
import { UserServiceService } from '../../Services/user-service.service';


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

  async onSubmit(): Promise<void> {
    await this.userService.login(this.model);
  }
}
