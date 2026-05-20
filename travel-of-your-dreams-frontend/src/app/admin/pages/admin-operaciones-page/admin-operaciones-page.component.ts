import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-admin-operaciones-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div>
          <h1>Operacion</h1>
          <p class="muted">Horarios, tickets, pagos, facturas y moderacion de resenias.</p>
        </div>
        <button class="btn secondary" type="button" (click)="cargarTodo()">Actualizar</button>
      </header>

      <div class="grid two">
        <form class="panel form-grid" (ngSubmit)="guardarHorario()">
          <h2>{{ horarioGuid() ? 'Editar horario' : 'Horario reservable' }}</h2>
          <label>Atraccion
            <select name="horAtraccionId" [(ngModel)]="horario.atraccionId" required>
              <option [ngValue]="0">Selecciona atraccion</option>
              @for (atraccion of atracciones(); track atraccion.id) {
                <option [ngValue]="atraccion.id">{{ atraccion.nombre }}</option>
              }
            </select>
          </label>
          <label>Hora reservable <input type="time" name="horaInicio" [(ngModel)]="horario.horaInicio" required /></label>
          <label>Fin <input type="time" name="horaFin" [(ngModel)]="horario.horaFin" /></label>
          <label>Cupos <input type="number" min="1" name="cupos" [(ngModel)]="horario.cuposDisponibles" required /></label>
          <div class="wide day-grid">
            <strong>Dias de atencion</strong>
            @for (dia of diasSemana; track dia.valor) {
              <label class="check-inline">
                <input type="checkbox" [checked]="horario.diasSemana.includes(dia.valor)" (change)="toggleDia(dia.valor, $event)" />
                {{ dia.nombre }}
              </label>
            }
          </div>
          <div class="actions wide">
            <button class="btn" type="submit">{{ horarioGuid() ? 'Guardar horario' : 'Crear horario' }}</button>
            <button class="btn secondary" type="button" (click)="limpiarHorario()">Limpiar</button>
            <button class="btn secondary" type="button" (click)="desactivarHorarios()">Desactivar vencidos</button>
          </div>
        </form>

        <form class="panel form-grid" (ngSubmit)="guardarTicket()">
          <h2>{{ ticketGuid() ? 'Editar ticket' : 'Ticket' }}</h2>
          <label>Atraccion
            <select name="tAtraccionId" [(ngModel)]="ticket.atraccionId" required>
              <option [ngValue]="0">Selecciona atraccion</option>
              @for (atraccion of atracciones(); track atraccion.id) {
                <option [ngValue]="atraccion.id">{{ atraccion.nombre }}</option>
              }
            </select>
          </label>
          <label>Titulo <input name="titulo" maxlength="150" [(ngModel)]="ticket.titulo" required /></label>
          <label>Precio <input type="number" min="0" name="precio" [(ngModel)]="ticket.precio" required /></label>
          <label>Moneda <input name="moneda" maxlength="3" [(ngModel)]="ticket.moneda" /></label>
          <label>Participante <input name="tipoParticipante" maxlength="30" [(ngModel)]="ticket.tipoParticipante" /></label>
          <label>Capacidad <input type="number" min="1" name="capacidadMaxima" [(ngModel)]="ticket.capacidadMaxima" /></label>
          <label>Estado
            <select name="ticketEstado" [(ngModel)]="ticket.estado">
              <option value="A">Activo</option>
              <option value="I">Inactivo</option>
            </select>
          </label>
          <div class="actions wide">
            <button class="btn" type="submit">Guardar ticket</button>
            <button class="btn secondary" type="button" (click)="limpiarTicket()">Limpiar</button>
          </div>
        </form>
      </div>

      <div class="grid two">
        <div class="panel stack">
          <h2>Horarios</h2>
          <input name="buscarHorarios" placeholder="Buscar horario o atraccion" [(ngModel)]="busquedas.horarios" />
          @for (item of horariosFiltrados(); track item.guid || item.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ item.hora_inicio || item.horaInicio }} - {{ item.hora_fin || item.horaFin || 'sin fin' }}</strong>
                <small>{{ nombreAtraccion(item.atraccion_id ?? item.atraccionId) }} - cupos {{ item.cupos_disponibles || item.cuposDisponibles }} - {{ diasTexto(item.dias_semana || item.diasSemana) }} - {{ item.estado }}</small>
              </span>
              <span class="actions">
                <button class="btn secondary" type="button" (click)="editarHorario(item)">Editar</button>
                <button class="btn" type="button" (click)="estadoHorario(item.guid, 'A')">Activar</button>
                <button class="btn danger" type="button" (click)="estadoHorario(item.guid, 'I')">Desactivar</button>
              </span>
            </div>
          } @empty {
            <p class="muted">Sin horarios.</p>
          }
        </div>

        <div class="panel stack">
          <h2>Tickets</h2>
          <input name="buscarTickets" placeholder="Buscar ticket o atraccion" [(ngModel)]="busquedas.tickets" />
          @for (item of ticketsFiltrados(); track item.guid || item.id || $index) {
            <div class="compact-row">
              <span><strong>{{ item.titulo }}</strong><small>{{ item.precio }} {{ item.moneda }} · {{ item.estado }}</small></span>
              <span class="actions">
                <button class="btn secondary" type="button" (click)="editarTicket(item)">Editar</button>
                <button class="btn danger" type="button" (click)="eliminarTicket(item.guid)">Eliminar</button>
              </span>
            </div>
          } @empty {
            <p class="muted">Sin tickets.</p>
          }
        </div>
      </div>

      <div class="grid two">
        <div class="panel stack">
          <h2>Resenias</h2>
          <input name="buscarResenias" placeholder="Buscar comentario o usuario" [(ngModel)]="busquedas.resenias" />
          @for (item of reseniasFiltradas(); track item.id || $index) {
            <div class="compact-row">
              <span><strong>{{ item.rating || item.calificacion || '-' }}/5</strong><small>{{ item.comentario || item.estado }}</small></span>
              <span class="actions">
                <button class="btn" type="button" (click)="estadoResenia(item.guid, 'A')">Mostrar</button>
                <button class="btn danger" type="button" (click)="estadoResenia(item.guid, 'I')">Ocultar</button>
              </span>
            </div>
          } @empty {
            <p class="muted">Sin resenias.</p>
          }
        </div>

        <div class="panel stack">
          <h2>Pagos y facturas</h2>
          <button class="btn secondary" type="button" (click)="cargarPagosFacturas()">Consultar</button>
          <input name="buscarPagosFacturas" placeholder="Buscar pago, factura o estado" [(ngModel)]="busquedas.pagosFacturas" />
          <h3>Pagos</h3>
          @for (pago of pagosFiltrados(); track pago.guid || pago.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ pago.referencia || 'Pago' }}</strong>
                <small>{{ pago.metodo }} - {{ pago.estado }} - reserva #{{ pago.reserva_id || pago.reservaId }}</small>
              </span>
              <strong>{{ pago.monto }} {{ pago.moneda || 'USD' }}</strong>
            </div>
          } @empty {
            <p class="muted">Sin pagos cargados.</p>
          }
          <h3>Facturas</h3>
          @for (factura of facturasFiltradas(); track factura.guid || factura.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ factura.numero || factura.fac_numero || 'Factura' }}</strong>
                <small>{{ factura.estado }} - reserva #{{ factura.reserva_id || factura.reservaId }}</small>
              </span>
              <strong>{{ factura.total }} {{ factura.moneda || 'USD' }}</strong>
            </div>
          } @empty {
            <p class="muted">Sin facturas cargadas.</p>
          }
        </div>
      </div>

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `,
  styles: [`
    .day-grid { background: var(--surface-muted); border: 1px solid var(--line); border-radius: 8px; display: grid; gap: 8px; grid-template-columns: repeat(4, minmax(0, 1fr)); padding: 12px; }
    .day-grid strong { grid-column: 1 / -1; }
    .check-inline { align-items: center; display: flex; flex-direction: row; gap: 8px; }
    .check-inline input { width: auto; }
    @media (max-width: 760px) { .day-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); } }
    @media (max-width: 520px) { .day-grid { grid-template-columns: 1fr; } }
  `]
})
export class AdminOperacionesPageComponent implements OnInit {
  horarios = signal<any[]>([]);
  tickets = signal<any[]>([]);
  resenias = signal<any[]>([]);
  pagos = signal<any[]>([]);
  facturas = signal<any[]>([]);
  atracciones = signal<any[]>([]);
  mensaje = signal('');
  ticketGuid = signal<string | null>(null);
  horarioGuid = signal<string | null>(null);
  busquedas = { horarios: '', tickets: '', resenias: '', pagosFacturas: '' };
  diasSemana = [
    { valor: '1', nombre: 'Lun' },
    { valor: '2', nombre: 'Mar' },
    { valor: '3', nombre: 'Mie' },
    { valor: '4', nombre: 'Jue' },
    { valor: '5', nombre: 'Vie' },
    { valor: '6', nombre: 'Sab' },
    { valor: '0', nombre: 'Dom' }
  ];

  horario = { atraccionId: 0, horaInicio: '', horaFin: '', cuposDisponibles: 1, diasSemana: ['1', '2', '3', '4', '5', '6', '0'] };
  ticket = this.nuevoTicket();

  constructor(
    private readonly api: ApiService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.cargarTodo();
  }

  cargarTodo() {
    this.api.adminHorarios().subscribe((response) => this.horarios.set(response.data));
    this.api.adminTickets().subscribe((response) => this.tickets.set(response.data));
    this.api.adminResenias().subscribe((response) => this.resenias.set(response.data));
    this.api.adminAtracciones().subscribe((response) => this.atracciones.set(response.data));
  }

  guardarHorario() {
    const request = { ...this.horario, diasSemana: this.horario.diasSemana.join(',') };
    const action = this.horarioGuid()
      ? this.api.actualizarHorario(this.horarioGuid()!, request)
      : this.api.crearHorario(request);
    action.subscribe({
      next: () => {
        this.mensaje.set('Horario guardado.');
        this.notifications.success('Horario reservable guardado.');
        this.limpiarHorario();
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  editarHorario(item: any) {
    this.horarioGuid.set(item.guid);
    this.horario = {
      atraccionId: item.atraccion_id ?? item.atraccionId ?? 0,
      horaInicio: item.hora_inicio ?? item.horaInicio ?? '',
      horaFin: item.hora_fin ?? item.horaFin ?? '',
      cuposDisponibles: item.cupos_disponibles ?? item.cuposDisponibles ?? 1,
      diasSemana: String(item.dias_semana ?? item.diasSemana ?? '0,1,2,3,4,5,6').split(',')
    };
  }

  limpiarHorario() {
    this.horarioGuid.set(null);
    this.horario = { atraccionId: 0, horaInicio: '', horaFin: '', cuposDisponibles: 1, diasSemana: ['1', '2', '3', '4', '5', '6', '0'] };
  }

  estadoHorario(guid: string, estado: string) {
    this.api.cambiarEstadoHorario(guid, estado).subscribe({
      next: () => {
        this.mensaje.set('Horario actualizado.');
        this.notifications.success('Horario actualizado.');
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  desactivarHorarios() {
    this.api.desactivarHorariosVencidos().subscribe((response) => {
      this.mensaje.set(`Horarios desactivados: ${response.data.total}`);
      this.notifications.success(`Horarios desactivados: ${response.data.total}`);
      this.cargarTodo();
    });
  }

  guardarTicket() {
    const action = this.ticketGuid()
      ? this.api.actualizarTicket(this.ticketGuid()!, this.ticket)
      : this.api.crearTicket(this.ticket);
    action.subscribe({
      next: () => {
        this.mensaje.set('Ticket guardado.');
        this.notifications.success('Ticket guardado.');
        this.limpiarTicket();
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  editarTicket(item: any) {
    this.ticketGuid.set(item.guid);
    this.ticket = {
      atraccionId: item.atraccion_id ?? item.atraccionId ?? 0,
      titulo: item.titulo ?? '',
      precio: item.precio ?? 0,
      moneda: item.moneda ?? 'USD',
      tipoParticipante: item.tipo_participante ?? item.tipoParticipante ?? 'Adulto',
      capacidadMaxima: item.capacidad_maxima ?? item.capacidadMaxima ?? 1,
      estado: item.estado ?? 'A'
    };
  }

  eliminarTicket(guid: string) {
    if (!confirm('Eliminar este ticket?')) return;
    this.api.eliminarTicket(guid).subscribe({
      next: () => {
        this.mensaje.set('Ticket eliminado.');
        this.notifications.success('Ticket eliminado.');
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  limpiarTicket() {
    this.ticketGuid.set(null);
    this.ticket = this.nuevoTicket();
  }

  estadoResenia(guid: string, estado: string) {
    this.api.cambiarEstadoResenia(guid, estado).subscribe({
      next: () => {
        this.mensaje.set('Resenia moderada.');
        this.notifications.success('Resenia moderada.');
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  cargarPagosFacturas() {
    this.api.adminPagos({ limit: 20 }).subscribe((response) => this.pagos.set(response.data));
    this.api.adminFacturas({ limit: 20 }).subscribe((response) => this.facturas.set(response.data));
  }

  nombreAtraccion(id: number) {
    return this.atracciones().find((atraccion) => atraccion.id === id)?.nombre ?? `Atraccion #${id || '-'}`;
  }

  toggleDia(valor: string, event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    this.horario.diasSemana = checked
      ? Array.from(new Set([...this.horario.diasSemana, valor]))
      : this.horario.diasSemana.filter((dia) => dia !== valor);
  }

  diasTexto(value: string | string[] | undefined) {
    const dias = Array.isArray(value) ? value : String(value || '0,1,2,3,4,5,6').split(',');
    return this.diasSemana.filter((dia) => dias.includes(dia.valor)).map((dia) => dia.nombre).join(', ');
  }

  horariosFiltrados() {
    const q = this.busquedas.horarios.toLowerCase();
    return this.horarios().filter((item) => !q || `${this.nombreAtraccion(item.atraccion_id ?? item.atraccionId)} ${item.hora_inicio ?? item.horaInicio} ${item.estado}`.toLowerCase().includes(q));
  }

  ticketsFiltrados() {
    const q = this.busquedas.tickets.toLowerCase();
    return this.tickets().filter((item) => !q || `${this.nombreAtraccion(item.atraccion_id ?? item.atraccionId)} ${item.titulo} ${item.estado}`.toLowerCase().includes(q));
  }

  reseniasFiltradas() {
    const q = this.busquedas.resenias.toLowerCase();
    return this.resenias().filter((item) => !q || `${item.comentario} ${item.usuario_creacion ?? item.usuarioCreacion} ${item.estado}`.toLowerCase().includes(q));
  }

  pagosFiltrados() {
    const q = this.busquedas.pagosFacturas.toLowerCase();
    return this.pagos().filter((item) => !q || `${item.referencia} ${item.metodo} ${item.estado}`.toLowerCase().includes(q));
  }

  facturasFiltradas() {
    const q = this.busquedas.pagosFacturas.toLowerCase();
    return this.facturas().filter((item) => !q || `${item.numero ?? item.fac_numero} ${item.estado}`.toLowerCase().includes(q));
  }

  private nuevoTicket() {
    return {
      atraccionId: 0,
      titulo: '',
      precio: 0,
      moneda: 'USD',
      tipoParticipante: 'Adulto',
      capacidadMaxima: 1,
      estado: 'A'
    };
  }
}
