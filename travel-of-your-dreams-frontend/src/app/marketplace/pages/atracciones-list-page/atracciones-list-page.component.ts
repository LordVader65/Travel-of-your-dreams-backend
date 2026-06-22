import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../core/api/api.service';
import { AtraccionPublica } from '../../../shared/models/atraccion.model';

@Component({
  selector: 'app-atracciones-list-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <section class="page stack">
      <header class="hero">
        <div>
          <h1>Encuentra tu siguiente experiencia</h1>
          <p>Explora atracciones disponibles, compara precios y reserva horarios con cupos reales.</p>
        </div>
      </header>

      <form class="panel filters" (ngSubmit)="buscar()">
        <label>Destino <input name="destino" [(ngModel)]="filtros.destino" placeholder="Ej. Ecuador" /></label>
        <label>Desde <input type="date" name="fechaDesde" [min]="fechaMinima" [(ngModel)]="filtros.fecha_desde" /></label>
        <label>Hasta <input type="date" name="fechaHasta" [min]="filtros.fecha_desde || fechaMinima" [(ngModel)]="filtros.fecha_hasta" /></label>
        <button class="btn" type="submit">Filtrar</button>
        <button class="btn secondary" type="button" (click)="limpiarFiltros()">Limpiar</button>
      </form>

      <div class="grid three">
        @for (item of atracciones(); track item.guid) {
          <article class="card">
            <a [routerLink]="['/atracciones', item.guid]" class="media">
              <img [src]="imagen(item.guid)" [alt]="item.nombre" loading="lazy" (error)="imagenError(item.guid)">
            </a>
            <div class="card-body">
              <div class="card-head">
                <p class="eyebrow">{{ item.pais || 'Destino' }}</p>
                <span class="status-badge ok">{{ item.disponible ? 'Disponible' : 'No disponible' }}</span>
              </div>
              <h2>{{ item.nombre }}</h2>
              <p>{{ item.descripcion || 'Experiencia turistica disponible para reservar.' }}</p>
              <div class="card-badges">
                @if (item.free_cancellation) { <span class="data-chip">Cancelacion gratis</span> }
                @if (item.skip_the_line) { <span class="data-chip">Sin fila</span> }
                @if (item.duracion_minutos) { <span class="data-chip">{{ item.duracion_minutos }} min</span> }
              </div>
              <div class="meta">
                <strong>Desde {{ item.precio_referencia || 0 }} USD</strong>
                <span>{{ item.total_resenias }} resenias</span>
              </div>
              <a class="btn" [routerLink]="['/atracciones', item.guid]">Ver detalle</a>
            </div>
          </article>
        } @empty {
          <div class="empty-state">No hay atracciones disponibles con los filtros seleccionados.</div>
        }
      </div>
    </section>
  `,
  styles: [`
    .hero {
      background: linear-gradient(135deg, #0b3b36 0%, #115e59 100%);
      border-radius: 8px;
      box-shadow: var(--shadow-md);
      color: white;
      min-height: 320px;
      padding: 56px 40px;
      display: grid;
      align-items: center;
    }
    .hero h1 { font-size: clamp(32px, 5vw, 56px); margin: 0 0 12px; max-width: 820px; }
    .hero p { font-size: 18px; margin: 0; max-width: 680px; }
    .card { background: white; border: 1px solid var(--line); border-radius: 8px; overflow: hidden; transition: transform .18s ease, box-shadow .18s ease, border-color .18s ease; }
    .card:hover { border-color: rgba(15, 118, 110, .45); box-shadow: var(--shadow-md); transform: translateY(-2px); }
    .media { aspect-ratio: 4 / 3; background: #d8dee6; display: block; }
    .media img { display: block; height: 100%; object-fit: cover; width: 100%; }
    .card-body { align-content: start; display: grid; gap: 12px; padding: 16px; }
    .card h2 { font-size: 20px; margin: 0; }
    .card-head { align-items: center; display: flex; gap: 10px; justify-content: space-between; }
    .card-badges { display: flex; flex-wrap: wrap; gap: 8px; min-height: 28px; }
    .eyebrow { color: var(--accent); font-size: 12px; font-weight: 800; margin: 0; }
    .meta { color: var(--muted); display: flex; justify-content: space-between; gap: 12px; }
    .meta strong { color: var(--text); }
    .filters { align-items: end; display: grid; gap: 12px; grid-template-columns: repeat(5, minmax(0, 1fr)); }
    @media (max-width: 980px) { .filters { grid-template-columns: repeat(2, minmax(0, 1fr)); } }
    @media (max-width: 560px) { .filters { grid-template-columns: 1fr; } }
  `]
})
export class AtraccionesListPageComponent implements OnInit {
  atracciones = signal<AtraccionPublica[]>([]);
  imagenes = signal<Record<string, string>>({});
  fechaMinima = this.today();
  filtros = { destino: '', fecha_desde: '', fecha_hasta: '' };

  constructor(private readonly api: ApiService) {}

  ngOnInit() {
    this.api.filtrosAtracciones().subscribe();
    this.buscar();
  }

  buscar() {
    if (this.filtros.fecha_desde && this.filtros.fecha_desde < this.fechaMinima) this.filtros.fecha_desde = this.fechaMinima;
    if (this.filtros.fecha_hasta && this.filtros.fecha_desde && this.filtros.fecha_hasta < this.filtros.fecha_desde) this.filtros.fecha_hasta = this.filtros.fecha_desde;
    this.api.listarAtraccionesPublicas({ page: 1, limit: 12, disponible: true, ciudad: this.filtros.destino }).subscribe((response) => {
      const rawItems = this.itemsFromResponse(response.data);
      const items = rawItems.map((item) => this.toAtraccion(item));
      this.atracciones.set(items);
      rawItems.forEach((item) => {
        const guid = item?.guid ?? item?.id;
        const url = this.urlImagen(item?.imagen_principal ?? item?.imagenPrincipal);
        if (guid && url) this.imagenes.update((current) => ({ ...current, [guid]: url }));
      });
      items.forEach((item) => {
        this.api.obtenerAtraccionPublica(item.guid).subscribe((detail) => {
          const data = detail.data as any;
          const url = this.urlImagen(data.imagen_principal ?? data.imagenPrincipal ?? data.imagenes?.[0]?.url ?? data.imagenes?.[0]);
          if (url) this.imagenes.update((current) => ({ ...current, [item.guid]: url }));
        });
      });
    });
  }

  imagen(guid: string) {
    return this.imagenes()[guid] ?? 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80';
  }

  imagenError(guid: string) {
    this.imagenes.update((current) => {
      const next = { ...current };
      delete next[guid];
      return next;
    });
  }

  limpiarFiltros() {
    this.filtros = { destino: '', fecha_desde: '', fecha_hasta: '' };
    this.buscar();
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

  private urlImagen(value: unknown): string | null {
    if (typeof value !== 'string') return null;
    const url = value.trim();
    if (!url || url.includes('example.com')) return null;
    return /^https?:\/\//i.test(url) ? url : null;
  }

  private toAtraccion(value: any): AtraccionPublica {
    const guid = value?.guid ?? value?.id ?? '';
    return {
      id: value?.numeric_id ?? value?.numericId ?? 0,
      guid,
      nombre: value?.nombre ?? '',
      descripcion: value?.descripcion_corta ?? value?.descripcionCorta ?? value?.descripcion ?? null,
      pais: value?.pais ?? value?.ciudad ?? null,
      direccion: value?.direccion ?? null,
      duracion_minutos: value?.duracion_minutos ?? value?.duracionMinutos ?? null,
      precio_referencia: Number(value?.precio_referencia ?? value?.precioReferencia ?? value?.precio_desde ?? value?.precioDesde ?? 0),
      disponible: Boolean(value?.disponible ?? value?.disponibilidad?.disponible ?? true),
      free_cancellation: Boolean(value?.free_cancellation ?? value?.freeCancellation ?? value?.etiquetas?.includes?.('free_cancellation') ?? false),
      skip_the_line: Boolean(value?.skip_the_line ?? value?.skipTheLine ?? value?.etiquetas?.includes?.('skip_the_line') ?? false),
      total_resenias: Number(value?.total_resenias ?? value?.totalResenias ?? value?.total_resenas ?? value?.totalResenas ?? 0)
    };
  }
}
