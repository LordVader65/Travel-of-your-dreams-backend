using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface ICategoriaRepository : IRepositoryBase<CategoriaEntity>
{
    Task<CategoriaEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
}
