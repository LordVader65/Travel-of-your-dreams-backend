import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';

@Component({
  selector: 'app-perfil-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <h1>Perfil</h1>

      <div class="grid two">
        <form class="panel form-grid" (ngSubmit)="guardarPerfil()">
          <h2>Datos personales</h2>
          <label>Nombres <input name="nombres" maxlength="100" [(ngModel)]="perfil.nombres" /></label>
          <label>Apellidos <input name="apellidos" maxlength="100" [(ngModel)]="perfil.apellidos" /></label>
          <label>Correo <input name="correo" maxlength="150" [(ngModel)]="perfil.correo" required /></label>
          <label>Telefono <input inputmode="numeric" pattern="[0-9]*" maxlength="10" name="telefono" (input)="soloNumeros($event)" [(ngModel)]="perfil.telefono" /></label>
          <label class="wide">Direccion <input name="direccion" maxlength="300" [(ngModel)]="perfil.direccion" /></label>
          <button class="btn wide" type="submit">Guardar perfil</button>
        </form>

        <form class="panel form-grid" (ngSubmit)="cambiarPassword()">
          <h2>Password</h2>
          <label>Password actual <input type="password" name="actual" [(ngModel)]="password.passwordActual" /></label>
          <label>Nuevo password <input type="password" name="nuevo" [(ngModel)]="password.nuevoPassword" /></label>
          <button class="btn wide" type="submit">Cambiar password</button>
        </form>
      </div>

      <div class="grid two">
        <form class="panel form-grid" (ngSubmit)="guardarDato()">
          <h2>{{ datoGuid() ? 'Editar facturacion' : 'Nuevo dato de facturacion' }}</h2>
          <label>Tipo ID
            <select name="tipoIdentificacion" [(ngModel)]="dato.tipoIdentificacion">
              <option value="CEDULA">Cedula</option>
              <option value="RUC">RUC</option>
              <option value="PASAPORTE">Pasaporte</option>
            </select>
          </label>
          <label>Numero ID <input inputmode="numeric" pattern="[0-9]*" maxlength="30" name="numeroIdentificacion" (input)="soloNumeros($event)" [(ngModel)]="dato.numeroIdentificacion" /></label>
          <label>Nombre <input name="nombre" maxlength="100" [(ngModel)]="dato.nombre" /></label>
          <label>Apellido <input name="apellido" maxlength="100" [(ngModel)]="dato.apellido" /></label>
          <label>Razon social <input name="razonSocial" maxlength="200" [(ngModel)]="dato.razonSocial" /></label>
          <label>Correo <input name="datoCorreo" maxlength="150" [(ngModel)]="dato.correo" /></label>
          <label>Telefono <input inputmode="numeric" pattern="[0-9]*" maxlength="10" name="datoTelefono" (input)="soloNumeros($event)" [(ngModel)]="dato.telefono" /></label>
          <label>Estado
            <select name="datoEstado" [(ngModel)]="dato.estado">
              <option value="A">Activo</option>
              <option value="I">Inactivo</option>
            </select>
          </label>
          <label class="wide">Direccion <input name="datoDireccion" maxlength="300" [(ngModel)]="dato.direccion" /></label>
          <div class="actions wide">
            <button class="btn" type="submit">Guardar</button>
            <button class="btn secondary" type="button" (click)="limpiarDato()">Limpiar</button>
          </div>
        </form>

        <div class="panel stack">
          <h2>Datos de facturacion</h2>
          @for (item of datos(); track item.guid || item.id || $index) {
            <div class="compact-row">
              <span><strong>{{ item.nombre || item.razon_social || item.correo }}</strong><small>{{ item.numero_identificacion || item.numeroIdentificacion }}</small></span>
              <span class="actions">
                <button class="btn secondary" type="button" (click)="editarDato(item)">Editar</button>
                <button class="btn danger" type="button" (click)="eliminarDato(item.id)">Eliminar</button>
              </span>
            </div>
          } @empty {
            <p class="muted">Sin datos registrados.</p>
          }
        </div>
      </div>

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `
})
export class PerfilPageComponent implements OnInit {
  perfil: any = {};
  datos = signal<any[]>([]);
  datoGuid = signal<string | null>(null);
  mensaje = signal('');
  password = { passwordActual: '', nuevoPassword: '' };
  dato = this.nuevoDato();

  constructor(private readonly api: ApiService) {}

  ngOnInit() {
    this.cargar();
  }

  cargar() {
    this.api.miPerfil().subscribe((response) => this.perfil = response.data ?? {});
    this.api.misDatosFacturacion().subscribe((response) => this.datos.set(response.data));
  }

  guardarPerfil() {
    this.api.actualizarMiPerfil(this.perfil).subscribe(() => this.mensaje.set('Perfil actualizado.'));
  }

  cambiarPassword() {
    this.api.cambiarMiPassword(this.password).subscribe(() => {
      this.mensaje.set('Password actualizado.');
      this.password = { passwordActual: '', nuevoPassword: '' };
    });
  }

  guardarDato() {
    const action = this.datoGuid()
      ? this.api.actualizarDatosFacturacion(this.datoGuid()!, this.dato)
      : this.api.crearDatosFacturacion(this.dato);
    action.subscribe(() => {
      this.mensaje.set('Dato de facturacion guardado.');
      this.limpiarDato();
      this.cargar();
    });
  }

  editarDato(item: any) {
    this.datoGuid.set(item.guid);
    this.dato = {
      tipoIdentificacion: item.tipo_identificacion ?? item.tipoIdentificacion ?? '',
      numeroIdentificacion: item.numero_identificacion ?? item.numeroIdentificacion ?? '',
      razonSocial: item.razon_social ?? item.razonSocial ?? '',
      nombre: item.nombre ?? '',
      apellido: item.apellido ?? '',
      correo: item.correo ?? '',
      telefono: item.telefono ?? '',
      direccion: item.direccion ?? '',
      estado: item.estado ?? 'A'
    };
  }

  eliminarDato(id: number) {
    if (!confirm('Eliminar dato de facturacion?')) return;
    this.api.eliminarDatosFacturacion(id).subscribe(() => {
      this.mensaje.set('Dato eliminado.');
      this.cargar();
    });
  }

  limpiarDato() {
    this.datoGuid.set(null);
    this.dato = this.nuevoDato();
  }

  soloNumeros(event: Event) {
    const input = event.target as HTMLInputElement;
    const max = input.maxLength > 0 ? input.maxLength : input.value.length;
    input.value = input.value.replace(/\D/g, '').slice(0, max);
  }

  private nuevoDato() {
    return {
      tipoIdentificacion: 'CEDULA',
      numeroIdentificacion: '',
      razonSocial: '',
      nombre: '',
      apellido: '',
      correo: '',
      telefono: '',
      direccion: '',
      estado: 'A'
    };
  }
}
