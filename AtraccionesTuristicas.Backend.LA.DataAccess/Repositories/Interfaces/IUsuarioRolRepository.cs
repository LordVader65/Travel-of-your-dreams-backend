using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IUsuarioRolRepository : IRepositoryBase<UsuarioRolEntity>
{
    Task<IReadOnlyList<UsuarioRolEntity>> ListarPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default);
}
