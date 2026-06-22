import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="admin-shell">
      <aside>
        <div class="brand-block">
          <strong>TOYD</strong>
          <span>Administracion</span>
        </div>
        <nav aria-label="Navegacion administrativa">
          <a routerLink="/admin" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Panel</a>
          <a routerLink="/admin/atracciones" routerLinkActive="active">Atracciones</a>
          <a routerLink="/admin/catalogos" routerLinkActive="active">Catalogos</a>
          <a routerLink="/admin/reservas" routerLinkActive="active">Reservas</a>
          <a routerLink="/admin/clientes" routerLinkActive="active">Clientes</a>
          <a routerLink="/admin/operacion" routerLinkActive="active">Operacion</a>
          <a routerLink="/admin/seguridad" routerLinkActive="active">Seguridad</a>
        </nav>
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
      grid-template-columns: 260px minmax(0, 1fr);
      min-height: 100vh;
    }
    aside {
      background: linear-gradient(180deg, #102825 0%, #172026 100%);
      color: white;
      display: grid;
      align-content: start;
      gap: 18px;
      padding: 24px;
      position: sticky;
      top: 0;
      height: 100vh;
    }
    .brand-block {
      border-bottom: 1px solid rgba(255,255,255,.12);
      display: grid;
      gap: 4px;
      padding-bottom: 18px;
    }
    .brand-block strong { font-size: 22px; letter-spacing: 0; }
    .brand-block span { color: #b6c8c3; font-size: 13px; font-weight: 800; text-transform: uppercase; }
    nav { display: grid; gap: 6px; }
    a, button {
      background: transparent;
      border-radius: 6px;
      color: #dce5ea;
      padding: 10px 12px;
      text-align: left;
      transition: background .18s ease, color .18s ease;
    }
    a:hover, button:hover, a.active {
      background: rgba(255,255,255,.11);
      color: #fff;
    }
    a.active {
      box-shadow: inset 3px 0 0 #5eead4;
    }
    button {
      margin-top: 8px;
    }
    main { min-width: 0; }
    @media (max-width: 860px) {
      .admin-shell { grid-template-columns: 1fr; }
      aside {
        height: auto;
        position: static;
      }
      nav { grid-template-columns: repeat(2, minmax(0, 1fr)); }
    }
  `]
})
export class AdminLayoutComponent {
  constructor(readonly auth: AuthService) {}
}
