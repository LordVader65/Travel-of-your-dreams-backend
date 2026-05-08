using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class ReservaDetalleRepository : RepositoryBase<ReservaDetalleEntity>, IReservaDetalleRepository
{
    public ReservaDetalleRepository(AtraccionesDbContext context) : base(context) { }
}
