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
        <div>
          <h1>Clientes</h1>
          <p class="muted">Consulta, activacion, desactivacion y datos de facturacion.</p>
        </div>
        <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
      </header>

      <div class="grid two">
        <div class="panel stack">
          <input name="buscarClientes" placeholder="Buscar cliente, correo o identificacion" [(ngModel)]="busqueda" />
          @for (cliente of clientesFiltrados(); track cliente.guid) {
            <div class="compact-row">
              <span>
                <strong>{{ cliente.nombres || cliente.razon_social || cliente.correo }}</strong>
                <small>{{ cliente.correo }} · {{ cliente.estado }}</small>
              </span>
              <span class="actions">
                <button class="btn secondary" type="button" (click)="ver(cliente.guid)">Ver</button>
                <button class="btn" type="button" (click)="estado(cliente.guid, 'A')">Activar</button>
                <button class="btn danger" type="button" (click)="estado(cliente.guid, 'I')">Desactivar</button>
              </span>
            </div>
          } @empty {
            <p class="muted">No hay clientes para mostrar.</p>
          }
        </div>

        <div class="panel stack">
          <h2>Detalle</h2>
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
                  <strong>{{ dato.razon_social || dato.nombre || dato.correo }}</strong>
                  <small>{{ dato.tipo_identificacion || dato.tipoIdentificacion }} - {{ dato.numero_identificacion || dato.numeroIdentificacion }}</small>
                </span>
                <span>{{ dato.estado }}</span>
              </div>
            } @empty {
              <p class="muted">Sin datos de facturacion.</p>
            }
          } @else {
            <p class="muted">Selecciona un cliente.</p>
          }
        </div>
      </div>

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `
})
export class AdminClientesPageComponent implements OnInit {
  clientes = signal<any[]>([]);
  seleccionado = signal<any | null>(null);
  facturacion = signal<any[]>([]);
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

  estado(guid: string, estado: string) {
    this.api.cambiarEstadoCliente(guid, estado).subscribe({
      next: () => {
        this.mensaje.set('Estado de cliente actualizado.');
        this.notifications.success('Estado de cliente actualizado.');
        this.cargar();
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo actualizar el cliente.')
    });
  }

  clienteResumen(cliente: any) {
    return [
      { label: 'Nombre', value: cliente.nombres || cliente.razon_social || '-' },
      { label: 'Apellido', value: cliente.apellidos || '-' },
      { label: 'Correo', value: cliente.correo || '-' },
      { label: 'Telefono', value: cliente.telefono || '-' },
      { label: 'Estado', value: cliente.estado || '-' }
    ];
  }

  clientesFiltrados() {
    const q = this.busqueda.toLowerCase();
    return this.clientes().filter((cliente) => !q || `${cliente.nombres} ${cliente.apellidos} ${cliente.razon_social ?? cliente.razonSocial} ${cliente.correo} ${cliente.numero_identificacion ?? cliente.numeroIdentificacion} ${cliente.estado}`.toLowerCase().includes(q));
  }
}
