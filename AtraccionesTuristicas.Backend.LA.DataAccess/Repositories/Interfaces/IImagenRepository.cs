using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IImagenRepository : IRepositoryBase<ImagenEntity>
{
    Task<ImagenEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
}
