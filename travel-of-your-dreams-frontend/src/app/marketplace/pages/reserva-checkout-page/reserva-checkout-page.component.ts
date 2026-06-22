import { Component, Input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../../core/api/api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { AtraccionDetalle, HorarioDisponible, Ticket } from '../../../shared/models/atraccion.model';

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
              <select name="horGuid" [(ngModel)]="form.horGuid" (change)="resumen.set(null)">
                <option value="">Selecciona horario</option>
                @for (horario of horarios(); track horario.guid) {
                  <option [value]="horario.guid">{{ horario.fecha }} {{ horario.hora_inicio }} - {{ horario.cupos_disponibles }} cupos</option>
                }
              </select>
            </label>

            <div class="stack">
              <strong>Tickets</strong>
              @for (ticket of item.tickets; track ticket.guid) {
                <div class="compact-row">
                  <span>
                    <strong>{{ ticket.titulo }}</strong>
                    <small>{{ ticket.precio }} {{ ticket.moneda }} - max {{ ticket.capacidad_maxima }}</small>
                  </span>
                  <input class="qty-input" type="number" min="0" [max]="maxTicket(ticket.capacidad_maxima)" [name]="'cantidad_' + ticket.guid" [(ngModel)]="cantidades[ticket.guid]" (change)="normalizarCantidad(ticket.guid, ticket.capacidad_maxima)" />
                </div>
              } @empty {
                <p class="muted">Esta atraccion todavia no tiene tickets reservables.</p>
              }
            </div>

            <label>IVA %
              <input type="number" name="iva" [ngModel]="form.porcentajeIva" disabled />
            </label>
            <label>Dato para facturacion
              <select name="datoFacturacion" [(ngModel)]="datoFacturacionGuid">
                <option value="">Seleccionar al pagar</option>
                @for (dato of datosFacturacion(); track dato.guid) {
                  <option [value]="dato.guid">{{ dato.razon_social || dato.nombre || dato.correo }} - {{ dato.numero_identificacion }}</option>
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
              <div class="stack summary">
                <strong>{{ resumen()?.atraccion }}</strong>
                <span>{{ resumen()?.fecha }} - {{ resumen()?.horario }}</span>
                <span class="muted">La reserva expira en {{ form.expiracionMinutos }} minutos si no se confirma el pago.</span>

                <div class="summary-lines">
                  @for (linea of resumen()?.lineas; track linea.ticketGuid) {
                    <div class="summary-line">
                      <span>{{ linea.titulo }} x{{ linea.cantidad }}</span>
                      <strong>{{ linea.subtotal }} {{ resumen()?.moneda }}</strong>
                    </div>
                  }
                </div>

                <div class="summary-total">
                  <span>Subtotal</span>
                  <strong>{{ resumen()?.subtotal }} {{ resumen()?.moneda }}</strong>
                </div>
                <div class="summary-total">
                  <span>IVA {{ form.porcentajeIva }}%</span>
                  <strong>{{ resumen()?.valor_iva }} {{ resumen()?.moneda }}</strong>
                </div>
                <div class="summary-total grand">
                  <span>Total</span>
                  <strong>{{ resumen()?.total }} {{ resumen()?.moneda }}</strong>
                </div>
              </div>
            } @else {
              <p class="muted">Selecciona horario y tickets para previsualizar.</p>
            }
          </div>
        </div>
      }
    </section>
  `,
  styles: [`
    .summary-lines {
      display: grid;
      gap: 0.5rem;
      padding: 0.75rem 0;
      border-top: 1px solid var(--border);
      border-bottom: 1px solid var(--border);
    }

    .summary-line,
    .summary-total {
      display: flex;
      justify-content: space-between;
      gap: 1rem;
    }

    .summary-total.grand {
      font-size: 1.2rem;
      padding-top: 0.5rem;
      border-top: 1px solid var(--border);
    }
  `]
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
    private readonly auth: AuthService,
    private readonly router: Router,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.api.obtenerAtraccion(this.guid).subscribe((response) => {
      this.atraccion.set(this.toDetalle(response.data));
      this.cargarTickets();
    });

    if (this.auth.isAuthenticated) {
      this.api.misDatosFacturacion().subscribe((response) => {
        this.datosFacturacion.set(this.itemsFromResponse(response.data));
        this.datoFacturacionGuid = localStorage.getItem('toyd_facturacion_guid') ?? '';
      });
    }

    this.cargarHorarios();
  }

  cargarTickets() {
    this.api.listarTickets(this.guid).subscribe((response) => {
      const tickets = this.itemsFromResponse(response.data).map((item) => this.toTicket(item));
      this.atraccion.update((item) => item ? { ...item, tickets } : item);
      this.cantidades = {};
      tickets.forEach((ticket) => this.cantidades[ticket.guid] = 0);
      this.resumen.set(null);
    });
  }

  cargarHorarios() {
    if (this.form.fecha < this.fechaMinima) {
      this.form.fecha = this.fechaMinima;
      this.notifications.error('La fecha de reserva no puede ser anterior a hoy.');
    }

    this.api.listarHorarios(this.guid, this.form.fecha).subscribe((response) => {
      const horarios = this.itemsFromResponse(response.data)
        .map((item) => this.toHorario(item))
        .filter((horario) => !this.form.fecha || horario.fecha === this.form.fecha);
      this.horarios.set(horarios);
      this.form.horGuid = horarios[0]?.guid ?? '';
      this.resumen.set(null);
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
    this.resumen.set(null);
  }

  previsualizar() {
    if (!this.validarParaReserva()) return;

    const atraccion = this.atraccion();
    if (!atraccion) return;

    const horario = this.horarios().find((item) => item.guid === this.form.horGuid);
    const detalleLineas = this.lineasSeleccionadas().map((linea) => {
      const ticket = atraccion.tickets.find((item) => item.guid === linea.ticketGuid);
      const precio = Number(ticket?.precio ?? 0);
      return {
        ...linea,
        titulo: ticket?.titulo ?? 'Ticket',
        precio,
        subtotal: Number((precio * linea.cantidad).toFixed(2))
      };
    });
    const subtotal = detalleLineas.reduce((total, linea) => total + linea.subtotal, 0);
    const valorIva = Number((subtotal * (this.form.porcentajeIva / 100)).toFixed(2));
    this.resumen.set({
      atraccion: atraccion.nombre,
      fecha: horario?.fecha ?? this.form.fecha,
      horario: this.descripcionHorario(horario),
      lineas: detalleLineas,
      subtotal,
      valor_iva: valorIva,
      total: Number((subtotal + valorIva).toFixed(2)),
      moneda: atraccion.tickets[0]?.moneda ?? 'USD'
    });
    this.notifications.success('Resumen actualizado.');
  }

  crear() {
    if (!this.validarParaReserva()) return;

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
    return {
      atraccionGuid: this.atraccion()?.guid || this.guid,
      horarioGuid: this.form.horGuid,
      fecha: this.form.fecha,
      lineas: this.lineasSeleccionadas(),
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

  private validarParaReserva() {
    if (!this.validarFechaReserva()) return false;
    if (!this.atraccion()?.guid && !this.guid) {
      this.notifications.error('No se pudo identificar la atraccion seleccionada.');
      return false;
    }
    if (!this.form.horGuid) {
      this.notifications.error('Selecciona un horario disponible.');
      return false;
    }
    if (!this.lineasSeleccionadas().length) {
      this.notifications.error('Selecciona al menos un ticket.');
      return false;
    }
    return true;
  }

  private today() {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 10);
  }

  private itemsFromResponse(value: any): any[] {
    if (Array.isArray(value)) return value;
    if (Array.isArray(value?.items)) return value.items;
    if (Array.isArray(value?.data)) return value.data;
    return [];
  }

  private toDetalle(value: any): AtraccionDetalle {
    return {
      ...(value ?? {}),
      id: Number(value?.id ?? value?.numeric_id ?? value?.numericId ?? 0),
      guid: value?.guid ?? value?.at_guid ?? value?.atGuid ?? this.guid,
      nombre: value?.nombre ?? '',
      descripcion: value?.descripcion ?? null,
      pais: value?.pais ?? value?.ciudad ?? null,
      direccion: value?.direccion ?? value?.punto_encuentro ?? value?.puntoEncuentro ?? null,
      duracion_minutos: value?.duracion_minutos ?? value?.duracionMinutos ?? null,
      precio_referencia: Number(value?.precio_referencia ?? value?.precioReferencia ?? 0),
      disponible: Boolean(value?.disponible ?? true),
      free_cancellation: Boolean(value?.free_cancellation ?? value?.freeCancellation ?? false),
      skip_the_line: Boolean(value?.skip_the_line ?? value?.skipTheLine ?? false),
      total_resenias: Number(value?.total_resenias ?? value?.totalResenias ?? 0),
      categorias: this.itemsFromResponse(value?.categorias),
      idiomas: this.itemsFromResponse(value?.idiomas),
      imagenes: this.itemsFromResponse(value?.imagenes),
      incluye: this.itemsFromResponse(value?.incluye),
      tickets: this.itemsFromResponse(value?.tickets).map((item) => this.toTicket(item)),
      resenias: this.itemsFromResponse(value?.resenias)
    };
  }

  private toTicket(value: any): Ticket {
    return {
      id: Number(value?.id ?? 0),
      guid: value?.guid ?? value?.tck_guid ?? value?.tckGuid ?? '',
      atraccion_id: Number(value?.atraccion_id ?? value?.atraccionId ?? 0),
      titulo: value?.titulo ?? value?.tipo ?? 'Ticket',
      precio: Number(value?.precio ?? 0),
      moneda: value?.moneda ?? 'USD',
      tipo_participante: value?.tipo_participante ?? value?.tipoParticipante ?? value?.tipo ?? '',
      capacidad_maxima: Number(value?.capacidad_maxima ?? value?.capacidadMaxima ?? 999),
      estado: value?.estado ?? 'A'
    };
  }

  private toHorario(value: any): HorarioDisponible {
    return {
      ...value,
      id: Number(value?.id ?? 0),
      guid: value?.guid ?? value?.hor_guid ?? value?.horGuid ?? '',
      atraccion_id: Number(value?.atraccion_id ?? value?.atraccionId ?? 0),
      fecha: value?.fecha ?? '',
      hora_inicio: value?.hora_inicio ?? value?.horaInicio ?? '',
      hora_fin: value?.hora_fin ?? value?.horaFin ?? null,
      cupos_disponibles: Number(value?.cupos_disponibles ?? value?.cuposDisponibles ?? value?.cupos ?? 0),
      estado: value?.estado ?? 'A'
    };
  }

  private descripcionHorario(horario?: HorarioDisponible) {
    if (!horario) return 'Horario seleccionado';
    return `${horario.hora_inicio}${horario.hora_fin ? ' - ' + horario.hora_fin : ''}`;
  }
}
