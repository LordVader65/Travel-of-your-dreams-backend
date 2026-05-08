using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class IdiomaAtraccionRepository : RepositoryBase<IdiomaAtraccionEntity>, IIdiomaAtraccionRepository
{
    public IdiomaAtraccionRepository(AtraccionesDbContext context) : base(context) { }
}
