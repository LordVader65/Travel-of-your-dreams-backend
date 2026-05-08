using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class ReservaRepository : RepositoryBase<ReservaEntity>, IReservaRepository
{
    public ReservaRepository(AtraccionesDbContext context) : base(context) { }

    public Task<ReservaEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.rev_guid == guid, cancellationToken);

    public Task<ReservaEntity?> ObtenerPorIdConReseniaAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking()
            .Include(x => x.Resenia)
            .FirstOrDefaultAsync(x => x.rev_id == id, cancellationToken);

    public Task<ReservaEntity?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.rev_codigo == codigo, cancellationToken);

    public Task<ReservaEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.rev_guid == guid, cancellationToken);

    public Task<ReservaEntity?> ObtenerConDetalleAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking()
            .Include(x => x.Detalles).ThenInclude(x => x.Ticket)
            .Include(x => x.Horario).ThenInclude(x => x!.Atraccion)
            .Include(x => x.Pagos)
            .Include(x => x.Factura)
            .FirstOrDefaultAsync(x => x.rev_guid == guid, cancellationToken);

    public async Task<Guid> CrearReservaAsync(
        Guid clienteGuid,
        Guid horarioGuid,
        string ticketsJson,
        string usuario,
        string ip,
        string? origenCanal = null,
        int expiracionMinutos = 15,
        decimal porcentajeIva = 0,
        CancellationToken cancellationToken = default)
    {
        var parameters = new object[]
        {
            new NpgsqlParameter("p_cli_guid", NpgsqlDbType.Uuid) { Value = clienteGuid },
            new NpgsqlParameter("p_hor_guid", NpgsqlDbType.Uuid) { Value = horarioGuid },
            new NpgsqlParameter("p_tickets", NpgsqlDbType.Jsonb) { Value = ticketsJson },
            new NpgsqlParameter("p_usuario", NpgsqlDbType.Varchar) { Value = usuario },
            new NpgsqlParameter("p_ip", NpgsqlDbType.Varchar) { Value = ip },
            new NpgsqlParameter("p_origen_canal", NpgsqlDbType.Varchar) { Value = (object?)origenCanal ?? DBNull.Value },
            new NpgsqlParameter("p_expiracion_minutos", NpgsqlDbType.Integer) { Value = expiracionMinutos },
            new NpgsqlParameter("p_porcentaje_iva", NpgsqlDbType.Numeric) { Value = porcentajeIva }
        };

        return await Context.Database
            .SqlQueryRaw<Guid>(
                """
                SELECT fn_crear_reserva(
                    @p_cli_guid,
                    @p_hor_guid,
                    @p_tickets,
                    @p_usuario,
                    @p_ip,
                    @p_origen_canal,
                    @p_expiracion_minutos,
                    @p_porcentaje_iva
                ) AS "Value"
                """,
                parameters)
            .SingleAsync(cancellationToken);
    }

    public async Task CancelarReservaAsync(
        Guid reservaGuid,
        string usuario,
        string ip,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        var parameters = new object[]
        {
            new NpgsqlParameter("p_rev_guid", NpgsqlDbType.Uuid) { Value = reservaGuid },
            new NpgsqlParameter("p_usuario", NpgsqlDbType.Varchar) { Value = usuario },
            new NpgsqlParameter("p_ip", NpgsqlDbType.Varchar) { Value = ip },
            new NpgsqlParameter("p_motivo", NpgsqlDbType.Varchar) { Value = motivo }
        };

        await Context.Database.ExecuteSqlRawAsync(
            "SELECT fn_cancelar_reserva(@p_rev_guid, @p_usuario, @p_ip, @p_motivo);",
            parameters,
            cancellationToken);
    }

    public async Task CambiarEstadoAsync(
        Guid reservaGuid,
        string nuevoEstado,
        string usuario,
        string ip,
        string? observacion = null,
        CancellationToken cancellationToken = default)
    {
        var reserva = await DbSet.FirstOrDefaultAsync(x => x.rev_guid == reservaGuid, cancellationToken);
        if (reserva is null)
        {
            throw new InvalidOperationException($"Reserva no encontrada: {reservaGuid}");
        }

        var estadoAnterior = reserva.rev_estado;
        reserva.rev_estado = nuevoEstado;
        reserva.rev_fecha_mod = DateTime.UtcNow;
        reserva.rev_usuario_mod = usuario;
        reserva.rev_ip_mod = ip;

        await Context.ReservaEstadoHistorial.AddAsync(new ReservaEstadoHistorialEntity
        {
            rev_id = reserva.rev_id,
            reh_estado_anterior = estadoAnterior,
            reh_estado_nuevo = nuevoEstado,
            reh_fecha_utc = DateTime.UtcNow,
            reh_usuario = usuario,
            reh_ip = ip,
            reh_observacion = observacion
        }, cancellationToken);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ExpirarReservasPendientesAsync(
        string usuario = "system",
        string ip = "127.0.0.1",
        CancellationToken cancellationToken = default)
    {
        var parameters = new object[]
        {
            new NpgsqlParameter("p_usuario", NpgsqlDbType.Varchar) { Value = usuario },
            new NpgsqlParameter("p_ip", NpgsqlDbType.Varchar) { Value = ip }
        };

        return await Context.Database
            .SqlQueryRaw<int>(
                """SELECT fn_expirar_reservas_pendientes(@p_usuario, @p_ip) AS "Value" """,
                parameters)
            .SingleAsync(cancellationToken);
    }
}
