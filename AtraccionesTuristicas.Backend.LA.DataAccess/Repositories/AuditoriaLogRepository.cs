using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class AuditoriaLogRepository : RepositoryBase<AuditoriaLogEntity>, IAuditoriaLogRepository
{
    public AuditoriaLogRepository(AtraccionesDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AuditoriaLogEntity>> ConsultarPorTablaAsync(string tabla, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().Where(x => x.log_tabla == tabla).ToListAsync(cancellationToken);

    public async Task<long> RegistrarAuditoriaAsync(
        string tabla,
        string operacion,
        int? registroId,
        Guid? registroGuid,
        string? datosAnteriores,
        string? datosNuevos,
        string usuario,
        string ip,
        string? origenCanal = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new object[]
        {
            new NpgsqlParameter("p_tabla", NpgsqlDbType.Varchar) { Value = tabla },
            new NpgsqlParameter("p_operacion", NpgsqlDbType.Varchar) { Value = operacion },
            new NpgsqlParameter("p_registro_id", NpgsqlDbType.Integer) { Value = (object?)registroId ?? DBNull.Value },
            new NpgsqlParameter("p_registro_guid", NpgsqlDbType.Uuid) { Value = (object?)registroGuid ?? DBNull.Value },
            new NpgsqlParameter("p_datos_anteriores", NpgsqlDbType.Text) { Value = (object?)datosAnteriores ?? DBNull.Value },
            new NpgsqlParameter("p_datos_nuevos", NpgsqlDbType.Text) { Value = (object?)datosNuevos ?? DBNull.Value },
            new NpgsqlParameter("p_usuario", NpgsqlDbType.Varchar) { Value = usuario },
            new NpgsqlParameter("p_ip", NpgsqlDbType.Varchar) { Value = ip },
            new NpgsqlParameter("p_origen_canal", NpgsqlDbType.Varchar) { Value = (object?)origenCanal ?? DBNull.Value }
        };

        return await Context.Database
            .SqlQueryRaw<long>(
                """
                SELECT fn_registrar_auditoria(
                    @p_tabla,
                    @p_operacion,
                    @p_registro_id,
                    @p_registro_guid,
                    @p_datos_anteriores,
                    @p_datos_nuevos,
                    @p_usuario,
                    @p_ip,
                    @p_origen_canal
                ) AS "Value"
                """,
                parameters)
            .SingleAsync(cancellationToken);
    }
}
