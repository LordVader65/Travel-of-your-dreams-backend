import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';

type CatalogoNombre = 'destinos' | 'categorias' | 'idiomas' | 'imagenes' | 'incluye';
type ModalModo = 'crear' | 'editar' | 'ver';

@Component({
  selector: 'app-admin-catalogos-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div class="page-title">
          <h1>Catalogos</h1>
          <p class="muted">Destinos, categorias, idiomas, imagenes e inclusiones usados por las atracciones.</p>
        </div>
        <span class="actions">
          <button class="btn secondary" type="button" (click)="cargar()">Actualizar</button>
          <button class="btn" type="button" (click)="abrirCrear()">Crear {{ singularCatalogo() }}</button>
        </span>
      </header>

      <nav class="app-tabs" aria-label="Catalogos">
        <button class="tab-button" [class.active]="catalogoActivo === 'destinos'" type="button" (click)="cambiarCatalogo('destinos')">Destinos</button>
        <button class="tab-button" [class.active]="catalogoActivo === 'categorias'" type="button" (click)="cambiarCatalogo('categorias')">Categorias</button>
        <button class="tab-button" [class.active]="catalogoActivo === 'idiomas'" type="button" (click)="cambiarCatalogo('idiomas')">Idiomas</button>
        <button class="tab-button" [class.active]="catalogoActivo === 'imagenes'" type="button" (click)="cambiarCatalogo('imagenes')">Imagenes</button>
        <button class="tab-button" [class.active]="catalogoActivo === 'incluye'" type="button" (click)="cambiarCatalogo('incluye')">Incluye / No incluye</button>
      </nav>

      <div class="panel stack">
        <div class="section-toolbar">
          <div>
            <h2>Registros de {{ catalogoActivo }}</h2>
            <p class="muted">Gestiona cada registro desde sus acciones.</p>
          </div>
          <input class="toolbar-search" name="buscarCatalogo" placeholder="Buscar por nombre, codigo o estado" [(ngModel)]="busqueda" />
        </div>

        <div class="catalog-records">
          @for (item of itemsFiltrados(); track item.id || item.guid || $index) {
            <article class="record-card catalog-card">
              <span class="record-main">
                <strong>{{ tituloRegistro(item) }}</strong>
                <small>{{ subtituloRegistro(item) }}</small>
              </span>
              <span class="actions">
                <button class="btn secondary" type="button" (click)="abrirModal('ver', item)">Ver</button>
                <button class="btn secondary" type="button" (click)="abrirModal('editar', item)">Editar</button>
                <button class="btn danger" type="button" (click)="eliminar(item.id)">Eliminar</button>
              </span>
            </article>
          } @empty {
            <p class="muted">No hay registros para mostrar.</p>
          }
        </div>
      </div>

      @if (modalAbierto()) {
        <div class="modal-backdrop" (click)="cerrarModal()"></div>
        <aside class="drawer-panel stack" role="dialog" aria-modal="true" aria-label="Detalle de catalogo">
          <header class="drawer-header">
            <div>
              <h2>{{ tituloModal() }}</h2>
              <p class="muted">{{ descripcionModal() }}</p>
            </div>
            <button class="btn secondary" type="button" (click)="cerrarModal()">Cerrar</button>
          </header>

          @if (modalModo() === 'ver') {
            <div class="view-summary">
              @for (campo of camposVista(); track campo.label) {
                <div class="view-field" [class.wide]="campo.wide">
                  <span>{{ campo.label }}</span>
                  <strong>{{ campo.value }}</strong>
                </div>
              }
            </div>
          } @else {
            <form class="form-grid" (ngSubmit)="guardarModal()">
              @if (catalogoActivo === 'destinos') {
                <label>Nombre <input name="modalDestinoNombre" maxlength="150" [(ngModel)]="modalForm.nombre" required /></label>
                <label>Pais <input name="modalPais" maxlength="100" [(ngModel)]="modalForm.pais" required /></label>
                <label class="wide">Imagen URL <input name="modalImagenUrlDestino" maxlength="500" [(ngModel)]="modalForm.imagenUrl" /></label>
              }

              @if (catalogoActivo === 'categorias') {
                <label>Nombre <input name="modalCategoriaNombre" maxlength="100" [(ngModel)]="modalForm.nombre" required /></label>
                <label>Tag <input name="modalTagName" maxlength="80" [(ngModel)]="modalForm.tagName" /></label>
                <label>Categoria padre
                  <select name="modalParentId" [(ngModel)]="modalForm.parentId">
                    <option [ngValue]="null">Sin padre</option>
                    @for (categoria of categoriasPadreDisponibles(); track categoria.id) {
                      <option [ngValue]="categoria.id">{{ categoria.nombre }}</option>
                    }
                  </select>
                </label>
              }

              @if (catalogoActivo === 'idiomas') {
                <label>Codigo <input name="modalCodigo" maxlength="10" [(ngModel)]="modalForm.codigo" required /></label>
                <label>Descripcion <input name="modalDescripcionIdioma" maxlength="80" [(ngModel)]="modalForm.descripcion" required /></label>
              }

              @if (catalogoActivo === 'imagenes') {
                <label class="wide">URL <input name="modalImagenUrl" maxlength="500" [(ngModel)]="modalForm.imagenUrl" required /></label>
                <label class="wide">Descripcion <input name="modalDescripcionImagen" maxlength="200" [(ngModel)]="modalForm.descripcion" /></label>
              }

              @if (catalogoActivo === 'incluye') {
                <label>Descripcion <input name="modalDescripcionIncluye" maxlength="200" [(ngModel)]="modalForm.descripcion" required /></label>
                <label>Tipo
                  <select name="modalTipo" [(ngModel)]="modalForm.tipo">
                    <option value="INCLUYE">Incluye</option>
                    <option value="NO_INCLUYE">No incluye</option>
                  </select>
                </label>
              }

              <label>Estado
                <select name="modalEstado" [(ngModel)]="modalForm.estado">
                  <option value="A">Activo</option>
                  <option value="I">Inactivo</option>
                </select>
              </label>

              <div class="actions wide">
                <button class="btn" type="submit">{{ modalModo() === 'crear' ? 'Crear' : 'Guardar cambios' }}</button>
                <button class="btn secondary" type="button" (click)="cerrarModal()">Cancelar</button>
              </div>
            </form>
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
    .toolbar-search { max-width: 420px; }
    .catalog-records { display: grid; gap: 12px; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); }
    .catalog-card { align-items: start; flex-direction: column; min-height: 150px; }
    .record-main { display: grid; gap: 6px; width: 100%; }
    .record-main strong { overflow-wrap: anywhere; }
    .record-main small { color: var(--muted); display: block; line-height: 1.45; }
    .modal-backdrop { background: rgba(2, 24, 22, 0.42); inset: 0; position: fixed; z-index: 30; }
    .drawer-panel { background: #fff; border-left: 1px solid var(--line); box-shadow: -22px 0 60px rgba(15, 23, 42, 0.18); inset: 0 0 0 auto; max-width: min(760px, calc(100vw - 24px)); overflow: auto; padding: 28px; position: fixed; width: 58vw; z-index: 31; }
    .drawer-header { align-items: start; border-bottom: 1px solid var(--line); display: flex; gap: 18px; justify-content: space-between; padding-bottom: 18px; }
    .view-summary { display: grid; gap: 14px; grid-template-columns: repeat(2, minmax(0, 1fr)); }
    .view-field { background: var(--surface-muted); border: 1px solid var(--line); border-radius: 8px; display: grid; gap: 8px; min-height: 92px; padding: 14px; }
    .view-field span { color: var(--muted); font-weight: 700; }
    .view-field strong { overflow-wrap: anywhere; }
    .view-field.wide { grid-column: 1 / -1; }
    @media (max-width: 820px) {
      .drawer-panel { max-width: 100vw; width: 100vw; }
      .drawer-header, .section-toolbar { align-items: stretch; flex-direction: column; }
      .toolbar-search { max-width: none; }
      .view-summary { grid-template-columns: 1fr; }
    }
  `]
})
export class AdminCatalogosPageComponent implements OnInit {
  catalogoActivo: CatalogoNombre = 'destinos';
  items = signal<any[]>([]);
  categoriasPadre = signal<any[]>([]);
  editId = signal<number | null>(null);
  modalAbierto = signal(false);
  modalModo = signal<ModalModo>('crear');
  mensaje = signal('');
  busqueda = '';
  modalForm: any = {};

  constructor(private readonly api: ApiService, private readonly notifications: NotificationService) {}

  ngOnInit() {
    this.cargar();
  }

  cambiarCatalogo(catalogo: CatalogoNombre) {
    this.catalogoActivo = catalogo;
    this.busqueda = '';
    this.cerrarModal();
    this.cargar();
  }

  cargar() {
    this.api.catalogo(this.catalogoActivo).subscribe((response) => this.items.set(response.data));
    this.api.catalogo('categorias').subscribe((response) => this.categoriasPadre.set(response.data));
  }

  abrirCrear() {
    this.modalModo.set('crear');
    this.editId.set(null);
    this.modalForm = this.nuevoFormulario();
    this.modalAbierto.set(true);
  }

  abrirModal(modo: ModalModo, item: any) {
    this.modalModo.set(modo);
    this.editId.set(item.id);
    this.modalForm = this.desdeItem(item);
    this.modalAbierto.set(true);
  }

  cerrarModal() {
    this.modalAbierto.set(false);
    this.editId.set(null);
    this.modalForm = {};
  }

  guardarModal() {
    const request = this.normalizarRequest(this.modalForm);
    const action = this.modalModo() === 'crear'
      ? this.api.crearCatalogo(this.catalogoActivo, request)
      : this.api.actualizarCatalogo(this.catalogoActivo, this.editId()!, request);

    action.subscribe({
      next: () => {
        const message = this.modalModo() === 'crear' ? 'Catalogo creado.' : 'Catalogo actualizado.';
        this.mensaje.set(message);
        this.notifications.success(message);
        this.cerrarModal();
        this.cargar();
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo guardar el catalogo.')
    });
  }

  eliminar(id: number) {
    if (!id || !confirm('Eliminar este registro?')) return;
    this.api.eliminarCatalogo(this.catalogoActivo, id).subscribe({
      next: () => {
        this.mensaje.set('Registro eliminado.');
        this.notifications.success('Registro eliminado.');
        this.cargar();
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo eliminar el registro.')
    });
  }

  itemsFiltrados() {
    const q = this.busqueda.trim().toLowerCase();
    return this.items().filter((item) => !q || `${this.tituloRegistro(item)} ${this.subtituloRegistro(item)}`.toLowerCase().includes(q));
  }

  tituloRegistro(item: any) {
    return item.nombre || item.descripcion || item.codigo || item.imagenUrl || item.imagen_url || item.url || 'Registro';
  }

  subtituloRegistro(item: any) {
    if (this.catalogoActivo === 'destinos') {
      return [item.pais, this.estadoTexto(item.estado)].filter(Boolean).join(' · ');
    }
    if (this.catalogoActivo === 'categorias') {
      const parentName = this.nombreCategoria(item.parent_id ?? item.parentId);
      const tag = item.codigo ?? item.tag_name ?? item.tagName;
      return [tag ? `tag: ${tag}` : 'sin tag', parentName ? `padre: ${parentName}` : 'categoria raiz', this.estadoTexto(item.estado)].join(' · ');
    }
    if (this.catalogoActivo === 'idiomas') {
      return [item.codigo, this.estadoTexto(item.estado)].filter(Boolean).join(' · ');
    }
    if (this.catalogoActivo === 'imagenes') {
      return [item.descripcion, this.estadoTexto(item.estado)].filter(Boolean).join(' · ');
    }
    return [item.tipo, this.estadoTexto(item.estado)].filter(Boolean).join(' · ');
  }

  camposVista() {
    const item = this.modalForm;
    const common = [{ label: 'Estado', value: this.estadoTexto(item.estado), wide: false }];
    if (this.catalogoActivo === 'destinos') {
      return [
        { label: 'Nombre', value: this.valor(item.nombre), wide: false },
        { label: 'Pais', value: this.valor(item.pais), wide: false },
        { label: 'Imagen URL', value: this.valor(item.imagenUrl), wide: true },
        ...common
      ];
    }
    if (this.catalogoActivo === 'categorias') {
      return [
        { label: 'Nombre', value: this.valor(item.nombre), wide: false },
        { label: 'Tag', value: this.valor(item.tagName || item.codigo), wide: false },
        { label: 'Categoria padre', value: this.valor(this.nombreCategoria(item.parentId)), wide: false },
        ...common
      ];
    }
    if (this.catalogoActivo === 'idiomas') {
      return [
        { label: 'Codigo', value: this.valor(item.codigo), wide: false },
        { label: 'Descripcion', value: this.valor(item.descripcion), wide: false },
        ...common
      ];
    }
    if (this.catalogoActivo === 'imagenes') {
      return [
        { label: 'URL', value: this.valor(item.imagenUrl), wide: true },
        { label: 'Descripcion', value: this.valor(item.descripcion), wide: true },
        ...common
      ];
    }
    return [
      { label: 'Descripcion', value: this.valor(item.descripcion), wide: false },
      { label: 'Tipo', value: this.valor(item.tipo), wide: false },
      ...common
    ];
  }

  categoriasPadreDisponibles() {
    const currentId = this.editId();
    return this.categoriasPadre().filter((categoria) => categoria.id !== currentId);
  }

  singularCatalogo() {
    return this.catalogoActivo === 'destinos' ? 'destino'
      : this.catalogoActivo === 'categorias' ? 'categoria'
      : this.catalogoActivo === 'idiomas' ? 'idioma'
      : this.catalogoActivo === 'imagenes' ? 'imagen'
      : 'incluye/no incluye';
  }

  tituloModal() {
    const action = this.modalModo() === 'crear' ? 'Crear' : this.modalModo() === 'ver' ? 'Ver' : 'Editar';
    return `${action} ${this.singularCatalogo()}`;
  }

  descripcionModal() {
    return this.modalModo() === 'ver'
      ? 'Consulta visual del registro seleccionado.'
      : 'Completa la informacion y guarda los cambios.';
  }

  private desdeItem(item: any) {
    return {
      ...item,
      parentId: item.parent_id ?? item.parentId ?? null,
      tagName: item.codigo ?? item.tag_name ?? item.tagName ?? '',
      codigo: item.codigo ?? item.tag_name ?? item.tagName ?? '',
      descripcion: item.descripcion ?? (this.catalogoActivo === 'incluye' || this.catalogoActivo === 'idiomas' ? item.nombre : '') ?? '',
      imagenUrl: item.imagen_url ?? item.imagenUrl ?? item.url ?? '',
      tipo: item.tipo ?? 'INCLUYE',
      estado: item.estado ?? 'A'
    };
  }

  private nuevoFormulario() {
    return this.catalogoActivo === 'destinos'
      ? { nombre: '', pais: '', imagenUrl: '', estado: 'A' }
      : this.catalogoActivo === 'categorias'
        ? { nombre: '', tagName: '', parentId: null, estado: 'A' }
        : this.catalogoActivo === 'idiomas'
          ? { codigo: '', descripcion: '', estado: 'A' }
          : this.catalogoActivo === 'imagenes'
            ? { imagenUrl: '', descripcion: '', estado: 'A' }
            : { descripcion: '', tipo: 'INCLUYE', estado: 'A' };
  }

  private normalizarRequest(source: any) {
    const request = { ...source };
    if (this.catalogoActivo === 'imagenes') {
      request.imagenUrl = request.imagenUrl || request.url || '';
      request.nombre = request.descripcion || request.imagenUrl;
    }
    if (this.catalogoActivo === 'categorias') {
      request.codigo = request.codigo || request.tagName || request.tag_name || '';
    }
    if (this.catalogoActivo === 'idiomas' || this.catalogoActivo === 'incluye') {
      request.nombre = request.descripcion || request.nombre || '';
    }
    return request;
  }

  private nombreCategoria(id: number | null | undefined) {
    if (!id) return '';
    return this.categoriasPadre().find((categoria) => categoria.id === id)?.nombre ?? `#${id}`;
  }

  private estadoTexto(value: string | undefined) {
    return value === 'I' ? 'Inactivo' : 'Activo';
  }

  private valor(value: any) {
    return value === null || value === undefined || value === '' ? '-' : value;
  }
}
