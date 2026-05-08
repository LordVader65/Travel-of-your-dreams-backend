using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class ReservaEstadoHistorialRepository : RepositoryBase<ReservaEstadoHistorialEntity>, IReservaEstadoHistorialRepository
{
    public ReservaEstadoHistorialRepository(AtraccionesDbContext context) : base(context) { }
}
