using TravelDreams.MsAuditoria.Business.DTOs;

namespace TravelDreams.MsAuditoria.Business.Interfaces;

public interface IAuditoriaLogService
{
    Task<PagedResponse<AuditoriaLogResponse>> ConsultarAsync(AuditoriaLogFiltroRequest filtro, CancellationToken ct = default);
    Task<IReadOnlyList<AuditoriaLogResponse>> ConsultarPorTablaAsync(string tabla, CancellationToken ct = default);
    Task<AuditoriaLogResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default);
    Task<long> RegistrarAsync(RegistrarAuditoriaRequest request, CancellationToken ct = default);
}
