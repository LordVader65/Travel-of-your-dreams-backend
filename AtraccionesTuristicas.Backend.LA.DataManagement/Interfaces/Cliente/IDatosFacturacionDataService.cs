using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Cliente;

public interface IDatosFacturacionDataService
{
    Task<IReadOnlyList<DatosFacturacionDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<DatosFacturacionDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DatosFacturacionDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DatosFacturacionDataModel>> ListarActivosPorClienteAsync(Guid clienteGuid, CancellationToken cancellationToken = default);
    Task<DatosFacturacionDataModel> CrearAsync(DatosFacturacionDataModel model, CancellationToken cancellationToken = default);
    Task<DatosFacturacionDataModel> ActualizarAsync(DatosFacturacionDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
