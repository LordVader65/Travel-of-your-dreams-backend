using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class RolRepository : RepositoryBase<RolEntity>, IRolRepository
{
    public RolRepository(AtraccionesDbContext context) : base(context) { }

    public Task<RolEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.rol_guid == guid, cancellationToken);

    public Task<RolEntity?> ObtenerPorDescripcionAsync(string descripcion, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.rol_descripcion == descripcion, cancellationToken);
}
