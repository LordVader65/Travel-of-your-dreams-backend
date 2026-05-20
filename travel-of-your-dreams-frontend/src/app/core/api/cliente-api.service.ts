import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ClienteApiService {
  constructor(private readonly api: ApiService) {}

  miPerfil() {
    return this.api.miPerfil();
  }

  actualizarMiPerfil(request: unknown) {
    return this.api.actualizarMiPerfil(request);
  }

  cambiarMiPassword(request: unknown) {
    return this.api.cambiarMiPassword(request);
  }

  misDatosFacturacion() {
    return this.api.misDatosFacturacion();
  }

  crearDatosFacturacion(request: unknown) {
    return this.api.crearDatosFacturacion(request);
  }

  actualizarDatosFacturacion(guid: string, request: unknown) {
    return this.api.actualizarDatosFacturacion(guid, request);
  }

  eliminarDatosFacturacion(guid: string) {
    return this.api.eliminarDatosFacturacion(guid);
  }
}
