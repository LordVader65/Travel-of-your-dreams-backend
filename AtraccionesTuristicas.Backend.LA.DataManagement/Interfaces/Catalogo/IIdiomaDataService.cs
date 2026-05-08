using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;

public interface IIdiomaDataService
{
    Task<IReadOnlyList<IdiomaDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IdiomaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IdiomaDataModel?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<IdiomaDataModel> CrearAsync(IdiomaDataModel model, CancellationToken cancellationToken = default);
    Task<IdiomaDataModel> ActualizarAsync(IdiomaDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
