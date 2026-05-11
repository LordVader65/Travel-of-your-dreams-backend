using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class IdiomaAtraccionRepository : RepositoryBase<IdiomaAtraccionEntity>, IIdiomaAtraccionRepository
{
    public IdiomaAtraccionRepository(AtraccionesDbContext context) : base(context) { }

    public Task<IdiomaAtraccionEntity?> ObtenerPorRelacionAsync(int atraccionId, int idiomaId, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.id_id == idiomaId, cancellationToken);
}
