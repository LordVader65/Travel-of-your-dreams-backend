using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class DatosFacturacionRepository : RepositoryBase<DatosFacturacionEntity>, IDatosFacturacionRepository
{
    public DatosFacturacionRepository(AtraccionesDbContext context) : base(context) { }

    public Task<DatosFacturacionEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.dfac_guid == guid, cancellationToken);

    public async Task<IReadOnlyList<DatosFacturacionEntity>> ListarActivosPorClienteAsync(Guid clienteGuid, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Include(x => x.Cliente)
            .Where(x => x.Cliente != null && x.Cliente.cli_guid == clienteGuid && x.dfac_estado == "A")
            .OrderByDescending(x => x.dfac_fecha_ingreso)
            .ToListAsync(cancellationToken);

    public Task<DatosFacturacionEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.dfac_guid == guid, cancellationToken);
}
