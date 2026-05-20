import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class AdminIdentidadApiService {
  constructor(private readonly api: ApiService) {}

  adminUsuarios() {
    return this.api.adminUsuarios();
  }

  crearUsuario(request: unknown) {
    return this.api.crearUsuario(request);
  }

  obtenerUsuario(guid: string) {
    return this.api.obtenerUsuario(guid);
  }

  cambiarEstadoUsuario(guid: string, estado: string) {
    return this.api.cambiarEstadoUsuario(guid, estado);
  }

  cambiarRolesUsuario(usuarioGuid: string, rolIds: number[]) {
    return this.api.cambiarRolesUsuario(usuarioGuid, rolIds);
  }

  roles() {
    return this.api.roles();
  }
}
