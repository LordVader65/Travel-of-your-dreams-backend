import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';

type OperacionTab = 'horarios' | 'tickets' | 'reservas' | 'pagos' | 'facturas' | 'resenias';
type OperacionModal = 'horario' | 'ticket' | null;

@Component({
  selector: 'app-admin-operaciones-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div class="page-title">
          <h1>Operacion</h1>
          <p class="muted">Horarios, tickets, pagos, facturas y moderacion de resenias.</p>
        </div>
        <button class="btn secondary" type="button" (click)="cargarTodo()">Actualizar</button>
      </header>

      <nav class="app-tabs" aria-label="Operaciones">
        <button class="tab-button" [class.active]="tab() === 'horarios'" type="button" (click)="tab.set('horarios')">Horarios</button>
        <button class="tab-button" [class.active]="tab() === 'tickets'" type="button" (click)="tab.set('tickets')">Tickets</button>
        <button class="tab-button" [class.active]="tab() === 'reservas'" type="button" (click)="tab.set('reservas')">Reservas</button>
        <button class="tab-button" [class.active]="tab() === 'pagos'" type="button" (click)="tab.set('pagos'); cargarPagos()">Pagos</button>
        <button class="tab-button" [class.active]="tab() === 'facturas'" type="button" (click)="tab.set('facturas'); cargarFacturas()">Facturas</button>
        <button class="tab-button" [class.active]="tab() === 'resenias'" type="button" (click)="tab.set('resenias')">Resenias</button>
      </nav>

      @if (tab() === 'horarios') {
        <div class="panel stack">
          <div class="section-toolbar">
            <div>
              <h2>Horarios por atraccion</h2>
              <p class="muted">Selecciona una atraccion para ver sus horarios reservables.</p>
            </div>
            <span class="actions">
              <button class="btn secondary" type="button" (click)="desactivarHorarios()">Desactivar vencidos</button>
              <button class="btn" type="button" (click)="abrirCrearHorario()">Crear horario</button>
            </span>
          </div>
          <input name="buscarHorarios" placeholder="Buscar atraccion u horario" [(ngModel)]="busquedas.horarios" />
          <div class="group-layout">
            <div class="group-list">
              @for (grupo of gruposHorarios(); track grupo.id) {
                <button class="group-card" [class.active]="atraccionHorariosSeleccionada() === grupo.id" type="button" (click)="atraccionHorariosSeleccionada.set(grupo.id)">
                  <strong>{{ grupo.nombre }}</strong>
                  <small>{{ grupo.total }} horarios · {{ grupo.activos }} activos</small>
                </button>
              } @empty {
                <p class="muted">Sin horarios.</p>
              }
            </div>
            <div class="group-detail">
              @for (item of horariosSeleccionados(); track item.guid || item.id || $index) {
                <div class="compact-row">
                  <span>
                    <strong>{{ item.fecha || item.hor_fecha || '-' }} · {{ item.hora_inicio || item.horaInicio }} - {{ item.hora_fin || item.horaFin || 'sin fin' }}</strong>
                    <small>Cupos {{ item.cupos_disponibles || item.cuposDisponibles }} · {{ diasTexto(item.dias_semana || item.diasSemana) }} · {{ item.estado }}</small>
                  </span>
                  <span class="actions">
                    <button class="btn secondary" type="button" (click)="abrirEditarHorario(item)">Editar</button>
                    <button class="btn" type="button" (click)="estadoHorario(item.guid, 'A')">Activar</button>
                    <button class="btn danger" type="button" (click)="estadoHorario(item.guid, 'I')">Desactivar</button>
                  </span>
                </div>
              } @empty {
                <p class="muted">Selecciona una atraccion para ver sus horarios.</p>
              }
            </div>
          </div>
        </div>
      }

      @if (tab() === 'tickets') {
        <div class="panel stack">
          <div class="section-toolbar">
            <div>
              <h2>Tickets por atraccion</h2>
              <p class="muted">Selecciona una atraccion para ver los tickets asociados.</p>
            </div>
            <button class="btn" type="button" (click)="abrirCrearTicket()">Crear ticket</button>
          </div>
          <input name="buscarTickets" placeholder="Buscar atraccion o ticket" [(ngModel)]="busquedas.tickets" />
          <div class="group-layout">
            <div class="group-list">
              @for (grupo of gruposTickets(); track grupo.id) {
                <button class="group-card" [class.active]="atraccionTicketsSeleccionada() === grupo.id" type="button" (click)="atraccionTicketsSeleccionada.set(grupo.id)">
                  <strong>{{ grupo.nombre }}</strong>
                  <small>{{ grupo.total }} tickets · {{ grupo.activos }} activos</small>
                </button>
              } @empty {
                <p class="muted">Sin tickets.</p>
              }
            </div>
            <div class="group-detail">
              @for (item of ticketsSeleccionados(); track item.guid || item.id || $index) {
                <div class="compact-row">
                  <span>
                    <strong>{{ item.titulo }}</strong>
                    <small>{{ item.precio }} {{ item.moneda }} · {{ item.tipo_participante || item.tipoParticipante || 'Participante' }} · {{ item.estado }}</small>
                  </span>
                  <span class="actions">
                    <button class="btn secondary" type="button" (click)="abrirEditarTicket(item)">Editar</button>
                    <button class="btn danger" type="button" (click)="eliminarTicket(item.guid)">Eliminar</button>
                  </span>
                </div>
              } @empty {
                <p class="muted">Selecciona una atraccion para ver sus tickets.</p>
              }
            </div>
          </div>
        </div>
      }

      @if (tab() === 'reservas') {
        <div class="panel stack">
          <h2>Reservas</h2>
          <input name="buscarReservas" placeholder="Buscar reserva, cliente, canal o estado" [(ngModel)]="busquedas.reservas" />
          @for (reserva of reservasFiltradas(); track reserva.guid || reserva.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ reserva.codigo || reserva.rev_codigo || 'Reserva' }}</strong>
                <small>{{ reserva.estado }} · {{ reserva.origenCanal || reserva.origen_canal || '-' }} · {{ reserva.total }} {{ reserva.moneda || 'USD' }}</small>
              </span>
              <button class="btn secondary" type="button" (click)="verReserva(reserva)">Ver detalle</button>
            </div>
          } @empty {
            <p class="muted">Sin reservas cargadas.</p>
          }
        </div>
      }

      @if (tab() === 'pagos') {
        <div class="panel stack">
          <div class="section-toolbar">
            <h2>Pagos</h2>
            <button class="btn secondary" type="button" (click)="cargarPagos()">Consultar</button>
          </div>
          <input name="buscarPagos" placeholder="Buscar pago, metodo o estado" [(ngModel)]="busquedas.pagos" />
          @for (pago of pagosFiltrados(); track pago.guid || pago.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ pago.referencia || 'Pago' }}</strong>
                <small>{{ pago.metodo }} · {{ pago.estado }} · reserva #{{ pago.reserva_id || pago.reservaId }}</small>
              </span>
              <span class="actions">
                <strong>{{ pago.monto }} {{ pago.moneda || 'USD' }}</strong>
                <button class="btn secondary" type="button" (click)="verPago(pago)">Ver detalle</button>
              </span>
            </div>
          } @empty {
            <p class="muted">Sin pagos cargados.</p>
          }
        </div>
      }

      @if (tab() === 'facturas') {
        <div class="panel stack">
          <div class="section-toolbar">
            <h2>Facturas</h2>
            <button class="btn secondary" type="button" (click)="cargarFacturas()">Consultar</button>
          </div>
          <input name="buscarFacturas" placeholder="Buscar factura o estado" [(ngModel)]="busquedas.facturas" />
          @for (factura of facturasFiltradas(); track factura.guid || factura.id || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ factura.numero || factura.fac_numero || 'Factura' }}</strong>
                <small>{{ factura.estado }} · reserva #{{ factura.reserva_id || factura.reservaId }}</small>
              </span>
              <span class="actions">
                <strong>{{ factura.total }} {{ factura.moneda || 'USD' }}</strong>
                <button class="btn secondary" type="button" (click)="verFactura(factura)">Ver detalle</button>
              </span>
            </div>
          } @empty {
            <p class="muted">Sin facturas cargadas.</p>
          }
        </div>
      }

      @if (tab() === 'resenias') {
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
      }

      @if (modalOperacion()) {
        <div class="modal-backdrop" (click)="cerrarModalOperacion()"></div>
        <aside class="drawer-panel stack" role="dialog" aria-modal="true">
          <header class="drawer-header">
            <div>
              <h2>{{ modalOperacion() === 'horario' ? (horarioGuid() ? 'Editar horario' : 'Crear horarios') : (ticketGuid() ? 'Editar ticket' : 'Crear ticket') }}</h2>
              <p class="muted">{{ modalOperacion() === 'horario' ? 'Gestiona una regla de disponibilidad o un horario puntual.' : 'Gestiona los tickets reservables por atraccion.' }}</p>
            </div>
            <button class="btn secondary" type="button" (click)="cerrarModalOperacion()">Cerrar</button>
          </header>

          @if (modalOperacion() === 'horario') {
            <form class="form-grid" (ngSubmit)="guardarHorario()">
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
              @if (!horarioGuid()) {
                <label>Vigente desde <input type="date" name="fechaInicio" [(ngModel)]="horario.fechaInicio" required /></label>
                <label>Vigente hasta <input type="date" name="fechaFin" [(ngModel)]="horario.fechaFin" required /></label>
              }
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
                <button class="btn" type="submit">{{ horarioGuid() ? 'Guardar horario' : 'Generar horarios' }}</button>
                <button class="btn secondary" type="button" (click)="cerrarModalOperacion()">Cancelar</button>
              </div>
            </form>
          }

          @if (modalOperacion() === 'ticket') {
            <form class="form-grid" (ngSubmit)="guardarTicket()">
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
                <button class="btn secondary" type="button" (click)="cerrarModalOperacion()">Cancelar</button>
              </div>
            </form>
          }
        </aside>
      }

      @if (detalleFacturacion(); as detalle) {
        <div class="modal-backdrop print-hide" (click)="cerrarDetalleFacturacion()"></div>
        <aside class="drawer-panel detail-drawer stack" [class.invoice-detail-drawer]="detalle.tipo === 'factura' && mostrarFacturaAdmin()" role="dialog" aria-modal="true">
          <div class="admin-header print-hide">
            <div>
              <h2>{{ tituloDetalle(detalle.tipo) }}</h2>
              <p class="muted">{{ detalle.titulo }}</p>
            </div>
            <span class="actions">
              @if (detalle.tipo === 'factura') {
                <button class="btn secondary" type="button" (click)="toggleFormatoFactura()">{{ mostrarFacturaAdmin() ? 'Ver detalle' : 'Ver formato factura' }}</button>
                @if (mostrarFacturaAdmin()) {
                  <button class="btn" type="button" (click)="imprimir()">Imprimir</button>
                }
              }
              <button class="btn secondary" type="button" (click)="cerrarDetalleFacturacion()">Cerrar</button>
            </span>
          </div>
          @if (detalle.tipo !== 'factura' || !mostrarFacturaAdmin()) {
          <div class="detail-grid">
            <div>
              <h3>Operacion</h3>
              <p><strong>Estado:</strong> {{ valor(detalle.data.estado) }}</p>
              <p><strong>Canal:</strong> {{ valor(detalle.data.origenCanal || detalle.data.origen_canal || detalle.data.pago?.origenCanal || detalle.data.pago?.origen_canal) }}</p>
              <p><strong>Reserva:</strong> {{ valor(codigoReservaDetalle(detalle.data)) }}</p>
              <p><strong>Cliente GUID:</strong> {{ valor(clienteGuidDetalle(detalle.data)) }}</p>
              <p><strong>Observacion:</strong> {{ valor(detalle.data.observacion) }}</p>
            </div>
            <div>
              <h3>Cliente</h3>
              @if (detalle.cliente; as cliente) {
                <p><strong>Nombre:</strong> {{ nombreCliente(cliente) }}</p>
                <p><strong>Identificacion:</strong> {{ valor(cliente.tipoIdentificacion || cliente.tipo_identificacion) }} {{ valor(cliente.numeroIdentificacion || cliente.numero_identificacion) }}</p>
                <p><strong>Correo:</strong> {{ valor(cliente.correo) }}</p>
                <p><strong>Telefono:</strong> {{ valor(cliente.telefono) }}</p>
                <p><strong>Direccion:</strong> {{ valor(cliente.direccion) }}</p>
              } @else {
                <p class="muted">No se pudo resolver el detalle del cliente.</p>
              }
            </div>
            @if (detalle.tipo === 'pago') {
            <div>
              <h3>Pago</h3>
              @if (pagoDetalle(detalle.data); as pago) {
                <p><strong>Metodo:</strong> {{ valor(pago.metodo) }}</p>
                <p><strong>Referencia:</strong> {{ valor(pago.referencia) }}</p>
                <p><strong>Monto:</strong> {{ valor(pago.monto) }} {{ pago.moneda || detalle.data.moneda || 'USD' }}</p>
                <p><strong>Fecha UTC:</strong> {{ valor(pago.fechaUtc || pago.fecha_utc) }}</p>
              } @else {
                <p class="muted">Sin pago asociado.</p>
              }
            </div>
            }
            @if (detalle.tipo === 'reserva') {
            <div>
              <h3>Reserva</h3>
                <p><strong>Atraccion:</strong> {{ valor(detalle.data.atraccionNombre || detalle.data.atraccion_nombre) }}</p>
                <p><strong>Fecha/hora:</strong> {{ valor(detalle.data.horFecha || detalle.data.hor_fecha) }} {{ valor(detalle.data.horHoraInicio || detalle.data.hor_hora_inicio) }} - {{ valor(detalle.data.horHoraFin || detalle.data.hor_hora_fin) }}</p>
                <p><strong>Total:</strong> {{ valor(detalle.data.total) }} {{ detalle.data.moneda || 'USD' }}</p>
            </div>
            }
            @if (detalle.tipo === 'factura') {
            <div>
              <h3>Factura</h3>
                <p><strong>Numero:</strong> {{ valor(detalle.data.numero || detalle.data.fac_numero) }}</p>
                <p><strong>Subtotal:</strong> {{ valor(detalle.data.subtotal) }}</p>
                <p><strong>IVA:</strong> {{ valor(detalle.data.valorIva || detalle.data.valor_iva) }}</p>
                <p><strong>Total:</strong> {{ valor(detalle.data.total) }} {{ detalle.data.moneda || 'USD' }}</p>
                <p><strong>Fecha emision UTC:</strong> {{ valor(detalle.data.fechaEmisionUtc || detalle.data.fecha_emision_utc) }}</p>
            </div>
            }
            @if (detalle.tipo === 'factura') {
            <div>
              <h3>Datos receptor/facturacion</h3>
              @if (datosFacturacionDetalle(detalle.data); as datos) {
                <p><strong>Identificacion:</strong> {{ valor(datos.tipoIdentificacion || datos.tipo_identificacion) }} {{ valor(datos.numeroIdentificacion || datos.numero_identificacion) }}</p>
                <p><strong>Nombre:</strong> {{ nombreReceptor(datos) }}</p>
                <p><strong>Correo:</strong> {{ valor(datos.correo) }}</p>
                <p><strong>Telefono:</strong> {{ valor(datos.telefono) }}</p>
                <p><strong>Direccion:</strong> {{ valor(datos.direccion) }}</p>
              } @else {
                <p class="muted">Sin datos de receptor en este registro.</p>
              }
            </div>
            }
            @if (lineasReserva(detalle.data).length) {
              <div class="wide-detail">
                <h3>Detalle de tickets</h3>
                @for (linea of lineasReserva(detalle.data); track linea.ticketGuid || linea.ticket_guid || $index) {
                  <p><strong>{{ valor(linea.ticketTitulo || linea.ticket_titulo) }}</strong> · cantidad {{ valor(linea.cantidad) }} · subtotal {{ valor(linea.subtotal) }}</p>
                }
              </div>
            }
          </div>
          }
          @if (detalle.tipo === 'factura' && mostrarFacturaAdmin()) {
            <div class="invoice-doc admin-invoice-print stack">
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
                  <span><strong>N. de factura:</strong> {{ detalle.data.numero || detalle.data.fac_numero || '-' }}</span>
                  <span><strong>Fecha:</strong> {{ formatearFecha(fechaFactura(detalle.data)) }}</span>
                  <span><strong>Reserva:</strong> {{ codigoReservaDetalle(detalle.data) || '-' }}</span>
                </div>
              </div>
              <div class="section-head">
                <div>
                  <h2>{{ detalle.data.numero || detalle.data.fac_numero || 'Factura' }}</h2>
                  <p class="muted">{{ detalle.data.estado }} - {{ detalle.data.total }} {{ detalle.data.moneda || 'USD' }}</p>
                </div>
              </div>
              <div class="info-grid">
                @for (item of facturaResumen(detalle.data); track item.label) {
                  <div class="info-item">
                    <span>{{ item.label }}</span>
                    <strong>{{ item.value }}</strong>
                  </div>
                }
              </div>
              <div class="invoice-sections">
                <section>
                  <h3>Facturar a</h3>
                  <p><strong>{{ facturadoA(detalle.data) }}</strong></p>
                  <p>{{ facturadoDocumento(detalle.data) }}</p>
                  <p>{{ facturadoContacto(detalle.data) }}</p>
                  <p>{{ facturadoDireccion(detalle.data) }}</p>
                </section>
                <section>
                  <h3>Atraccion</h3>
                  <p><strong>{{ atraccionFactura(detalle.data).nombre }}</strong></p>
                  <p>{{ atraccionFactura(detalle.data).fecha }} {{ atraccionFactura(detalle.data).hora }}</p>
                  <p>Reserva: {{ codigoReservaDetalle(detalle.data) || '-' }}</p>
                  <p>Estado reserva: {{ reservaFactura(detalle.data)?.estado || '-' }}</p>
                </section>
                <section>
                  <h3>Pago</h3>
                  @if (pagoDetalle(detalle.data); as pago) {
                    <p><strong>{{ pago.metodo || '-' }}</strong></p>
                    <p>Referencia: {{ pago.referencia || '-' }}</p>
                    <p>Estado: {{ pago.estado || '-' }}</p>
                  }
                </section>
              </div>
              @if (lineasFactura(detalle.data).length) {
                <div class="invoice-table">
                  <div class="invoice-row invoice-head">
                    <span>Descripcion</span>
                    <span>Cantidad</span>
                    <span>Precio</span>
                    <span>Total</span>
                  </div>
                  @for (linea of lineasFactura(detalle.data); track linea.guid || linea.ticketGuid || linea.ticket_guid || linea.id || $index) {
                    <div class="invoice-row">
                      <span>{{ linea.titulo || linea.ticketTitulo || linea.ticket_titulo || ('Ticket #' + (linea.ticket_id || linea.ticketId || '-')) }}</span>
                      <span>{{ linea.cantidad }}</span>
                      <span>{{ linea.precio_unitario || linea.precioUnitario }} {{ detalle.data.moneda || 'USD' }}</span>
                      <span>{{ linea.subtotal }} {{ detalle.data.moneda || 'USD' }}</span>
                    </div>
                  }
                  <div class="invoice-total-row"><span>Subtotal</span><strong>{{ detalle.data.subtotal ?? '-' }} {{ detalle.data.moneda || 'USD' }}</strong></div>
                  <div class="invoice-total-row"><span>Total IVA</span><strong>{{ detalle.data.valor_iva ?? detalle.data.valorIva ?? '-' }} {{ detalle.data.moneda || 'USD' }}</strong></div>
                  <div class="invoice-total-row grand"><span>Total</span><strong>{{ detalle.data.total ?? '-' }} {{ detalle.data.moneda || 'USD' }}</strong></div>
                </div>
              }
              <div class="invoice-footer-note">
                <span><strong>Condiciones de pago:</strong> Tarjeta</span>
                <span><strong>Contacto:</strong> Travel of Your Dreams</span>
                <strong>GRACIAS POR SU CONFIANZA</strong>
              </div>
            </div>
          }
        </aside>
      }

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `,
  styles: [`
    .section-toolbar { align-items: end; display: flex; gap: 18px; justify-content: space-between; }
    .group-layout { display: grid; gap: 16px; grid-template-columns: minmax(240px, 320px) 1fr; }
    .group-list, .group-detail { display: grid; gap: 10px; align-content: start; }
    .group-card { background: #fff; border: 1px solid var(--line); border-radius: 8px; cursor: pointer; display: grid; gap: 6px; padding: 14px; text-align: left; }
    .group-card.active { border-color: var(--primary); box-shadow: inset 4px 0 0 var(--primary); }
    .group-card small { color: var(--muted); }
    .modal-backdrop { background: rgba(2, 24, 22, 0.42); inset: 0; position: fixed; z-index: 30; }
    .drawer-panel { background: #fff; border-left: 1px solid var(--line); box-shadow: -22px 0 60px rgba(15, 23, 42, 0.18); inset: 0 0 0 auto; max-width: min(760px, calc(100vw - 24px)); overflow: auto; padding: 28px; position: fixed; width: 58vw; z-index: 31; }
    .detail-drawer { max-width: min(980px, calc(100vw - 24px)); width: 72vw; }
    .invoice-detail-drawer { max-width: min(1040px, calc(100vw - 24px)); width: 76vw; }
    .admin-invoice-print { border: 1px solid var(--line); box-shadow: none; max-width: 860px; }
    .drawer-header { align-items: start; border-bottom: 1px solid var(--line); display: flex; gap: 18px; justify-content: space-between; padding-bottom: 18px; }
    .day-grid { background: var(--surface-muted); border: 1px solid var(--line); border-radius: 8px; display: grid; gap: 8px; grid-template-columns: repeat(4, minmax(0, 1fr)); padding: 12px; }
    .day-grid strong { grid-column: 1 / -1; }
    .check-inline { align-items: center; display: flex; flex-direction: row; gap: 8px; }
    .check-inline input { width: auto; }
    .detail-grid { display: grid; gap: 16px; grid-template-columns: repeat(2, minmax(0, 1fr)); }
    .detail-grid div { background: var(--surface-muted); border: 1px solid var(--line); border-radius: 8px; padding: 14px; }
    .detail-grid p { margin: 6px 0; overflow-wrap: anywhere; }
    .wide-detail { grid-column: 1 / -1; }
    @media (max-width: 860px) {
      .group-layout, .detail-grid { grid-template-columns: 1fr; }
      .drawer-panel { max-width: 100vw; width: 100vw; }
      .section-toolbar, .drawer-header { align-items: stretch; flex-direction: column; }
    }
    @media (max-width: 520px) { .day-grid { grid-template-columns: 1fr; } }
  `]
})
export class AdminOperacionesPageComponent implements OnInit {
  horarios = signal<any[]>([]);
  tickets = signal<any[]>([]);
  reservas = signal<any[]>([]);
  resenias = signal<any[]>([]);
  pagos = signal<any[]>([]);
  facturas = signal<any[]>([]);
  atracciones = signal<any[]>([]);
  mensaje = signal('');
  ticketGuid = signal<string | null>(null);
  horarioGuid = signal<string | null>(null);
  modalOperacion = signal<OperacionModal>(null);
  detalleFacturacion = signal<{ tipo: string; titulo: string; data: any; cliente?: any } | null>(null);
  mostrarFacturaAdmin = signal(false);
  tab = signal<OperacionTab>('horarios');
  atraccionHorariosSeleccionada = signal<number | null>(null);
  atraccionTicketsSeleccionada = signal<number | null>(null);
  busquedas = { horarios: '', tickets: '', reservas: '', resenias: '', pagos: '', facturas: '' };
  diasSemana = [
    { valor: '1', nombre: 'Lun' },
    { valor: '2', nombre: 'Mar' },
    { valor: '3', nombre: 'Mie' },
    { valor: '4', nombre: 'Jue' },
    { valor: '5', nombre: 'Vie' },
    { valor: '6', nombre: 'Sab' },
    { valor: '0', nombre: 'Dom' }
  ];

  horario = this.nuevoHorario();
  ticket = this.nuevoTicket();

  constructor(
    private readonly api: ApiService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.cargarTodo();
  }

  cargarTodo() {
    this.api.adminHorarios().subscribe((response) => {
      this.horarios.set(response.data);
      this.ensureSelectedGroups();
    });
    this.api.adminTickets().subscribe((response) => {
      this.tickets.set(response.data);
      this.ensureSelectedGroups();
    });
    this.api.adminReservas().subscribe((response) => this.reservas.set(this.itemsFromResponse(response.data)));
    this.api.adminResenias().subscribe((response) => this.resenias.set(response.data));
    this.api.adminAtracciones().subscribe((response) => {
      this.atracciones.set(response.data);
      this.ensureSelectedGroups();
    });
  }

  abrirCrearHorario() {
    this.horarioGuid.set(null);
    this.horario = this.nuevoHorario(this.atraccionHorariosSeleccionada() ?? 0);
    this.modalOperacion.set('horario');
  }

  abrirEditarHorario(item: any) {
    this.horarioGuid.set(item.guid);
    this.horario = {
      atraccionId: item.atraccion_id ?? item.atraccionId ?? 0,
      fecha: item.fecha ?? item.hor_fecha ?? '',
      fechaInicio: item.fecha ?? item.hor_fecha ?? this.today(),
      fechaFin: item.fecha ?? item.hor_fecha ?? this.addDays(this.today(), 89),
      horaInicio: item.hora_inicio ?? item.horaInicio ?? '',
      horaFin: item.hora_fin ?? item.horaFin ?? '',
      cuposDisponibles: item.cupos_disponibles ?? item.cuposDisponibles ?? 1,
      diasSemana: String(item.dias_semana ?? item.diasSemana ?? '0,1,2,3,4,5,6').split(',')
    };
    this.modalOperacion.set('horario');
  }

  guardarHorario() {
    const request = this.horarioGuid()
      ? {
          atraccionId: this.horario.atraccionId,
          fecha: this.horario.fecha,
          horaInicio: this.horario.horaInicio,
          horaFin: this.horario.horaFin,
          cuposDisponibles: this.horario.cuposDisponibles,
          diasSemana: this.horario.diasSemana.join(',')
        }
      : {
          atraccionId: this.horario.atraccionId,
          horaInicio: this.horario.horaInicio,
          horaFin: this.horario.horaFin,
          cuposDisponibles: this.horario.cuposDisponibles,
          diasSemana: this.horario.diasSemana.join(','),
          fechaInicio: this.horario.fechaInicio,
          fechaFin: this.horario.fechaFin
        };
    const action = this.horarioGuid()
      ? this.api.actualizarHorario(this.horarioGuid()!, request)
      : this.api.crearHorario(request);
    action.subscribe({
      next: () => {
        this.notifications.success(this.horarioGuid() ? 'Horario puntual guardado.' : 'Horarios generados desde la regla.');
        this.cerrarModalOperacion();
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  abrirCrearTicket() {
    this.ticketGuid.set(null);
    this.ticket = this.nuevoTicket(this.atraccionTicketsSeleccionada() ?? 0);
    this.modalOperacion.set('ticket');
  }

  abrirEditarTicket(item: any) {
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
    this.modalOperacion.set('ticket');
  }

  guardarTicket() {
    const action = this.ticketGuid()
      ? this.api.actualizarTicket(this.ticketGuid()!, this.ticket)
      : this.api.crearTicket(this.ticket);
    action.subscribe({
      next: () => {
        this.notifications.success('Ticket guardado.');
        this.cerrarModalOperacion();
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  cerrarModalOperacion() {
    this.modalOperacion.set(null);
    this.horarioGuid.set(null);
    this.ticketGuid.set(null);
    this.horario = this.nuevoHorario();
    this.ticket = this.nuevoTicket();
  }

  estadoHorario(guid: string, estado: string) {
    this.api.cambiarEstadoHorario(guid, estado).subscribe({
      next: () => this.notifications.success('El cambio rapido de estado no esta expuesto; usa editar horario.'),
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  desactivarHorarios() {
    this.api.desactivarHorariosVencidos().subscribe((response) => {
      this.notifications.success(`Horarios desactivados: ${response.data.total}`);
      this.cargarTodo();
    });
  }

  eliminarTicket(guid: string) {
    if (!confirm('Eliminar este ticket?')) return;
    this.api.eliminarTicket(guid).subscribe({
      next: () => {
        this.notifications.success('Ticket eliminado.');
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  estadoResenia(guid: string, estado: string) {
    this.api.cambiarEstadoResenia(guid, estado).subscribe({
      next: () => {
        this.notifications.success('Resenia moderada.');
        this.cargarTodo();
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  cargarPagos() {
    this.api.adminPagos({ limit: 20 }).subscribe((response) => this.pagos.set(this.itemsFromResponse(response.data)));
  }

  cargarFacturas() {
    this.api.adminFacturas({ limit: 20 }).subscribe((response) => this.facturas.set(this.itemsFromResponse(response.data)));
  }

  verReserva(item: any) {
    const guid = item.guid || item.reservaGuid || item.reserva_guid;
    if (!guid) return this.notifications.error('La reserva no tiene GUID para consultar detalle.');
    this.api.adminReserva(guid).subscribe({
      next: (response) => this.mostrarDetalleConCliente('reserva', response.data?.codigo || guid, response.data),
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo cargar el detalle de la reserva.')
    });
  }

  verPago(item: any) {
    const guid = item.guid || item.pagoGuid || item.pago_guid;
    if (!guid) return this.notifications.error('El pago no tiene GUID para consultar detalle.');
    this.api.adminPago(guid).subscribe({
      next: (response) => this.mostrarDetalleConCliente('pago', item.referencia || guid, response.data),
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo cargar el detalle del pago.')
    });
  }

  verFactura(item: any) {
    const guid = item.guid || item.facturaGuid || item.factura_guid || item.fac_guid;
    if (!guid) return this.notifications.error('La factura no tiene GUID para consultar detalle.');
    this.api.adminFactura(guid).subscribe({
      next: (response) => this.mostrarFacturaConReserva(item.numero || item.fac_numero || guid, response.data),
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo cargar el detalle de la factura.')
    });
  }

  mostrarFacturaConReserva(titulo: string, factura: any) {
    const reservaGuid = this.reservaGuidDetalle(factura);
    if (!reservaGuid) {
      this.mostrarDetalleConCliente('factura', titulo, factura);
      return;
    }

    this.api.adminReserva(reservaGuid).subscribe({
      next: (response) => {
        const reserva: any = response.data;
        this.mostrarDetalleConCliente('factura', titulo, {
          ...factura,
          reserva,
          detalles: factura?.detalles ?? factura?.detalle ?? reserva?.detalles,
          clienteGuid: factura?.clienteGuid ?? factura?.cliente_guid ?? reserva?.clienteGuid ?? reserva?.cliente_guid,
          cliente_guid: factura?.cliente_guid ?? factura?.clienteGuid ?? reserva?.cliente_guid ?? reserva?.clienteGuid
        });
      },
      error: () => this.mostrarDetalleConCliente('factura', titulo, factura)
    });
  }

  mostrarDetalleConCliente(tipo: string, titulo: string, data: any) {
    const detalle = { tipo, titulo, data, cliente: null as any };
    this.mostrarFacturaAdmin.set(false);
    this.detalleFacturacion.set(detalle);
    const clienteGuid = this.clienteGuidDetalle(data);
    if (!clienteGuid) return;
    this.api.adminCliente(clienteGuid).subscribe({
      next: (response) => {
        const cliente = response.data;
        this.detalleFacturacion.set({ ...detalle, data: { ...data, cliente }, cliente });
      },
      error: () => this.detalleFacturacion.set(detalle)
    });
  }

  cerrarDetalleFacturacion() {
    this.mostrarFacturaAdmin.set(false);
    this.detalleFacturacion.set(null);
  }

  toggleFormatoFactura() {
    this.mostrarFacturaAdmin.update((value) => !value);
  }

  tituloDetalle(tipo: string) {
    if (tipo === 'reserva') return 'Detalle de reserva';
    if (tipo === 'pago') return 'Detalle de pago';
    if (tipo === 'factura') return this.mostrarFacturaAdmin() ? 'Formato de factura' : 'Detalle de factura';
    return 'Detalle';
  }

  imprimir() {
    window.print();
  }

  fechaFactura(value: any) {
    return value?.fecha_emision_utc ?? value?.fechaEmisionUtc ?? value?.fecha_emision ?? value?.fechaEmision;
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

  facturaResumen(value: any) {
    return [
      { label: 'Numero', value: value?.numero || value?.fac_numero || '-' },
      { label: 'Fecha emision', value: this.formatearFecha(this.fechaFactura(value)) },
      { label: 'Subtotal', value: `${value?.subtotal ?? '-'} ${value?.moneda ?? 'USD'}` },
      { label: 'IVA', value: `${value?.valor_iva ?? value?.valorIva ?? '-'} ${value?.moneda ?? 'USD'}` },
      { label: 'Total', value: `${value?.total ?? '-'} ${value?.moneda ?? 'USD'}` }
    ];
  }

  reservaFactura(value: any) {
    return value?.reserva ?? value?.Reserva ?? null;
  }

  reservaGuidDetalle(value: any) {
    const reserva = this.reservaFactura(value);
    return value?.reservaGuid
      || value?.reserva_guid
      || value?.revGuid
      || value?.rev_guid
      || value?.pago?.reservaGuid
      || value?.pago?.reserva_guid
      || reserva?.guid
      || reserva?.reservaGuid
      || reserva?.reserva_guid;
  }

  codigoReservaDetalle(value: any) {
    const reserva = this.reservaFactura(value);
    return value?.codigo
      || value?.rev_codigo
      || value?.reservaCodigo
      || value?.reserva_codigo
      || reserva?.codigo
      || reserva?.rev_codigo
      || reserva?.reservaCodigo
      || reserva?.reserva_codigo
      || this.reservaGuidDetalle(value)
      || value?.guid;
  }

  clienteGuidDetalle(value: any) {
    const reserva = this.reservaFactura(value);
    return value?.clienteGuid
      || value?.cliente_guid
      || value?.cliGuid
      || value?.cli_guid
      || value?.pago?.clienteGuid
      || value?.pago?.cliente_guid
      || reserva?.clienteGuid
      || reserva?.cliente_guid;
  }

  lineasFactura(value: any): any[] {
    return this.itemsFromResponse(value?.detalles ?? value?.detalle ?? this.reservaFactura(value)?.detalles);
  }

  facturadoA(value: any) {
    const dato = this.datosFacturacionDetalle(value);
    const cliente = value?.cliente;
    return dato?.razon_social
      || dato?.razonSocial
      || `${dato?.nombre || cliente?.nombres || ''} ${dato?.apellido || cliente?.apellidos || ''}`.trim()
      || cliente?.razonSocial
      || cliente?.razon_social
      || '-';
  }

  facturadoDocumento(value: any) {
    const dato = this.datosFacturacionDetalle(value);
    const cliente = value?.cliente;
    return `${dato?.tipo_identificacion || dato?.tipoIdentificacion || cliente?.tipo_identificacion || cliente?.tipoIdentificacion || ''} ${dato?.numero_identificacion || dato?.numeroIdentificacion || cliente?.numero_identificacion || cliente?.numeroIdentificacion || ''}`.trim() || '-';
  }

  facturadoContacto(value: any) {
    const dato = this.datosFacturacionDetalle(value);
    const cliente = value?.cliente;
    const telefono = dato?.telefono || cliente?.telefono;
    return `${dato?.correo || cliente?.correo || '-'}${telefono ? ' - ' + telefono : ''}`;
  }

  facturadoDireccion(value: any) {
    const dato = this.datosFacturacionDetalle(value);
    const cliente = value?.cliente;
    return dato?.direccion || cliente?.direccion || '-';
  }

  atraccionFactura(value: any) {
    const reserva = this.reservaFactura(value);
    const fecha = reserva?.horFecha ?? reserva?.hor_fecha ?? reserva?.fecha ?? value?.horFecha ?? value?.hor_fecha ?? value?.fecha ?? '-';
    const inicio = reserva?.horHoraInicio ?? reserva?.hor_hora_inicio ?? value?.horHoraInicio ?? value?.hor_hora_inicio ?? '';
    const fin = reserva?.horHoraFin ?? reserva?.hor_hora_fin ?? value?.horHoraFin ?? value?.hor_hora_fin ?? '';

    return {
      nombre: reserva?.atraccionNombre ?? reserva?.atraccion_nombre ?? value?.atraccionNombre ?? value?.atraccion_nombre ?? value?.atraccion?.nombre ?? '-',
      fecha,
      hora: `${inicio}${fin ? ' - ' + fin : ''}`.trim() || '-'
    };
  }

  gruposHorarios() {
    const q = this.busquedas.horarios.toLowerCase();
    return this.atracciones().map((atraccion) => {
      const items = this.horarios().filter((item) => (item.atraccion_id ?? item.atraccionId) === atraccion.id);
      return { id: atraccion.id, nombre: atraccion.nombre, total: items.length, activos: items.filter((x) => (x.estado ?? 'A') === 'A').length };
    }).filter((grupo) => grupo.total > 0 && (!q || grupo.nombre.toLowerCase().includes(q)));
  }

  gruposTickets() {
    const q = this.busquedas.tickets.toLowerCase();
    return this.atracciones().map((atraccion) => {
      const items = this.tickets().filter((item) => (item.atraccion_id ?? item.atraccionId) === atraccion.id);
      return { id: atraccion.id, nombre: atraccion.nombre, total: items.length, activos: items.filter((x) => (x.estado ?? 'A') === 'A').length };
    }).filter((grupo) => grupo.total > 0 && (!q || grupo.nombre.toLowerCase().includes(q)));
  }

  horariosSeleccionados() {
    const selected = this.atraccionHorariosSeleccionada();
    const q = this.busquedas.horarios.toLowerCase();
    if (!selected) return [];
    return this.horarios().filter((item) => {
      const text = `${item.fecha ?? item.hor_fecha} ${item.hora_inicio ?? item.horaInicio} ${item.estado}`.toLowerCase();
      return (item.atraccion_id ?? item.atraccionId) === selected && (!q || text.includes(q) || this.nombreAtraccion(selected).toLowerCase().includes(q));
    });
  }

  ticketsSeleccionados() {
    const selected = this.atraccionTicketsSeleccionada();
    const q = this.busquedas.tickets.toLowerCase();
    if (!selected) return [];
    return this.tickets().filter((item) => {
      const text = `${item.titulo} ${item.estado}`.toLowerCase();
      return (item.atraccion_id ?? item.atraccionId) === selected && (!q || text.includes(q) || this.nombreAtraccion(selected).toLowerCase().includes(q));
    });
  }

  reservasFiltradas() {
    const q = this.busquedas.reservas.toLowerCase();
    return this.itemsFromResponse(this.reservas()).filter((item) => !q || `${item.codigo} ${item.estado} ${item.origenCanal ?? item.origen_canal} ${item.clienteGuid ?? item.cliente_guid}`.toLowerCase().includes(q));
  }

  pagosFiltrados() {
    const q = this.busquedas.pagos.toLowerCase();
    return this.itemsFromResponse(this.pagos()).filter((item) => !q || `${item.referencia} ${item.metodo} ${item.estado}`.toLowerCase().includes(q));
  }

  facturasFiltradas() {
    const q = this.busquedas.facturas.toLowerCase();
    return this.itemsFromResponse(this.facturas()).filter((item) => !q || `${item.numero ?? item.fac_numero} ${item.estado}`.toLowerCase().includes(q));
  }

  reseniasFiltradas() {
    const q = this.busquedas.resenias.toLowerCase();
    return this.resenias().filter((item) => !q || `${item.comentario} ${item.usuario_creacion ?? item.usuarioCreacion} ${item.estado}`.toLowerCase().includes(q));
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

  pagoDetalle(data: any) {
    return data?.pago ?? data?.Pago ?? (data?.metodo || data?.referencia ? data : null);
  }

  datosFacturacionDetalle(data: any) {
    const nested = data?.datosFacturacion ?? data?.datos_facturacion ?? data?.DatosFacturacion;
    if (nested) return nested;
    const hasFlatBilling = data?.tipoIdentificacion
      || data?.tipo_identificacion
      || data?.numeroIdentificacion
      || data?.numero_identificacion
      || data?.nombreReceptor
      || data?.nombre_receptor
      || data?.apellidoReceptor
      || data?.apellido_receptor
      || data?.correoReceptor
      || data?.correo_receptor;
    if (!hasFlatBilling) return null;
    return {
      tipoIdentificacion: data?.tipoIdentificacion || data?.tipo_identificacion,
      numeroIdentificacion: data?.numeroIdentificacion || data?.numero_identificacion,
      nombre: data?.nombre || data?.nombreReceptor || data?.nombre_receptor,
      apellido: data?.apellido || data?.apellidoReceptor || data?.apellido_receptor,
      razonSocial: data?.razonSocial || data?.razon_social,
      correo: data?.correo || data?.correoReceptor || data?.correo_receptor,
      telefono: data?.telefono || data?.telefonoReceptor || data?.telefono_receptor,
      direccion: data?.direccion
    };
  }

  nombreReceptor(datos: any) {
    return [datos?.nombre, datos?.apellido].filter(Boolean).join(' ') || '-';
  }

  nombreCliente(cliente: any) {
    return [cliente?.nombres, cliente?.apellidos].filter(Boolean).join(' ') || cliente?.razonSocial || cliente?.razon_social || '-';
  }

  lineasReserva(data: any): any[] {
    const value = data?.detalles ?? data?.detalle ?? data?.lineas ?? [];
    return Array.isArray(value) ? value : [];
  }

  valor(value: any) {
    return value === null || value === undefined || value === '' ? '-' : value;
  }

  private ensureSelectedGroups() {
    if (!this.atraccionHorariosSeleccionada() && this.gruposHorarios()[0]) this.atraccionHorariosSeleccionada.set(this.gruposHorarios()[0].id);
    if (!this.atraccionTicketsSeleccionada() && this.gruposTickets()[0]) this.atraccionTicketsSeleccionada.set(this.gruposTickets()[0].id);
  }

  private itemsFromResponse(value: any): any[] {
    if (Array.isArray(value)) return value;
    if (Array.isArray(value?.items)) return value.items;
    if (Array.isArray(value?.data)) return value.data;
    return [];
  }

  private nuevoTicket(atraccionId = 0) {
    return {
      atraccionId,
      titulo: '',
      precio: 0,
      moneda: 'USD',
      tipoParticipante: 'Adulto',
      capacidadMaxima: 1,
      estado: 'A'
    };
  }

  private nuevoHorario(atraccionId = 0) {
    const today = this.today();
    return {
      atraccionId,
      fecha: '',
      fechaInicio: today,
      fechaFin: this.addDays(today, 89),
      horaInicio: '',
      horaFin: '',
      cuposDisponibles: 1,
      diasSemana: ['1', '2', '3', '4', '5', '6', '0']
    };
  }

  private today() {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 10);
  }

  private addDays(value: string, days: number) {
    const date = new Date(`${value}T00:00:00`);
    date.setDate(date.getDate() + days);
    date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
    return date.toISOString().slice(0, 10);
  }
}
