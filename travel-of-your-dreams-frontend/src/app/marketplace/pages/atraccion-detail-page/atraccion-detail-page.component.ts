import { Component, Input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../core/api/api.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { AtraccionDetalle, HorarioDisponible } from '../../../shared/models/atraccion.model';
import { Reserva } from '../../../shared/models/reserva.model';

@Component({
  selector: 'app-atraccion-detail-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    @if (atraccion(); as item) {
      <section class="page detail-page stack">
        <header class="detail-heading">
          <div>
            <p class="eyebrow">{{ item.pais || 'Destino' }}</p>
            <h1>{{ item.nombre }}</h1>
            <p class="muted">{{ item.total_resenias }} resenias · {{ item.direccion || 'Direccion por confirmar' }}</p>
          </div>
        </header>

        <div class="gallery">
          <img class="gallery-main" [src]="imagenPrincipal(item)" [alt]="item.nombre">
          @for (imagen of galeria(item).slice(1, 5); track imagen.url) {
            <img [src]="imagen.url" [alt]="imagen.descripcion || item.nombre">
          }
        </div>

        <div class="detail-layout">
          <main class="stack">
            <nav class="tabs-line">
              <a href="#overview">Resumen</a>
              <a href="#caracteristicas">Caracteristicas</a>
              <a href="#tickets">Tickets</a>
              <a href="#horarios">Horarios</a>
              <a href="#resenias">Resenias</a>
            </nav>

            <section id="overview" class="panel stack">
              <h2>{{ item.nombre }}</h2>
              <p>{{ item.descripcion || 'Experiencia turistica disponible para reservar.' }}</p>
              <div class="feature-list">
                <span>{{ item.free_cancellation ? 'Cancelacion gratis' : 'Politica de cancelacion segun reserva' }}</span>
                <span>{{ item.skip_the_line ? 'Sin fila' : 'Ingreso regular' }}</span>
                <span>{{ item.duracion_minutos || '-' }} minutos</span>
              </div>
            </section>

            <section id="caracteristicas" class="panel stack">
              <h2>Caracteristicas</h2>
              <div class="feature-groups">
                <div>
                  <h3>Categorias</h3>
                  <div class="chip-list">
                    @for (categoria of activas(item.categorias); track categoria.id || categoria.nombre) {
                      <span class="chip">{{ categoria.nombre || categoria.tag_name }}</span>
                    } @empty {
                      <span class="muted">Sin categorias asociadas.</span>
                    }
                  </div>
                </div>
                <div>
                  <h3>Idiomas</h3>
                  <div class="chip-list">
                    @for (idioma of activas(item.idiomas); track idioma.id || idioma.codigo) {
                      <span class="chip">{{ idioma.descripcion || idioma.codigo }}</span>
                    } @empty {
                      <span class="muted">Sin idiomas asociados.</span>
                    }
                  </div>
                </div>
                <div>
                  <h3>Incluye</h3>
                  <div class="amenity-list">
                    @for (incluido of incluidos(item); track incluido.id || incluido.descripcion) {
                      <span>{{ incluido.descripcion }}</span>
                    } @empty {
                      <span class="muted">Sin inclusiones asociadas.</span>
                    }
                  </div>
                </div>
                <div>
                  <h3>No incluye</h3>
                  <div class="amenity-list">
                    @for (noIncluido of noIncluidos(item); track noIncluido.id || noIncluido.descripcion) {
                      <span>{{ noIncluido.descripcion }}</span>
                    } @empty {
                      <span class="muted">Sin exclusiones asociadas.</span>
                    }
                  </div>
                </div>
              </div>
            </section>

            <section id="tickets" class="panel stack">
              <h2>Tickets</h2>
              @for (ticket of item.tickets; track ticket.guid) {
                <div class="row">
                  <span>{{ ticket.tipo_participante }}</span>
                  <strong>{{ ticket.precio }} {{ ticket.moneda }}</strong>
                </div>
              }
            </section>

            <section id="horarios" class="panel stack">
              <h2>Horarios disponibles</h2>
              @for (horario of horariosUnicos(); track horario.guid) {
                <div class="row">
                  <span>{{ horario.hora_inicio }}{{ horario.hora_fin ? ' - ' + horario.hora_fin : '' }}</span>
                  <strong>{{ horario.cupos_disponibles }} cupos</strong>
                </div>
              }
            </section>

            <section id="resenias" class="grid two">
              <form class="panel form-grid" (ngSubmit)="crearResenia(item)">
                <h2>Crear resenia</h2>
                <label>Rating <input type="number" name="rating" min="1" max="5" [(ngModel)]="resenia.rating" /></label>
                <label class="wide">Comentario <textarea name="comentario" maxlength="500" [(ngModel)]="resenia.comentario"></textarea></label>
                <p class="muted wide">La reserva usada se identifica automaticamente para esta atraccion.</p>
                <button class="btn wide" type="submit">Publicar</button>
              </form>
              <div class="panel stack">
                <h2>Resenias</h2>
                @for (r of reseniasAtraccion(item); track r.id || $index) {
                  <div class="compact-row">
                    <span>
                      <strong>{{ r.rating || r.calificacion || '-' }}/5 · {{ nombreResenia(r) }}</strong>
                      <small>{{ r.comentario }}</small>
                    </span>
                  </div>
                } @empty {
                  <p class="muted">Sin resenias.</p>
                }
              </div>
            </section>
          </main>

          <aside class="booking-card">
            <strong>{{ item.precio_referencia || 0 }} USD</strong>
            <small>precio referencial</small>
            <a class="btn" [routerLink]="['/checkout', item.guid]">Reservar ahora</a>
          </aside>
        </div>
      </section>
    }
  `,
  styles: [`
    .detail-page { max-width: 1120px; margin: 0 auto; }
    .detail-heading h1 { font-size: 32px; margin: 0 0 8px; }
    .eyebrow { color: var(--accent); font-size: 12px; font-weight: 800; margin: 0 0 6px; }
    .gallery { display: grid; gap: 12px; grid-template-columns: 2fr 1.2fr 1.2fr; grid-template-rows: repeat(2, 180px); }
    .gallery img { border-radius: 8px; height: 100%; object-fit: cover; width: 100%; }
    .gallery-main { grid-row: span 2; }
    .detail-layout { align-items: start; display: grid; gap: 28px; grid-template-columns: minmax(0, 1fr) 300px; }
    .tabs-line { border-bottom: 1px solid var(--line); display: flex; gap: 28px; padding-bottom: 12px; }
    .tabs-line a { color: var(--muted); font-size: 14px; font-weight: 700; }
    .feature-list { display: grid; gap: 12px; grid-template-columns: repeat(3, minmax(0, 1fr)); }
    .feature-list span { background: var(--surface-muted); border-radius: 8px; padding: 14px; }
    .feature-groups { display: grid; gap: 18px; grid-template-columns: repeat(2, minmax(0, 1fr)); }
    .feature-groups h3 { font-size: 15px; margin: 0 0 10px; }
    .chip-list, .amenity-list { display: flex; flex-wrap: wrap; gap: 8px; }
    .chip, .amenity-list span:not(.muted) { background: var(--surface-muted); border-radius: 999px; color: var(--text); display: inline-flex; font-size: 13px; font-weight: 700; padding: 8px 10px; }
    .amenity-list span:not(.muted) { border-radius: 6px; }
    .booking-card { background: white; border: 1px solid var(--line); border-radius: 8px; box-shadow: 0 16px 28px rgba(16,24,40,.08); display: grid; gap: 8px; padding: 20px; position: sticky; top: 20px; }
    .booking-card strong { font-size: 26px; }
    .row { align-items: center; border-top: 1px solid var(--line); display: flex; justify-content: space-between; padding-top: 12px; }
    @media (max-width: 860px) {
      .gallery { grid-template-columns: 1fr 1fr; grid-template-rows: auto; }
      .gallery-main { grid-column: 1 / -1; grid-row: auto; }
      .detail-layout { grid-template-columns: 1fr; }
      .booking-card { position: static; }
      .feature-list, .feature-groups { grid-template-columns: 1fr; }
    }
  `]
})
export class AtraccionDetailPageComponent implements OnInit {
  @Input() guid = '';
  atraccion = signal<AtraccionDetalle | null>(null);
  horarios = signal<HorarioDisponible[]>([]);
  resenias = signal<any[]>([]);
  misReservas = signal<Reserva[]>([]);
  resenia = { rating: 5, comentario: '' };

  constructor(
    private readonly api: ApiService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.api.obtenerAtraccion(this.guid).subscribe((response) => this.atraccion.set(response.data));
    this.api.listarHorarios(this.guid).subscribe((response) => this.horarios.set(response.data));
    this.api.resenias().subscribe((response) => this.resenias.set(response.data));
    this.api.misReservas().subscribe({ next: (response) => this.misReservas.set(response.data), error: () => this.misReservas.set([]) });
  }

  crearResenia(item: AtraccionDetalle) {
    const reserva = this.reservaUsadaParaAtraccion(item);
    if (!reserva) {
      this.notifications.error('Para publicar una resenia necesitas una reserva usada de esta atraccion.');
      return;
    }

    this.api.crearResenia({
      atraccionId: item.id,
      reservaId: reserva.id,
      rating: this.resenia.rating,
      comentario: this.resenia.comentario
    }).subscribe({
      next: () => {
        this.notifications.success('Resenia publicada.');
        this.resenia = { rating: 5, comentario: '' };
        this.api.resenias().subscribe((response) => this.resenias.set(response.data));
      },
      error: (error: Error) => this.notifications.error(error.message)
    });
  }

  horariosUnicos() {
    const unicos = new Map<string, HorarioDisponible>();
    this.horarios().forEach((horario) => {
      const key = `${horario.hora_inicio}|${horario.hora_fin ?? ''}`;
      const actual = unicos.get(key);
      if (!actual || horario.cupos_disponibles > actual.cupos_disponibles) {
        unicos.set(key, horario);
      }
    });
    return Array.from(unicos.values());
  }

  reseniasAtraccion(item: AtraccionDetalle) {
    const detalle = (item.resenias || []) as any[];
    const listado = detalle.length ? detalle : this.resenias();
    return listado.filter((resenia) => {
      const atraccionId = resenia.atraccion_id ?? resenia.atraccionId;
      return !atraccionId || atraccionId === item.id;
    });
  }

  galeria(item: AtraccionDetalle) {
    const fallback = [{ url: 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1200&q=80', descripcion: item.nombre }];
    return item.imagenes?.length ? item.imagenes : fallback;
  }

  imagenPrincipal(item: AtraccionDetalle) {
    return this.galeria(item)[0].url;
  }

  activas<T extends { estado?: string }>(items: T[] = []) {
    return items.filter((item) => (item.estado ?? 'A') === 'A');
  }

  incluidos(item: AtraccionDetalle) {
    return this.activas(item.incluye).filter((incluido) => (incluido.tipo ?? 'INCLUYE') === 'INCLUYE');
  }

  noIncluidos(item: AtraccionDetalle) {
    return this.activas(item.incluye).filter((incluido) => incluido.tipo === 'NO_INCLUYE');
  }

  nombreResenia(resenia: any) {
    const usuario = resenia.usuario_creacion || resenia.usuarioCreacion || 'Cliente';
    return String(usuario).includes('@') ? String(usuario).split('@')[0] : usuario;
  }

  private reservaUsadaParaAtraccion(item: AtraccionDetalle) {
    const ticketIds = new Set(item.tickets.map((ticket) => ticket.id));
    return this.misReservas().find((reserva) =>
      reserva.estado === 'USADA' &&
      reserva.detalles?.some((detalle) => ticketIds.has(detalle.ticket_id))
    );
  }
}
