using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;

public interface IFacturaQueryRepository
{
    Task<FacturaEntity?> ObtenerPorNumeroAsync(string numero, CancellationToken cancellationToken = default);
    Task<PagedResult<FacturaEntity>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default);
    Task<PagedResult<FacturaEntity>> ListarAsync(
        Guid? clienteGuid,
        Guid? reservaGuid,
        string? numero,
        string? estado,
        DateTime? fechaDesdeUtc,
        DateTime? fechaHastaUtc,
        int page,
        int limit,
        CancellationToken cancellationToken = default);
}
