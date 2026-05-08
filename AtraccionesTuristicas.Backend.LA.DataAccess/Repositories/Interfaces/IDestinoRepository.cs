using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IDestinoRepository : IRepositoryBase<DestinoEntity>
{
    Task<DestinoEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
}
