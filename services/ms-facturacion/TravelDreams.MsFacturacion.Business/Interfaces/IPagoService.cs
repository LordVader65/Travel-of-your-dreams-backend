using TravelDreams.MsFacturacion.Business.DTOs;

namespace TravelDreams.MsFacturacion.Business.Interfaces;

public interface IPagoService
{
    Task<PagedResponse<PagoResponse>> ListarAsync(PagoFiltroRequest filtro, CancellationToken ct = default);
    Task<PagoResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default);
    Task<FacturaResponse> ConfirmarPagoYGenerarFacturaAsync(Guid reservaGuid, ConfirmarPagoRequest request, CancellationToken ct = default);
    Task<FacturaResponse> ConfirmarPagoConReceptorAsync(Guid reservaGuid, ConfirmarPagoReceptorRequest request, CancellationToken ct = default);
}
