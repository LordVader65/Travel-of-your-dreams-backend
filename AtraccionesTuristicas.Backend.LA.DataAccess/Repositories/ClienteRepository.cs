using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class ClienteRepository : RepositoryBase<ClienteEntity>, IClienteRepository
{
    public ClienteRepository(AtraccionesDbContext context) : base(context) { }

    public Task<ClienteEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.cli_guid == guid, cancellationToken);

    public Task<ClienteEntity?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.cli_numero_identificacion == numeroIdentificacion, cancellationToken);

    public Task<ClienteEntity?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.usu_id == usuarioId, cancellationToken);

    public Task<ClienteEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.cli_guid == guid, cancellationToken);
}
