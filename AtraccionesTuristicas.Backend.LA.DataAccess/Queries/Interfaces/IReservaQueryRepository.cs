using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;

public interface IReservaQueryRepository
{
    Task<ReservaEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<ReservaEntity?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<PagedResult<ReservaEntity>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default);
    Task<PagedResult<ReservaEntity>> ListarAsync(
        Guid? clienteGuid,
        Guid? atraccionGuid,
        string? codigo,
        string? estado,
        DateOnly? fechaDesde,
        DateOnly? fechaHasta,
        int page,
        int limit,
        CancellationToken cancellationToken = default);
}
