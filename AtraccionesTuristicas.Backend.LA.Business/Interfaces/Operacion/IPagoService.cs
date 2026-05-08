namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;

public interface IPagoService { Task<PagoResponse> ObtenerPorGuidAsync(Guid guid, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessPagedResult<PagoResponse>> ListarAsync(PagoFiltroRequest filtro, CurrentUserData user, CancellationToken cancellationToken = default); Task<Guid> ConfirmarPagoAsync(CrearPagoRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<PagoConfirmadoResponse> ConfirmarPagoYGenerarFacturaAsync(CrearPagoRequest request, Guid? datosFacturacionGuid, CurrentUserData user, CancellationToken cancellationToken = default); }

