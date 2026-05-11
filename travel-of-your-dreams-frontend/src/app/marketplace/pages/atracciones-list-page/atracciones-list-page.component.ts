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
              <img [src]="imagen(item.guid)" [alt]="item.nombre" loading="lazy">
            </a>
            <div class="card-body">
              <p class="eyebrow">{{ item.pais || 'Destino' }}</p>
              <h2>{{ item.nombre }}</h2>
              <p>{{ item.descripcion || 'Experiencia turistica disponible para reservar.' }}</p>
              <div class="meta">
                <span>Desde {{ item.precio_referencia || 0 }} USD</span>
                <span>{{ item.total_resenias }} resenias</span>
              </div>
              <a class="btn" [routerLink]="['/atracciones', item.guid]">Ver detalle</a>
            </div>
          </article>
        } @empty {
          <div class="panel">No hay atracciones disponibles.</div>
        }
      </div>
    </section>
  `,
  styles: [`
    .hero {
      background: #113c38;
      border-radius: 0;
      color: white;
      padding: 48px 32px;
    }
    .hero h1 { font-size: clamp(32px, 5vw, 56px); margin: 0 0 12px; max-width: 820px; }
    .hero p { font-size: 18px; margin: 0; max-width: 680px; }
    .card { background: white; border: 1px solid var(--line); border-radius: 8px; overflow: hidden; }
    .media { aspect-ratio: 4 / 3; background: #d8dee6; display: block; }
    .media img { display: block; height: 100%; object-fit: cover; width: 100%; }
    .card-body { align-content: start; display: grid; gap: 12px; padding: 16px; }
    .card h2 { font-size: 20px; margin: 0; }
    .eyebrow { color: var(--accent); font-size: 12px; font-weight: 800; margin: 0; }
    .meta { color: var(--muted); display: flex; justify-content: space-between; gap: 12px; }
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
    this.api.listarAtracciones({ page: 1, limit: 12, disponible: true, ...this.filtros }).subscribe((response) => {
      this.atracciones.set(response.data);
      response.data.forEach((item) => {
        this.api.obtenerAtraccion(item.guid).subscribe((detail) => {
          const url = detail.data.imagenes?.[0]?.url;
          if (url) this.imagenes.update((current) => ({ ...current, [item.guid]: url }));
        });
      });
    });
  }

  imagen(guid: string) {
    return this.imagenes()[guid] ?? 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80';
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
}
