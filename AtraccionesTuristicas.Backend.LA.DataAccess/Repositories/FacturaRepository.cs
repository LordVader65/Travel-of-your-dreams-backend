using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class FacturaRepository : RepositoryBase<FacturaEntity>, IFacturaRepository
{
    public FacturaRepository(AtraccionesDbContext context) : base(context) { }

    public Task<FacturaEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.fac_guid == guid, cancellationToken);

    public Task<FacturaEntity?> ObtenerPorReservaIdAsync(int reservaId, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.rev_id == reservaId, cancellationToken);

    public Task<FacturaEntity?> ObtenerPorPagoIdAsync(int pagoId, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.pag_id == pagoId, cancellationToken);

    public async Task<Guid> GenerarFacturaAsync(
        Guid reservaGuid,
        Guid? datosFacturacionGuid,
        string usuario,
        string ip,
        string? observacion = null,
        string? origenCanal = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new object[]
        {
            new NpgsqlParameter("p_rev_guid", NpgsqlDbType.Uuid) { Value = reservaGuid },
            new NpgsqlParameter("p_dfac_guid", NpgsqlDbType.Uuid) { Value = (object?)datosFacturacionGuid ?? DBNull.Value },
            new NpgsqlParameter("p_usuario", NpgsqlDbType.Varchar) { Value = usuario },
            new NpgsqlParameter("p_ip", NpgsqlDbType.Varchar) { Value = ip },
            new NpgsqlParameter("p_observacion", NpgsqlDbType.Varchar) { Value = (object?)observacion ?? DBNull.Value },
            new NpgsqlParameter("p_origen_canal", NpgsqlDbType.Varchar) { Value = (object?)origenCanal ?? DBNull.Value }
        };

        return await Context.Database
            .SqlQueryRaw<Guid>(
                """
                SELECT fn_generar_factura(
                    @p_rev_guid,
                    @p_dfac_guid,
                    @p_usuario,
                    @p_ip,
                    @p_observacion,
                    @p_origen_canal
                ) AS "Value"
                """,
                parameters)
            .SingleAsync(cancellationToken);
    }
}
