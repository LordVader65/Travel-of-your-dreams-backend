import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ApiService } from '../../../core/api/api.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page login-page">
      <div class="panel stack">
        <h1>Ingresar</h1>
        <div class="tabs">
          <button type="button" [class.active]="modo() === 'login'" (click)="modo.set('login')">Login</button>
          <button type="button" [class.active]="modo() === 'registro'" (click)="modo.set('registro')">Registro</button>
        </div>
        @if (modo() === 'registro') {
          <label>Tipo identificacion
            <select name="tipoIdentificacion" [(ngModel)]="registro.tipoIdentificacion" required maxlength="20">
              <option value="CEDULA">Cedula</option>
              <option value="RUC">RUC</option>
              <option value="PASAPORTE">Pasaporte</option>
            </select>
          </label>
          <label>Numero identificacion <input name="numeroIdentificacion" inputmode="numeric" pattern="[0-9]*" maxlength="20" (input)="soloNumeros($event)" [(ngModel)]="registro.numeroIdentificacion"></label>
          <label>Nombres <input name="nombres" maxlength="100" [(ngModel)]="registro.nombres"></label>
          <label>Apellidos <input name="apellidos" maxlength="100" [(ngModel)]="registro.apellidos"></label>
          <label>Telefono <input name="telefono" inputmode="numeric" pattern="[0-9]*" maxlength="10" (input)="soloNumeros($event)" [(ngModel)]="registro.telefono"></label>
          <label>Direccion <input name="direccion" maxlength="300" [(ngModel)]="registro.direccion"></label>
        }
        <label>
          Correo
          <input name="login" [(ngModel)]="login" type="email" autocomplete="username" maxlength="150">
        </label>
        <label>
          Password
          <input name="password" [(ngModel)]="password" type="password" autocomplete="current-password" maxlength="200">
        </label>
        @if (error()) {
          <p class="error">{{ error() }}</p>
        }
        <button class="btn" type="button" (click)="submit()">{{ modo() === 'login' ? 'Ingresar' : 'Crear cuenta' }}</button>
      </div>
    </section>
  `,
  styles: [`
    .login-page { display: grid; place-items: center; min-height: calc(100vh - 64px); }
    .panel { width: min(100%, 420px); }
    h1 { margin: 0; }
    label { color: var(--muted); display: grid; gap: 8px; font-weight: 700; }
    input, select { border: 1px solid var(--line); border-radius: 6px; min-height: 42px; padding: 0 12px; }
    .check { align-items: center; display: flex; }
    .check input { min-height: auto; }
    .error { color: var(--danger); margin: 0; }
    .tabs { background: var(--surface-muted); border-radius: 6px; display: grid; grid-template-columns: 1fr 1fr; padding: 4px; }
    .tabs button { background: transparent; border-radius: 4px; min-height: 36px; }
    .tabs button.active { background: white; font-weight: 800; }
  `]
})
export class LoginPageComponent {
  modo = signal<'login' | 'registro'>('login');
  login = '';
  password = '';
  error = signal<string | null>(null);
  registro = {
    tipoIdentificacion: 'CEDULA',
    numeroIdentificacion: '',
    nombres: '',
    apellidos: '',
    razonSocial: '',
    telefono: '',
    direccion: ''
  };

  constructor(
    private readonly auth: AuthService,
    private readonly api: ApiService,
    private readonly router: Router
  ) {}

  submit() {
    this.error.set(null);
    if (this.modo() === 'registro') {
      this.api.register({
        login: this.login,
        TipoIdentificacion: this.registro.tipoIdentificacion,
        NumeroIdentificacion: this.registro.numeroIdentificacion,
        Nombres: this.registro.nombres,
        Apellidos: this.registro.apellidos,
        RazonSocial: this.registro.razonSocial,
        Correo: this.login,
        Password: this.password,
        Telefono: this.registro.telefono,
        Direccion: this.registro.direccion
      }).subscribe({
        next: () => this.auth.login({ login: this.login, password: this.password }).subscribe({
          next: () => {
            this.api.actualizarMiPerfil({
              tipoIdentificacion: this.registro.tipoIdentificacion,
              numeroIdentificacion: this.registro.numeroIdentificacion,
              nombres: this.registro.nombres,
              apellidos: this.registro.apellidos,
              razonSocial: this.registro.razonSocial,
              correo: this.login,
              telefono: this.registro.telefono,
              direccion: this.registro.direccion
            }).subscribe({
              next: () => void this.router.navigateByUrl('/'),
              error: (error: Error) => this.error.set(error.message)
            });
          },
          error: (error: Error) => this.error.set(error.message)
        }),
        error: (error: Error) => this.error.set(error.message)
      });
      return;
    }

    this.auth.login({ login: this.login, password: this.password }).subscribe({
      next: (response) => {
        const roles = response.data.roles.map((role) => role.toUpperCase());
        void this.router.navigateByUrl(roles.includes('ADMIN') ? '/admin' : '/');
      },
      error: (error: Error) => this.error.set(error.message)
    });
  }

  soloNumeros(event: Event) {
    const input = event.target as HTMLInputElement;
    const max = input.maxLength > 0 ? input.maxLength : input.value.length;
    input.value = input.value.replace(/\D/g, '').slice(0, max);
  }
}
