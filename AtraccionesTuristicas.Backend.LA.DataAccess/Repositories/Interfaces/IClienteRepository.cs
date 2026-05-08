using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IClienteRepository : IRepositoryBase<ClienteEntity>
{
    Task<ClienteEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default);
}
