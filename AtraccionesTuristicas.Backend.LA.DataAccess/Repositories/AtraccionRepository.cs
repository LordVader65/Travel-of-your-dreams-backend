using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class AtraccionRepository : RepositoryBase<AtraccionEntity>, IAtraccionRepository
{
    public AtraccionRepository(AtraccionesDbContext context) : base(context) { }

    public Task<AtraccionEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.at_guid == guid, cancellationToken);

    public Task<AtraccionEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.at_guid == guid, cancellationToken);

    public async Task<IReadOnlyList<AtraccionEntity>> ListarActivasAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().Where(x => x.at_estado == DatabaseConstants.EstadoActivo).ToListAsync(cancellationToken);
}
