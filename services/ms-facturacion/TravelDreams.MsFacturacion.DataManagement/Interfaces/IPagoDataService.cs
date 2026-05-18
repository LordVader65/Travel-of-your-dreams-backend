using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.DataManagement.Interfaces;

public interface IPagoDataService
{
    Task<PagoDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default);
    Task<PagedResult<PagoDataModel>> ListarAsync(PagoFiltroDataModel filtro, CancellationToken ct = default);
}
