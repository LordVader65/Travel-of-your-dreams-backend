import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class AdminAtraccionesApiService {
  constructor(private readonly api: ApiService) {}

  adminAtracciones() {
    return this.api.adminAtracciones();
  }

  adminCrearAtraccion(request: unknown) {
    return this.api.adminCrearAtraccion(request);
  }

  adminActualizarAtraccion(guid: string, request: unknown) {
    return this.api.adminActualizarAtraccion(guid, request);
  }

  adminEliminarAtraccion(guid: string) {
    return this.api.adminEliminarAtraccion(guid);
  }

  catalogo(nombre: 'destinos' | 'categorias' | 'idiomas' | 'imagenes' | 'incluye') {
    return this.api.catalogo(nombre);
  }
}
