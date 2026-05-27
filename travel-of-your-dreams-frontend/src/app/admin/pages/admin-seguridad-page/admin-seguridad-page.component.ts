import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-admin-seguridad-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div class="page-title">
          <h1>Seguridad y auditoria</h1>
          <p class="muted">Usuarios vinculados a personas/clientes, roles y trazabilidad de operaciones.</p>
        </div>
        <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
      </header>

      <nav class="app-tabs" aria-label="Seguridad">
        <button class="tab-button" [class.active]="tab() === 'usuarios'" type="button" (click)="tab.set('usuarios')">Usuarios</button>
        <button class="tab-button" [class.active]="tab() === 'gestion'" type="button" (click)="tab.set('gestion')">Gestion de roles</button>
        <button class="tab-button" [class.active]="tab() === 'auditoria'" type="button" (click)="tab.set('auditoria')">Auditoria</button>
      </nav>

      <div class="grid two" [hidden]="tab() !== 'gestion'">
        <form class="panel form-grid" (ngSubmit)="crearUsuario()">
          <h2>Crear usuario</h2>
          <label>Tipo identificacion
            <select name="tipoIdentificacion" [(ngModel)]="usuario.tipoIdentificacion" required>
              <option value="CEDULA">Cedula</option>
              <option value="RUC">RUC</option>
              <option value="PASAPORTE">Pasaporte</option>
            </select>
          </label>
          <label>Identificacion <input name="numeroIdentificacion" maxlength="20" inputmode="numeric" [(ngModel)]="usuario.numeroIdentificacion" required /></label>
          <label>Nombres <input name="nombres" maxlength="100" [(ngModel)]="usuario.nombres" /></label>
          <label>Apellidos <input name="apellidos" maxlength="100" [(ngModel)]="usuario.apellidos" /></label>
          <label class="wide">Razon social <input name="razonSocial" maxlength="200" [(ngModel)]="usuario.razonSocial" /></label>
          <label>Correo / login <input type="email" name="correo" maxlength="150" [(ngModel)]="usuario.correo" required /></label>
          <label>Telefono <input name="telefono" maxlength="10" inputmode="numeric" pattern="[0-9]*" [(ngModel)]="usuario.telefono" (input)="soloNumerosTelefono()" /></label>
          <label class="wide">Direccion <input name="direccion" maxlength="300" [(ngModel)]="usuario.direccion" /></label>
          <label>Password <input type="password" name="password" maxlength="100" [(ngModel)]="usuario.password" required /></label>
          <div class="wide stack">
            <strong>Roles</strong>
            <div class="check-list">
              @for (rol of rolesActivos(); track rol.id) {
                <label class="toggle-line"><input type="checkbox" [checked]="usuario.rolIds.includes(rol.id)" (change)="toggleRolCreacion(rol.id)" /> {{ rol.descripcion || rol.nombre || rol.name }}</label>
              }
            </div>
          </div>
          <button class="btn wide" type="submit">Crear usuario y persona</button>
        </form>

        <form class="panel form-grid" (ngSubmit)="buscarUsuario()">
          <h2>Gestion de usuario</h2>
          <label class="wide">Usuario
            <select name="usuarioGuid" [(ngModel)]="gestion.guid" (change)="seleccionarUsuario()">
              <option value="">Selecciona usuario</option>
              @for (usuario of usuarios(); track usuario.guid) {
                <option [value]="usuario.guid">{{ etiquetaUsuario(usuario) }}</option>
              }
            </select>
          </label>
          <div class="wide stack">
            <strong>Roles asignados</strong>
            <div class="check-list">
              @for (rol of rolesActivos(); track rol.id) {
                <label class="toggle-line"><input type="checkbox" [checked]="gestion.rolIds.includes(rol.id)" [disabled]="usuarioProtegido(seleccionado())" (change)="toggleRolGestion(rol.id)" /> {{ rol.descripcion || rol.nombre || rol.name }}</label>
              }
            </div>
            @if (usuarioProtegido(seleccionado())) {
              <small class="muted">El administrador principal esta protegido: no se puede desactivar ni cambiar sus roles.</small>
            }
          </div>
          <div class="actions wide">
            <button class="btn secondary" type="submit">Ver detalle</button>
            <button class="btn" type="button" [disabled]="usuarioProtegido(seleccionado())" (click)="estadoUsuario('A')">Activar</button>
            <button class="btn danger" type="button" [disabled]="usuarioProtegido(seleccionado())" (click)="estadoUsuario('I')">Desactivar</button>
            <button class="btn" type="button" [disabled]="usuarioProtegido(seleccionado())" (click)="asignarRoles()">Guardar roles</button>
          </div>
        </form>
      </div>

      <div class="grid two" [hidden]="tab() !== 'usuarios'">
        <div class="panel stack">
          <h2>Usuarios</h2>
          <input name="buscarUsuarios" placeholder="Buscar usuario, persona o rol" [(ngModel)]="busquedaUsuarios" />
          @for (usuario of usuariosFiltrados(); track usuario.guid) {
            <div class="compact-row">
              <span>
                <strong>{{ etiquetaUsuario(usuario) }}</strong>
                <small>{{ usuario.estado }} · {{ rolesTexto(usuario) }}</small>
              </span>
              <button class="btn secondary" type="button" (click)="usarUsuario(usuario)">Gestionar</button>
            </div>
          } @empty {
            <p class="muted">Sin usuarios registrados.</p>
          }
        </div>
        <div class="panel stack">
          <h2>Usuario seleccionado</h2>
          @if (seleccionado()) {
            <div class="info-grid">
              @for (item of usuarioResumen(seleccionado()); track item.label) {
                <div class="info-item">
                  <span>{{ item.label }}</span>
                  <strong>{{ item.value }}</strong>
                </div>
              }
            </div>
          } @else {
            <p class="muted">Selecciona un usuario desde la lista.</p>
          }
        </div>
      </div>

      <form class="panel form-grid" [hidden]="tab() !== 'auditoria'" (ngSubmit)="cargarAuditoria()">
        <h2>Auditoria</h2>
        <label>Tabla
          <select name="tabla" [(ngModel)]="auditoriaFiltro.tabla">
            <option value="">Todas</option>
            <option value="usuario">Usuario</option>
            <option value="usuarioxroles">Usuario roles</option>
            <option value="clientes">Clientes</option>
            <option value="reservas">Reservas</option>
            <option value="pagos">Pagos</option>
            <option value="facturas">Facturas</option>
          </select>
        </label>
        <label>Operacion
          <select name="operacion" [(ngModel)]="auditoriaFiltro.operacion">
            <option value="">Todas</option>
            <option value="INSERT">INSERT</option>
            <option value="UPDATE">UPDATE</option>
            <option value="DELETE">DELETE</option>
          </select>
        </label>
        <label>Usuario <input name="audUsuario" maxlength="100" [(ngModel)]="auditoriaFiltro.usuario" /></label>
        <label>Desde UTC <input type="datetime-local" name="desdeUtc" [(ngModel)]="auditoriaFiltro.desdeUtc" /></label>
        <label>Hasta UTC <input type="datetime-local" name="hastaUtc" [(ngModel)]="auditoriaFiltro.hastaUtc" /></label>
        <button class="btn" type="submit">Consultar</button>
      </form>

      <div class="panel stack" [hidden]="tab() !== 'auditoria'">
        <h2>Resultados auditoria</h2>
        <input name="buscarAuditoria" placeholder="Buscar auditoria" [(ngModel)]="busquedaAuditoria" />
        @for (item of auditoriaFiltrada(); track item.id || $index) {
          <div class="compact-row">
            <span>
              <strong>{{ item.tabla || item.aud_tabla || 'Operacion' }} · {{ item.operacion || item.aud_operacion || '-' }}</strong>
              <small>{{ item.usuario || item.aud_usuario || '-' }} · {{ item.fecha_utc || item.fechaUtc || item.aud_fecha_utc || '-' }}</small>
            </span>
            <span>{{ item.registro_id || item.registroId || item.id || item.aud_id || '' }}</span>
          </div>
        } @empty {
          <p class="muted">Sin registros de auditoria cargados.</p>
        }
      </div>

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `
})
export class AdminSeguridadPageComponent implements OnInit {
  roles = signal<any[]>([]);
  usuarios = signal<any[]>([]);
  seleccionado = signal<any | null>(null);
  auditoria = signal<any[]>([]);
  tab = signal<'usuarios' | 'gestion' | 'auditoria'>('usuarios');
  mensaje = signal('');
  busquedaUsuarios = '';
  busquedaAuditoria = '';
  usuario = this.nuevoUsuario();
  gestion = { guid: '', usuarioId: 0, rolIds: [] as number[] };
  auditoriaFiltro = { tabla: '', operacion: '', usuario: '', desdeUtc: '', hastaUtc: '', page: 1, limit: 20 };

  constructor(private readonly api: ApiService, private readonly notifications: NotificationService) {}

  ngOnInit() {
    this.cargar();
  }

  cargar() {
    this.api.roles().subscribe((response) => this.roles.set(this.itemsFromResponse(response.data)));
    this.api.adminUsuarios().subscribe((response) => this.usuarios.set(this.itemsFromResponse(response.data)));
    this.cargarAuditoria();
  }

  crearUsuario() {
    if (!this.usuario.correo || !this.usuario.password || !this.usuario.numeroIdentificacion) {
      this.notifications.error('Correo, password e identificacion son obligatorios.');
      return;
    }

    this.api.adminClientes().subscribe({
      next: (clientesResponse) => {
        const clientes = this.itemsFromResponse(clientesResponse.data);
        const duplicado = clientes.find((cliente) =>
          String(cliente.numeroIdentificacion ?? cliente.numero_identificacion ?? '') === this.usuario.numeroIdentificacion
        );
        if (duplicado) {
          this.notifications.error('Ya existe un cliente con esa identificacion. No se crea login para evitar duplicados.');
          return;
        }

        const request = { login: this.usuario.correo, password: this.usuario.password, rolIds: this.usuario.rolIds };
        this.api.crearUsuario(request).subscribe({
          next: (usuarioResponse) => {
            const usuarioGuid = (usuarioResponse.data as any)?.guid;
            if (!usuarioGuid) {
              this.notifications.error('El usuario fue creado, pero no se pudo obtener su GUID para vincular el cliente.');
              this.cargar();
              return;
            }

            this.api.adminCrearClienteExterno({
              usuarioGuid,
              tipoIdentificacion: this.usuario.tipoIdentificacion,
              numeroIdentificacion: this.usuario.numeroIdentificacion,
              nombres: this.usuario.nombres,
              apellidos: this.usuario.apellidos,
              razonSocial: this.usuario.razonSocial,
              correo: this.usuario.correo,
              telefono: this.usuario.telefono,
              direccion: this.usuario.direccion
            }).subscribe({
              next: () => {
                this.mensaje.set('Usuario y persona creados.');
                this.notifications.success('Usuario y persona creados.');
                this.usuario = this.nuevoUsuario();
                this.cargar();
              },
              error: (error) => {
                this.notifications.error(this.mensajeError(error, 'El usuario fue creado, pero no se pudo registrar el cliente vinculado.'));
                this.cargar();
              }
            });
          },
          error: (error) => this.notifications.error(this.mensajeError(error, 'No se pudo crear el usuario.'))
        });
      },
      error: (error) => this.notifications.error(this.mensajeError(error, 'No se pudo validar si el cliente ya existe.'))
    });
  }

  buscarUsuario() {
    if (!this.gestion.guid) return;
    this.api.obtenerUsuario(this.gestion.guid).subscribe((response) => {
      this.seleccionado.set(response.data);
      this.prepararRolesGestion(response.data);
    });
  }

  seleccionarUsuario() {
    const usuario = this.usuarios().find((item) => item.guid === this.gestion.guid);
    if (usuario) this.usarUsuario(usuario);
  }

  usarUsuario(usuario: any) {
    this.seleccionado.set(usuario);
    this.gestion.guid = usuario.guid;
    this.gestion.usuarioId = usuario.id;
    this.prepararRolesGestion(usuario);
  }

  estadoUsuario(estado: string) {
    if (!this.gestion.guid) return;
    if (this.usuarioProtegido(this.seleccionado())) {
      this.notifications.error('El administrador principal no puede desactivarse.');
      return;
    }

    this.api.cambiarEstadoUsuario(this.gestion.guid, estado).subscribe({
      next: () => {
        this.mensaje.set('Estado actualizado.');
        this.notifications.success('Estado actualizado.');
        this.cargar();
      },
      error: (error) => this.notifications.error(this.mensajeError(error, 'No se pudo cambiar el estado.'))
    });
  }

  asignarRoles() {
    if (this.usuarioProtegido(this.seleccionado())) {
      this.notifications.error('El administrador principal no puede cambiar de rol.');
      return;
    }

    if (!this.gestion.guid || this.gestion.rolIds.length === 0) {
      this.notifications.error('Selecciona un usuario y al menos un rol.');
      return;
    }

    this.api.cambiarRolesUsuario(this.gestion.guid, this.gestion.rolIds).subscribe({
      next: () => {
        this.mensaje.set('Roles actualizados.');
        this.notifications.success('Roles actualizados.');
        this.cargar();
      },
      error: (error) => this.notifications.error(this.mensajeError(error, 'No se pudieron actualizar los roles.'))
    });
  }

  cargarAuditoria() {
    const filtro = {
      ...this.auditoriaFiltro,
      desdeUtc: this.toUtcQuery(this.auditoriaFiltro.desdeUtc),
      hastaUtc: this.toUtcQuery(this.auditoriaFiltro.hastaUtc)
    };
    this.api.auditoria(filtro).subscribe((response) => this.auditoria.set(this.itemsFromResponse(response.data)));
  }

  rolesActivos() {
    return this.roles().filter((rol) => (rol.estado ?? 'A') === 'A');
  }

  toggleRolCreacion(rolId: number) {
    this.usuario.rolIds = this.toggleId(this.usuario.rolIds, rolId);
  }

  toggleRolGestion(rolId: number) {
    this.gestion.rolIds = this.toggleId(this.gestion.rolIds, rolId);
  }

  soloNumerosTelefono() {
    this.usuario.telefono = this.usuario.telefono.replace(/\D/g, '').slice(0, 10);
  }

  etiquetaUsuario(usuario: any) {
    return usuario.cliente_nombre || usuario.clienteNombre || usuario.login || usuario.usr_login || 'Usuario';
  }

  rolesTexto(usuario: any) {
    return (usuario.roles || []).join(', ') || 'Sin roles';
  }

  usuarioProtegido(usuario: any) {
    const login = usuario?.login || usuario?.usr_login || '';
    return login.toLowerCase() === 'admin@travelofyourdreams.local';
  }

  usuarioResumen(usuario: any) {
    return [
      { label: 'Persona', value: usuario.cliente_nombre || usuario.clienteNombre || '-' },
      { label: 'Login', value: usuario.login || usuario.usr_login || '-' },
      { label: 'Identificacion', value: usuario.cliente_identificacion || usuario.clienteIdentificacion || '-' },
      { label: 'Correo', value: usuario.cliente_correo || usuario.clienteCorreo || usuario.login || '-' },
      { label: 'Estado', value: usuario.estado || usuario.usr_estado || '-' },
      { label: 'Roles', value: this.rolesTexto(usuario) }
    ];
  }

  usuariosFiltrados() {
    const q = this.busquedaUsuarios.toLowerCase();
    return this.itemsFromResponse(this.usuarios()).filter((usuario) => !q || `${this.etiquetaUsuario(usuario)} ${usuario.login} ${this.rolesTexto(usuario)} ${usuario.estado}`.toLowerCase().includes(q));
  }

  auditoriaFiltrada() {
    const q = this.busquedaAuditoria.toLowerCase();
    return this.itemsFromResponse(this.auditoria()).filter((item) => !q || `${item.tabla || item.aud_tabla} ${item.operacion || item.aud_operacion} ${item.usuario || item.aud_usuario}`.toLowerCase().includes(q));
  }

  private itemsFromResponse(value: any): any[] {
    if (Array.isArray(value)) return value;
    if (Array.isArray(value?.items)) return value.items;
    if (Array.isArray(value?.data)) return value.data;
    return [];
  }

  private prepararRolesGestion(usuario: any) {
    const nombres = usuario?.roles || [];
    this.gestion.usuarioId = usuario?.id || 0;
    this.gestion.rolIds = this.roles()
      .filter((rol) => nombres.some((nombre: string) => nombre === rol.descripcion || nombre === rol.nombre || nombre === rol.name))
      .map((rol) => rol.id);
  }

  private toggleId(ids: number[], id: number) {
    return ids.includes(id) ? ids.filter((item) => item !== id) : [...ids, id];
  }

  private toUtcQuery(value: string) {
    return value ? new Date(value).toISOString() : '';
  }

  private mensajeError(error: any, fallback: string) {
    const body = error?.error;
    if (typeof body === 'string' && body.trim()) return body;
    if (Array.isArray(body?.details) && body.details.length) return body.details.filter(Boolean).join(' ');
    if (typeof body?.error === 'string' && body.error.trim()) return body.error;
    if (typeof body?.message === 'string' && body.message.trim()) return body.message;
    if (typeof body?.title === 'string' && body.title.trim()) return body.title;
    if (typeof error?.message === 'string' && error.message.trim()) return error.message;
    return fallback;
  }

  private nuevoUsuario() {
    return {
      login: '',
      password: '',
      rolIds: [] as number[],
      tipoIdentificacion: 'CEDULA',
      numeroIdentificacion: '',
      nombres: '',
      apellidos: '',
      razonSocial: '',
      correo: '',
      telefono: '',
      direccion: ''
    };
  }
}
