using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;

public interface IImagenAtraccionDataService
{
    Task<IReadOnlyList<ImagenAtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ImagenAtraccionDataModel> CrearAsync(ImagenAtraccionDataModel model, CancellationToken cancellationToken = default);
    Task<ImagenAtraccionDataModel> GuardarAsync(ImagenAtraccionDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task RemoverAsync(int atraccionId, int imagenId, CancellationToken cancellationToken = default);
}
