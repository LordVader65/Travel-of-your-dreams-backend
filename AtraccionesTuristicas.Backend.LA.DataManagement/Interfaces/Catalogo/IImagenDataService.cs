using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;

public interface IImagenDataService
{
    Task<IReadOnlyList<ImagenDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ImagenDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ImagenDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<ImagenDataModel> CrearAsync(ImagenDataModel model, CancellationToken cancellationToken = default);
    Task<ImagenDataModel> ActualizarAsync(ImagenDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
