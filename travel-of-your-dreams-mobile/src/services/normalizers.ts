import { Attraction, CustomerProfile, Invoice, Reservation, Schedule, Ticket } from '../types/models';

const fallbackImage =
  'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1200&q=80';

export function unwrap(value: any): any {
  if (value && typeof value === 'object' && 'data' in value) return unwrap(value.data);
  return value;
}

export function items(value: any): any[] {
  const unwrapped = unwrap(value);
  if (Array.isArray(unwrapped)) return unwrapped;
  if (Array.isArray(unwrapped?.items)) return unwrapped.items;
  if (Array.isArray(unwrapped?.data)) return unwrapped.data;
  return [];
}

export function attraction(value: any): Attraction {
  const categories = value?.categorias ?? [];
  const languages = value?.idiomas ?? [];
  const includeItems = value?.incluye ?? [];
  const images = value?.imagenes ?? [];
  const image =
    value?.imagen_principal ??
    value?.imagenPrincipal ??
    value?.imagen_principal_url ??
    value?.imagenPrincipalUrl ??
    value?.imageUrl ??
    value?.image?.url ??
    images?.[0]?.url ??
    images?.[0];
  const name = value?.nombre ?? value?.at_nombre ?? value?.name ?? 'Atracción';
  const city = value?.ciudad ?? value?.destino_nombre ?? value?.destinoNombre;

  return {
    guid: String(value?.guid ?? value?.at_guid ?? value?.id ?? ''),
    name,
    description: value?.descripcion_corta ?? value?.descripcionCorta ?? value?.descripcion ?? '',
    country: value?.pais ?? city ?? 'Destino',
    address: value?.direccion ?? value?.punto_encuentro ?? '',
    durationMinutes: numberOrUndefined(value?.duracion_minutos ?? value?.duracionMinutos),
    price: number(value?.precio_referencia ?? value?.precioReferencia ?? value?.precio_desde ?? value?.precioDesde),
    available: Boolean(value?.disponible ?? value?.disponibilidad?.disponible ?? true),
    freeCancellation: Boolean(value?.free_cancellation ?? value?.freeCancellation),
    skipLine: Boolean(value?.skip_the_line ?? value?.skipTheLine),
    includesTransport: Boolean(value?.incluye_transporte ?? value?.incluyeTransporte),
    includesGuide: Boolean(value?.incluye_acompaniante ?? value?.incluyeAcompaniante),
    reviews: number(value?.total_resenias ?? value?.totalResenias ?? value?.total_resenas),
    imageUrl: validUrl(image) ? image : fallbackAttractionImage(name, city),
    taxPercentage: numberOrUndefined(value?.porcentajeIva ?? value?.porcentaje_iva ?? value?.iva_porcentaje ?? value?.ivaPorcentaje),
    categories: categories.map((x: any) => x?.nombre ?? x?.tag_name).filter(Boolean),
    languages: languages.map((x: any) => x?.descripcion ?? x?.codigo).filter(Boolean),
    includes: includeItems
      .filter((x: any) => (x?.tipo ?? 'INCLUYE') === 'INCLUYE')
      .map((x: any) => x?.descripcion)
      .filter(Boolean),
    excludes: (value?.no_incluye ?? includeItems.filter((x: any) => x?.tipo === 'NO_INCLUYE'))
      .map((x: any) => x?.descripcion ?? x)
      .filter(Boolean),
  };
}

export function customerProfile(value: any, login = ''): CustomerProfile {
  const names = value?.nombres ?? value?.names;
  const lastNames = value?.apellidos ?? value?.lastNames;
  const businessName = value?.razonSocial ?? value?.razon_social;
  const fullName = [names, lastNames].filter(Boolean).join(' ').trim();
  const displayName = businessName || fullName || value?.correo || login || 'Cliente';

  return {
    displayName,
    names,
    lastNames,
    businessName,
    email: value?.correo ?? value?.email ?? login,
    phone: value?.telefono ?? value?.phone,
    address: value?.direccion ?? value?.address,
    identificationType: value?.tipoIdentificacion ?? value?.tipo_identificacion,
    identificationNumber: value?.numeroIdentificacion ?? value?.numero_identificacion,
  };
}

export function schedule(value: any): Schedule {
  return {
    guid: String(value?.guid ?? value?.hor_guid ?? value?.id ?? ''),
    date: value?.fecha ?? value?.hor_fecha ?? '',
    startTime: value?.hora_inicio ?? value?.horaInicio ?? value?.hor_hora_inicio ?? '',
    endTime: value?.hora_fin ?? value?.horaFin ?? value?.hor_hora_fin ?? undefined,
    availableSlots: number(value?.cupos_disponibles ?? value?.cuposDisponibles ?? value?.hor_cupos_disponibles),
    status: value?.estado ?? value?.hor_estado ?? 'A',
  };
}

export function ticket(value: any): Ticket {
  return {
    guid: String(value?.guid ?? value?.tck_guid ?? value?.id ?? ''),
    title: value?.titulo ?? value?.tck_titulo ?? value?.tipo_participante ?? 'Entrada',
    participantType: value?.tipo_participante ?? value?.tipoParticipante ?? 'Participante',
    price: number(value?.precio ?? value?.tck_precio),
    currency: value?.moneda ?? value?.tck_moneda ?? 'USD',
    capacity: number(value?.capacidad_maxima ?? value?.capacidadMaxima ?? 1),
    status: value?.estado ?? value?.tck_estado ?? 'A',
  };
}

export function reservation(value: any): Reservation {
  return {
    guid: String(value?.guid ?? value?.rev_guid ?? value?.reservaGuid ?? ''),
    code: value?.codigo ?? value?.rev_codigo ?? value?.reservaCodigo ?? 'Reserva',
    status: value?.estado ?? value?.rev_estado ?? 'PENDIENTE',
    attractionGuid: String(value?.atraccionGuid ?? value?.atraccion_guid ?? value?.at_guid ?? ''),
    scheduleGuid: String(value?.horarioGuid ?? value?.horario_guid ?? value?.hor_guid ?? ''),
    attractionName: value?.atraccion_nombre ?? value?.atraccionNombre ?? value?.atraccion?.nombre ?? 'Atracción',
    date: value?.horFecha ?? value?.hor_fecha ?? value?.fecha ?? value?.horario?.fecha ?? '',
    startTime: value?.horHoraInicio ?? value?.hor_hora_inicio ?? value?.horaInicio ?? value?.hora_inicio ?? value?.horario?.horaInicio ?? value?.horario?.hora_inicio ?? '',
    endTime: value?.horHoraFin ?? value?.hor_hora_fin ?? value?.horaFin ?? value?.hora_fin ?? value?.horario?.horaFin ?? value?.horario?.hora_fin ?? undefined,
    subtotal: numberOrUndefined(value?.subtotal ?? value?.rev_subtotal),
    tax: numberOrUndefined(value?.valorIva ?? value?.valor_iva ?? value?.iva ?? value?.rev_iva),
    total: number(value?.total ?? value?.rev_total),
    currency: value?.moneda ?? value?.rev_moneda ?? 'USD',
    createdAt: value?.fechaReservaUtc ?? value?.fecha_reserva_utc ?? value?.rev_fecha_reserva_utc,
    expiresAt: value?.fechaExpiracionUtc ?? value?.fecha_expiracion_utc ?? value?.rev_fecha_expiracion_utc,
    details: value?.detalles ?? value?.detalle ?? [],
  };
}

export function invoice(value: any): Invoice {
  const subtotal = numberOrUndefined(value?.subtotal ?? value?.fac_subtotal);
  const tax = numberOrUndefined(value?.iva ?? value?.valorIva ?? value?.valor_iva ?? value?.impuesto ?? value?.fac_iva);
  const total = number(value?.total ?? value?.fac_total);

  return {
    guid: String(value?.guid ?? value?.fac_guid ?? ''),
    number: value?.numero ?? value?.fac_numero ?? 'Factura',
    status: value?.estado ?? value?.fac_estado ?? 'A',
    total,
    subtotal,
    tax: tax ?? (subtotal !== undefined && total >= subtotal ? total - subtotal : undefined),
    currency: value?.moneda ?? value?.fac_moneda ?? 'USD',
    issuedAt:
      value?.fecha_emision_utc ??
      value?.fechaEmisionUtc ??
      value?.fecha_emision ??
      value?.fechaEmision ??
      value?.fac_fecha_emision_utc,
    reservationGuid: String(value?.reservaGuid ?? value?.reserva_guid ?? value?.rev_guid ?? ''),
    reservationCode: value?.rev_codigo ?? value?.reserva_codigo ?? value?.reservaCodigo,
    paymentGuid: String(value?.pagoGuid ?? value?.pago_guid ?? value?.pag_guid ?? ''),
    observation: value?.observacion ?? value?.fac_observacion,
  };
}

function number(value: any) {
  const parsed = Number(value ?? 0);
  return Number.isFinite(parsed) ? parsed : 0;
}

function numberOrUndefined(value: any) {
  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : undefined;
}

function validUrl(value: any): value is string {
  return typeof value === 'string' && /^https?:\/\//i.test(value) && !value.includes('example.com');
}

function fallbackAttractionImage(name: string, city?: string) {
  const haystack = `${name} ${city ?? ''}`.toLowerCase();
  if (haystack.includes('quito') || haystack.includes('centro historico')) {
    return 'https://images.unsplash.com/photo-1588421357574-87938a86fa28?auto=format&fit=crop&w=1200&q=80';
  }
  if (haystack.includes('banos') || haystack.includes('baños') || haystack.includes('cascada')) {
    return 'https://images.unsplash.com/photo-1565018054866-968e244671af?auto=format&fit=crop&w=1200&q=80';
  }
  if (haystack.includes('universal')) {
    return 'https://images.unsplash.com/photo-1534447677768-be436bb09401?auto=format&fit=crop&w=1200&q=80';
  }
  return fallbackImage;
}
