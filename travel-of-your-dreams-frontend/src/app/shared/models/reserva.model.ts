export interface CrearReservaRequest {
  atraccionGuid: string;
  horarioGuid: string;
  fecha?: string;
  lineas: Array<{ ticketGuid: string; cantidad: number }>;
  origenCanal?: string | null;
  expiracionMinutos: number;
  porcentajeIva: number;
}

export interface Reserva {
  id: number;
  guid: string;
  codigo: string;
  cliente_id: number;
  horario_id: number;
  fecha_reserva_utc: string;
  fecha_expiracion_utc: string;
  subtotal: number;
  valor_iva: number;
  total: number;
  moneda: string;
  estado: string;
  detalles: ReservaDetalle[];
}

export interface ReservaDetalle {
  id: number;
  guid: string;
  reserva_id: number;
  ticket_id: number;
  cantidad: number;
  precio_unitario: number;
  subtotal: number;
  estado: string;
}
