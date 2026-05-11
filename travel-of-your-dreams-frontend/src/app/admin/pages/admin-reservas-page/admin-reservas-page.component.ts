import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { Reserva } from '../../../shared/models/reserva.model';

@Component({
  selector: 'app-admin-reservas-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div>
          <h1>Reservas</h1>
          <p class="muted">Gestion operativa de reservas, expiracion y facturacion.</p>
        </div>
        <span class="actions">
          <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
          <button class="btn" type="button" (click)="expirar()">Expirar pendientes</button>
        </span>
      </header>

      <div class="grid two">
        <form class="panel form-grid" (ngSubmit)="crearReserva()">
          <h2>Crear reserva por cliente</h2>
          <label class="wide">Tipo de cliente
            <select name="modoCliente" [(ngModel)]="modoCliente" (change)="cambiarModoCliente()">
              <option value="registrado">Cliente registrado</option>
              <option value="externo">Cliente externo</option>
            </select>
          </label>

          @if (modoCliente === 'registrado') {
            <label class="wide">Cliente
              <select name="clienteGuid" [(ngModel)]="nueva.clienteGuid" required>
                <option value="">Selecciona cliente</option>
                @for (cliente of clientes(); track cliente.guid) {
                  <option [value]="cliente.guid">{{ nombreCliente(cliente) }}</option>
                }
              </select>
            </label>
          } @else {
            <div class="wide nested-form">
              <h3>Datos del cliente externo y facturacion</h3>
              <label>Tipo ID
                <select name="extTipo" [(ngModel)]="clienteExterno.tipoIdentificacion">
                  <option value="CEDULA">CEDULA</option>
                  <option value="RUC">RUC</option>
                  <option value="PASAPORTE">PASAPORTE</option>
                </select>
              </label>
              <label>Numero ID <input name="extNumero" maxlength="20" [(ngModel)]="clienteExterno.numeroIdentificacion" required /></label>
              <label>Nombre <input name="extNombre" maxlength="100" [(ngModel)]="clienteExterno.nombre" required /></label>
              <label>Apellido <input name="extApellido" maxlength="100" [(ngModel)]="clienteExterno.apellido" /></label>
              <label class="wide">Razon social <input name="extRazon" maxlength="200" [(ngModel)]="clienteExterno.razonSocial" /></label>
              <label>Correo <input type="email" name="extCorreo" maxlength="150" [(ngModel)]="clienteExterno.correo" required /></label>
              <label>Telefono <input name="extTelefono" maxlength="10" inputmode="numeric" pattern="[0-9]*" [(ngModel)]="clienteExterno.telefono" (input)="soloNumerosTelefono('externo')" /></label>
              <label class="wide">Direccion <input name="extDireccion" maxlength="300" [(ngModel)]="clienteExterno.direccion" /></label>
            </div>
          }

          <label class="wide">Atraccion
            <select name="atraccionGuid" [(ngModel)]="nueva.atraccionGuid" (change)="cargarOpcionesAtraccion()" required>
              <option value="">Selecciona atraccion</option>
              @for (atraccion of atracciones(); track atraccion.guid) {
                <option [value]="atraccion.guid">{{ atraccion.nombre }}</option>
              }
            </select>
          </label>

          <label>Fecha
            <input type="date" name="fecha" [min]="fechaMinima" [(ngModel)]="nueva.fecha" (change)="cargarHorariosAtraccion()" required />
          </label>

          <label class="wide">Horario
            <select name="horGuid" [(ngModel)]="nueva.horGuid" required>
              <option value="">Selecciona horario</option>
              @for (horario of horariosDisponibles(); track horario.guid) {
                <option [value]="horario.guid">{{ horario.hora_inicio || horario.horaInicio }}{{ (horario.hora_fin || horario.horaFin) ? ' - ' + (horario.hora_fin || horario.horaFin) : '' }} - cupos {{ horario.cupos_disponibles || horario.cuposDisponibles }}</option>
              }
            </select>
          </label>

          <label class="wide">Ticket
            <select name="tckGuid" [(ngModel)]="nueva.tckGuid" required>
              <option value="">Selecciona ticket</option>
              @for (ticket of ticketsDisponibles(); track ticket.guid) {
                <option [value]="ticket.guid">{{ ticket.titulo }} - {{ ticket.tipo_participante || ticket.tipoParticipante }} - {{ ticket.precio }} {{ ticket.moneda }}</option>
              }
            </select>
          </label>

          <label>Cantidad <input type="number" min="1" name="cantidad" [(ngModel)]="nueva.cantidad" required /></label>
          <label>IVA % <input type="number" min="0" name="porcentajeIva" [(ngModel)]="nueva.porcentajeIva" /></label>
          <label>Expira en minutos <input type="number" min="1" name="expiracionMinutos" [(ngModel)]="nueva.expiracionMinutos" /></label>
          <label>Canal <input name="origenCanal" [(ngModel)]="nueva.origenCanal" /></label>
          <div class="actions wide"><button class="btn" type="submit">Crear reserva</button></div>
        </form>

        <form class="panel form-grid" (ngSubmit)="generarFactura()">
          <h2>Generar factura</h2>
          <label class="wide">Reserva
            <select name="factReservaGuid" [(ngModel)]="factura.reservaGuid" (change)="seleccionarReservaFactura()" required>
              <option value="">Selecciona reserva</option>
              @for (reserva of reservas(); track reserva.guid) {
                <option [value]="reserva.guid">{{ reserva.codigo }} - {{ reserva.total }} {{ reserva.moneda }}</option>
              }
            </select>
          </label>
          <label class="wide">Datos facturacion
            <select name="datosFacturacionGuid" [(ngModel)]="factura.datosFacturacionGuid" (change)="seleccionarDatoFacturacion()">
              <option value="">Llenar datos para esta factura</option>
              @for (dato of datosFacturacionFactura(); track dato.guid) {
                <option [value]="dato.guid">{{ etiquetaDatoFacturacion(dato) }}</option>
              }
            </select>
          </label>
          <label>Tipo ID
            <select name="factTipo" [(ngModel)]="facturacionForm.tipoIdentificacion" [disabled]="!!factura.datosFacturacionGuid">
              <option value="CEDULA">CEDULA</option>
              <option value="RUC">RUC</option>
              <option value="PASAPORTE">PASAPORTE</option>
            </select>
          </label>
          <label>Numero ID <input name="factNumero" maxlength="30" [(ngModel)]="facturacionForm.numeroIdentificacion" [disabled]="!!factura.datosFacturacionGuid" /></label>
          <label>Nombre <input name="factNombre" maxlength="100" [(ngModel)]="facturacionForm.nombre" [disabled]="!!factura.datosFacturacionGuid" /></label>
          <label>Apellido <input name="factApellido" maxlength="100" [(ngModel)]="facturacionForm.apellido" [disabled]="!!factura.datosFacturacionGuid" /></label>
          <label class="wide">Razon social <input name="factRazon" maxlength="200" [(ngModel)]="facturacionForm.razonSocial" [disabled]="!!factura.datosFacturacionGuid" /></label>
          <label>Correo <input name="factCorreo" maxlength="150" [(ngModel)]="facturacionForm.correo" [disabled]="!!factura.datosFacturacionGuid" /></label>
          <label>Telefono <input name="factTelefono" maxlength="10" inputmode="numeric" pattern="[0-9]*" [(ngModel)]="facturacionForm.telefono" [disabled]="!!factura.datosFacturacionGuid" (input)="soloNumerosTelefono('factura')" /></label>
          <label class="wide">Direccion <input name="factDireccion" maxlength="300" [(ngModel)]="facturacionForm.direccion" [disabled]="!!factura.datosFacturacionGuid" /></label>
          <label class="wide">Observacion <input name="observacion" [(ngModel)]="factura.observacion" /></label>
          <div class="actions wide"><button class="btn" type="submit">Generar</button></div>
        </form>
      </div>

      <div class="table panel">
        <input name="buscarReservas" placeholder="Buscar por codigo, estado o cliente" [(ngModel)]="busquedaReservas" />
        <div class="row head">
          <span>Codigo</span><span>Total</span><span>Estado</span><span>Acciones</span>
        </div>
        @for (reserva of reservasFiltradas(); track reserva.guid) {
          <div class="row">
            <span><strong>{{ reserva.codigo }}</strong><small>{{ reserva.guid }}</small></span>
            <span>{{ reserva.total }} {{ reserva.moneda }}</span>
            <span>{{ reserva.estado }}</span>
            <span class="actions">
              <button class="btn secondary" type="button" (click)="ver(reserva.guid)">Ver</button>
              <select class="state-select" [ngModel]="reserva.estado" [ngModelOptions]="{standalone: true}" (ngModelChange)="estado(reserva.guid, $event)">
                <option value="PENDIENTE">Pendiente</option>
                <option value="PAGADA">Pagada</option>
                <option value="CONFIRMADA">Confirmada</option>
                <option value="CANCELADA">Cancelada</option>
                <option value="EXPIRADA">Expirada</option>
                <option value="USADA">Usada</option>
                <option value="NO_SHOW">No se presento</option>
              </select>
            </span>
          </div>
        } @empty {
          <p class="muted">No hay reservas para mostrar.</p>
        }
      </div>

      @if (detalle()) {
        <div class="panel stack">
          <header class="section-head">
            <div>
              <h2>{{ detalle()?.codigo }}</h2>
              <p class="muted">{{ detalle()?.estado }} - {{ detalle()?.total }} {{ detalle()?.moneda }}</p>
            </div>
          </header>
          <div class="info-grid">
            @for (item of reservaResumen(detalle()); track item.label) {
              <div class="info-item">
                <span>{{ item.label }}</span>
                <strong>{{ item.value }}</strong>
              </div>
            }
          </div>
          <h3>Tickets</h3>
          @for (linea of detalle()?.detalles || []; track linea.guid || linea.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ linea.titulo || ('Ticket #' + (linea.ticket_id || linea.ticketId || '-')) }}</strong>
                <small>Cantidad {{ linea.cantidad }} - {{ linea.precio_unitario || linea.precioUnitario }} USD c/u</small>
              </span>
              <strong>{{ linea.subtotal }} USD</strong>
            </div>
          } @empty {
            <p class="muted">Sin tickets registrados.</p>
          }
        </div>
      }

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `,
  styles: [`
    :host { display: block; }
    .form-grid { align-items: start; grid-template-columns: repeat(2, minmax(0, 1fr)); }
    .nested-form { background: var(--surface-muted); border: 1px solid var(--line); border-radius: 8px; display: grid; gap: 14px; grid-template-columns: repeat(2, minmax(0, 1fr)); padding: 16px; }
    .nested-form h3 { grid-column: 1 / -1; margin: 0; }
    .table { overflow-x: auto; }
    .table .row { min-width: 720px; }
    @media (max-width: 920px) {
      .grid.two, .form-grid, .nested-form { grid-template-columns: 1fr; }
      .admin-header { align-items: stretch; flex-direction: column; }
      .admin-header .actions { justify-content: stretch; }
      .admin-header .actions .btn { flex: 1; }
      .wide, .nested-form h3 { grid-column: auto; }
    }
    @media (max-width: 560px) {
      .page { padding-inline: 12px; }
      .panel { padding: 16px; }
      .actions { flex-direction: column; }
      .actions .btn, .state-select { width: 100%; }
      .info-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class AdminReservasPageComponent implements OnInit {
  reservas = signal<Reserva[]>([]);
  clientes = signal<any[]>([]);
  atracciones = signal<any[]>([]);
  horariosDisponibles = signal<any[]>([]);
  ticketsDisponibles = signal<any[]>([]);
  datosFacturacionFactura = signal<any[]>([]);
  detalle = signal<any | null>(null);
  mensaje = signal('');
  fechaMinima = this.today();
  modoCliente: 'registrado' | 'externo' = 'registrado';
  busquedaReservas = '';
  nueva = {
    clienteGuid: '',
    atraccionGuid: '',
    fecha: this.today(),
    horGuid: '',
    tckGuid: '',
    cantidad: 1,
    porcentajeIva: 15,
    expiracionMinutos: 15,
    origenCanal: 'ADMIN'
  };
  factura = { reservaGuid: '', datosFacturacionGuid: '', observacion: '', origenCanal: 'ADMIN' };
  facturacionForm = this.nuevoDatoFacturacion();
  clienteExterno = this.nuevoDatoFacturacion();

  constructor(
    private readonly api: ApiService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.cargar();
  }

  cargar() {
    this.api.adminReservas().subscribe((response) => this.reservas.set(response.data));
    this.api.adminClientes().subscribe((response) => this.clientes.set(response.data));
    this.api.adminAtracciones().subscribe((response) => this.atracciones.set(response.data));
  }

  cargarOpcionesAtraccion() {
    this.nueva.horGuid = '';
    this.nueva.tckGuid = '';
    this.cargarHorariosAtraccion();
    this.cargarTicketsAtraccion();
  }

  cargarHorariosAtraccion() {
    this.nueva.horGuid = '';
    if (!this.nueva.atraccionGuid) {
      this.horariosDisponibles.set([]);
      return;
    }

    if (this.nueva.fecha < this.fechaMinima) {
      this.nueva.fecha = this.fechaMinima;
    }

    this.api.listarHorarios(this.nueva.atraccionGuid, this.nueva.fecha).subscribe((response) => {
      this.horariosDisponibles.set(response.data);
      this.nueva.horGuid = response.data[0]?.guid ?? '';
    });
  }

  cargarTicketsAtraccion() {
    this.nueva.tckGuid = '';
    if (!this.nueva.atraccionGuid) {
      this.ticketsDisponibles.set([]);
      return;
    }

    this.api.obtenerAtraccion(this.nueva.atraccionGuid).subscribe((response) => {
      this.ticketsDisponibles.set(response.data.tickets || []);
      this.nueva.tckGuid = response.data.tickets?.[0]?.guid ?? '';
    });
  }

  ver(guid: string) {
    this.api.adminReserva(guid).subscribe((response) => this.detalle.set(response.data));
  }

  crearReserva() {
    if (this.nueva.fecha < this.fechaMinima) {
      this.notifications.error('La fecha de reserva no puede ser anterior a hoy.');
      return;
    }

    if (this.modoCliente === 'externo') {
      this.crearReservaClienteExterno();
      return;
    }

    if (!this.nueva.clienteGuid) {
      this.notifications.error('Selecciona un cliente.');
      return;
    }

    this.crearReservaParaCliente(this.nueva.clienteGuid);
  }

  private crearReservaParaCliente(clienteGuid: string, datosFacturacionGuid?: string) {
    this.api.adminCrearReserva({
      clienteGuid,
      horGuid: this.nueva.horGuid,
      fecha: this.nueva.fecha,
      lineas: [{ tckGuid: this.nueva.tckGuid, cantidad: this.nueva.cantidad }],
      origenCanal: this.nueva.origenCanal,
      expiracionMinutos: this.nueva.expiracionMinutos,
      porcentajeIva: this.nueva.porcentajeIva
    }).subscribe({
      next: () => {
        this.mensaje.set('Reserva creada.');
        this.notifications.success('Reserva creada.');
        this.cargar();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  private crearReservaClienteExterno() {
    if (!this.clienteExterno.numeroIdentificacion || !this.clienteExterno.nombre || !this.clienteExterno.correo) {
      this.notifications.error('Completa los datos del cliente externo.');
      return;
    }

    this.api.adminCrearClienteExterno({
      tipoIdentificacion: this.clienteExterno.tipoIdentificacion,
      numeroIdentificacion: this.clienteExterno.numeroIdentificacion,
      nombres: this.clienteExterno.nombre,
      apellidos: this.clienteExterno.apellido,
      razonSocial: this.clienteExterno.razonSocial,
      correo: this.clienteExterno.correo,
      telefono: this.clienteExterno.telefono,
      direccion: this.clienteExterno.direccion
    }).subscribe({
      next: (usuarioResponse) => {
        const clienteGuid = (usuarioResponse.data as any)?.guid ?? (usuarioResponse.data as any)?.cliente_guid ?? (usuarioResponse.data as any)?.clienteGuid;
        if (!clienteGuid) {
          this.notifications.error('No se pudo obtener el cliente externo creado.');
          return;
        }

        this.api.adminCrearDatosFacturacionCliente(clienteGuid, this.clienteExterno).subscribe({
          next: (datosResponse) => {
            const datosGuid = (datosResponse.data as any)?.guid;
            this.api.adminCrearReserva({
              clienteGuid,
              horGuid: this.nueva.horGuid,
              fecha: this.nueva.fecha,
              lineas: [{ tckGuid: this.nueva.tckGuid, cantidad: this.nueva.cantidad }],
              origenCanal: this.nueva.origenCanal,
              expiracionMinutos: this.nueva.expiracionMinutos,
              porcentajeIva: this.nueva.porcentajeIva
            }).subscribe({
              next: (reservaResponse) => {
                const reservaGuid = (reservaResponse.data as any)?.guid;
                if (!reservaGuid || !datosGuid) {
                  this.notifications.success('Reserva creada. No se pudo generar la factura automaticamente.');
                  this.cargar();
                  return;
                }

                this.api.generarFactura({
                  reservaGuid,
                  datosFacturacionGuid: datosGuid,
                  observacion: 'Factura generada para cliente externo desde panel admin.',
                  origenCanal: 'ADMIN'
                }).subscribe({
                  next: () => {
                    this.mensaje.set('Reserva y factura creadas para cliente externo.');
                    this.notifications.success('Reserva y factura creadas.');
                    this.clienteExterno = this.nuevoDatoFacturacion();
                    this.cargar();
                  },
                  error: (error: Error) => this.notifications.error(error.message)
                });
              },
              error: (error: Error) => this.notifications.error(error.message)
            });
          },
          error: (error: Error) => this.notifications.error(error.message)
        });
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  cambiarModoCliente() {
    this.nueva.clienteGuid = '';
    this.clienteExterno = this.nuevoDatoFacturacion();
  }

  estado(guid: string, estado: string) {
    this.api.cambiarEstadoReserva(guid, estado, 'Actualizado desde panel admin').subscribe({
      next: () => {
        this.mensaje.set('Estado de reserva actualizado.');
        this.notifications.success('Estado de reserva actualizado.');
        this.cargar();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  expirar() {
    this.api.expirarReservasPendientes().subscribe((response) => {
      this.mensaje.set(`Reservas expiradas: ${response.data.total}`);
      this.notifications.success(`Reservas expiradas: ${response.data.total}`);
      this.cargar();
    });
  }

  generarFactura() {
    const generar = (datosFacturacionGuid: string | null) => this.api.generarFactura({
      reservaGuid: this.factura.reservaGuid,
      datosFacturacionGuid,
      observacion: this.factura.observacion,
      origenCanal: this.factura.origenCanal
    }).subscribe({
      next: (response) => {
        this.mensaje.set(`Factura generada: ${response.data.factura_guid}`);
        this.notifications.success('Factura generada.');
      },
      error: (error: Error) => this.notifications.error(error.message)
    });

    if (this.factura.datosFacturacionGuid) {
      generar(this.factura.datosFacturacionGuid);
      return;
    }

    const cliente = this.clienteDeReservaSeleccionada();
    if (!cliente?.guid) {
      this.notifications.error('Selecciona una reserva con cliente valido.');
      return;
    }

    if (!this.facturacionForm.numeroIdentificacion || !this.facturacionForm.nombre || !this.facturacionForm.correo) {
      this.notifications.error('Completa los datos de facturacion para generar la factura.');
      return;
    }

    this.api.adminCrearDatosFacturacionCliente(cliente.guid, this.facturacionForm).subscribe({
      next: (response) => {
        const guid = (response.data as any)?.guid;
        if (!guid) {
          this.notifications.error('No se pudo obtener el dato de facturacion creado.');
          return;
        }
        this.factura.datosFacturacionGuid = guid;
        generar(guid);
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  seleccionarReservaFactura() {
    this.factura.datosFacturacionGuid = '';
    this.facturacionForm = this.nuevoDatoFacturacion();
    this.datosFacturacionFactura.set([]);

    const cliente = this.clienteDeReservaSeleccionada();
    if (!cliente?.guid) return;

    this.api.adminDatosFacturacionCliente(cliente.guid).subscribe((response) => {
      this.datosFacturacionFactura.set(response.data);
      const primero = response.data[0] as any;
      if (primero?.guid) {
        this.factura.datosFacturacionGuid = primero.guid;
        this.seleccionarDatoFacturacion();
      }
    });
  }

  seleccionarDatoFacturacion() {
    const dato = this.datosFacturacionFactura().find((item) => item.guid === this.factura.datosFacturacionGuid);
    this.facturacionForm = dato ? {
      tipoIdentificacion: dato.tipo_identificacion || dato.tipoIdentificacion || 'CEDULA',
      numeroIdentificacion: dato.numero_identificacion || dato.numeroIdentificacion || '',
      nombre: dato.nombre || '',
      apellido: dato.apellido || '',
      razonSocial: dato.razon_social || dato.razonSocial || '',
      correo: dato.correo || '',
      telefono: dato.telefono || '',
      direccion: dato.direccion || ''
    } : this.nuevoDatoFacturacion();
  }

  reservaResumen(value: any) {
    return [
      { label: 'Cliente ID', value: value?.cliente_id ?? value?.clienteId ?? '-' },
      { label: 'Horario ID', value: value?.horario_id ?? value?.horarioId ?? '-' },
      { label: 'Subtotal', value: `${value?.subtotal ?? '-'} ${value?.moneda ?? 'USD'}` },
      { label: 'IVA', value: `${value?.valor_iva ?? value?.valorIva ?? '-'} ${value?.moneda ?? 'USD'}` },
      { label: 'Expira', value: this.formatearFecha(value?.fecha_expiracion_utc ?? value?.fechaExpiracionUtc) }
    ];
  }

  nombreCliente(cliente: any) {
    const nombre = [cliente.nombres, cliente.apellidos].filter(Boolean).join(' ');
    return `${nombre || cliente.razon_social || cliente.correo} - ${cliente.correo}`;
  }

  etiquetaDatoFacturacion(dato: any) {
    const nombre = dato.razon_social || dato.razonSocial || [dato.nombre, dato.apellido].filter(Boolean).join(' ');
    return `${nombre || dato.correo} - ${dato.numero_identificacion || dato.numeroIdentificacion}`;
  }

  soloNumerosTelefono(origen: 'externo' | 'factura') {
    if (origen === 'externo') {
      this.clienteExterno.telefono = this.clienteExterno.telefono.replace(/\D/g, '').slice(0, 10);
      return;
    }

    this.facturacionForm.telefono = this.facturacionForm.telefono.replace(/\D/g, '').slice(0, 10);
  }

  reservasFiltradas() {
    const q = this.busquedaReservas.toLowerCase();
    return this.reservas().filter((reserva) => !q || `${reserva.codigo} ${reserva.estado} ${this.nombreClientePorId(reserva.cliente_id ?? (reserva as any).clienteId)}`.toLowerCase().includes(q));
  }

  nombreClientePorId(id: number) {
    const cliente = this.clientes().find((item) => (item.id ?? item.cliente_id ?? item.clienteId) === id);
    return cliente ? this.nombreCliente(cliente) : '';
  }

  private clienteDeReservaSeleccionada() {
    const reserva = this.reservas().find((item) => item.guid === this.factura.reservaGuid);
    const clienteId = reserva?.cliente_id ?? (reserva as any)?.clienteId;
    return this.clientes().find((cliente) => (cliente.id ?? cliente.cliente_id ?? cliente.clienteId) === clienteId);
  }

  private nuevoDatoFacturacion() {
    return {
      tipoIdentificacion: 'CEDULA',
      numeroIdentificacion: '',
      nombre: '',
      apellido: '',
      razonSocial: '',
      correo: '',
      telefono: '',
      direccion: ''
    };
  }

  formatearFecha(value: string | null | undefined) {
    if (!value) return '-';
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium', timeStyle: 'short' }).format(date);
  }

  private today() {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 10);
  }
}
