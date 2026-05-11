using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IImagenAtraccionRepository : IRepositoryBase<ImagenAtraccionEntity>
{
    Task<ImagenAtraccionEntity?> ObtenerPorRelacionAsync(int atraccionId, int imagenId, CancellationToken cancellationToken = default);
}
