import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

type QueryValue = string | number | boolean | null | undefined;

@Injectable({ providedIn: 'root' })
export class FacturacionApiService {
  constructor(private readonly api: ApiService) {}

  misPagos(params: Record<string, QueryValue> = {}) {
    return this.api.misPagos(params);
  }

  confirmarPago(request: unknown) {
    return this.api.confirmarPago(request);
  }

  misFacturas(params: Record<string, QueryValue> = {}) {
    return this.api.misFacturas(params);
  }

  miFactura(guid: string) {
    return this.api.miFactura(guid);
  }
}
