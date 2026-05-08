using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Auditoria;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;

public interface IAuditoriaLogQueryRepository
{
    Task<PagedResult<AuditoriaLogEntity>> ConsultarAsync(string? tabla, string? operacion, string? usuario, int page, int limit, CancellationToken cancellationToken = default);
}
