export type JsonMap = Record<string, any>;

export interface Session {
  usuarioGuid: string;
  clienteGuid?: string | null;
  login: string;
  roles: string[];
  token: string;
  expiraEnUtc: string;
}

export interface CustomerProfile {
  displayName: string;
  names?: string;
  lastNames?: string;
  businessName?: string;
  email: string;
  phone?: string;
  address?: string;
  identificationType?: string;
  identificationNumber?: string;
}

export interface Attraction {
  guid: string;
  name: string;
  description: string;
  country: string;
  address: string;
  durationMinutes?: number;
  price: number;
  available: boolean;
  freeCancellation: boolean;
  skipLine: boolean;
  includesTransport: boolean;
  includesGuide: boolean;
  reviews: number;
  imageUrl: string;
  taxPercentage?: number;
  categories: string[];
  languages: string[];
  includes: string[];
  excludes: string[];
}

export interface Schedule {
  guid: string;
  date: string;
  startTime: string;
  endTime?: string;
  availableSlots: number;
  status: string;
}

export interface Ticket {
  guid: string;
  title: string;
  participantType: string;
  price: number;
  currency: string;
  capacity: number;
  status: string;
}

export interface Reservation {
  guid: string;
  code: string;
  status: string;
  attractionGuid?: string;
  scheduleGuid?: string;
  attractionName: string;
  date: string;
  startTime: string;
  endTime?: string;
  subtotal?: number;
  tax?: number;
  total: number;
  currency: string;
  createdAt?: string;
  expiresAt?: string;
  details: JsonMap[];
}

export interface Invoice {
  guid: string;
  number: string;
  status: string;
  total: number;
  subtotal?: number;
  tax?: number;
  currency: string;
  issuedAt?: string;
  reservationGuid?: string;
  reservationCode?: string;
  reservationAttractionName?: string;
  paymentGuid?: string;
  observation?: string;
}

export interface ProcessResponse {
  correlationId: string;
  state: string;
  message: string;
}

export interface ReservationProcess {
  correlationId: string;
  state: string;
  reservationGuid?: string | null;
  reservationCode?: string | null;
  error?: string | null;
}

export interface PaymentProcess {
  correlationId: string;
  state: string;
  reservationGuid?: string | null;
  invoiceGuid?: string | null;
  invoiceNumber?: string | null;
  error?: string | null;
}
