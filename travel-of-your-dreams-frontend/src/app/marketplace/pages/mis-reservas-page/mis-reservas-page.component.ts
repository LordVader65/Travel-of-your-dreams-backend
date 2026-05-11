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

      <div class="grid two">
        <div class="panel stack">
          @for (reserva of reservas(); track reserva.guid) {
            <article class="compact-row">
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
          <button class="btn wide" type="submit" [disabled]="!pago.reservaGuid">Confirmar pago</button>
        </form>
      </div>

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
                <span><strong>Fecha:</strong> {{ formatearFecha(detalle().fecha_emision || detalle().fechaEmision) }}</span>
                <span><strong>Reserva:</strong> {{ detalle().reserva?.codigo || '-' }}</span>
              </div>
            </div>
          }
          <header class="section-head">
            <div>
              <h2>{{ detalle().codigo || detalle().numero || 'Detalle' }}</h2>
              <p class="muted">{{ detalle().estado }} - {{ detalle().total }} {{ detalle().moneda }}</p>
            </div>
            @if (esFactura(detalle())) {
              <button class="btn secondary print-hide" type="button" (click)="imprimir()">Imprimir</button>
            }
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
                <p><strong>{{ detalle().atraccion?.nombre || '-' }}</strong></p>
                <p>{{ detalle().atraccion?.destino || '-' }} {{ detalle().atraccion?.pais ? '- ' + detalle().atraccion.pais : '' }}</p>
                <p>{{ detalle().atraccion?.fecha || '-' }} {{ detalle().atraccion?.hora_inicio || detalle().atraccion?.horaInicio || '' }}</p>
                <p>{{ detalle().atraccion?.direccion || '-' }}</p>
              </section>
              <section>
                <h3>Pago</h3>
                <p><strong>{{ detalle().pago?.metodo || '-' }}</strong></p>
                <p>Referencia: {{ detalle().pago?.referencia || '-' }}</p>
                <p>Estado: {{ detalle().pago?.estado || '-' }}</p>
              </section>
            </div>
          }
          @if ((detalle().detalles || []).length) {
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
                @for (linea of detalle().detalles || []; track linea.guid || linea.id || $index) {
                  <div class="invoice-row">
                    <span>{{ linea.titulo || linea.ticket_titulo || ('Ticket #' + (linea.ticket_id || linea.ticketId || '-')) }}</span>
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
              @for (linea of detalle().detalles || []; track linea.guid || linea.id || $index) {
                <div class="compact-row">
                  <span>
                    <strong>{{ linea.titulo || linea.ticket_titulo || ('Ticket #' + (linea.ticket_id || linea.ticketId || '-')) }}</strong>
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

      <div class="grid two">
        <div class="panel stack">
          <h2>Pagos</h2>
          @for (item of pagos(); track item.guid || item.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ item.referencia || 'Pago registrado' }}</strong>
                <small>{{ item.metodo }} - {{ item.estado }} - {{ formatearFecha(item.fecha_utc || item.fechaUtc) }}</small>
              </span>
              <strong>{{ item.monto }} {{ item.moneda || 'USD' }}</strong>
            </div>
          } @empty {
            <p class="muted">Sin pagos.</p>
          }
        </div>
        <div class="panel stack">
          <h2>Facturas</h2>
          @for (item of facturas(); track item.guid || item.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ item.numero || item.guid }}</strong>
                <small>{{ item.estado }} - {{ formatearFecha(item.fecha_emision || item.fechaEmision) }}</small>
              </span>
              <strong>{{ item.total }} {{ item.moneda || 'USD' }}</strong>
              <button class="btn secondary" type="button" (click)="verFactura(item.guid)">Ver</button>
            </div>
          } @empty {
            <p class="muted">Sin facturas.</p>
          }
        </div>
      </div>

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `
})
export class MisReservasPageComponent implements OnInit {
  reservas = signal<Reserva[]>([]);
  detalle = signal<any | null>(null);
  pagos = signal<any[]>([]);
  facturas = signal<any[]>([]);
  datosFacturacion = signal<any[]>([]);
  mensaje = signal('');
  pago = { reservaGuid: '', metodo: 'TARJETA', monto: 0, referencia: '', datosFacturacionGuid: '', origenCanal: 'WEB' };
  facturacionForm = this.nuevoDatoFacturacion();
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
      this.reservas.set(response.data);
      if (this.pago.reservaGuid && !response.data.some((reserva) => reserva.guid === this.pago.reservaGuid && reserva.estado === 'PENDIENTE')) {
        this.pago.reservaGuid = '';
        this.pago.monto = 0;
      }
    });
    this.api.misPagos({ limit: 20 }).subscribe((response) => this.pagos.set(response.data));
    this.api.misFacturas({ limit: 20 }).subscribe((response) => this.facturas.set(response.data));
    this.api.misDatosFacturacion().subscribe((response) => {
      this.datosFacturacion.set(response.data);
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
    return this.reservas().filter((reserva) => reserva.estado === 'PENDIENTE');
  }

  seleccionarReservaPago() {
    const reserva = this.reservas().find((item) => item.guid === this.pago.reservaGuid);
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

  verFactura(guid: string) {
    this.api.miFactura(guid).subscribe((response) => this.detalle.set(response.data));
  }

  seleccionarDatoFacturacion() {
    const dato = this.datosFacturacion().find((item) => item.guid === this.pago.datosFacturacionGuid);
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
        { label: 'Fecha emision', value: this.formatearFecha(value.fecha_emision ?? value.fechaEmision) },
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
}
