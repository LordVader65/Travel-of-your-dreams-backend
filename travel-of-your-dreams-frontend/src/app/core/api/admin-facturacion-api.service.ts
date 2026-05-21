import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

type QueryValue = string | number | boolean | null | undefined;

@Injectable({ providedIn: 'root' })
export class AdminFacturacionApiService {
  constructor(private readonly api: ApiService) {}

  adminPagos(params: Record<string, QueryValue> = {}) {
    return this.api.adminPagos(params);
  }

  adminFacturas(params: Record<string, QueryValue> = {}) {
    return this.api.adminFacturas(params);
  }

  generarFactura(request: unknown) {
    return this.api.generarFactura(request);
  }
}
