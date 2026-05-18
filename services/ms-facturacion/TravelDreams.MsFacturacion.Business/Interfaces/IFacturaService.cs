using TravelDreams.MsFacturacion.Business.DTOs;

namespace TravelDreams.MsFacturacion.Business.Interfaces;

public interface IFacturaService
{
    Task<PagedResponse<FacturaResponse>> ListarAsync(FacturaFiltroRequest filtro, CancellationToken ct = default);
    Task<FacturaResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default);
    Task<FacturaResponse?> ObtenerPorNumeroAsync(string numero, CancellationToken ct = default);
    Task<FacturaResponse> GenerarAsync(GenerarFacturaRequest request, CancellationToken ct = default);
}
