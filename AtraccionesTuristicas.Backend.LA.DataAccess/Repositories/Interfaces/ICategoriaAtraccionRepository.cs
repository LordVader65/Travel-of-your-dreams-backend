using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface ICategoriaAtraccionRepository : IRepositoryBase<CategoriaAtraccionEntity>
{
    Task<CategoriaAtraccionEntity?> ObtenerPorRelacionAsync(int atraccionId, int categoriaId, CancellationToken cancellationToken = default);
}
