import { Routes } from '@angular/router';
import { CreateAccountComponent } from './Pages/create-account/create-account.component';
import { LoginComponent } from './Pages/login/login.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'create-account', component: CreateAccountComponent }
];
