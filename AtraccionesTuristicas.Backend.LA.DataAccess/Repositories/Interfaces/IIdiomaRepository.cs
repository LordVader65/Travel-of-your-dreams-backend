using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IIdiomaRepository : IRepositoryBase<IdiomaEntity>
{
    Task<IdiomaEntity?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
}
