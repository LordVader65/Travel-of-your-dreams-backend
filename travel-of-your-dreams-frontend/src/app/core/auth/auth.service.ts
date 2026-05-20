import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { ApiService } from '../api/api.service';
import { LoginRequest, LoginResponse } from '../../shared/models/auth.model';

const sessionKey = 'toyd_session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly session = signal<LoginResponse | null>(this.readSession());

  constructor(
    private readonly api: ApiService,
    private readonly router: Router
  ) {}

  get token() {
    const session = this.session() as (LoginResponse & { accessToken?: string }) | null;
    return session?.token ?? session?.accessToken ?? null;
  }

  get isAuthenticated() {
    return Boolean(this.token);
  }

  hasRole(role: string) {
    return this.session()?.roles.some((item) => item.toUpperCase() === role.toUpperCase()) ?? false;
  }

  login(request: LoginRequest, admin = false) {
    const source = admin ? this.api.adminLogin(request) : this.api.login(request);
    return source.pipe(tap((response) => this.storeSession(response.data)));
  }

  logout() {
    this.api.logoutBackend().subscribe({ error: () => undefined });
    this.clearSession();
  }

  clearSession() {
    localStorage.removeItem(sessionKey);
    this.session.set(null);
    void this.router.navigateByUrl('/login');
  }

  private storeSession(session: LoginResponse) {
    const normalized = this.normalizeSession(session);
    localStorage.setItem(sessionKey, JSON.stringify(normalized));
    this.session.set(normalized);
  }

  private readSession(): LoginResponse | null {
    const raw = localStorage.getItem(sessionKey);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as LoginResponse;
    } catch {
      localStorage.removeItem(sessionKey);
      return null;
    }
  }

  private normalizeSession(session: LoginResponse): LoginResponse {
    const value = session as LoginResponse & {
      usuarioGuid?: string;
      clienteGuid?: string | null;
      expiraEnUtc?: string;
    };

    return {
      ...session,
      usuario_guid: session.usuario_guid ?? value.usuarioGuid ?? '',
      cliente_guid: session.cliente_guid ?? value.clienteGuid ?? null,
      expira_en_utc: session.expira_en_utc ?? value.expiraEnUtc ?? ''
    };
  }
}
