import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-public-layout',
  standalone: true,
  imports: [RouterLink, RouterOutlet],
  template: `
    <header class="topbar">
      <a routerLink="/" class="brand">Travel of Your Dreams</a>
      <nav>
        <a routerLink="/">Atracciones</a>
        @if (mostrarOpcionesCliente()) {
          <a routerLink="/mis-reservas">Mis reservas</a>
          <a routerLink="/perfil">Perfil</a>
        }
        @if (auth.isAuthenticated) {
          <button class="link-button" type="button" (click)="auth.logout()">Salir</button>
        } @else {
          <a routerLink="/login">Login</a>
        }
      </nav>
    </header>
    <main>
      <router-outlet />
    </main>
  `,
  styles: [`
    .topbar {
      align-items: center;
      background: #ffffff;
      border-bottom: 1px solid var(--line);
      box-shadow: 0 2px 12px rgba(28, 37, 48, .04);
      display: flex;
      justify-content: space-between;
      min-height: 64px;
      padding: 0 24px;
      position: sticky;
      top: 0;
      z-index: 10;
    }
    .brand { color: #0b2f2b; font-size: 18px; font-weight: 900; letter-spacing: 0; }
    nav { align-items: center; display: flex; gap: 16px; }
    nav a, .link-button { color: var(--muted); font-weight: 800; }
    nav a:hover, .link-button:hover { color: var(--primary); }
    .link-button { background: transparent; padding: 0; }
    @media (max-width: 760px) {
      .topbar { align-items: flex-start; flex-direction: column; gap: 10px; padding: 14px 16px; }
      nav { flex-wrap: wrap; }
    }
  `]
})
export class PublicLayoutComponent {
  constructor(readonly auth: AuthService) {}

  mostrarOpcionesCliente() {
    return this.auth.isAuthenticated && !this.auth.hasRole('ADMIN');
  }
}
