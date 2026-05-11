import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterLink, RouterOutlet],
  template: `
    <div class="admin-shell">
      <aside>
        <h1>Admin</h1>
        <a routerLink="/admin">Panel</a>
        <a routerLink="/admin/atracciones">Atracciones</a>
        <a routerLink="/admin/catalogos">Catalogos</a>
        <a routerLink="/admin/reservas">Reservas</a>
        <a routerLink="/admin/clientes">Clientes</a>
        <a routerLink="/admin/operacion">Operacion</a>
        <a routerLink="/admin/seguridad">Seguridad</a>
        <button type="button" (click)="auth.logout()">Salir</button>
      </aside>
      <main>
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .admin-shell {
      display: grid;
      grid-template-columns: 248px minmax(0, 1fr);
      min-height: 100vh;
    }
    aside {
      background: #172026;
      color: white;
      display: grid;
      align-content: start;
      gap: 8px;
      padding: 24px;
    }
    h1 { font-size: 20px; margin: 0 0 16px; }
    a, button {
      background: transparent;
      border-radius: 6px;
      color: #dce5ea;
      padding: 10px 12px;
      text-align: left;
    }
    a:hover, button:hover { background: rgba(255,255,255,.08); }
    main { min-width: 0; }
    @media (max-width: 860px) {
      .admin-shell { grid-template-columns: 1fr; }
      aside { grid-template-columns: repeat(2, minmax(0, 1fr)); }
      h1 { grid-column: 1 / -1; }
    }
  `]
})
export class AdminLayoutComponent {
  constructor(readonly auth: AuthService) {}
}
