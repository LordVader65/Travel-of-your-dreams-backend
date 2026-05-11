using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IIdiomaAtraccionRepository : IRepositoryBase<IdiomaAtraccionEntity>
{
    Task<IdiomaAtraccionEntity?> ObtenerPorRelacionAsync(int atraccionId, int idiomaId, CancellationToken cancellationToken = default);
}
