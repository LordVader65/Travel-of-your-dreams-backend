import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class AdminReservasApiService {
  constructor(private readonly api: ApiService) {}

  adminClientes() {
    return this.api.adminClientes();
  }

  adminReservas() {
    return this.api.adminReservas();
  }

  adminReserva(guid: string) {
    return this.api.adminReserva(guid);
  }

  cambiarEstadoReserva(guid: string, estado: string, observacion?: string) {
    return this.api.cambiarEstadoReserva(guid, estado, observacion);
  }

  expirarReservasPendientes() {
    return this.api.expirarReservasPendientes();
  }
}
