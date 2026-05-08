using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IUsuarioRepository : IRepositoryBase<UsuarioEntity>
{
    Task<UsuarioEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<UsuarioEntity?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<UsuarioEntity?> ObtenerConRolesAsync(string login, CancellationToken cancellationToken = default);
    Task<UsuarioEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default);
}
