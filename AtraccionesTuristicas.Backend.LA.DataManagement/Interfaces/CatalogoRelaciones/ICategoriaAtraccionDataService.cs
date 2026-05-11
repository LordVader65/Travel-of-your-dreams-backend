using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;

public interface ICategoriaAtraccionDataService
{
    Task<IReadOnlyList<CategoriaAtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<CategoriaAtraccionDataModel> CrearAsync(CategoriaAtraccionDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task RemoverAsync(int atraccionId, int categoriaId, CancellationToken cancellationToken = default);
}
