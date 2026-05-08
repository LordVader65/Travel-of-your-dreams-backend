using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;

public interface ICategoriaDataService
{
    Task<IReadOnlyList<CategoriaDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<CategoriaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoriaDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<CategoriaDataModel> CrearAsync(CategoriaDataModel model, CancellationToken cancellationToken = default);
    Task<CategoriaDataModel> ActualizarAsync(CategoriaDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
