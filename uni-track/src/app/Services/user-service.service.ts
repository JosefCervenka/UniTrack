import {computed, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {firstValueFrom, Observable} from 'rxjs';
import {LoginUser} from '../Models/login-user';
import {ReadUser} from '../Models/read-user';
import {CreateUser} from '../Models/create-user';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {

  private readonly loginUser = signal<ReadUser | null>(null);
  readonly currentUser = this.loginUser.asReadonly();
  readonly isLoggedIn = computed(() => this.loginUser() !== null);

  constructor(private http: HttpClient) {
    void this.loadCurrentUser();
  }

  createUser(user: CreateUser): Observable<string> {
    return this.http.post('/api/user', user, {responseType: 'text'});
  }

  async login(user: LoginUser): Promise<void> {
    await firstValueFrom(this.http.post('/api/user/login', user, {responseType: 'text'}));
    await this.loadCurrentUser();
  }

  async logout(): Promise<void> {
    try {
      await firstValueFrom(this.http.post('/api/user/logout', null, {responseType: 'text'}));
    } finally {
      this.loginUser.set(null);
    }
  }

  private async loadCurrentUser(): Promise<void> {
    try {
      this.loginUser.set(await firstValueFrom(this.http.get<ReadUser>('/api/user/current')));
    } catch {
      this.loginUser.set(null);
    }
  }
}
