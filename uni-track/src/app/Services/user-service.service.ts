import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { CreateUser } from '../Models/create-user';
import { LoginResponse, LoginUser } from '../Models/login-user';

const TOKEN_KEY = 'token';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {

  private readonly loggedIn = signal(localStorage.getItem(TOKEN_KEY) !== null);
  readonly isLoggedIn = this.loggedIn.asReadonly();

  constructor(private http: HttpClient) { }

  createUser(user: CreateUser): Observable<string> {
    return this.http.post('/api/user', user, { responseType: 'text' });
  }

  login(user: LoginUser): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/api/user/login', user).pipe(
      tap(response => {
        localStorage.setItem(TOKEN_KEY, response.token);
        this.loggedIn.set(true);
      })
    );
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.loggedIn.set(false);
  }
}
