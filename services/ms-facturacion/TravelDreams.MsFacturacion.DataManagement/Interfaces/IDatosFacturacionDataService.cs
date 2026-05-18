using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.DataManagement.Interfaces;

public interface IDatosFacturacionDataService
{
    Task<IReadOnlyList<DatosFacturacionDataModel>> ListarActivosPorClienteAsync(Guid clienteGuid, CancellationToken ct = default);
    Task<DatosFacturacionDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default);
    Task<DatosFacturacionDataModel> GuardarAsync(DatosFacturacionDataModel model, string usuario, string ip, CancellationToken ct = default);
    Task<bool> InactivarAsync(Guid guid, string usuario, string ip, CancellationToken ct = default);
}
