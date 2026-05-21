import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiListResponse, ApiResponse } from '../../shared/models/api-response.model';
import { AtraccionDetalle, AtraccionPublica, HorarioDisponible } from '../../shared/models/atraccion.model';
import { CrearReservaRequest, Reserva } from '../../shared/models/reserva.model';
import { LoginRequest, LoginResponse } from '../../shared/models/auth.model';

type QueryValue = string | number | boolean | null | undefined;
const sessionKey = 'toyd_session';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequest) {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/auth/login`, this.toRequestBody(request));
  }

  register(request: unknown) {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/auth/register`, this.toRequestBody(request));
  }

  logoutBackend() {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/auth/logout`, {});
  }

  adminLogin(request: LoginRequest) {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/admin/auth/login`, this.toRequestBody(request));
  }

  listarAtracciones(params: Record<string, QueryValue> = {}) {
    return this.http.get<ApiListResponse<AtraccionPublica>>(`${this.baseUrl}/atracciones`, {
      params: this.toParams(params)
    });
  }

  obtenerAtraccion(guid: string) {
    return this.http.get<ApiResponse<AtraccionDetalle>>(`${this.baseUrl}/atracciones/${guid}`);
  }

  filtrosAtracciones(ciudad?: string) {
    return this.http.get<ApiResponse<unknown>>(`${this.baseUrl}/atracciones/filtros`, {
      params: this.toParams({ ciudad })
    });
  }

  listarHorarios(atraccionGuid: string, fecha?: string) {
    return this.http.get<ApiResponse<HorarioDisponible[]>>(`${this.baseUrl}/atracciones/${atraccionGuid}/horarios`, {
      params: this.toParams({ fecha })
    });
  }

  previsualizarReserva(request: CrearReservaRequest) {
    return of({
      status: 200,
      message: 'La previsualizacion se calcula localmente en el frontend.',
      data: null
    } as ApiResponse<unknown>);
  }

  crearReserva(request: CrearReservaRequest) {
    return this.http.post<ApiResponse<Reserva>>(`${this.baseUrl}/reservas`, this.toRequestBody(request));
  }

  misReservas() {
    return this.http.get<ApiListResponse<Reserva>>(`${this.baseUrl}/reservas`);
  }

  obtenerMiReserva(guid: string) {
    return this.http.get<ApiResponse<Reserva>>(`${this.baseUrl}/reservas/${guid}`);
  }

  cancelarReserva(guid: string, motivo: string) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/reservas/${guid}/cancelar`, this.toRequestBody({ motivo }));
  }

  miPerfil() {
    return this.http.get<ApiResponse<unknown>>(`${this.baseUrl}/me`);
  }

  actualizarMiPerfil(request: unknown) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/me`, this.toRequestBody(request));
  }

  cambiarMiPassword(request: unknown) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/me/password`, this.toRequestBody(request));
  }

  misDatosFacturacion() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/me/datos-facturacion`, {
      params: this.toParams({ clienteGuid: this.currentClienteGuid() })
    });
  }

  crearDatosFacturacion(request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/me/datos-facturacion`, this.toRequestBody({
      clienteGuid: this.currentClienteGuid(),
      ...(request as Record<string, unknown>)
    }));
  }

  actualizarDatosFacturacion(guid: string, request: unknown) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/me/datos-facturacion/${guid}`, this.toRequestBody(request));
  }

  eliminarDatosFacturacion(guid: string) {
    return this.http.delete(`${this.baseUrl}/me/datos-facturacion/${guid}`);
  }

  misPagos(params: Record<string, QueryValue> = {}) {
    return this.http.get<ApiListResponse<unknown>>(`${this.baseUrl}/pagos`, { params: this.toParams(params) });
  }

  confirmarPago(request: unknown) {
    const body = { ...(request as Record<string, unknown>) };
    const reservaGuid = String(body['reservaGuid'] ?? body['reserva_guid'] ?? '');
    delete body['reservaGuid'];
    delete body['reserva_guid'];
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/reservas/${reservaGuid}/confirmar-pago`, this.toRequestBody(body));
  }

  misFacturas(params: Record<string, QueryValue> = {}) {
    return this.http.get<ApiListResponse<unknown>>(`${this.baseUrl}/facturas/mis-facturas`, { params: this.toParams(params) });
  }

  miFactura(guid: string) {
    return this.http.get<ApiResponse<unknown>>(`${this.baseUrl}/facturas/${guid}`);
  }

  resenias() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/resenias`);
  }

  crearResenia(request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/resenias`, this.toRequestBody(request));
  }

  adminClientes() {
    return this.http.get<ApiListResponse<unknown>>(`${this.baseUrl}/admin/clientes`);
  }

  adminCrearClienteExterno(request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/clientes`, this.toRequestBody(request));
  }

  adminCliente(guid: string) {
    return this.http.get<ApiResponse<unknown>>(`${this.baseUrl}/admin/clientes/${guid}`);
  }

  cambiarEstadoCliente(guid: string, estado: string) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/clientes/${guid}/estado`, this.toRequestBody({ estado }));
  }

  adminDatosFacturacionCliente(guid: string) {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/clientes/${guid}/datos-facturacion`);
  }

  adminCrearDatosFacturacionCliente(guid: string, request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/clientes/${guid}/datos-facturacion`, this.toRequestBody(request));
  }

  adminReservas() {
    return this.http.get<ApiListResponse<Reserva>>(`${this.baseUrl}/admin/reservas`);
  }

  adminReserva(guid: string) {
    return this.http.get<ApiResponse<Reserva>>(`${this.baseUrl}/admin/reservas/${guid}`);
  }

  adminCrearReserva(request: unknown) {
    return this.http.post<ApiResponse<Reserva>>(`${this.baseUrl}/reservas`, this.toRequestBody(request));
  }

  cambiarEstadoReserva(guid: string, estado: string, observacion?: string) {
    return this.http.put(`${this.baseUrl}/admin/reservas/${guid}/estado`, this.toRequestBody({ estado, observacion }));
  }

  expirarReservasPendientes() {
    return this.http.post<ApiResponse<{ total: number }>>(`${this.baseUrl}/admin/reservas/expirar-pendientes`, {});
  }

  adminAtracciones() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/atracciones`);
  }

  adminCrearAtraccion(request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/atracciones`, this.toRequestBody(request));
  }

  adminActualizarAtraccion(guid: string, request: unknown) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/atracciones/${guid}`, this.toRequestBody(request));
  }

  adminEliminarAtraccion(guid: string) {
    return this.http.delete(`${this.baseUrl}/admin/atracciones/${guid}`);
  }

  asociarCategoriaAtraccion(atraccionId: number, categoriaId: number) {
    return this.operacionNoImplementada('La asociacion directa de categorias se administra desde el backend al guardar la atraccion.');
  }

  desasociarCategoriaAtraccion(atraccionId: number, categoriaId: number) {
    return this.operacionNoImplementada('La desasociacion directa de categorias no esta disponible en microservicios.');
  }

  asociarIdiomaAtraccion(atraccionId: number, idiomaId: number) {
    return this.operacionNoImplementada('La asociacion directa de idiomas se administra desde el backend al guardar la atraccion.');
  }

  desasociarIdiomaAtraccion(atraccionId: number, idiomaId: number) {
    return this.operacionNoImplementada('La desasociacion directa de idiomas no esta disponible en microservicios.');
  }

  asociarImagenAtraccion(atraccionId: number, imagenId: number, esPrincipal = false, orden = 1) {
    return this.operacionNoImplementada('La asociacion directa de imagenes se administra desde el backend al guardar la atraccion.');
  }

  desasociarImagenAtraccion(atraccionId: number, imagenId: number) {
    return this.operacionNoImplementada('La desasociacion directa de imagenes no esta disponible en microservicios.');
  }

  asociarIncluyeAtraccion(atraccionId: number, incluyeId: number) {
    return this.operacionNoImplementada('La asociacion directa de incluye/no incluye se administra desde el backend al guardar la atraccion.');
  }

  desasociarIncluyeAtraccion(atraccionId: number, incluyeId: number) {
    return this.operacionNoImplementada('La desasociacion directa de incluye/no incluye no esta disponible en microservicios.');
  }

  catalogo(nombre: 'destinos' | 'categorias' | 'idiomas' | 'imagenes' | 'incluye') {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/catalogos/${nombre}`);
  }

  crearCatalogo(nombre: 'destinos' | 'categorias' | 'idiomas' | 'imagenes' | 'incluye', request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/catalogos/${nombre}`, this.toRequestBody(request));
  }

  actualizarCatalogo(nombre: 'destinos' | 'categorias' | 'idiomas' | 'imagenes' | 'incluye', id: number, request: unknown) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/catalogos/${nombre}/${id}`, this.toRequestBody(request));
  }

  eliminarCatalogo(nombre: 'destinos' | 'categorias' | 'idiomas' | 'imagenes' | 'incluye', id: number) {
    return this.http.delete(`${this.baseUrl}/admin/catalogos/${nombre}/${id}`);
  }

  adminHorarios() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/horarios`);
  }

  crearHorario(request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/horarios`, this.toRequestBody(request));
  }

  actualizarHorario(guid: string, request: unknown) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/horarios/${guid}`, this.toRequestBody(request));
  }

  cambiarEstadoHorario(guid: string, estado: string) {
    return this.operacionNoImplementada('El cambio rapido de estado de horarios no esta expuesto; usa editar horario o eliminar.');
  }

  desactivarHorariosVencidos() {
    return of({ status: 200, data: { total: 0 } } as ApiResponse<{ total: number }>);
  }

  adminTickets() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/tickets`);
  }

  crearTicket(request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/tickets`, this.toRequestBody(request));
  }

  actualizarTicket(guid: string, request: unknown) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/tickets/${guid}`, this.toRequestBody(request));
  }

  eliminarTicket(guid: string) {
    return this.http.delete(`${this.baseUrl}/admin/tickets/${guid}`);
  }

  adminResenias() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/resenias`);
  }

  cambiarEstadoResenia(guid: string, estado: string) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/resenias/${guid}/estado`, {}, {
      params: this.toParams({ estado })
    });
  }

  adminPagos(params: Record<string, QueryValue> = {}) {
    return this.http.get<ApiListResponse<unknown>>(`${this.baseUrl}/admin/pagos`, { params: this.toParams(params) });
  }

  adminFacturas(params: Record<string, QueryValue> = {}) {
    return this.http.get<ApiListResponse<unknown>>(`${this.baseUrl}/admin/facturas`, { params: this.toParams(params) });
  }

  generarFactura(request: unknown) {
    return this.http.post<ApiResponse<{ factura_guid: string }>>(`${this.baseUrl}/admin/facturas`, this.toRequestBody(request));
  }

  auditoria(params: Record<string, QueryValue> = {}) {
    return this.http.get<ApiListResponse<unknown>>(`${this.baseUrl}/admin/auditoria`, { params: this.toParams(params) });
  }

  adminUsuarios() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/usuarios`);
  }

  crearUsuario(request: unknown) {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/admin/usuarios`, this.toRequestBody(request));
  }

  obtenerUsuario(guid: string) {
    return this.http.get<ApiResponse<unknown>>(`${this.baseUrl}/admin/usuarios/${guid}`);
  }

  cambiarEstadoUsuario(guid: string, estado: string) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/usuarios/${guid}/estado`, this.toRequestBody({ estado }));
  }

  cambiarRolesUsuario(usuarioGuid: string, rolIds: number[]) {
    return this.http.put<ApiResponse<unknown>>(`${this.baseUrl}/admin/usuarios/${usuarioGuid}/roles`, this.toRequestBody({ rolIds }));
  }

  roles() {
    return this.http.get<ApiResponse<unknown[]>>(`${this.baseUrl}/admin/roles`);
  }

  private operacionNoImplementada(message: string) {
    return of({ status: 200, message, data: null } as ApiResponse<unknown>);
  }

  private currentClienteGuid() {
    const session = this.currentSession();
    return session?.cliente_guid ?? session?.clienteGuid ?? '';
  }

  private currentSession(): { cliente_guid?: string | null; clienteGuid?: string | null } | null {
    const raw = localStorage.getItem(sessionKey);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as { cliente_guid?: string | null; clienteGuid?: string | null };
    } catch {
      return null;
    }
  }

  private toParams(values: Record<string, QueryValue>) {
    let params = new HttpParams();
    Object.entries(values).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return params;
  }

  private toRequestBody(value: unknown): unknown {
    if (Array.isArray(value)) {
      return value.map((item) => this.toRequestBody(item));
    }

    if (!value || typeof value !== 'object' || value instanceof Date) {
      return value;
    }

    return Object.fromEntries(
      Object.entries(value as Record<string, unknown>).map(([key, item]) => [
        this.toSnakeCase(key),
        this.toRequestBody(item)
      ])
    );
  }

  private toSnakeCase(value: string) {
    return value
      .replace(/([A-Z]+)([A-Z][a-z])/g, '$1_$2')
      .replace(/([a-z0-9])([A-Z])/g, '$1_$2')
      .replace(/-/g, '_')
      .toLowerCase();
  }
}
