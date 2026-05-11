export interface CrearReservaRequest {
  hor_guid: string;
  fecha?: string;
  lineas: Array<{ tck_guid: string; cantidad: number }>;
  origen_canal?: string | null;
  expiracion_minutos: number;
  porcentaje_iva: number;
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
