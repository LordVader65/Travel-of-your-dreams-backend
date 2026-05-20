import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { LoginRequest } from '../../shared/models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  constructor(private readonly api: ApiService) {}

  login(request: LoginRequest) {
    return this.api.login(request);
  }

  adminLogin(request: LoginRequest) {
    return this.api.adminLogin(request);
  }

  register(request: unknown) {
    return this.api.register(request);
  }

  logoutBackend() {
    return this.api.logoutBackend();
  }
}
