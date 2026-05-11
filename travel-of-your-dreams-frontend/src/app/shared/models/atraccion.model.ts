export interface AtraccionPublica {
  id: number;
  guid: string;
  nombre: string;
  descripcion?: string | null;
  pais?: string | null;
  direccion?: string | null;
  duracion_minutos?: number | null;
  precio_referencia?: number | null;
  disponible: boolean;
  free_cancellation: boolean;
  skip_the_line: boolean;
  total_resenias: number;
}

export interface AtraccionDetalle extends AtraccionPublica {
  categorias: Array<{ id?: number; nombre?: string; tag_name?: string | null; estado?: string }>;
  idiomas: Array<{ id?: number; codigo?: string; descripcion?: string; estado?: string }>;
  imagenes: Array<{ id?: number; url: string; descripcion?: string | null }>;
  incluye: Array<{ id?: number; descripcion?: string; tipo?: string; estado?: string }>;
  tickets: Ticket[];
  resenias: unknown[];
}

export interface Ticket {
  id: number;
  guid: string;
  atraccion_id: number;
  titulo: string;
  precio: number;
  moneda: string;
  tipo_participante: string;
  capacidad_maxima: number;
  estado: string;
}

export interface HorarioDisponible {
  id: number;
  guid: string;
  atraccion_id: number;
  fecha: string;
  hora_inicio: string;
  hora_fin?: string | null;
  cupos_disponibles: number;
  estado: string;
}
