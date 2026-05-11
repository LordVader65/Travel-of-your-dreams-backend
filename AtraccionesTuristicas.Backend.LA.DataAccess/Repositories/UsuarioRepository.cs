using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class UsuarioRepository : RepositoryBase<UsuarioEntity>, IUsuarioRepository
{
    public UsuarioRepository(AtraccionesDbContext context) : base(context) { }

    public async Task<IReadOnlyList<UsuarioEntity>> ListarConRolesYClientesAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Include(x => x.UsuarioRoles).ThenInclude(x => x.Rol)
            .Include(x => x.Clientes)
            .OrderBy(x => x.usu_login)
            .ToListAsync(cancellationToken);

    public Task<UsuarioEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking()
            .Include(x => x.UsuarioRoles).ThenInclude(x => x.Rol)
            .Include(x => x.Clientes)
            .FirstOrDefaultAsync(x => x.usu_guid == guid, cancellationToken);

    public Task<UsuarioEntity?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.usu_login == login, cancellationToken);

    public Task<UsuarioEntity?> ObtenerConRolesAsync(string login, CancellationToken cancellationToken = default) =>
        DbSet.Include(x => x.UsuarioRoles).ThenInclude(x => x.Rol)
            .Include(x => x.Clientes)
            .FirstOrDefaultAsync(x => x.usu_login == login, cancellationToken);

    public Task<UsuarioEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.usu_guid == guid, cancellationToken);
}
