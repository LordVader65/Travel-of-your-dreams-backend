import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';

type CatalogoNombre = 'destinos' | 'categorias' | 'idiomas' | 'imagenes' | 'incluye';

@Component({
  selector: 'app-admin-catalogos-page',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="page stack">
      <header class="admin-header">
        <div>
          <h1>Catalogos</h1>
          <p class="muted">Destinos, categorias, idiomas, imagenes e inclusiones usados por las atracciones.</p>
        </div>
        <select [(ngModel)]="catalogoActivo" name="catalogoActivo" (change)="cargar()">
          <option value="destinos">Destinos</option>
          <option value="categorias">Categorias</option>
          <option value="idiomas">Idiomas</option>
          <option value="imagenes">Imagenes</option>
          <option value="incluye">Incluye</option>
        </select>
      </header>

      <div class="grid two">
        <form class="panel form-grid" (ngSubmit)="guardar()">
          <h2>{{ editId() ? 'Editar' : 'Crear' }} {{ catalogoActivo }}</h2>

          @if (catalogoActivo === 'destinos') {
            <label>Nombre <input name="destinoNombre" maxlength="150" [(ngModel)]="form.nombre" required /></label>
            <label>Pais <input name="pais" maxlength="100" [(ngModel)]="form.pais" required /></label>
            <label class="wide">Imagen URL <input name="imagenUrl" maxlength="500" [(ngModel)]="form.imagenUrl" /></label>
          }

          @if (catalogoActivo === 'categorias') {
            <label>Nombre <input name="categoriaNombre" maxlength="100" [(ngModel)]="form.nombre" required /></label>
            <label>Tag <input name="tagName" maxlength="80" [(ngModel)]="form.tagName" /></label>
            <label>Categoria padre
              <select name="parentId" [(ngModel)]="form.parentId">
                <option [ngValue]="null">Sin padre</option>
                @for (categoria of categoriasPadre(); track categoria.id) {
                  <option [ngValue]="categoria.id">{{ categoria.nombre }}</option>
                }
              </select>
            </label>
          }

          @if (catalogoActivo === 'idiomas') {
            <label>Codigo <input name="codigo" maxlength="10" [(ngModel)]="form.codigo" required /></label>
            <label>Descripcion <input name="descripcionIdioma" maxlength="80" [(ngModel)]="form.descripcion" required /></label>
          }

          @if (catalogoActivo === 'imagenes') {
            <label class="wide">URL <input name="url" maxlength="500" [(ngModel)]="form.url" required /></label>
            <label class="wide">Descripcion <input name="descripcionImagen" maxlength="200" [(ngModel)]="form.descripcion" /></label>
          }

          @if (catalogoActivo === 'incluye') {
            <label>Descripcion <input name="descripcionIncluye" maxlength="200" [(ngModel)]="form.descripcion" required /></label>
            <label>Tipo
              <select name="tipo" [(ngModel)]="form.tipo">
                <option value="INCLUYE">Incluye</option>
                <option value="NO_INCLUYE">No incluye</option>
              </select>
            </label>
          }

          @if (editId()) {
            <label>Estado
              <select name="estado" [(ngModel)]="form.estado">
                <option value="A">Activo</option>
                <option value="I">Inactivo</option>
              </select>
            </label>
          }

          <div class="actions wide">
            <button class="btn" type="submit">Guardar</button>
            <button class="btn secondary" type="button" (click)="limpiar()">Limpiar</button>
          </div>
        </form>

        <div class="panel stack">
          <h2>Registros</h2>
          @for (item of items(); track item.id || item.guid || $index) {
            <div class="compact-row">
              <span>
                <strong>{{ item.nombre || item.descripcion || item.codigo || item.url }}</strong>
                <small>{{ item.pais || item.tag_name || item.estado || item.tipo }}</small>
              </span>
              <span class="actions">
                <button class="btn secondary" type="button" (click)="editar(item)">Editar</button>
                <button class="btn danger" type="button" (click)="eliminar(item.id)">Eliminar</button>
              </span>
            </div>
          } @empty {
            <p class="muted">No hay registros.</p>
          }
        </div>
      </div>

      @if (mensaje()) {
        <div class="panel">{{ mensaje() }}</div>
      }
    </section>
  `
})
export class AdminCatalogosPageComponent implements OnInit {
  catalogoActivo: CatalogoNombre = 'destinos';
  items = signal<any[]>([]);
  categoriasPadre = signal<any[]>([]);
  editId = signal<number | null>(null);
  mensaje = signal('');
  form: any = {};

  constructor(private readonly api: ApiService, private readonly notifications: NotificationService) {}

  ngOnInit() {
    this.limpiar();
    this.cargar();
  }

  cargar() {
    this.api.catalogo(this.catalogoActivo).subscribe((response) => this.items.set(response.data));
    if (this.catalogoActivo !== 'categorias') {
      this.api.catalogo('categorias').subscribe((response) => this.categoriasPadre.set(response.data));
    } else {
      this.api.catalogo('categorias').subscribe((response) => {
        this.items.set(response.data);
        this.categoriasPadre.set(response.data);
      });
    }
  }

  guardar() {
    const request = { ...this.form };
    const action = this.editId()
      ? this.api.actualizarCatalogo(this.catalogoActivo, this.editId()!, request)
      : this.api.crearCatalogo(this.catalogoActivo, request);

    action.subscribe({
      next: () => {
        this.mensaje.set('Catalogo guardado.');
        this.notifications.success('Catalogo guardado.');
        this.limpiar();
        this.cargar();
      },
      error: (error) => this.notifications.error(error?.error?.message ?? 'No se pudo guardar el catalogo.')
    });
  }

  editar(item: any) {
    this.editId.set(item.id);
    this.form = {
      ...item,
      parentId: item.parent_id ?? item.parentId ?? null,
      tagName: item.tag_name ?? item.tagName ?? '',
      imagenUrl: item.imagen_url ?? item.imagenUrl ?? '',
      estado: item.estado ?? 'A'
    };
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

  limpiar() {
    this.editId.set(null);
    this.form = this.catalogoActivo === 'destinos'
      ? { nombre: '', pais: '', imagenUrl: '' }
      : this.catalogoActivo === 'categorias'
        ? { nombre: '', tagName: '', parentId: null }
        : this.catalogoActivo === 'idiomas'
          ? { codigo: '', descripcion: '' }
          : this.catalogoActivo === 'imagenes'
            ? { url: '', descripcion: '' }
            : { descripcion: '', tipo: 'INCLUYE' };
  }
}
