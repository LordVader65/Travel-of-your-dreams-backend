using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Auditoria;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Auditoria;

public interface IAuditoriaLogDataService
{
    Task<IReadOnlyList<AuditoriaLogDataModel>> ConsultarPorTablaAsync(string tabla, CancellationToken cancellationToken = default);
    Task<DataPagedResult<AuditoriaLogDataModel>> ConsultarAsync(string? tabla, string? operacion, string? usuario, DateTime? desdeUtc, DateTime? hastaUtc, int page, int limit, CancellationToken cancellationToken = default);
    Task<long> RegistrarAsync(AuditoriaLogDataModel model, CancellationToken cancellationToken = default);
}
