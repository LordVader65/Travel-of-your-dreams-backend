using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;

public interface IIncluyeDataService
{
    Task<IReadOnlyList<IncluyeDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IncluyeDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IncluyeDataModel> CrearAsync(IncluyeDataModel model, CancellationToken cancellationToken = default);
    Task<IncluyeDataModel> ActualizarAsync(IncluyeDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
