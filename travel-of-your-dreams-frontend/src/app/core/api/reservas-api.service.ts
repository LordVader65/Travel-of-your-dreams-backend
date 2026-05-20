import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { CrearReservaRequest } from '../../shared/models/reserva.model';

@Injectable({ providedIn: 'root' })
export class ReservasApiService {
  constructor(private readonly api: ApiService) {}

  crearReserva(request: CrearReservaRequest) {
    return this.api.crearReserva(request);
  }

  misReservas() {
    return this.api.misReservas();
  }

  obtenerMiReserva(guid: string) {
    return this.api.obtenerMiReserva(guid);
  }

  cancelarReserva(guid: string, motivo: string) {
    return this.api.cancelarReserva(guid, motivo);
  }
}
