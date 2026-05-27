import { Component, Input, OnInit, signal } from '@angular/core';
import { Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../core/api/api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { AtraccionDetalle, HorarioDisponible } from '../../../shared/models/atraccion.model';

@Component({
  selector: 'app-atraccion-detail-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    @if (atraccion(); as item) {
      <section class="page detail-page stack">
        <button class="btn secondary local-back" type="button" (click)="volver()">Volver</button>
        <header class="detail-heading">
          <div>
            <p class="eyebrow">{{ item.pais || 'Destino' }}</p>
            <h1>{{ item.nombre }}</h1>
            <p class="muted">{{ item.total_resenias }} resenias · {{ item.direccion || 'Direccion por confirmar' }}</p>
          </div>
          <div class="detail-kpis">
            <span class="status-badge ok">Reservable</span>
            <span class="data-chip">{{ item.precio_referencia || 0 }} USD</span>
            @if (item.duracion_minutos) { <span class="data-chip">{{ item.duracion_minutos }} min</span> }
          </div>
        </header>

        <div class="gallery">
          <img class="gallery-main" [src]="imagenPrincipal(item)" [alt]="item.nombre" (error)="imagenDetalleError()">
          @for (imagen of galeria(item).slice(1, 5); track imagen.url) {
            <img [src]="imagen.url" [alt]="imagen.descripcion || item.nombre" (error)="imagenDetalleError()">
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
                @if (incluyeTransporte(item)) {
                  <span>Incluye transporte</span>
                }
                @if (incluyeAcompaniante(item)) {
                  <span>Incluye acompaniante/guia</span>
                }
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
              @for (horario of proximosHorarios(); track horario.guid) {
                <div class="row">
                  <span>{{ formatearFechaHorario(horario.fecha) }} - {{ horario.hora_inicio }}{{ horario.hora_fin ? ' - ' + horario.hora_fin : '' }}</span>
                  <strong>{{ horario.cupos_disponibles }} cupos</strong>
                </div>
              } @empty {
                <p class="muted">No hay horarios disponibles para hoy.</p>
              }
            </section>

            <section id="resenias" class="grid two">
              <form class="panel form-grid" (ngSubmit)="crearResenia(item)">
                <h2>Crear resenia</h2>
                <label>Rating <input type="number" name="rating" min="1" max="5" [(ngModel)]="resenia.rating" /></label>
                <label class="wide">Comentario <textarea name="comentario" maxlength="500" [(ngModel)]="resenia.comentario"></textarea></label>
                <p class="muted wide">Los clientes registrados pueden publicar una resenia de esta atraccion.</p>
                <button class="btn wide" type="submit">Publicar</button>
              </form>
              <div class="panel stack">
                <h2>Resenias</h2>
                @for (r of reseniasAtraccion(item); track r.guid || r.id || $index) {
                  <div class="compact-row">
                    <span>
                      <strong>{{ calificacionResenia(r) }}/5 · {{ nombreResenia(r) }}</strong>
                      <small>{{ comentarioResenia(r) }}</small>
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
            @if (proximosHorarios()[0]; as proximo) {
              <span class="next-slot">Hoy: {{ proximo.hora_inicio }}{{ proximo.hora_fin ? ' - ' + proximo.hora_fin : '' }} · {{ proximo.cupos_disponibles }} cupos</span>
            }
            <a class="btn" [routerLink]="['/checkout', item.guid]">Reservar ahora</a>
          </aside>
        </div>
      </section>
    }
  `,
  styles: [`
    .detail-page { max-width: 1120px; margin: 0 auto; }
    .local-back { align-self: flex-start; margin-bottom: 2px; }
    .detail-heading { align-items: end; display: flex; gap: 18px; justify-content: space-between; }
    .detail-heading h1 { font-size: clamp(32px, 4vw, 48px); margin: 0 0 8px; }
    .detail-kpis { display: flex; flex-wrap: wrap; gap: 8px; justify-content: flex-end; }
    .eyebrow { color: var(--accent); font-size: 12px; font-weight: 800; margin: 0 0 6px; }
    .gallery { display: grid; gap: 12px; grid-template-columns: 2fr 1.2fr 1.2fr; grid-template-rows: repeat(2, 180px); }
    .gallery img { border-radius: 8px; height: 100%; object-fit: cover; width: 100%; }
    .gallery-main { grid-row: span 2; }
    .detail-layout { align-items: start; display: grid; gap: 28px; grid-template-columns: minmax(0, 1fr) 300px; }
    .tabs-line { border-bottom: 1px solid var(--line); display: flex; gap: 28px; padding-bottom: 12px; }
    .tabs-line a { color: var(--muted); font-size: 14px; font-weight: 700; }
    .feature-list { display: grid; gap: 12px; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); }
    .feature-list span { background: var(--surface-muted); border-radius: 8px; padding: 14px; }
    .feature-groups { display: grid; gap: 18px; grid-template-columns: repeat(2, minmax(0, 1fr)); }
    .feature-groups h3 { font-size: 15px; margin: 0 0 10px; }
    .chip-list, .amenity-list { display: flex; flex-wrap: wrap; gap: 8px; }
    .chip, .amenity-list span:not(.muted) { background: var(--surface-muted); border-radius: 999px; color: var(--text); display: inline-flex; font-size: 13px; font-weight: 700; padding: 8px 10px; }
    .amenity-list span:not(.muted) { border-radius: 6px; }
    .booking-card { background: white; border: 1px solid var(--line); border-radius: 8px; box-shadow: var(--shadow-md); display: grid; gap: 10px; padding: 20px; position: sticky; top: 84px; }
    .booking-card strong { font-size: 26px; }
    .next-slot { background: var(--primary-soft); border-radius: 6px; color: var(--primary-strong); font-size: 13px; font-weight: 800; padding: 10px; }
    .row { align-items: center; border-top: 1px solid var(--line); display: flex; justify-content: space-between; padding-top: 12px; }
    @media (max-width: 860px) {
      .gallery { grid-template-columns: 1fr 1fr; grid-template-rows: auto; }
      .gallery-main { grid-column: 1 / -1; grid-row: auto; }
      .detail-layout { grid-template-columns: 1fr; }
      .booking-card { position: static; }
      .detail-heading { align-items: start; flex-direction: column; }
      .detail-kpis { justify-content: start; }
      .feature-list, .feature-groups { grid-template-columns: 1fr; }
    }
  `]
})
export class AtraccionDetailPageComponent implements OnInit {
  @Input() guid = '';
  atraccion = signal<AtraccionDetalle | null>(null);
  horarios = signal<HorarioDisponible[]>([]);
  resenias = signal<any[]>([]);
  resenia = { rating: 5, comentario: '' };

  constructor(
    private readonly api: ApiService,
    private readonly auth: AuthService,
    private readonly location: Location,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit() {
    this.api.obtenerAtraccionPublica(this.guid).subscribe((response) => this.atraccion.set(this.toDetalle(response.data)));
    this.cargarContextoInternoAtraccion();
    this.api.listarHorarios(this.guid).subscribe((response) => this.horarios.set(this.itemsFromResponse(response.data).map((item) => this.toHorario(item))));
    this.cargarResenias();
  }

  crearResenia(item: AtraccionDetalle) {
    if (!this.auth.isAuthenticated) {
      this.notifications.error('Debes iniciar sesion para publicar una resenia.');
      return;
    }

    const rating = Number(this.resenia.rating);
    if (!Number.isFinite(rating) || rating < 1 || rating > 5) {
      this.notifications.error('La calificacion debe estar entre 1 y 5.');
      return;
    }

    this.api.crearReseniaAtraccion(item.guid, {
      rating,
      comentario: this.resenia.comentario
    }).subscribe({
      next: () => {
        this.notifications.success('Resenia publicada.');
        this.resenia = { rating: 5, comentario: '' };
        this.cargarResenias();
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

  volver() {
    this.location.back();
  }

  proximosHorarios() {
    const hoy = this.fechaActual();
    return this.horarios()
      .filter((horario) => horario.fecha === hoy)
      .sort((a, b) => `${a.fecha} ${a.hora_inicio}`.localeCompare(`${b.fecha} ${b.hora_inicio}`))
      .slice(0, 6);
  }

  reseniasAtraccion(item: AtraccionDetalle) {
    const detalle = (item.resenias || []) as any[];
    const listado = detalle.length ? detalle : this.resenias();
    return listado.filter((resenia) => {
      const atraccionId = resenia.atraccion_id ?? resenia.atraccionId ?? resenia.AtraccionId;
      const atraccionGuid = resenia.atraccion_guid ?? resenia.atraccionGuid ?? resenia.AtraccionGuid;
      if (atraccionGuid) return atraccionGuid === item.guid;
      if (atraccionId && item.id) return Number(atraccionId) === Number(item.id);
      return false;
    });
  }

  private cargarContextoInternoAtraccion() {
    this.api.obtenerAtraccion(this.guid).subscribe({
      next: (response) => {
        const detalleInterno = this.toDetalle(response.data);
        this.atraccion.update((actual) => actual ? {
          ...actual,
          id: actual.id || detalleInterno.id,
          resenias: detalleInterno.resenias.length ? detalleInterno.resenias : actual.resenias
        } : detalleInterno);
      },
      error: () => undefined
    });
  }

  private cargarResenias() {
    this.api.reseniasAtraccion(this.guid).subscribe({
      next: (response) => {
        const resenias = this.itemsFromResponse(response.data);
        if (resenias.length) {
          this.resenias.set(resenias);
          return;
        }

        this.cargarReseniasGlobales();
      },
      error: () => this.cargarReseniasGlobales()
    });
  }

  private cargarReseniasGlobales() {
    this.api.resenias().subscribe({
      next: (response) => this.resenias.set(this.itemsFromResponse(response.data)),
      error: () => this.resenias.set([])
    });
  }

  galeria(item: AtraccionDetalle) {
    const fallback = [{ url: 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1200&q=80', descripcion: item.nombre }];
    return item.imagenes?.length ? item.imagenes : fallback;
  }

  imagenPrincipal(item: AtraccionDetalle) {
    return this.galeria(item)[0].url;
  }

  imagenDetalleError() {
    this.atraccion.update((item) => item ? { ...item, imagenes: [] } : item);
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

  incluyeTransporte(item: any) {
    return Boolean(item.incluye_transporte ?? item.incluyeTransporte);
  }

  incluyeAcompaniante(item: any) {
    return Boolean(item.incluye_acompaniante ?? item.incluyeAcompaniante);
  }

  nombreResenia(resenia: any) {
    const usuario = resenia.usuario_creacion || resenia.usuarioCreacion || 'Cliente';
    return String(usuario).includes('@') ? String(usuario).split('@')[0] : usuario;
  }

  calificacionResenia(resenia: any) {
    return resenia.rating ?? resenia.calificacion ?? resenia.Rating ?? resenia.Calificacion ?? '-';
  }

  comentarioResenia(resenia: any) {
    return resenia.comentario ?? resenia.Comentario ?? 'Sin comentario.';
  }

  private itemsFromResponse(value: any): any[] {
    if (Array.isArray(value)) return value;
    if (Array.isArray(value?.items)) return value.items;
    if (Array.isArray(value?.data)) return value.data;
    return [];
  }

  private toDetalle(value: any): AtraccionDetalle {
    const guid = value?.guid ?? value?.id ?? this.guid;
    const incluye = this.toIncluye(value?.incluye, 'INCLUYE');
    const noIncluye = this.toIncluye(value?.no_incluye ?? value?.noIncluye, 'NO_INCLUYE');

    return {
      id: value?.numeric_id ?? value?.numericId ?? 0,
      guid,
      nombre: value?.nombre ?? '',
      descripcion: value?.descripcion ?? value?.descripcion_corta ?? value?.descripcionCorta ?? null,
      pais: value?.pais ?? value?.ciudad ?? null,
      direccion: value?.direccion ?? value?.punto_encuentro ?? value?.puntoEncuentro ?? null,
      duracion_minutos: value?.duracion_minutos ?? value?.duracionMinutos ?? null,
      precio_referencia: Number(value?.precio_referencia ?? value?.precioReferencia ?? value?.precio_desde ?? value?.precioDesde ?? 0),
      disponible: Boolean(value?.disponible ?? value?.disponibilidad?.disponible ?? true),
      free_cancellation: Boolean(value?.free_cancellation ?? value?.freeCancellation ?? value?.etiquetas?.includes?.('free_cancellation') ?? false),
      skip_the_line: Boolean(value?.skip_the_line ?? value?.skipTheLine ?? value?.etiquetas?.includes?.('skip_the_line') ?? false),
      incluye_transporte: Boolean(value?.incluye_transporte ?? value?.incluyeTransporte ?? false),
      incluye_acompaniante: Boolean(value?.incluye_acompaniante ?? value?.incluyeAcompaniante ?? false),
      total_resenias: Number(value?.total_resenias ?? value?.totalResenias ?? value?.total_resenas ?? value?.totalResenas ?? 0),
      categorias: [
        ...(value?.tipo_nombre || value?.tipoNombre ? [{ nombre: value.tipo_nombre ?? value.tipoNombre, tag_name: value.tipo_tagname ?? value.tipoTagname, estado: 'A' }] : []),
        ...(value?.subtipo_nombre || value?.subtipoNombre ? [{ nombre: value.subtipo_nombre ?? value.subtipoNombre, tag_name: value.subtipo_tagname ?? value.subtipoTagname, estado: 'A' }] : [])
      ],
      idiomas: this.itemsFromResponse(value?.idiomas_disponibles ?? value?.idiomasDisponibles).map((codigo) => ({ codigo, descripcion: codigo, estado: 'A' })),
      imagenes: this.toImagenes(value),
      incluye: [...incluye, ...noIncluye],
      tickets: this.itemsFromResponse(value?.tickets).map((ticket) => ({
        id: ticket?.id ?? 0,
        guid: ticket?.guid ?? ticket?.tck_guid ?? ticket?.tckGuid ?? '',
        atraccion_id: ticket?.atraccion_id ?? ticket?.atraccionId ?? 0,
        titulo: ticket?.titulo ?? ticket?.tipo ?? 'Ticket',
        precio: Number(ticket?.precio ?? 0),
        moneda: ticket?.moneda ?? 'USD',
        tipo_participante: ticket?.tipo_participante ?? ticket?.tipoParticipante ?? ticket?.tipo ?? '',
        capacidad_maxima: Number(ticket?.capacidad_maxima ?? ticket?.capacidadMaxima ?? 999),
        estado: ticket?.estado ?? 'A'
      })),
      resenias: this.itemsFromResponse(value?.resenias)
    };
  }

  private toIncluye(value: any, tipo: string) {
    return this.itemsFromResponse(value).map((item) => ({
      descripcion: typeof item === 'string' ? item : item?.descripcion,
      tipo,
      estado: 'A'
    }));
  }

  private toImagenes(value: any) {
    const imagenPrincipal = this.urlImagen(value?.imagen_principal ?? value?.imagenPrincipal);
    const imagenes = this.itemsFromResponse(value?.imagenes)
      .map((imagen) => typeof imagen === 'string' ? { url: imagen } : imagen)
      .map((imagen) => ({ ...imagen, url: this.urlImagen(imagen?.url) }))
      .filter((imagen) => Boolean(imagen.url)) as Array<{ id?: number; url: string; descripcion?: string | null }>;

    if (imagenPrincipal && !imagenes.some((imagen) => imagen.url === imagenPrincipal)) {
      imagenes.unshift({ url: imagenPrincipal, descripcion: value?.nombre ?? null });
    }

    return imagenes;
  }

  private urlImagen(value: unknown): string | null {
    if (typeof value !== 'string') return null;
    const url = value.trim();
    if (!url || url.includes('example.com')) return null;
    return /^https?:\/\//i.test(url) ? url : null;
  }

  private toHorario(value: any): HorarioDisponible {
    return {
      ...value,
      guid: value?.guid ?? value?.hor_guid ?? value?.horGuid ?? '',
      fecha: value?.fecha ?? '',
      hora_inicio: value?.hora_inicio ?? value?.horaInicio ?? '',
      hora_fin: value?.hora_fin ?? value?.horaFin ?? null,
      cupos_disponibles: Number(value?.cupos_disponibles ?? value?.cuposDisponibles ?? value?.cupos ?? 0)
    };
  }

  formatearFechaHorario(value: string | null | undefined) {
    if (!value) return 'Fecha por confirmar';
    const [year, month, day] = value.split('-').map(Number);
    if (!year || !month || !day) return value;
    return new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium' }).format(new Date(year, month - 1, day));
  }

  private fechaActual() {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 10);
  }
}
