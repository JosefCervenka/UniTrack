import {computed, Injectable, signal} from '@angular/core';
import {LoginUser} from '../Models/login-user';
import {ReadUser} from '../Models/read-user';
import {CreateUser} from '../Models/create-user';
import {ApiValidationError} from '../Models/error-model';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {

  private readonly loginUser = signal<ReadUser | null>(null);
  readonly currentUser = this.loginUser.asReadonly();
  readonly isLoggedIn = computed(() => this.loginUser() !== null);

  constructor() {
    void this.loadCurrentUser();
  }

  async createUser(user: CreateUser): Promise<ApiValidationError[] | void> {
    const response = await fetch('/api/user', {
      method: 'POST',
      headers: {'Content-Type': 'application/json'},
      credentials: 'include',
      body: JSON.stringify(user)
    });

    if (!response.ok) {
      return await this.extractErrors(response);
    }
  }

  async login(user: LoginUser): Promise<ApiValidationError[] | void> {
    const response = await fetch('/api/user/login', {
      method: 'POST',
      headers: {'Content-Type': 'application/json'},
      credentials: 'include',
      body: JSON.stringify(user)
    });

    if (!response.ok) {
      return await this.extractErrors(response);
    }

    await this.loadCurrentUser();
  }

  async logout(): Promise<void> {
    try {
      await fetch('/api/user/logout', {
        method: 'POST',
        credentials: 'include'
      });
    } finally {
      this.loginUser.set(null);
    }
  }

  private async loadCurrentUser(): Promise<void> {
    try {
      const response = await fetch('/api/user/current', {
        credentials: 'include'
      });

      if (!response.ok) {
        this.loginUser.set(null);
        return;
      }

      const current: ReadUser = await response.json();
      this.loginUser.set(current);
    } catch {
      this.loginUser.set(null);
    }
  }

  private async extractErrors(response: Response): Promise<ApiValidationError[]> {
    try {
      const data: unknown = await response.json();
      if (Array.isArray(data)) {
        return data as ApiValidationError[];
      }
    } catch {
      console.error('Response was not valid JSON');
    }

    return [
      {
        field: '',
        message: response.statusText || `Request failed (${response.status})`,
        errorCode: response.status,
        state: null
      }
    ];
  }
}
