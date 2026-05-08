using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;

public interface IPagoQueryRepository
{
    Task<PagedResult<PagoEntity>> ListarPorReservaAsync(Guid reservaGuid, int page, int limit, CancellationToken cancellationToken = default);
    Task<PagedResult<PagoEntity>> ListarAsync(
        Guid? reservaGuid,
        Guid? clienteGuid,
        string? metodo,
        string? estado,
        DateTime? fechaDesdeUtc,
        DateTime? fechaHastaUtc,
        int page,
        int limit,
        CancellationToken cancellationToken = default);
}
