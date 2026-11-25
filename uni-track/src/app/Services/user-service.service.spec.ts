import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { UserServiceService } from './user-service.service';

describe('UserServiceService', () => {
  let service: UserServiceService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(UserServiceService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('posts the user and reads the text response', () => {
    const user = {
      userName: 'user', givenName: 'Jane', surname: 'Doe', email: 'jane@example.com',
      phoneNumber: '123456789', password: 'password', passwordAgain: 'password'
    };
    let response: string | undefined;

    service.createUser(user).subscribe(value => response = value);

    const request = http.expectOne('/api/user');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(user);
    expect(request.request.responseType).toBe('text');
    request.flush('Hello World!');
    expect(response).toBe('Hello World!');
  });
});
