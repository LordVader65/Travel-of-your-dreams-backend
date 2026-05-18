using TravelDreams.MsAuditoria.DataManagement.Models;

namespace TravelDreams.MsAuditoria.DataManagement.Interfaces;

public interface IAuditoriaLogDataService
{
    Task<PagedResult<AuditoriaLogDataModel>> ConsultarAsync(AuditoriaFiltroDataModel filtro, CancellationToken ct = default);
    Task<IReadOnlyList<AuditoriaLogDataModel>> ConsultarPorTablaAsync(string tabla, CancellationToken ct = default);
    Task<AuditoriaLogDataModel?> ObtenerAsync(Guid guid, CancellationToken ct = default);
    Task<long> RegistrarAsync(AuditoriaLogDataModel model, CancellationToken ct = default);
    Task<bool> EventoProcesadoAsync(Guid eventoId, CancellationToken ct = default);
}
