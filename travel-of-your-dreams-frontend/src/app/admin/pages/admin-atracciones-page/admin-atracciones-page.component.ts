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
        <div class="page-title">
          <h1>Atracciones</h1>
          <p class="muted">Gestion comercial, relaciones y disponibilidad del catalogo publico.</p>
        </div>
        <span class="actions">
          <button class="btn" type="button" (click)="abrirNuevaAtraccion()">Nueva atraccion</button>
          <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
        </span>
      </header>

      @if (modalAbierto()) {
        <div class="modal-backdrop" (click)="cerrarModal()"></div>
        <aside class="drawer-panel stack" role="dialog" aria-modal="true" aria-label="Editor de atraccion">
          <header class="drawer-header">
            <div>
              <h2>{{ tituloModal() }}</h2>
              <p class="muted">{{ soloLectura() ? 'Consulta visual de la informacion registrada.' : 'Datos generales y caracteristicas de catalogo en un solo lugar.' }}</p>
            </div>
            <button class="btn secondary" type="button" (click)="cerrarModal()">Cerrar</button>
          </header>

        <form class="form-grid" (ngSubmit)="guardar()">
          <fieldset class="wide form-grid fieldset-reset" [disabled]="soloLectura()">
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
          <label>Punto de encuentro <input name="puntoEncuentro" maxlength="300" [(ngModel)]="form.puntoEncuentro" /></label>
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
          <label class="toggle-line"><input type="checkbox" name="incluyeTransporte" [(ngModel)]="form.incluyeTransporte" /> Incluye transporte</label>
          <label class="toggle-line"><input type="checkbox" name="incluyeAcompaniante" [(ngModel)]="form.incluyeAcompaniante" /> Incluye acompaniante/guia</label>
          </fieldset>
          <div class="actions wide">
            @if (!soloLectura()) {
              <button class="btn" type="submit">{{ editGuid() ? 'Guardar informacion' : 'Crear atraccion' }}</button>
              <button class="btn secondary" type="button" (click)="limpiar()">Limpiar</button>
            }
          </div>
        </form>

          <form class="form-grid drawer-section" (ngSubmit)="guardarCaracteristicas()">
            <h2>Caracteristicas</h2>
            <p class="muted wide">Marca las opciones activas que quieres asociar a {{ nombreAtraccionCaracteristicas() }}. En una atraccion nueva se guardaran despues de crear el registro principal.</p>

            <div class="wide feature-admin-grid">
              <div class="nested-form stack">
                <h3>Categorias</h3>
                @for (item of catalogoActivo(categorias()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [disabled]="soloLectura()" [checked]="seleccion.categorias.includes(item.id)" (change)="toggleCategoria(item)" /> {{ etiquetaCategoria(item) }}</label>
                  @if (seleccion.categorias.includes(item.id)) {
                    <label class="toggle-line"><input type="radio" [disabled]="soloLectura()" name="categoriaPrincipal" [value]="item.id" [(ngModel)]="categoriaPrincipalId" /> Principal Booking</label>
                  }
                }
              </div>
              <div class="nested-form stack">
                <h3>Idiomas</h3>
                @for (item of catalogoActivo(idiomas()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [disabled]="soloLectura()" [checked]="seleccion.idiomas.includes(item.id)" (change)="toggleSeleccion('idiomas', item.id)" /> {{ etiquetaCatalogo(item) }}</label>
                }
              </div>
              <div class="nested-form stack">
                <h3>Incluye / No incluye</h3>
                @for (item of catalogoActivo(incluye()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [disabled]="soloLectura()" [checked]="seleccion.incluye.includes(item.id)" (change)="toggleSeleccion('incluye', item.id)" /> {{ etiquetaCatalogo(item) }} <small>{{ item.tipo }}</small></label>
                }
              </div>
              <div class="nested-form stack">
                <h3>Imagenes</h3>
                @for (item of catalogoActivo(imagenes()); track item.id) {
                  <label class="toggle-line"><input type="checkbox" [disabled]="soloLectura()" [checked]="seleccion.imagenes.includes(item.id)" (change)="toggleSeleccion('imagenes', item.id)" /> {{ etiquetaCatalogo(item) }}</label>
                  @if (seleccion.imagenes.includes(item.id)) {
                    <label class="toggle-line"><input type="radio" [disabled]="soloLectura()" name="imagenPrincipal" [value]="item.id" [(ngModel)]="imagenPrincipalId" /> Imagen principal</label>
                  }
                }
                <label>Orden en galeria <input type="number" min="1" name="ordenImagenPrincipal" [(ngModel)]="ordenImagenPrincipal" [disabled]="soloLectura()" /></label>
              </div>
            </div>

            <div class="actions wide">
              @if (!soloLectura()) {
                <button class="btn" type="submit" [disabled]="!atraccionCaracteristicas()?.id">Guardar caracteristicas</button>
                <button class="btn secondary" type="button" (click)="recargarCaracteristicas()">Revertir cambios</button>
              }
            </div>
          </form>
        </aside>
      }

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
              <button class="btn" type="button" (click)="abrirEditorCompleto(item)">Editar</button>
              <button class="btn secondary" type="button" (click)="abrirVisualizacion(item)">Ver</button>
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
  `,
  styles: [`
    .modal-backdrop { background: rgba(2, 24, 22, 0.42); inset: 0; position: fixed; z-index: 30; }
    .drawer-panel { background: #fff; border-left: 1px solid var(--line); box-shadow: -22px 0 60px rgba(15, 23, 42, 0.18); inset: 0 0 0 auto; max-width: min(980px, calc(100vw - 24px)); overflow: auto; padding: 28px; position: fixed; width: 76vw; z-index: 31; }
    .drawer-header { align-items: start; border-bottom: 1px solid var(--line); display: flex; gap: 18px; justify-content: space-between; padding-bottom: 18px; }
    .drawer-header h2 { margin: 0 0 6px; }
    .drawer-section { border-top: 1px solid var(--line); margin-top: 20px; padding-top: 20px; }
    .fieldset-reset { border: 0; margin: 0; padding: 0; }
    .table .row { grid-template-columns: minmax(220px, 1.5fr) minmax(120px, 0.7fr) 90px minmax(250px, 0.9fr); }
    @media (max-width: 820px) {
      .drawer-panel { max-width: 100vw; width: 100vw; }
      .drawer-header { flex-direction: column; }
      .table .row { grid-template-columns: 1fr; }
    }
  `]
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
  tab = signal<'editor' | 'caracteristicas' | 'listado'>('editor');
  modalAbierto = signal(false);
  modalModo = signal<'crear' | 'editar' | 'ver'>('crear');
  mensaje = signal('');
  busqueda = '';
  form = this.nuevaForma();
  seleccion = this.nuevaSeleccion();
  seleccionOriginal = this.nuevaSeleccion();
  imagenPrincipalId = 0;
  ordenImagenPrincipal = 1;
  categoriaPrincipalId = 0;

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

  abrirNuevaAtraccion() {
    this.limpiar();
    this.cerrarCaracteristicas();
    this.modalModo.set('crear');
    this.modalAbierto.set(true);
  }

  abrirEditorCompleto(item: any) {
    this.editar(item);
    this.abrirCaracteristicas(item);
    this.modalModo.set('editar');
    this.modalAbierto.set(true);
  }

  abrirVisualizacion(item: any) {
    this.editar(item);
    this.abrirCaracteristicas(item);
    this.modalModo.set('ver');
    this.modalAbierto.set(true);
  }

  cerrarModal() {
    this.modalAbierto.set(false);
    this.limpiar();
    this.cerrarCaracteristicas();
  }

  recargarCaracteristicas() {
    const item = this.atraccionCaracteristicas();
    if (item) this.abrirCaracteristicas(item);
  }

  soloLectura() {
    return this.modalModo() === 'ver';
  }

  tituloModal() {
    if (this.modalModo() === 'ver') return 'Detalle de atraccion';
    return this.editGuid() ? 'Editar atraccion' : 'Nueva atraccion';
  }

  guardar() {
    const request = { ...this.form, disponible: this.form.estado === 'A' };
    const creando = !this.editGuid();
    const action = this.editGuid()
      ? this.api.adminActualizarAtraccion(this.editGuid()!, request)
      : this.api.adminCrearAtraccion(request);

    action.subscribe({
      next: (response: any) => {
        this.mensaje.set('Atraccion guardada.');
        this.notifications.success('Atraccion guardada.');
        if (this.modalAbierto()) {
          const saved = response?.data ?? response;
          if (creando && saved) {
            const item = {
              ...saved,
              id: saved.id ?? saved.numeric_id ?? saved.numericId,
              guid: saved.guid ?? saved.at_guid ?? saved.atGuid,
              nombre: saved.nombre ?? this.form.nombre
            };
            if (item.guid) this.editGuid.set(item.guid);
            if (item.id) {
              this.atraccionCaracteristicas.set(item);
              this.seleccionOriginal = this.nuevaSeleccion();
              if (this.haySeleccionCaracteristicas()) {
                this.guardarCaracteristicas();
              }
            }
          }
        } else {
          this.limpiar();
        }
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
    this.categoriaPrincipalId = 0;
    this.api.obtenerCaracteristicasAtraccion(item.id).subscribe({
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
        this.imagenPrincipalId = (detail.imagenes || []).find((x: any) => x.es_principal || x.esPrincipal)?.id ?? this.seleccion.imagenes[0] ?? 0;
        this.ordenImagenPrincipal = (detail.imagenes || []).find((x: any) => x.id === this.imagenPrincipalId)?.orden ?? 1;
        this.categoriaPrincipalId = (detail.categorias || []).find((x: any) => x.es_principal || x.esPrincipal)?.id ?? this.seleccion.categorias[0] ?? 0;
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudieron cargar las caracteristicas.')
    });
  }

  cerrarCaracteristicas() {
    this.atraccionCaracteristicas.set(null);
    this.seleccion = this.nuevaSeleccion();
    this.seleccionOriginal = this.nuevaSeleccion();
    this.imagenPrincipalId = 0;
    this.categoriaPrincipalId = 0;
  }

  etiquetaCatalogo(item: any) {
    return item.nombre || item.descripcion || item.codigo || item.url || `Registro ${item.id}`;
  }

  nombreAtraccionCaracteristicas() {
    return this.atraccionCaracteristicas()?.nombre || this.form.nombre || 'la nueva atraccion';
  }

  toggleSeleccion(tipo: 'categorias' | 'idiomas' | 'imagenes' | 'incluye', id: number) {
    this.seleccion[tipo] = this.seleccion[tipo].includes(id)
      ? this.seleccion[tipo].filter((item) => item !== id)
      : [...this.seleccion[tipo], id];
    if (tipo === 'imagenes' && !this.seleccion.imagenes.includes(this.imagenPrincipalId)) {
      this.imagenPrincipalId = this.seleccion.imagenes[0] ?? 0;
    }
  }

  toggleCategoria(item: any) {
    const id = Number(item.id);
    const parentId = Number(item.parent_id ?? item.parentId ?? 0);
    const removing = this.seleccion.categorias.includes(id);

    if (removing) {
      this.seleccion.categorias = this.seleccion.categorias
        .filter((categoriaId) => categoriaId !== id)
        .filter((categoriaId) => Number(this.categorias().find((categoria) => categoria.id === categoriaId)?.parent_id ?? this.categorias().find((categoria) => categoria.id === categoriaId)?.parentId ?? 0) !== id);
      if (!this.seleccion.categorias.includes(this.categoriaPrincipalId)) {
        this.categoriaPrincipalId = this.seleccion.categorias[0] ?? 0;
      }
      return;
    }

    this.seleccion.categorias = [...this.seleccion.categorias, id];
    if (parentId && !this.seleccion.categorias.includes(parentId)) {
      this.seleccion.categorias = [parentId, ...this.seleccion.categorias];
    }
    this.categoriaPrincipalId = id;
  }

  guardarCaracteristicas() {
    const atraccionId = this.atraccionCaracteristicas()?.id ?? 0;
    if (!atraccionId) {
      this.notifications.error('Selecciona una atraccion.');
      return;
    }

    const nuevasCategorias = this.nuevosIds('categorias').map((id) => this.api.asociarCategoriaAtraccion(atraccionId, id, id === this.categoriaPrincipalId));
    const actualizarCategoriaPrincipal = this.categoriaPrincipalId && this.seleccion.categorias.includes(this.categoriaPrincipalId)
      ? [this.api.asociarCategoriaAtraccion(atraccionId, this.categoriaPrincipalId, true)]
      : [];
    const nuevosIdiomas = this.nuevosIds('idiomas').map((id) => this.api.asociarIdiomaAtraccion(atraccionId, id));
    const nuevosIncluye = this.nuevosIds('incluye').map((id) => this.api.asociarIncluyeAtraccion(atraccionId, id));
    const imagenesPorGuardar = this.seleccion.imagenes
      .filter((id) => !this.seleccionOriginal.imagenes.includes(id) || id === this.imagenPrincipalId)
      .map((id) => this.api.asociarImagenAtraccion(atraccionId, id, id === this.imagenPrincipalId, id === this.imagenPrincipalId ? this.ordenImagenPrincipal || 1 : 2));
    const removerCategorias = this.removidosIds('categorias').map((id) => this.api.desasociarCategoriaAtraccion(atraccionId, id));
    const removerIdiomas = this.removidosIds('idiomas').map((id) => this.api.desasociarIdiomaAtraccion(atraccionId, id));
    const removerIncluye = this.removidosIds('incluye').map((id) => this.api.desasociarIncluyeAtraccion(atraccionId, id));
    const removerImagenes = this.removidosIds('imagenes').map((id) => this.api.desasociarImagenAtraccion(atraccionId, id));
    const operaciones = [...nuevasCategorias, ...actualizarCategoriaPrincipal, ...nuevosIdiomas, ...nuevosIncluye, ...imagenesPorGuardar, ...removerCategorias, ...removerIdiomas, ...removerIncluye, ...removerImagenes];

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

  private haySeleccionCaracteristicas() {
    return this.seleccion.categorias.length > 0 ||
      this.seleccion.idiomas.length > 0 ||
      this.seleccion.imagenes.length > 0 ||
      this.seleccion.incluye.length > 0;
  }

  etiquetaCategoria(item: any) {
    const parentId = item.parent_id ?? item.parentId;
    if (!parentId) return `${this.etiquetaCatalogo(item)} (tipo)`;
    const parent = this.categorias().find((categoria) => categoria.id === parentId);
    return `${parent?.nombre ?? 'Tipo'} / ${this.etiquetaCatalogo(item)} (subtipo)`;
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
