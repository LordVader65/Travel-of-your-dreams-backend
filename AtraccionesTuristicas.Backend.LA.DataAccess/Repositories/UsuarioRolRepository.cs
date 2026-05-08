using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class UsuarioRolRepository : RepositoryBase<UsuarioRolEntity>, IUsuarioRolRepository
{
    public UsuarioRolRepository(AtraccionesDbContext context) : base(context) { }

    public async Task<IReadOnlyList<UsuarioRolEntity>> ListarPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default) =>
        await DbSet.Where(x => x.usu_id == usuarioId).ToListAsync(cancellationToken);
}
