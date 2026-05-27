import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { Reserva } from '../../../shared/models/reserva.model';

@Component({
  selector: 'app-mis-reservas-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div>
          <h1>Mis reservas</h1>
          <p class="muted">Consulta, cancelacion, pagos y facturas.</p>
        </div>
        <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
      </header>

      <nav class="app-tabs activity-tabs" aria-label="Actividad del cliente">
        <button class="tab-button" [class.active]="actividad() === 'reservas'" type="button" (click)="actividad.set('reservas')">Mis reservas</button>
        <button class="tab-button" [class.active]="actividad() === 'facturas'" type="button" (click)="actividad.set('facturas')">Mis facturas</button>
        <button class="tab-button" [class.active]="actividad() === 'pagos'" type="button" (click)="actividad.set('pagos')">Pagos realizados</button>
      </nav>

      @if (actividad() === 'reservas') {
      <div class="activity-layout">
        <div class="panel stack">
          @for (reserva of reservas(); track reserva.guid) {
            <article class="compact-row activity-item">
              <span>
                <strong>{{ reserva.codigo }}</strong>
                <small>{{ reserva.estado }} · {{ reserva.total }} {{ reserva.moneda }}</small>
              </span>
              <span class="actions">
                <button class="btn secondary" type="button" (click)="ver(reserva.guid)">Ver</button>
                @if (reserva.estado === 'PENDIENTE') {
                  <button class="btn danger" type="button" (click)="abrirCancelacion(reserva.guid)">Cancelar</button>
                }
              </span>
            </article>
          } @empty {
            <div>No tienes reservas registradas.</div>
          }
        </div>

        @if (cancelacion.reservaGuid) {
          <form class="panel form-grid" (ngSubmit)="confirmarCancelacion()">
            <h2>Cancelar reserva</h2>
            <p class="muted wide">Indica el motivo de cancelacion para liberar los cupos y actualizar el estado.</p>
            <label class="wide">Motivo
              <textarea name="motivoCancelacion" maxlength="300" rows="4" [(ngModel)]="cancelacion.motivo" required></textarea>
            </label>
            <div class="actions wide">
              <button class="btn danger" type="submit">Confirmar cancelacion</button>
              <button class="btn secondary" type="button" (click)="cerrarCancelacion()">Cerrar</button>
            </div>
          </form>
        }

        <form class="panel form-grid" (ngSubmit)="pagar()">
          <h2>Registrar pago</h2>
          <label class="wide">Reserva
            <select name="reservaGuid" [(ngModel)]="pago.reservaGuid" (change)="seleccionarReservaPago()" required>
              <option value="">Selecciona una reserva pendiente</option>
              @for (reserva of reservasPendientes(); track reserva.guid) {
                <option [value]="reserva.guid">{{ reserva.codigo }} · {{ reserva.total }} {{ reserva.moneda }}</option>
              }
            </select>
          </label>
          <label>Metodo <input name="metodo" maxlength="50" [(ngModel)]="pago.metodo" readonly /></label>
          <label>Monto <input type="number" min="0" name="monto" [(ngModel)]="pago.monto" readonly /></label>
          <label class="wide">Datos facturacion
            <select name="datosFacturacionGuid" [(ngModel)]="pago.datosFacturacionGuid" (change)="seleccionarDatoFacturacion()">
              <option value="">Llenar datos para esta factura</option>
              @for (dato of datosFacturacion(); track dato.guid) {
                <option [value]="dato.guid">{{ dato.razon_social || dato.nombre || dato.correo }} · {{ dato.numero_identificacion }}</option>
              }
            </select>
          </label>
          <div class="wide form-grid nested-form">
            <label>Tipo ID
              <select name="factTipo" [(ngModel)]="facturacionForm.tipoIdentificacion" [disabled]="!!pago.datosFacturacionGuid">
                <option value="CEDULA">Cedula</option>
                <option value="RUC">RUC</option>
                <option value="PASAPORTE">Pasaporte</option>
              </select>
            </label>
            <label>Numero ID <input name="factNumero" maxlength="30" [(ngModel)]="facturacionForm.numeroIdentificacion" [disabled]="!!pago.datosFacturacionGuid" /></label>
            <label>Nombre <input name="factNombre" maxlength="100" [(ngModel)]="facturacionForm.nombre" [disabled]="!!pago.datosFacturacionGuid" /></label>
            <label>Apellido <input name="factApellido" maxlength="100" [(ngModel)]="facturacionForm.apellido" [disabled]="!!pago.datosFacturacionGuid" /></label>
            <label>Razon social <input name="factRazon" maxlength="200" [(ngModel)]="facturacionForm.razonSocial" [disabled]="!!pago.datosFacturacionGuid" /></label>
            <label>Correo <input name="factCorreo" maxlength="150" [(ngModel)]="facturacionForm.correo" [disabled]="!!pago.datosFacturacionGuid" /></label>
            <label>Telefono <input name="factTelefono" maxlength="10" inputmode="numeric" pattern="[0-9]*" [(ngModel)]="facturacionForm.telefono" [disabled]="!!pago.datosFacturacionGuid" (input)="soloNumerosTelefono()" /></label>
            <label class="wide">Direccion <input name="factDireccion" maxlength="300" [(ngModel)]="facturacionForm.direccion" [disabled]="!!pago.datosFacturacionGuid" /></label>
          </div>
          <div class="wide card-form">
            <div class="card-brand-row">
              <span class="card-brand">{{ marcaTarjeta() }}</span>
            </div>
            <label class="wide">Titular
              <input name="cardTitular" maxlength="100" autocomplete="off" [(ngModel)]="tarjeta.titular" />
            </label>
            <label class="wide">Numero de tarjeta
              <input name="cardNumero" maxlength="19" inputmode="numeric" autocomplete="off" placeholder="4111 1111 1111 1111" [(ngModel)]="tarjeta.numero" (input)="formatearTarjeta()" />
            </label>
            <label>Expira
              <input name="cardExpira" maxlength="5" inputmode="numeric" autocomplete="off" placeholder="MM/AA" [(ngModel)]="tarjeta.expira" (input)="formatearExpiracion()" />
            </label>
            <label>CVV
              <input name="cardCvv" maxlength="4" inputmode="numeric" autocomplete="off" type="password" [(ngModel)]="tarjeta.cvv" (input)="soloNumerosCvv()" />
            </label>
          </div>
          <button class="btn wide" type="submit" [disabled]="!pago.reservaGuid">Confirmar pago</button>
        </form>
      </div>
      }

      @if (actividad() === 'facturas') {
        <div class="panel stack activity-panel">
          <h2>Mis facturas</h2>
          @for (item of facturas(); track item.guid || item.id || $index) {
            <div class="compact-row activity-item">
              <span>
                <strong>{{ item.numero || item.guid }}</strong>
                <small>{{ item.estado }} · {{ formatearFecha(item.fecha_emision || item.fechaEmision) }}</small>
              </span>
              <span class="actions">
                <strong>{{ item.total }} {{ item.moneda || 'USD' }}</strong>
                <button class="btn secondary" type="button" (click)="verFactura(item.guid)">Ver</button>
              </span>
            </div>
          } @empty {
            <p class="muted">Aun no tienes facturas registradas.</p>
          }
        </div>
      }

      @if (actividad() === 'pagos') {
        <div class="panel stack activity-panel">
          <h2>Pagos realizados</h2>
          @for (item of pagos(); track item.guid || item.id || $index) {
            <div class="compact-row activity-item">
              <span>
                <strong>{{ item.referencia || 'Pago registrado' }}</strong>
                <small>{{ item.metodo }} · {{ item.estado }} · {{ formatearFecha(item.fecha_utc || item.fechaUtc) }}</small>
              </span>
              <strong>{{ item.monto }} {{ item.moneda || 'USD' }}</strong>
            </div>
          } @empty {
            <p class="muted">Aun no tienes pagos registrados.</p>
          }
        </div>
      }

      @if (detalle()) {
        <div class="panel stack" [class.invoice-doc]="esFactura(detalle())">
          @if (esFactura(detalle())) {
            <div class="invoice-top">
              <div>
                <h1>FACTURA</h1>
                <div class="invoice-company">
                  <strong>Travel of Your Dreams</strong>
                  <span>Servicios turisticos y reservas</span>
                  <span>Quito, Ecuador</span>
                  <span>contacto&#64;travelofyourdreams.com</span>
                </div>
              </div>
              <div class="invoice-seal">TOYD</div>
              <div class="invoice-meta">
                <span><strong>N. de factura:</strong> {{ detalle().numero || '-' }}</span>
                <span><strong>Fecha:</strong> {{ formatearFecha(fechaFactura(detalle())) }}</span>
                <span><strong>Reserva:</strong> {{ reservaFactura(detalle())?.codigo || '-' }}</span>
              </div>
            </div>
          }
          <header class="section-head">
            <div>
              <h2>{{ detalle().codigo || detalle().numero || 'Detalle' }}</h2>
              <p class="muted">{{ detalle().estado }} - {{ detalle().total }} {{ detalle().moneda }}</p>
            </div>
            <span class="actions print-hide">
              @if (esFactura(detalle())) {
                <button class="btn secondary" type="button" (click)="imprimir()">Imprimir</button>
              }
              <button class="btn secondary" type="button" (click)="cerrarDetalle()">Cerrar</button>
            </span>
          </header>
          <div class="info-grid">
            @for (item of reservaResumen(detalle()); track item.label) {
              <div class="info-item">
                <span>{{ item.label }}</span>
                <strong>{{ item.value }}</strong>
              </div>
            }
          </div>
          @if (esFactura(detalle())) {
            <div class="invoice-sections">
              <section>
                <h3>Facturar a</h3>
                <p><strong>{{ facturadoA(detalle()) }}</strong></p>
                <p>{{ facturadoDocumento(detalle()) }}</p>
                <p>{{ facturadoContacto(detalle()) }}</p>
                <p>{{ facturadoDireccion(detalle()) }}</p>
              </section>
              <section>
                <h3>Atraccion</h3>
                <p><strong>{{ atraccionFactura(detalle()).nombre }}</strong></p>
                <p>{{ atraccionFactura(detalle()).fecha }} {{ atraccionFactura(detalle()).hora }}</p>
                <p>Reserva: {{ reservaFactura(detalle())?.codigo || '-' }}</p>
                <p>Estado reserva: {{ reservaFactura(detalle())?.estado || '-' }}</p>
              </section>
              <section>
                <h3>Pago</h3>
                <p><strong>{{ detalle().pago?.metodo || '-' }}</strong></p>
                <p>Referencia: {{ detalle().pago?.referencia || '-' }}</p>
                <p>Estado: {{ detalle().pago?.estado || '-' }}</p>
              </section>
            </div>
          }
          @if (lineasFactura(detalle()).length) {
          <div class="stack">
            <h3>{{ esFactura(detalle()) ? 'Detalle facturado' : 'Tickets' }}</h3>
            @if (esFactura(detalle())) {
              <div class="invoice-table">
                <div class="invoice-row invoice-head">
                  <span>Descripcion</span>
                  <span>Cantidad</span>
                  <span>Precio</span>
                  <span>Total</span>
                </div>
                @for (linea of lineasFactura(detalle()); track linea.guid || linea.ticketGuid || linea.ticket_guid || linea.id || $index) {
                  <div class="invoice-row">
                    <span>{{ linea.titulo || linea.ticketTitulo || linea.ticket_titulo || ('Ticket #' + (linea.ticket_id || linea.ticketId || '-')) }}</span>
                    <span>{{ linea.cantidad }}</span>
                    <span>{{ linea.precio_unitario || linea.precioUnitario }} {{ detalle().moneda || 'USD' }}</span>
                    <span>{{ linea.subtotal }} {{ detalle().moneda || 'USD' }}</span>
                  </div>
                }
                <div class="invoice-total-row"><span>Subtotal</span><strong>{{ detalle().subtotal }} {{ detalle().moneda || 'USD' }}</strong></div>
                <div class="invoice-total-row"><span>Total IVA</span><strong>{{ detalle().valor_iva || detalle().valorIva }} {{ detalle().moneda || 'USD' }}</strong></div>
                <div class="invoice-total-row grand"><span>Total</span><strong>{{ detalle().total }} {{ detalle().moneda || 'USD' }}</strong></div>
              </div>
              <div class="invoice-footer-note">
                <span><strong>Condiciones de pago:</strong> Tarjeta</span>
                <span><strong>Contacto:</strong> Travel of Your Dreams</span>
                <strong>GRACIAS POR SU CONFIANZA</strong>
              </div>
            } @else {
              @for (linea of lineasFactura(detalle()); track linea.guid || linea.ticketGuid || linea.ticket_guid || linea.id || $index) {
                <div class="compact-row">
                  <span>
                    <strong>{{ linea.titulo || linea.ticketTitulo || linea.ticket_titulo || ('Ticket #' + (linea.ticket_id || linea.ticketId || '-')) }}</strong>
                    <small>{{ linea.tipo_participante || linea.tipoParticipante || '' }} - Cantidad {{ linea.cantidad }} - {{ linea.precio_unitario || linea.precioUnitario }} USD c/u</small>
                  </span>
                  <strong>{{ linea.subtotal }} USD</strong>
                </div>
              }
            }
          </div>
          }
        </div>
      }
      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `,
  styles: [`
    .activity-tabs { position: sticky; top: 0; z-index: 2; }
    .activity-layout { align-items: start; display: grid; gap: 18px; grid-template-columns: minmax(0, 1fr) minmax(360px, 0.9fr); }
    .activity-panel { min-height: 240px; }
    .activity-item { min-height: auto; padding: 14px 0; }
    .activity-item span:first-child { min-width: 0; }
    .activity-item small { display: block; margin-top: 4px; }
    .card-form { background: var(--surface-muted); border: 1px solid var(--line); border-radius: 8px; display: grid; gap: 14px; grid-template-columns: repeat(2, minmax(0, 1fr)); padding: 16px; }
    .card-brand-row { display: flex; grid-column: 1 / -1; justify-content: flex-end; }
    .card-brand { align-self: start; background: #0f766e; border-radius: 999px; color: #fff; font-weight: 800; padding: 8px 12px; }
    .section-head .actions { display: flex; flex-wrap: wrap; gap: 10px; justify-content: flex-end; }
    @media (max-width: 980px) {
      .activity-layout { grid-template-columns: 1fr; }
      .activity-tabs { position: static; }
      .card-form { grid-template-columns: 1fr; }
    }
  `]
})
export class MisReservasPageComponent implements OnInit {
  reservas = signal<Reserva[]>([]);
  detalle = signal<any | null>(null);
  pagos = signal<any[]>([]);
  facturas = signal<any[]>([]);
  datosFacturacion = signal<any[]>([]);
  mensaje = signal('');
  actividad = signal<'reservas' | 'facturas' | 'pagos'>('reservas');
  pago = { reservaGuid: '', metodo: 'TARJETA', monto: 0, referencia: '', datosFacturacionGuid: '', origenCanal: 'WEB' };
  facturacionForm = this.nuevoDatoFacturacion();
  tarjeta = { titular: '', numero: '', expira: '', cvv: '' };
  cancelacion = { reservaGuid: '', motivo: '' };

  constructor(
    private readonly api: ApiService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.cargar();
  }

  cargar() {
    this.api.misReservas().subscribe((response) => {
      const reservas = this.itemsFromResponse(response.data) as Reserva[];
      this.reservas.set(reservas);
      if (this.pago.reservaGuid && !reservas.some((reserva) => reserva.guid === this.pago.reservaGuid && reserva.estado === 'PENDIENTE')) {
        this.pago.reservaGuid = '';
        this.pago.monto = 0;
      }
    });
    this.api.misPagos({ limit: 20 }).subscribe((response) => this.pagos.set(this.itemsFromResponse(response.data)));
    this.api.misFacturas({ limit: 20 }).subscribe((response) => this.facturas.set(this.itemsFromResponse(response.data)));
    this.api.misDatosFacturacion().subscribe((response) => {
      this.datosFacturacion.set(this.itemsFromResponse(response.data));
      this.pago.datosFacturacionGuid ||= localStorage.getItem('toyd_facturacion_guid') ?? '';
      this.seleccionarDatoFacturacion();
    });
  }

  ver(guid: string) {
    this.api.obtenerMiReserva(guid).subscribe((response) => {
      this.detalle.set(response.data);
      if (response.data.estado === 'PENDIENTE') {
        this.pago.reservaGuid = guid;
        this.pago.monto = response.data.total;
      }
    });
  }

  reservasPendientes() {
    return this.itemsFromResponse(this.reservas()).filter((reserva) => reserva.estado === 'PENDIENTE') as Reserva[];
  }

  seleccionarReservaPago() {
    const reserva = (this.itemsFromResponse(this.reservas()) as Reserva[]).find((item) => item.guid === this.pago.reservaGuid);
    this.pago.monto = reserva?.total ?? 0;
  }

  abrirCancelacion(guid: string) {
    this.cancelacion = { reservaGuid: guid, motivo: '' };
  }

  cerrarCancelacion() {
    this.cancelacion = { reservaGuid: '', motivo: '' };
  }

  confirmarCancelacion() {
    const motivo = this.cancelacion.motivo.trim();
    if (!this.cancelacion.reservaGuid || !motivo) {
      this.notifications.error('Ingresa el motivo de cancelacion.');
      return;
    }

    this.api.cancelarReserva(this.cancelacion.reservaGuid, motivo).subscribe({
      next: () => {
        this.mensaje.set('Reserva cancelada.');
        this.notifications.success('Reserva cancelada.');
        this.cerrarCancelacion();
        this.cargar();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  pagar() {
    if (!this.pago.reservaGuid) {
      this.notifications.error('Selecciona una reserva pendiente.');
      return;
    }

    if (!this.tarjetaValida()) {
      return;
    }

    const confirmar = (datosFacturacionGuid: string | null) =>
    this.api.confirmarPago({
      ...this.pago,
      metodo: 'TARJETA',
      referencia: this.generarReferenciaPago(),
      monto: this.normalizarMonto(this.pago.monto),
      datosFacturacionGuid
    }).subscribe({
      next: () => {
        this.mensaje.set('Pago confirmado.');
        this.notifications.success('Pago confirmado y factura generada.');
        this.limpiarTarjeta();
        this.cargar();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });

    if (this.pago.datosFacturacionGuid) {
      confirmar(this.pago.datosFacturacionGuid);
      return;
    }

    if (!this.facturacionForm.numeroIdentificacion || !this.facturacionForm.nombre || !this.facturacionForm.correo) {
      this.notifications.error('Completa los datos de facturacion para generar la factura.');
      return;
    }

    this.api.crearDatosFacturacion(this.facturacionForm).subscribe({
      next: (response: any) => {
        const guid = response.data?.guid ?? response.data?.dfac_guid ?? null;
        if (!guid) {
          this.notifications.error('No se pudo obtener el dato de facturacion creado.');
          return;
        }
        this.pago.datosFacturacionGuid = guid;
        confirmar(guid);
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  private normalizarMonto(value: number | string) {
    return Number(String(value).replace(',', '.'));
  }

  private generarReferenciaPago() {
    const random = Math.random().toString(36).slice(2, 8).toUpperCase();
    return `PAY-${Date.now()}-${random}`;
  }

  formatearTarjeta() {
    const digits = this.tarjeta.numero.replace(/\D/g, '').slice(0, 16);
    this.tarjeta.numero = digits.replace(/(.{4})/g, '$1 ').trim();
  }

  formatearExpiracion() {
    const digits = this.tarjeta.expira.replace(/\D/g, '').slice(0, 4);
    this.tarjeta.expira = digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits;
  }

  soloNumerosCvv() {
    this.tarjeta.cvv = this.tarjeta.cvv.replace(/\D/g, '').slice(0, 4);
  }

  marcaTarjeta() {
    const digits = this.tarjeta.numero.replace(/\D/g, '');
    if (digits.startsWith('4')) return 'Visa';
    if (/^5[1-5]/.test(digits)) return 'Mastercard';
    if (/^3[47]/.test(digits)) return 'Amex';
    return 'Tarjeta';
  }

  private tarjetaValida() {
    const digits = this.tarjeta.numero.replace(/\D/g, '');
    if (!this.tarjeta.titular.trim()) {
      this.notifications.error('Ingresa el titular de la tarjeta ficticia.');
      return false;
    }
    if (digits.length < 13) {
      this.notifications.error('Ingresa un numero de tarjeta ficticia valido.');
      return false;
    }
    if (!/^(0[1-9]|1[0-2])\/\d{2}$/.test(this.tarjeta.expira)) {
      this.notifications.error('Ingresa la expiracion ficticia en formato MM/AA.');
      return false;
    }
    if (!/^\d{3,4}$/.test(this.tarjeta.cvv)) {
      this.notifications.error('Ingresa un CVV ficticio valido.');
      return false;
    }
    return true;
  }

  private limpiarTarjeta() {
    this.tarjeta = { titular: '', numero: '', expira: '', cvv: '' };
  }

  verFactura(guid: string) {
    this.api.miFactura(guid).subscribe((response) => {
      const factura = response.data as any;
      const reservaGuid = factura?.reservaGuid ?? factura?.reserva_guid;

      if (!reservaGuid) {
        this.detalle.set(factura);
        return;
      }

      this.api.obtenerMiReserva(reservaGuid).subscribe({
        next: (reservaResponse) => this.detalle.set({
          ...factura,
          reserva: reservaResponse.data,
          detalles: reservaResponse.data.detalles ?? []
        }),
        error: () => this.detalle.set(factura)
      });
    });
  }

  cerrarDetalle() {
    this.detalle.set(null);
  }

  seleccionarDatoFacturacion() {
    const dato = this.itemsFromResponse(this.datosFacturacion()).find((item) => item.guid === this.pago.datosFacturacionGuid);
    this.facturacionForm = dato ? {
      tipoIdentificacion: dato.tipo_identificacion ?? dato.tipoIdentificacion ?? 'CEDULA',
      numeroIdentificacion: dato.numero_identificacion ?? dato.numeroIdentificacion ?? '',
      razonSocial: dato.razon_social ?? dato.razonSocial ?? '',
      nombre: dato.nombre ?? '',
      apellido: dato.apellido ?? '',
      correo: dato.correo ?? '',
      telefono: dato.telefono ?? '',
      direccion: dato.direccion ?? '',
      estado: 'A'
    } : this.nuevoDatoFacturacion();
  }

  esFactura(value: any) {
    return !!(value?.numero || value?.fecha_emision || value?.fechaEmision);
  }

  facturadoA(value: any) {
    const dato = value?.datos_facturacion || value?.datosFacturacion;
    const cliente = value?.cliente;
    return dato?.razon_social || dato?.razonSocial || `${dato?.nombre || cliente?.nombres || ''} ${dato?.apellido || cliente?.apellidos || ''}`.trim() || cliente?.razon_social || '-';
  }

  facturadoDocumento(value: any) {
    const dato = value?.datos_facturacion || value?.datosFacturacion;
    const cliente = value?.cliente;
    return `${dato?.tipo_identificacion || dato?.tipoIdentificacion || cliente?.tipo_identificacion || ''} ${dato?.numero_identificacion || dato?.numeroIdentificacion || cliente?.numero_identificacion || ''}`.trim() || '-';
  }

  facturadoContacto(value: any) {
    const dato = value?.datos_facturacion || value?.datosFacturacion;
    const cliente = value?.cliente;
    return `${dato?.correo || cliente?.correo || '-'} ${dato?.telefono || cliente?.telefono ? '- ' + (dato?.telefono || cliente?.telefono) : ''}`;
  }

  facturadoDireccion(value: any) {
    const dato = value?.datos_facturacion || value?.datosFacturacion;
    const cliente = value?.cliente;
    return dato?.direccion || cliente?.direccion || '-';
  }

  imprimir() {
    window.print();
  }

  soloNumerosTelefono() {
    this.facturacionForm.telefono = this.facturacionForm.telefono.replace(/\D/g, '').slice(0, 10);
  }

  reservaResumen(value: any) {
    if (this.esFactura(value)) {
      return [
        { label: 'Numero', value: value.numero || '-' },
        { label: 'Fecha emision', value: this.formatearFecha(this.fechaFactura(value)) },
        { label: 'Subtotal', value: `${value.subtotal ?? '-'} ${value.moneda ?? 'USD'}` },
        { label: 'IVA', value: `${value.valor_iva ?? value.valorIva ?? '-'} ${value.moneda ?? 'USD'}` },
        { label: 'Total', value: `${value.total ?? '-'} ${value.moneda ?? 'USD'}` }
      ];
    }

    return [
      { label: 'Subtotal', value: `${value.subtotal ?? '-'} ${value.moneda ?? 'USD'}` },
      { label: 'IVA', value: `${value.valor_iva ?? value.valorIva ?? '-'} ${value.moneda ?? 'USD'}` },
      { label: 'Total', value: `${value.total ?? '-'} ${value.moneda ?? 'USD'}` },
      { label: 'Expira', value: this.formatearFecha(value.fecha_expiracion_utc ?? value.fechaExpiracionUtc) },
      { label: 'Reserva creada', value: this.formatearFecha(value.fecha_reserva_utc ?? value.fechaReservaUtc) }
    ];
  }

  formatearFecha(value: string | null | undefined) {
    if (!value) return '-';
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return new Intl.DateTimeFormat('es-EC', {
      dateStyle: 'medium',
      timeStyle: 'short'
    }).format(date);
  }

  private nuevoDatoFacturacion() {
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

  private itemsFromResponse(value: any): any[] {
    if (Array.isArray(value)) return value;
    if (Array.isArray(value?.items)) return value.items;
    if (Array.isArray(value?.data)) return value.data;
    return [];
  }

  fechaFactura(value: any) {
    return value?.fecha_emision_utc ?? value?.fechaEmisionUtc ?? value?.fecha_emision ?? value?.fechaEmision;
  }

  reservaFactura(value: any) {
    return value?.reserva ?? value?.Reserva ?? null;
  }

  lineasFactura(value: any): any[] {
    return this.itemsFromResponse(value?.detalles ?? value?.detalle ?? this.reservaFactura(value)?.detalles);
  }

  atraccionFactura(value: any) {
    const reserva = this.reservaFactura(value);
    const fecha = reserva?.horFecha ?? reserva?.hor_fecha ?? reserva?.fecha ?? '-';
    const inicio = reserva?.horHoraInicio ?? reserva?.hor_hora_inicio ?? '';
    const fin = reserva?.horHoraFin ?? reserva?.hor_hora_fin ?? '';

    return {
      nombre: reserva?.atraccionNombre ?? reserva?.atraccion_nombre ?? value?.atraccion?.nombre ?? '-',
      fecha,
      hora: `${inicio}${fin ? ' - ' + fin : ''}`.trim() || '-'
    };
  }
}
