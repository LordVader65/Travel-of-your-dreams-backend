import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-admin-clientes-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div class="page-title">
          <h1>Clientes</h1>
          <p class="muted">Consulta, activacion, desactivacion y datos de facturacion.</p>
        </div>
        <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
      </header>

      <div class="panel stack">
        <input name="buscarClientes" placeholder="Buscar cliente, correo o identificacion" [(ngModel)]="busqueda" />
        @for (cliente of clientesFiltrados(); track cliente.guid) {
          <div class="compact-row">
            <span>
              <strong>{{ nombreCliente(cliente) }}</strong>
              <small>{{ cliente.correo }} - {{ cliente.numeroIdentificacion || cliente.numero_identificacion || 'Sin identificacion' }} - {{ cliente.estado }}</small>
            </span>
            <span class="actions">
              <button class="btn secondary" type="button" (click)="abrirDetalle(cliente.guid)">Ver</button>
              <button class="btn" type="button" (click)="estado(cliente.guid, 'A')">Activar</button>
              <button class="btn danger" type="button" (click)="estado(cliente.guid, 'I')">Desactivar</button>
            </span>
          </div>
        } @empty {
          <p class="muted">No hay clientes para mostrar.</p>
        }
      </div>

      @if (detalleAbierto()) {
        <div class="modal-backdrop" (click)="cerrarDetalle()"></div>
        <aside class="drawer-panel stack" role="dialog" aria-modal="true" aria-label="Detalle de cliente">
          <header class="section-head">
            <div>
              <h2>Detalle del cliente</h2>
              <p class="muted">{{ seleccionado() ? nombreCliente(seleccionado()) : 'Cargando informacion del cliente.' }}</p>
            </div>
            <button class="btn secondary" type="button" (click)="cerrarDetalle()">Cerrar</button>
          </header>

          @if (seleccionado()) {
            <div class="info-grid">
              @for (item of clienteResumen(seleccionado()); track item.label) {
                <div class="info-item">
                  <span>{{ item.label }}</span>
                  <strong>{{ item.value }}</strong>
                </div>
              }
            </div>
            <h3>Datos de facturacion</h3>
            @for (dato of facturacion(); track dato.guid || dato.id || $index) {
              <div class="compact-row">
                <span>
                  <strong>{{ dato.razon_social || dato.razonSocial || dato.nombre || dato.correo }}</strong>
                  <small>{{ dato.tipo_identificacion || dato.tipoIdentificacion }} - {{ dato.numero_identificacion || dato.numeroIdentificacion }}</small>
                </span>
                <span>{{ dato.estado }}</span>
              </div>
            } @empty {
              <p class="muted">Sin datos de facturacion.</p>
            }
          } @else {
            <p class="muted">Cargando cliente...</p>
          }
        </aside>
      }

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `,
  styles: [`
    .modal-backdrop { background: rgba(2, 24, 22, 0.42); inset: 0; position: fixed; z-index: 30; }
    .drawer-panel { background: #fff; border-left: 1px solid var(--line); box-shadow: -22px 0 60px rgba(15, 23, 42, 0.18); inset: 0 0 0 auto; max-width: min(860px, calc(100vw - 24px)); overflow: auto; padding: 28px; position: fixed; width: 68vw; z-index: 31; }
    .section-head { align-items: flex-start; display: flex; gap: 16px; justify-content: space-between; }
    .info-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
    @media (max-width: 800px) {
      .drawer-panel { max-width: 100vw; width: 100vw; }
      .compact-row { align-items: stretch; flex-direction: column; }
      .actions { width: 100%; }
      .actions .btn { flex: 1; }
      .info-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class AdminClientesPageComponent implements OnInit {
  clientes = signal<any[]>([]);
  seleccionado = signal<any | null>(null);
  facturacion = signal<any[]>([]);
  detalleAbierto = signal(false);
  mensaje = signal('');
  busqueda = '';

  constructor(private readonly api: ApiService, private readonly notifications: NotificationService) {}

  ngOnInit() {
    this.cargar();
  }

  cargar() {
    this.api.adminClientes().subscribe((response) => this.clientes.set(response.data));
  }

  ver(guid: string) {
    this.api.adminCliente(guid).subscribe((response) => this.seleccionado.set(response.data));
    this.api.adminDatosFacturacionCliente(guid).subscribe((response) => this.facturacion.set(response.data));
  }

  abrirDetalle(guid: string) {
    this.seleccionado.set(null);
    this.facturacion.set([]);
    this.detalleAbierto.set(true);
    this.ver(guid);
  }

  cerrarDetalle() {
    this.detalleAbierto.set(false);
    this.seleccionado.set(null);
    this.facturacion.set([]);
  }

  estado(guid: string, estado: string) {
    this.api.cambiarEstadoCliente(guid, estado).subscribe({
      next: () => {
        this.mensaje.set('Estado de cliente actualizado.');
        this.notifications.success('Estado de cliente actualizado.');
        this.cargar();
      },
      error: (error) => this.notifications.error(error?.error?.message ?? error?.error?.error ?? 'No se pudo actualizar el cliente.')
    });
  }

  clienteResumen(cliente: any) {
    return [
      { label: 'Tipo identificacion', value: cliente.tipoIdentificacion || cliente.tipo_identificacion || '-' },
      { label: 'Identificacion', value: cliente.numeroIdentificacion || cliente.numero_identificacion || '-' },
      { label: 'Nombres', value: cliente.nombres || '-' },
      { label: 'Apellidos', value: cliente.apellidos || '-' },
      { label: 'Razon social', value: cliente.razonSocial || cliente.razon_social || '-' },
      { label: 'Correo', value: cliente.correo || '-' },
      { label: 'Telefono', value: cliente.telefono || '-' },
      { label: 'Direccion', value: cliente.direccion || '-' },
      { label: 'Usuario vinculado', value: cliente.usuarioGuid || cliente.usuario_guid || cliente.usu_guid || 'Cliente invitado / sin login' },
      { label: 'Estado', value: cliente.estado || '-' }
    ];
  }

  nombreCliente(cliente: any) {
    return [cliente?.nombres, cliente?.apellidos].filter(Boolean).join(' ') || cliente?.razonSocial || cliente?.razon_social || cliente?.correo || 'Cliente';
  }

  clientesFiltrados() {
    const q = this.busqueda.toLowerCase();
    return this.clientes().filter((cliente) => !q || `${cliente.nombres} ${cliente.apellidos} ${cliente.razon_social ?? cliente.razonSocial} ${cliente.correo} ${cliente.numero_identificacion ?? cliente.numeroIdentificacion} ${cliente.estado}`.toLowerCase().includes(q));
  }
}
