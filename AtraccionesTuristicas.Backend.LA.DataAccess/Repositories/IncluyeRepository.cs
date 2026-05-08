using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class IncluyeRepository : RepositoryBase<IncluyeEntity>, IIncluyeRepository
{
    public IncluyeRepository(AtraccionesDbContext context) : base(context) { }
}
