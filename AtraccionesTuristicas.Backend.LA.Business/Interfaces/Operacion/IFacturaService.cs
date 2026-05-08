namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;

public interface IFacturaService { Task<FacturaResponse> ObtenerPorGuidAsync(Guid guid, CurrentUserData user, CancellationToken cancellationToken = default); Task<FacturaResponse?> ObtenerPorNumeroAsync(string numero, CancellationToken cancellationToken = default); Task<BusinessPagedResult<FacturaResponse>> ListarAsync(FacturaFiltroRequest filtro, CurrentUserData user, CancellationToken cancellationToken = default); Task<Guid> GenerarAsync(Guid reservaGuid, Guid? datosFacturacionGuid, CurrentUserData user, string? observacion = null, string? origenCanal = null, CancellationToken cancellationToken = default); }

