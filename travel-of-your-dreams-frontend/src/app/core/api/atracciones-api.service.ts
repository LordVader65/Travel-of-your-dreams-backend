import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

type QueryValue = string | number | boolean | null | undefined;

@Injectable({ providedIn: 'root' })
export class AtraccionesApiService {
  constructor(private readonly api: ApiService) {}

  listarAtracciones(params: Record<string, QueryValue> = {}) {
    return this.api.listarAtracciones(params);
  }

  obtenerAtraccion(guid: string) {
    return this.api.obtenerAtraccion(guid);
  }

  filtrosAtracciones(ciudad?: string) {
    return this.api.filtrosAtracciones(ciudad);
  }

  listarHorarios(atraccionGuid: string, fecha?: string) {
    return this.api.listarHorarios(atraccionGuid, fecha);
  }

  resenias() {
    return this.api.resenias();
  }

  crearResenia(request: unknown) {
    return this.api.crearResenia(request);
  }
}
