using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class AtraccionIncluyeRepository : RepositoryBase<AtraccionIncluyeEntity>, IAtraccionIncluyeRepository
{
    public AtraccionIncluyeRepository(AtraccionesDbContext context) : base(context) { }

    public Task<AtraccionIncluyeEntity?> ObtenerPorRelacionAsync(int atraccionId, int incluyeId, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.inc_id == incluyeId, cancellationToken);
}
