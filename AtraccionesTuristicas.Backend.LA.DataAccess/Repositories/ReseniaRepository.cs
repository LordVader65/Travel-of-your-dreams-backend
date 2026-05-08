using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class ReseniaRepository : RepositoryBase<ReseniaEntity>, IReseniaRepository
{
    public ReseniaRepository(AtraccionesDbContext context) : base(context) { }
}
