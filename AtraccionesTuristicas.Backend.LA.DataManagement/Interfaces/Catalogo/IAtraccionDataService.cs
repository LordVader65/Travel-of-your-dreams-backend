using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;

public interface IAtraccionDataService
{
    Task<IReadOnlyList<AtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<AtraccionDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<AtraccionDataModel> CrearAsync(AtraccionDataModel model, CancellationToken cancellationToken = default);
    Task<AtraccionDataModel?> ActualizarAsync(AtraccionDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<DataPagedResult<AtraccionPublicaDataModel>> ListarPublicasAsync(AtraccionFiltroDataModel filtro, CancellationToken cancellationToken = default);
    Task<AtraccionPublicaDataModel?> ObtenerDetallePublicoAsync(Guid guid, CancellationToken cancellationToken = default);
}
