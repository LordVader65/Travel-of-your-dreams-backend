using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;

public interface IAtraccionIncluyeDataService
{
    Task<IReadOnlyList<AtraccionIncluyeDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<AtraccionIncluyeDataModel> CrearAsync(AtraccionIncluyeDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
