using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IAtraccionRepository : IRepositoryBase<AtraccionEntity>
{
    Task<AtraccionEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<AtraccionEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AtraccionEntity>> ListarActivasAsync(CancellationToken cancellationToken = default);
}
