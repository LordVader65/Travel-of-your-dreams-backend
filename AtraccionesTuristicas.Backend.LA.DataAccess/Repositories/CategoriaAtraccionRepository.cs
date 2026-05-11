using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class CategoriaAtraccionRepository : RepositoryBase<CategoriaAtraccionEntity>, ICategoriaAtraccionRepository
{
    public CategoriaAtraccionRepository(AtraccionesDbContext context) : base(context) { }

    public Task<CategoriaAtraccionEntity?> ObtenerPorRelacionAsync(int atraccionId, int categoriaId, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.cat_id == categoriaId, cancellationToken);
}
