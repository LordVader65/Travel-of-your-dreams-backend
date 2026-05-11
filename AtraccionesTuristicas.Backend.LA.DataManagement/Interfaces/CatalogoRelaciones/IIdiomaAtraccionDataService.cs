using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;

public interface IIdiomaAtraccionDataService
{
    Task<IReadOnlyList<IdiomaAtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IdiomaAtraccionDataModel> CrearAsync(IdiomaAtraccionDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task RemoverAsync(int atraccionId, int idiomaId, CancellationToken cancellationToken = default);
}
