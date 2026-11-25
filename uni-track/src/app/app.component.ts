import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { UserServiceService } from './Services/user-service.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatButtonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'uni-track';
  readonly year = new Date().getFullYear();

  constructor(readonly userService: UserServiceService, private router: Router) {
  }

  logout(): void {
    this.userService.logout();
    this.router.navigate(['/login']);
  }
}
