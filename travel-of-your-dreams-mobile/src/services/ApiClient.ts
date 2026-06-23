import { environment } from '../config/environment';
import {
  CustomerProfile,
  Invoice,
  PaymentProcess,
  ProcessResponse,
  Reservation,
  ReservationProcess,
  Session,
} from '../types/models';
import { attraction, customerProfile, invoice, items, reservation, schedule, ticket, unwrap } from './normalizers';

type TokenProvider = () => string | null;

export class ApiClient {
  constructor(private readonly tokenProvider: TokenProvider) {}

  async login(login: string, password: string): Promise<Session> {
    const response = await this.request('/api/v1/auth/login', {
      method: 'POST',
      body: JSON.stringify({ login, password }),
    });
    const value = unwrap(response);
    return {
      usuarioGuid: value?.usuario_guid ?? value?.usuarioGuid ?? '',
      clienteGuid: value?.cliente_guid ?? value?.clienteGuid ?? null,
      login: value?.login ?? login,
      roles: value?.roles ?? [],
      token: value?.token ?? value?.accessToken ?? '',
      expiraEnUtc: value?.expira_en_utc ?? value?.expiraEnUtc ?? '',
    };
  }

  async attractions() {
    const response = await this.request('/api/v2/atracciones?page=1&limit=50', { method: 'GET' }, false);
    return items(response).map(attraction);
  }

  async attraction(guid: string) {
    const response = await this.request(`/api/v2/atracciones/${guid}`, { method: 'GET' }, false);
    return attraction(unwrap(response));
  }

  async profile(login = ''): Promise<CustomerProfile> {
    const response = await this.request('/api/v1/me', { method: 'GET' }, true);
    return customerProfile(unwrap(response), login);
  }

  async schedules(attractionGuid: string) {
    const value = await this.graphql<{ horarios: any }>(
      'query Horarios($guid: UUID!) { horarios(atraccionGuid: $guid) }',
      { guid: attractionGuid },
      false,
    );
    return items(value.horarios).map(schedule).filter((x) => x.guid && x.availableSlots > 0);
  }

  async tickets(attractionGuid: string) {
    const value = await this.graphql<{ tickets: any }>(
      'query Tickets($guid: UUID!) { tickets(atraccionGuid: $guid) }',
      { guid: attractionGuid },
      false,
    );
    return items(value.tickets).map(ticket).filter((x) => x.guid && x.status !== 'I');
  }

  async requestReservation(input: {
    attractionGuid: string;
    scheduleGuid: string;
    lines: Array<{ ticketGuid: string; quantity: number }>;
    taxPercentage?: number;
  }): Promise<ProcessResponse> {
    const value = await this.graphql<{ solicitarReserva: any }>(
      `mutation Reservar($input: SolicitarReservaInput!) {
        solicitarReserva(input: $input) { correlationId estado mensaje }
      }`,
      {
        input: {
          atraccionGuid: input.attractionGuid,
          horarioGuid: input.scheduleGuid,
          lineas: input.lines.map((x) => ({ ticketGuid: x.ticketGuid, cantidad: x.quantity })),
          expiracionMinutos: 15,
          porcentajeIva: input.taxPercentage ?? 12,
        },
      },
    );
    return processResponse(value.solicitarReserva);
  }

  async reservationStatus(correlationId: string): Promise<ReservationProcess> {
    const value = await this.graphql<{ estadoReserva: any }>(
      `query EstadoReserva($id: UUID!) {
        estadoReserva(correlationId: $id) {
          correlationId estado reservaGuid reservaCodigo error updatedAtUtc
        }
      }`,
      { id: correlationId },
    );
    const state = value.estadoReserva;
    return {
      correlationId: state.correlationId,
      state: state.estado,
      reservationGuid: state.reservaGuid,
      reservationCode: state.reservaCodigo,
      error: state.error,
    };
  }

  async reservations(state?: string): Promise<Reservation[]> {
    const value = await this.graphql<{ misReservas: any }>(
      'query Reservas($estado: String) { misReservas(estado: $estado) }',
      { estado: state || null },
    );
    return items(value.misReservas).map(reservation);
  }

  async reservation(guid: string): Promise<Reservation> {
    const value = await this.graphql<{ reserva: any }>(
      'query Reserva($guid: UUID!) { reserva(guid: $guid) }',
      { guid },
    );
    return reservation(unwrap(value.reserva));
  }

  async requestPayment(input: {
    reservationGuid: string;
    amount: number;
    method?: string;
    reference?: string;
  }): Promise<ProcessResponse> {
    const value = await this.graphql<{ confirmarPago: any }>(
      `mutation Pagar($input: ConfirmarPagoInput!) {
        confirmarPago(input: $input) { correlationId estado mensaje }
      }`,
      {
        input: {
          reservaGuid: input.reservationGuid,
          monto: input.amount,
          metodo: input.method ?? 'TARJETA',
          moneda: 'USD',
          referencia: input.reference ?? `MOBILE-${Date.now()}`,
          observacion: 'Pago procesado desde marketplace móvil',
        },
      },
    );
    return processResponse(value.confirmarPago);
  }

  async paymentStatus(correlationId: string): Promise<PaymentProcess> {
    const value = await this.graphql<{ estadoPago: any }>(
      `query EstadoPago($id: UUID!) {
        estadoPago(correlationId: $id) {
          correlationId estado reservaGuid facturaGuid facturaNumero error updatedAtUtc
        }
      }`,
      { id: correlationId },
    );
    const state = value.estadoPago;
    return {
      correlationId: state.correlationId,
      state: state.estado,
      reservationGuid: state.reservaGuid,
      invoiceGuid: state.facturaGuid,
      invoiceNumber: state.facturaNumero,
      error: state.error,
    };
  }

  async invoices(): Promise<Invoice[]> {
    const value = await this.graphql<{ misFacturas: any }>(
      'query { misFacturas(page: 1, limit: 50) }',
    );
    return items(value.misFacturas).map(invoice);
  }

  async invoice(guid: string): Promise<Invoice> {
    const response = await this.request(`/api/v1/facturas/${guid}`, { method: 'GET' }, true);
    return invoice(unwrap(response));
  }

  async waitForReservation(correlationId: string, attempts = 24) {
    return this.poll(
      () => this.reservationStatus(correlationId),
      (value) => ['RESERVA_CREADA', 'RESERVA_RECHAZADA'].includes(value.state),
      attempts,
    );
  }

  async waitForPayment(correlationId: string, attempts = 24) {
    return this.poll(
      () => this.paymentStatus(correlationId),
      (value) => ['FACTURA_EMITIDA', 'PAGO_RECHAZADO'].includes(value.state),
      attempts,
    );
  }

  private async graphql<T>(query: string, variables: Record<string, any> = {}, authenticated = true): Promise<T> {
    const response = await this.request(
      '/graphql',
      { method: 'POST', body: JSON.stringify({ query, variables }) },
      authenticated,
    );
    if (response?.errors?.length) {
      throw new Error(response.errors.map((x: any) => x.message).join('\n'));
    }
    return response.data as T;
  }

  private async request(path: string, init: RequestInit, authenticated = false) {
    const token = this.tokenProvider();
    if (authenticated && !token) throw new Error('Tu sesión ha expirado. Inicia sesión nuevamente.');

    const response = await fetch(`${environment.gatewayUrl}${path}`, {
      ...init,
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...(init.headers ?? {}),
      },
    });
    const raw = await response.text();
    let body: any = null;
    try {
      body = raw ? JSON.parse(raw) : null;
    } catch {
      body = raw;
    }
    if (!response.ok) {
      throw new Error(errorMessage(body) || `La solicitud falló (${response.status}).`);
    }
    return body;
  }

  private async poll<T>(load: () => Promise<T>, done: (value: T) => boolean, attempts: number): Promise<T> {
    let lastError: unknown;
    for (let attempt = 0; attempt < attempts; attempt += 1) {
      try {
        const value = await load();
        if (done(value)) return value;
      } catch (error) {
        lastError = error;
      }
      await delay(1500);
    }
    if (lastError instanceof Error) throw lastError;
    throw new Error('El procesamiento está tardando más de lo esperado. Revisa tu actividad en unos segundos.');
  }
}

function processResponse(value: any): ProcessResponse {
  return {
    correlationId: value?.correlationId,
    state: value?.estado,
    message: value?.mensaje,
  };
}

function errorMessage(body: any): string {
  if (typeof body === 'string') return body;
  if (Array.isArray(body?.details) && body.details.length) return body.details.join('\n');
  return body?.error ?? body?.message ?? body?.title ?? '';
}

function delay(milliseconds: number) {
  return new Promise((resolve) => setTimeout(resolve, milliseconds));
}
