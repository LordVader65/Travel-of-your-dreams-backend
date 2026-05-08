using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class PagoRepository : RepositoryBase<PagoEntity>, IPagoRepository
{
    public PagoRepository(AtraccionesDbContext context) : base(context) { }

    public Task<PagoEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.pag_guid == guid, cancellationToken);

    public async Task<IReadOnlyList<PagoEntity>> ObtenerPorReservaIdAsync(int reservaId, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().Where(x => x.rev_id == reservaId).ToListAsync(cancellationToken);

    public async Task<Guid> ConfirmarPagoAsync(
        Guid reservaGuid,
        string metodo,
        decimal monto,
        string referencia,
        string usuario,
        string ip,
        string? origenCanal = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new object[]
        {
            new NpgsqlParameter("p_rev_guid", NpgsqlDbType.Uuid) { Value = reservaGuid },
            new NpgsqlParameter("p_metodo", NpgsqlDbType.Varchar) { Value = metodo },
            new NpgsqlParameter("p_monto", NpgsqlDbType.Numeric) { Value = monto },
            new NpgsqlParameter("p_referencia", NpgsqlDbType.Varchar) { Value = referencia },
            new NpgsqlParameter("p_usuario", NpgsqlDbType.Varchar) { Value = usuario },
            new NpgsqlParameter("p_ip", NpgsqlDbType.Varchar) { Value = ip },
            new NpgsqlParameter("p_origen_canal", NpgsqlDbType.Varchar) { Value = (object?)origenCanal ?? DBNull.Value }
        };

        return await Context.Database
            .SqlQueryRaw<Guid>(
                """
                SELECT fn_confirmar_pago(
                    @p_rev_guid,
                    @p_metodo,
                    @p_monto,
                    @p_referencia,
                    @p_usuario,
                    @p_ip,
                    @p_origen_canal
                ) AS "Value"
                """,
                parameters)
            .SingleAsync(cancellationToken);
    }
}
