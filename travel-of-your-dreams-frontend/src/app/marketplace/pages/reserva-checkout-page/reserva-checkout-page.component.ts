import { Component, Input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { AtraccionDetalle, HorarioDisponible } from '../../../shared/models/atraccion.model';

@Component({
  selector: 'app-reserva-checkout-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <h1>Reserva</h1>
      @if (atraccion(); as item) {
        <div class="grid two">
          <div class="panel stack">
            <h2>{{ item.nombre }}</h2>
            <label>Fecha
              <input type="date" name="fecha" [min]="fechaMinima" [(ngModel)]="form.fecha" (change)="cargarHorarios()" />
            </label>
            <label>Horario
              <select name="horGuid" [(ngModel)]="form.horGuid">
                <option value="">Selecciona horario</option>
                @for (horario of horarios(); track horario.guid) {
                  <option [value]="horario.guid">{{ horario.fecha }} {{ horario.hora_inicio }} · {{ horario.cupos_disponibles }} cupos</option>
                }
              </select>
            </label>

            <div class="stack">
              <strong>Tickets</strong>
              @for (ticket of item.tickets; track ticket.guid) {
                <div class="compact-row">
                  <span>
                    <strong>{{ ticket.titulo }}</strong>
                    <small>{{ ticket.precio }} {{ ticket.moneda }} · max {{ ticket.capacidad_maxima }}</small>
                  </span>
                  <input class="qty-input" type="number" min="0" [max]="maxTicket(ticket.capacidad_maxima)" [name]="'cantidad_' + ticket.guid" [(ngModel)]="cantidades[ticket.guid]" (change)="normalizarCantidad(ticket.guid, ticket.capacidad_maxima)" />
                </div>
              }
            </div>

            <label>IVA %
              <input type="number" name="iva" [ngModel]="form.porcentajeIva" disabled />
            </label>
            <label>Dato para facturacion
              <select name="datoFacturacion" [(ngModel)]="datoFacturacionGuid">
                <option value="">Seleccionar al pagar</option>
                @for (dato of datosFacturacion(); track dato.guid) {
                  <option [value]="dato.guid">{{ dato.razon_social || dato.nombre || dato.correo }} · {{ dato.numero_identificacion }}</option>
                }
              </select>
            </label>

            <div class="actions">
              <button class="btn secondary" type="button" (click)="previsualizar()">Previsualizar</button>
              <button class="btn" type="button" (click)="crear()">Crear reserva</button>
            </div>
          </div>

          <div class="panel stack">
            <h2>Resumen</h2>
            @if (resumen()) {
              <div class="stack">
                <strong>Total: {{ resumen()?.total }} {{ resumen()?.moneda }}</strong>
                <span class="muted">Subtotal {{ resumen()?.subtotal }} · IVA {{ resumen()?.valor_iva }}</span>
              </div>
            } @else {
              <p class="muted">Selecciona horario y tickets para previsualizar.</p>
            }
          </div>
        </div>
      }
    </section>
  `
})
export class ReservaCheckoutPageComponent implements OnInit {
  @Input() guid = '';
  atraccion = signal<AtraccionDetalle | null>(null);
  horarios = signal<HorarioDisponible[]>([]);
  datosFacturacion = signal<any[]>([]);
  resumen = signal<any | null>(null);
  cantidades: Record<string, number> = {};
  datoFacturacionGuid = '';
  fechaMinima = this.today();
  form = { fecha: this.today(), horGuid: '', origenCanal: 'WEB', expiracionMinutos: 15, porcentajeIva: 12 };

  constructor(
    private readonly api: ApiService,
    private readonly router: Router,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.api.obtenerAtraccion(this.guid).subscribe((response) => {
      this.atraccion.set(response.data);
      response.data.tickets.forEach((ticket) => this.cantidades[ticket.guid] = 0);
    });
    this.api.misDatosFacturacion().subscribe((response) => {
      this.datosFacturacion.set(response.data);
      this.datoFacturacionGuid = localStorage.getItem('toyd_facturacion_guid') ?? '';
    });
    this.cargarHorarios();
  }

  cargarHorarios() {
    if (this.form.fecha < this.fechaMinima) {
      this.form.fecha = this.fechaMinima;
      this.notifications.error('La fecha de reserva no puede ser anterior a hoy.');
    }

    this.api.listarHorarios(this.guid, this.form.fecha).subscribe((response) => {
      this.horarios.set(response.data);
      this.form.horGuid = response.data[0]?.guid ?? '';
    });
  }

  maxTicket(capacidadMaxima: number) {
    const cupos = this.horarios().find((horario) => horario.guid === this.form.horGuid)?.cupos_disponibles ?? capacidadMaxima;
    return Math.max(0, Math.min(capacidadMaxima, cupos));
  }

  normalizarCantidad(ticketGuid: string, capacidadMaxima: number) {
    const maximo = this.maxTicket(capacidadMaxima);
    const actual = Number(this.cantidades[ticketGuid] ?? 0);
    this.cantidades[ticketGuid] = Math.max(0, Math.min(actual, maximo));
  }

  previsualizar() {
    if (!this.validarFechaReserva()) return;

    const atraccion = this.atraccion();
    if (!atraccion) return;
    const lineas = this.lineasSeleccionadas();
    const subtotal = lineas.reduce((total, linea) => {
      const ticket = atraccion.tickets.find((item) => item.guid === linea.ticketGuid);
      return total + Number(ticket?.precio ?? 0) * linea.cantidad;
    }, 0);
    const valorIva = Number((subtotal * (this.form.porcentajeIva / 100)).toFixed(2));
    this.resumen.set({
      subtotal,
      valor_iva: valorIva,
      total: Number((subtotal + valorIva).toFixed(2)),
      moneda: atraccion.tickets[0]?.moneda ?? 'USD'
    });
    this.notifications.success('Resumen actualizado.');
  }

  crear() {
    if (!this.validarFechaReserva()) return;

    this.api.crearReserva(this.request()).subscribe({
      next: () => {
        if (this.datoFacturacionGuid) localStorage.setItem('toyd_facturacion_guid', this.datoFacturacionGuid);
        this.notifications.success('Reserva creada.');
        void this.router.navigateByUrl('/mis-reservas');
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  private request() {
    const lineas = this.lineasSeleccionadas();

    return {
      atraccionGuid: this.guid,
      horarioGuid: this.form.horGuid,
      fecha: this.form.fecha,
      lineas,
      origenCanal: this.form.origenCanal,
      expiracionMinutos: this.form.expiracionMinutos,
      porcentajeIva: this.form.porcentajeIva
    };
  }

  private lineasSeleccionadas() {
    return Object.entries(this.cantidades)
      .filter(([, cantidad]) => Number(cantidad) > 0)
      .map(([ticketGuid, cantidad]) => ({ ticketGuid, cantidad: Number(cantidad) }));
  }

  private validarFechaReserva() {
    if (this.form.fecha >= this.fechaMinima) return true;

    this.form.fecha = this.fechaMinima;
    this.notifications.error('La fecha de reserva no puede ser anterior a hoy.');
    this.cargarHorarios();
    return false;
  }

  private today() {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 10);
  }
}
