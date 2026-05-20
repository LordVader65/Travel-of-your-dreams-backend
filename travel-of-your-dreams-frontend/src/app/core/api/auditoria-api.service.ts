import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

type QueryValue = string | number | boolean | null | undefined;

@Injectable({ providedIn: 'root' })
export class AuditoriaApiService {
  constructor(private readonly api: ApiService) {}

  auditoria(params: Record<string, QueryValue> = {}) {
    return this.api.auditoria(params);
  }
}
