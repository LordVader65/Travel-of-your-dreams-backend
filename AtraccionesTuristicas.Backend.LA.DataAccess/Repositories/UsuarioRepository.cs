using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class UsuarioRepository : RepositoryBase<UsuarioEntity>, IUsuarioRepository
{
    public UsuarioRepository(AtraccionesDbContext context) : base(context) { }

    public Task<UsuarioEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.usu_guid == guid, cancellationToken);

    public Task<UsuarioEntity?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.usu_login == login, cancellationToken);

    public Task<UsuarioEntity?> ObtenerConRolesAsync(string login, CancellationToken cancellationToken = default) =>
        DbSet.Include(x => x.UsuarioRoles).ThenInclude(x => x.Rol)
            .FirstOrDefaultAsync(x => x.usu_login == login, cancellationToken);

    public Task<UsuarioEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.usu_guid == guid, cancellationToken);
}
