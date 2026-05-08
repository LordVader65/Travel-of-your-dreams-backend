using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class TicketRepository : RepositoryBase<TicketEntity>, ITicketRepository
{
    public TicketRepository(AtraccionesDbContext context) : base(context) { }

    public Task<TicketEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.tck_guid == guid, cancellationToken);

    public async Task<IReadOnlyList<TicketEntity>> ListarActivosPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(x => x.at_id == atraccionId && x.tck_estado == DatabaseConstants.EstadoActivo)
            .ToListAsync(cancellationToken);
}
