using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class IdiomaRepository : RepositoryBase<IdiomaEntity>, IIdiomaRepository
{
    public IdiomaRepository(AtraccionesDbContext context) : base(context) { }

    public Task<IdiomaEntity?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.id_codigo == codigo, cancellationToken);
}
