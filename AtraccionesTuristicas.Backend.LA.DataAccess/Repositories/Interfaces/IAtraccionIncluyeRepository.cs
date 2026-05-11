using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IAtraccionIncluyeRepository : IRepositoryBase<AtraccionIncluyeEntity>
{
    Task<AtraccionIncluyeEntity?> ObtenerPorRelacionAsync(int atraccionId, int incluyeId, CancellationToken cancellationToken = default);
}
