import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-admin-atracciones-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div>
          <h1>Atracciones</h1>
          <p class="muted">Gestion comercial, relaciones y disponibilidad del catalogo publico.</p>
        </div>
        <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
      </header>

      <div class="grid two">
        <form class="panel form-grid" (ngSubmit)="guardar()">
          <h2>{{ editGuid() ? 'Editar atraccion' : 'Nueva atraccion' }}</h2>
          <label>Destino
            <select name="destinoId" [(ngModel)]="form.destinoId" required>
              <option [ngValue]="0">Selecciona destino</option>
              @for (destino of destinos(); track destino.id) {
                <option [ngValue]="destino.id">{{ destino.nombre }} · {{ destino.pais }}</option>
              }
            </select>
          </label>
          <label>Nombre <input name="nombre" maxlength="200" [(ngModel)]="form.nombre" required /></label>
          <label>Direccion <input name="direccion" maxlength="300" [(ngModel)]="form.direccion" /></label>
          <label>Duracion minutos <input type="number" min="0" name="duracionMinutos" [(ngModel)]="form.duracionMinutos" /></label>
          <label>Precio referencia <input type="number" min="0" name="precioReferencia" [(ngModel)]="form.precioReferencia" /></label>
          <label>Estado
            <select name="estado" [(ngModel)]="form.estado">
              <option value="A">Activo</option>
              <option value="I">Inactivo</option>
            </select>
          </label>
          <label class="wide">Descripcion <textarea name="descripcion" maxlength="2000" [(ngModel)]="form.descripcion"></textarea></label>
          <label class="toggle-line"><input type="checkbox" name="freeCancellation" [(ngModel)]="form.freeCancellation" /> Cancelacion gratis</label>
          <label class="toggle-line"><input type="checkbox" name="skipTheLine" [(ngModel)]="form.skipTheLine" /> Sin fila</label>
          <div class="actions wide">
            <button class="btn" type="submit">Guardar</button>
            <button class="btn secondary" type="button" (click)="limpiar()">Limpiar</button>
          </div>
        </form>

        @if (atraccionCaracteristicas(); as atraccion) {
          <form class="panel form-grid" (ngSubmit)="guardarCaracteristicas()">
            <h2>Caracteristicas</h2>
            <p class="muted wide">Marca las opciones activas que quieres asociar a {{ atraccion.nombre }}. Las que ya estaban asociadas aparecen seleccionadas.</p>

            <div class="wide feature-admin-grid">
              <div class="nested-form stack">
                <h3>Categorias</h3>
                @for (item of catalogoActivo(categorias()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [checked]="seleccion.categorias.includes(item.id)" (change)="toggleSeleccion('categorias', item.id)" /> {{ etiquetaCatalogo(item) }}</label>
                }
              </div>
              <div class="nested-form stack">
                <h3>Idiomas</h3>
                @for (item of catalogoActivo(idiomas()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [checked]="seleccion.idiomas.includes(item.id)" (change)="toggleSeleccion('idiomas', item.id)" /> {{ etiquetaCatalogo(item) }}</label>
                }
              </div>
              <div class="nested-form stack">
                <h3>Incluye / No incluye</h3>
                @for (item of catalogoActivo(incluye()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [checked]="seleccion.incluye.includes(item.id)" (change)="toggleSeleccion('incluye', item.id)" /> {{ etiquetaCatalogo(item) }} <small>{{ item.tipo }}</small></label>
                }
              </div>
              <div class="nested-form stack">
                <h3>Imagenes</h3>
                @for (item of catalogoActivo(imagenes()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [checked]="seleccion.imagenes.includes(item.id)" (change)="toggleSeleccion('imagenes', item.id)" /> {{ etiquetaCatalogo(item) }}</label>
                  @if (seleccion.imagenes.includes(item.id)) {
                    <label class="toggle-line"><input type="radio" name="imagenPrincipal" [value]="item.id" [(ngModel)]="imagenPrincipalId" /> Imagen principal</label>
                  }
                }
                <label>Orden en galeria <input type="number" min="1" name="ordenImagenPrincipal" [(ngModel)]="ordenImagenPrincipal" /></label>
              </div>
            </div>

            <div class="actions wide">
              <button class="btn" type="submit">Guardar caracteristicas</button>
              <button class="btn secondary" type="button" (click)="cerrarCaracteristicas()">Cerrar</button>
            </div>
          </form>
        } @else {
          <div class="panel stack">
            <h2>Caracteristicas</h2>
            <p class="muted">Selecciona el boton Caracteristicas en una atraccion del listado para relacionar categorias, idiomas, imagenes e inclusiones.</p>
          </div>
        }
      </div>

      <div class="table panel">
        <input name="buscarAtracciones" placeholder="Buscar atraccion, destino o estado" [(ngModel)]="busqueda" />
        <div class="row head">
          <span>Nombre</span><span>Destino</span><span>Estado</span><span>Acciones</span>
        </div>
        @for (item of atraccionesFiltradas(); track item.guid) {
          <div class="row">
            <span><strong>{{ item.nombre }}</strong><small>{{ item.guid }}</small></span>
            <span>{{ item.destino_id || item.destinoId || '-' }}</span>
            <span>{{ item.estado || (item.disponible ? 'A' : 'I') }}</span>
            <span class="actions">
              <button class="btn" type="button" (click)="abrirCaracteristicas(item)">Caracteristicas</button>
              <button class="btn secondary" type="button" (click)="editar(item)">Editar</button>
              <button class="btn danger" type="button" (click)="eliminar(item.guid)">Eliminar</button>
            </span>
          </div>
        } @empty {
          <p class="muted">No hay atracciones para mostrar.</p>
        }
      </div>

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `
})
export class AdminAtraccionesPageComponent implements OnInit {
  atracciones = signal<any[]>([]);
  destinos = signal<any[]>([]);
  categorias = signal<any[]>([]);
  idiomas = signal<any[]>([]);
  imagenes = signal<any[]>([]);
  incluye = signal<any[]>([]);
  editGuid = signal<string | null>(null);
  atraccionCaracteristicas = signal<any | null>(null);
  mensaje = signal('');
  busqueda = '';
  form = this.nuevaForma();
  seleccion = this.nuevaSeleccion();
  seleccionOriginal = this.nuevaSeleccion();
  imagenPrincipalId = 0;
  ordenImagenPrincipal = 1;

  constructor(private readonly api: ApiService, private readonly notifications: NotificationService) {}

  ngOnInit() {
    this.cargar();
  }

  cargar() {
    this.api.adminAtracciones().subscribe((response) => this.atracciones.set(response.data));
    this.api.catalogo('destinos').subscribe((response) => this.destinos.set(response.data));
    this.api.catalogo('categorias').subscribe((response) => this.categorias.set(response.data));
    this.api.catalogo('idiomas').subscribe((response) => this.idiomas.set(response.data));
    this.api.catalogo('imagenes').subscribe((response) => this.imagenes.set(response.data));
    this.api.catalogo('incluye').subscribe((response) => this.incluye.set(response.data));
  }

  catalogoActivo(items: any[]) {
    return items.filter((item) => (item.estado ?? 'A') === 'A');
  }

  guardar() {
    const request = { ...this.form, disponible: this.form.estado === 'A' };
    const action = this.editGuid()
      ? this.api.adminActualizarAtraccion(this.editGuid()!, request)
      : this.api.adminCrearAtraccion(request);

    action.subscribe({
      next: () => {
        this.mensaje.set('Atraccion guardada.');
        this.notifications.success('Atraccion guardada.');
        this.limpiar();
        this.cargar();
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo guardar la atraccion.')
    });
  }

  editar(item: any) {
    this.editGuid.set(item.guid);
    this.form = {
      destinoId: item.destino_id ?? item.destinoId ?? 0,
      numeroEstablecimiento: item.numero_establecimiento ?? item.numeroEstablecimiento ?? '',
      nombre: item.nombre ?? '',
      descripcion: item.descripcion ?? '',
      direccion: item.direccion ?? '',
      duracionMinutos: item.duracion_minutos ?? item.duracionMinutos ?? null,
      puntoEncuentro: item.punto_encuentro ?? item.puntoEncuentro ?? '',
      precioReferencia: item.precio_referencia ?? item.precioReferencia ?? null,
      incluyeAcompaniante: item.incluye_acompaniante ?? item.incluyeAcompaniante ?? false,
      incluyeTransporte: item.incluye_transporte ?? item.incluyeTransporte ?? false,
      disponible: (item.estado ?? 'A') === 'A',
      freeCancellation: item.free_cancellation ?? item.freeCancellation ?? false,
      skipTheLine: item.skip_the_line ?? item.skipTheLine ?? false,
      estado: item.estado ?? 'A'
    };
  }

  eliminar(guid: string) {
    if (!confirm('Eliminar esta atraccion?')) return;
    this.api.adminEliminarAtraccion(guid).subscribe({
      next: () => {
        this.mensaje.set('Atraccion eliminada.');
        this.notifications.success('Atraccion eliminada.');
        this.cargar();
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo eliminar la atraccion.')
    });
  }

  abrirCaracteristicas(item: any) {
    this.atraccionCaracteristicas.set(item);
    this.seleccion = this.nuevaSeleccion();
    this.seleccionOriginal = this.nuevaSeleccion();
    this.imagenPrincipalId = 0;
    this.api.obtenerAtraccion(item.guid).subscribe({
      next: (response) => {
        const detail: any = response.data;
        this.seleccion = {
          categorias: (detail.categorias || []).map((x: any) => x.id).filter(Boolean),
          idiomas: (detail.idiomas || []).map((x: any) => x.id).filter(Boolean),
          imagenes: (detail.imagenes || []).map((x: any) => x.id).filter(Boolean),
          incluye: (detail.incluye || []).map((x: any) => x.id).filter(Boolean)
        };
        this.seleccionOriginal = {
          categorias: [...this.seleccion.categorias],
          idiomas: [...this.seleccion.idiomas],
          imagenes: [...this.seleccion.imagenes],
          incluye: [...this.seleccion.incluye]
        };
        this.imagenPrincipalId = this.seleccion.imagenes[0] ?? 0;
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudieron cargar las caracteristicas.')
    });
  }

  cerrarCaracteristicas() {
    this.atraccionCaracteristicas.set(null);
    this.seleccion = this.nuevaSeleccion();
    this.seleccionOriginal = this.nuevaSeleccion();
    this.imagenPrincipalId = 0;
  }

  etiquetaCatalogo(item: any) {
    return item.nombre || item.descripcion || item.codigo || item.url || `Registro ${item.id}`;
  }

  toggleSeleccion(tipo: 'categorias' | 'idiomas' | 'imagenes' | 'incluye', id: number) {
    this.seleccion[tipo] = this.seleccion[tipo].includes(id)
      ? this.seleccion[tipo].filter((item) => item !== id)
      : [...this.seleccion[tipo], id];
    if (tipo === 'imagenes' && !this.seleccion.imagenes.includes(this.imagenPrincipalId)) {
      this.imagenPrincipalId = this.seleccion.imagenes[0] ?? 0;
    }
  }

  guardarCaracteristicas() {
    const atraccionId = this.atraccionCaracteristicas()?.id ?? 0;
    if (!atraccionId) {
      this.notifications.error('Selecciona una atraccion.');
      return;
    }

    const nuevasCategorias = this.nuevosIds('categorias').map((id) => this.api.asociarCategoriaAtraccion(atraccionId, id));
    const nuevosIdiomas = this.nuevosIds('idiomas').map((id) => this.api.asociarIdiomaAtraccion(atraccionId, id));
    const nuevosIncluye = this.nuevosIds('incluye').map((id) => this.api.asociarIncluyeAtraccion(atraccionId, id));
    const imagenesPorGuardar = this.seleccion.imagenes
      .filter((id) => !this.seleccionOriginal.imagenes.includes(id) || id === this.imagenPrincipalId)
      .map((id) => this.api.asociarImagenAtraccion(atraccionId, id, id === this.imagenPrincipalId, id === this.imagenPrincipalId ? this.ordenImagenPrincipal || 1 : 2));
    const removerCategorias = this.removidosIds('categorias').map((id) => this.api.desasociarCategoriaAtraccion(atraccionId, id));
    const removerIdiomas = this.removidosIds('idiomas').map((id) => this.api.desasociarIdiomaAtraccion(atraccionId, id));
    const removerIncluye = this.removidosIds('incluye').map((id) => this.api.desasociarIncluyeAtraccion(atraccionId, id));
    const removerImagenes = this.removidosIds('imagenes').map((id) => this.api.desasociarImagenAtraccion(atraccionId, id));
    const operaciones = [...nuevasCategorias, ...nuevosIdiomas, ...nuevosIncluye, ...imagenesPorGuardar, ...removerCategorias, ...removerIdiomas, ...removerIncluye, ...removerImagenes];

    if (operaciones.length === 0) {
      this.notifications.success('No hay caracteristicas nuevas por agregar.');
      return;
    }

    forkJoin(operaciones).subscribe({
      next: () => {
        this.mensaje.set('Caracteristicas guardadas.');
        this.notifications.success('Caracteristicas guardadas.');
        const item = this.atraccionCaracteristicas();
        if (item) this.abrirCaracteristicas(item);
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudieron guardar las caracteristicas.')
    });
  }

  private nuevosIds(tipo: 'categorias' | 'idiomas' | 'imagenes' | 'incluye') {
    return this.seleccion[tipo].filter((id) => !this.seleccionOriginal[tipo].includes(id));
  }

  private removidosIds(tipo: 'categorias' | 'idiomas' | 'imagenes' | 'incluye') {
    return this.seleccionOriginal[tipo].filter((id) => !this.seleccion[tipo].includes(id));
  }

  atraccionesFiltradas() {
    const q = this.busqueda.toLowerCase();
    return this.atracciones().filter((item) => !q || `${item.nombre} ${item.guid} ${item.estado} ${item.destino_id ?? item.destinoId}`.toLowerCase().includes(q));
  }

  limpiar() {
    this.editGuid.set(null);
    this.form = this.nuevaForma();
  }

  private nuevaForma() {
    return {
      destinoId: 0,
      numeroEstablecimiento: '',
      nombre: '',
      descripcion: '',
      direccion: '',
      duracionMinutos: null as number | null,
      puntoEncuentro: '',
      precioReferencia: null as number | null,
      incluyeAcompaniante: false,
      incluyeTransporte: false,
      disponible: true,
      freeCancellation: false,
      skipTheLine: false,
      estado: 'A'
    };
  }

  private nuevaSeleccion() {
    return {
      categorias: [] as number[],
      idiomas: [] as number[],
      imagenes: [] as number[],
      incluye: [] as number[]
    };
  }
}
