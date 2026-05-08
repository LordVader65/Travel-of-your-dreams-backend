using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IRolRepository : IRepositoryBase<RolEntity>
{
    Task<RolEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<RolEntity?> ObtenerPorDescripcionAsync(string descripcion, CancellationToken cancellationToken = default);
}
