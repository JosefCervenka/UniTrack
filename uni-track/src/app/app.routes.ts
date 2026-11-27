import { Routes } from '@angular/router';
import { CreateAccountComponent } from './Pages/create-account/create-account.component';
import { LoginComponent } from './Pages/login/login.component';
import { SearchLostItemsComponent } from './Pages/search-lost-items/search-lost-items.component';
import { CreateLostItemComponent } from './Pages/create-lost-item/create-lost-item.component';

export const routes: Routes = [
  { path: '', redirectTo: 'search-lost-items', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'create-account', component: CreateAccountComponent },
  { path: 'search-lost-items', component: SearchLostItemsComponent },
  { path: 'create-lost-item', component: CreateLostItemComponent }
];
