using Microsoft.EntityFrameworkCore;
using TravelDreams.MsReservas.DataAccess.Common;
using TravelDreams.MsReservas.DataAccess.Context;
using TravelDreams.MsReservas.DataAccess.Entities;
using TravelDreams.MsReservas.DataManagement.Interfaces;
using TravelDreams.MsReservas.DataManagement.Models;

namespace TravelDreams.MsReservas.DataManagement.Services;

public sealed class ReservasDataService : IReservasDataService
{
    private readonly ReservasDbContext _db;

    public ReservasDataService(ReservasDbContext db) => _db = db;

    public async Task<ClienteDataModel> UpsertClienteAsync(ClienteDataModel model, CancellationToken ct = default)
    {
        var entity = model.Guid.HasValue
            ? await _db.Clientes.FirstOrDefaultAsync(x => x.cli_guid == model.Guid, ct)
            : await _db.Clientes.FirstOrDefaultAsync(x => x.cli_numero_identificacion == model.NumeroIdentificacion, ct);

        if (entity is null)
        {
            entity = new ClienteEntity
            {
                usu_guid = model.UsuarioGuid,
                cli_tipo_identificacion = model.TipoIdentificacion,
                cli_numero_identificacion = model.NumeroIdentificacion,
                cli_usuario_ingreso = "ms-reservas",
                cli_ip_ingreso = "internal"
            };
            _db.Clientes.Add(entity);
        }

        entity.cli_nombres = model.Nombres;
        entity.cli_apellidos = model.Apellidos;
        entity.cli_razon_social = model.RazonSocial;
        entity.cli_correo = model.Correo;
        entity.cli_telefono = model.Telefono;
        entity.cli_direccion = model.Direccion;
        entity.usu_guid = model.UsuarioGuid ?? entity.usu_guid;
        entity.cli_estado = DatabaseConstants.EstadoActivo;
        await _db.SaveChangesAsync(ct);

        model.Guid = entity.cli_guid;
        return model;
    }

    public async Task<IReadOnlyList<ReservaDataModel>> ListarAsync(Guid? clienteGuid, string? estado, CancellationToken ct = default)
    {
        var query = _db.Reservas.Include(x => x.Cliente).Include(x => x.Detalles).AsNoTracking();
        if (clienteGuid.HasValue) query = query.Where(x => x.Cliente != null && x.Cliente.cli_guid == clienteGuid);
        if (!string.IsNullOrWhiteSpace(estado)) query = query.Where(x => x.rev_estado == estado);
        return await query.OrderByDescending(x => x.rev_fecha_reserva_utc).Select(x => MapReserva(x)).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ReservaDataModel>> ListarPorCanalAsync(string origenCanal, string? estado, CancellationToken ct = default)
    {
        var normalized = origenCanal.Trim().ToUpperInvariant();
        var query = _db.Reservas.Include(x => x.Cliente).Include(x => x.Detalles).AsNoTracking()
            .Where(x => x.rev_origen_canal != null &&
                        x.rev_origen_canal.ToUpper() == normalized);

        if (!string.IsNullOrWhiteSpace(estado)) query = query.Where(x => x.rev_estado == estado);
        return await query.OrderByDescending(x => x.rev_fecha_reserva_utc).Select(x => MapReserva(x)).ToListAsync(ct);
    }

    public async Task<ReservaDataModel?> ObtenerAsync(Guid reservaGuid, CancellationToken ct = default)
    {
        var reserva = await _db.Reservas.Include(x => x.Cliente).Include(x => x.Detalles).AsNoTracking().FirstOrDefaultAsync(x => x.rev_guid == reservaGuid, ct);
        return reserva is null ? null : MapReserva(reserva);
    }

    public async Task<Guid> CrearAsync(CrearReservaDataModel model, CancellationToken ct = default)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(x => x.cli_guid == model.ClienteGuid && x.cli_estado == "A", ct)
            ?? throw new InvalidOperationException("Cliente activo no encontrado.");

        var pendiente = await _db.Reservas.AnyAsync(x => x.cli_id == cliente.cli_id && x.at_guid == model.AtraccionGuid && x.rev_estado == DatabaseConstants.ReservaPendiente, ct);
        if (pendiente) throw new InvalidOperationException("Ya existe una reserva pendiente para esta atraccion.");

        var subtotal = model.Lineas.Sum(x => x.PrecioUnitario * x.Cantidad);
        var iva = Math.Round(subtotal * model.PorcentajeIva / 100, 2, MidpointRounding.AwayFromZero);
        var codigo = $"REV-{Guid.NewGuid():N}"[..16].ToUpperInvariant();

        var reserva = new ReservaEntity
        {
            rev_codigo = codigo,
            cli_id = cliente.cli_id,
            at_guid = model.AtraccionGuid,
            hor_guid = model.HorarioGuid,
            rev_atraccion_nombre = model.AtraccionNombre,
            rev_hor_fecha = model.HorFecha,
            rev_hor_hora_inicio = model.HorHoraInicio,
            rev_hor_hora_fin = model.HorHoraFin,
            rev_fecha_reserva_utc = DateTime.UtcNow,
            rev_fecha_expiracion_utc = DateTime.UtcNow.AddMinutes(model.ExpiracionMinutos),
            rev_subtotal = subtotal,
            rev_valor_iva = iva,
            rev_total = subtotal + iva,
            rev_origen_canal = model.OrigenCanal,
            rev_usuario_ingreso = model.Usuario,
            rev_ip_ingreso = model.Ip,
            rev_estado = DatabaseConstants.ReservaPendiente,
            Detalles = model.Lineas.Select(x => new ReservaDetalleEntity
            {
                tck_guid = x.TicketGuid,
                rdet_ticket_titulo = x.TicketTitulo,
                rdet_tipo_participante = x.TipoParticipante,
                rdet_cantidad = x.Cantidad,
                rdet_precio_unit = x.PrecioUnitario,
                rdet_subtotal = x.PrecioUnitario * x.Cantidad,
                rdet_usuario_ingreso = model.Usuario,
                rdet_ip_ingreso = model.Ip
            }).ToList(),
            EstadoHistorial =
            [
                new ReservaEstadoHistorialEntity { reh_estado_nuevo = DatabaseConstants.ReservaPendiente, reh_usuario = model.Usuario, reh_ip = model.Ip, reh_observacion = "Reserva creada" }
            ]
        };

        _db.Reservas.Add(reserva);
        await _db.SaveChangesAsync(ct);
        return reserva.rev_guid;
    }

    public async Task<bool> CancelarAsync(Guid reservaGuid, string motivo, string usuario, string ip, CancellationToken ct = default)
    {
        var reserva = await _db.Reservas.Include(x => x.Detalles).FirstOrDefaultAsync(x => x.rev_guid == reservaGuid, ct);
        if (reserva is null) return false;
        if (reserva.rev_estado != DatabaseConstants.ReservaPendiente) throw new InvalidOperationException("Solo se puede cancelar una reserva pendiente.");
        reserva.rev_estado = DatabaseConstants.ReservaCancelada;
        reserva.rev_fecha_cancelacion = DateTime.UtcNow;
        reserva.rev_usuario_cancelacion = usuario;
        reserva.rev_ip_cancelacion = ip;
        reserva.rev_motivo_cancelacion = motivo;
        _db.ReservaEstadoHistorial.Add(new ReservaEstadoHistorialEntity { rev_id = reserva.rev_id, reh_estado_anterior = DatabaseConstants.ReservaPendiente, reh_estado_nuevo = DatabaseConstants.ReservaCancelada, reh_usuario = usuario, reh_ip = ip, reh_observacion = motivo });
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> ExpirarPendientesAsync(string usuario, string ip, Func<Guid, int, CancellationToken, Task> releaseAvailability, CancellationToken ct = default)
    {
        var pendientes = await _db.Reservas.Include(x => x.Detalles)
            .Where(x => x.rev_estado == DatabaseConstants.ReservaPendiente && x.rev_fecha_expiracion_utc <= DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var reserva in pendientes)
        {
            var cantidad = reserva.Detalles.Sum(x => x.rdet_cantidad);
            await releaseAvailability(reserva.hor_guid, cantidad, ct);
            reserva.rev_estado = DatabaseConstants.ReservaExpirada;
            reserva.rev_fecha_mod = DateTime.UtcNow;
            reserva.rev_usuario_mod = usuario;
            reserva.rev_ip_mod = ip;
            _db.ReservaEstadoHistorial.Add(new ReservaEstadoHistorialEntity { rev_id = reserva.rev_id, reh_estado_anterior = DatabaseConstants.ReservaPendiente, reh_estado_nuevo = DatabaseConstants.ReservaExpirada, reh_usuario = usuario, reh_ip = ip, reh_observacion = "Reserva expirada" });
        }

        await _db.SaveChangesAsync(ct);
        return pendientes.Count;
    }

    public async Task<bool> CambiarEstadoAsync(Guid reservaGuid, string estado, string usuario, string ip, string? observacion, CancellationToken ct = default)
    {
        var reserva = await _db.Reservas.FirstOrDefaultAsync(x => x.rev_guid == reservaGuid, ct);
        if (reserva is null) return false;
        var anterior = reserva.rev_estado;
        reserva.rev_estado = estado;
        reserva.rev_fecha_mod = DateTime.UtcNow;
        reserva.rev_usuario_mod = usuario;
        reserva.rev_ip_mod = ip;
        _db.ReservaEstadoHistorial.Add(new ReservaEstadoHistorialEntity { rev_id = reserva.rev_id, reh_estado_anterior = anterior, reh_estado_nuevo = estado, reh_usuario = usuario, reh_ip = ip, reh_observacion = observacion });
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static ReservaDataModel MapReserva(ReservaEntity x) => new()
    {
        Guid = x.rev_guid,
        Codigo = x.rev_codigo,
        ClienteGuid = x.Cliente?.cli_guid ?? Guid.Empty,
        AtraccionGuid = x.at_guid,
        HorarioGuid = x.hor_guid,
        AtraccionNombre = x.rev_atraccion_nombre,
        HorFecha = x.rev_hor_fecha,
        HorHoraInicio = x.rev_hor_hora_inicio,
        HorHoraFin = x.rev_hor_hora_fin,
        FechaReservaUtc = x.rev_fecha_reserva_utc,
        FechaExpiracionUtc = x.rev_fecha_expiracion_utc,
        Subtotal = x.rev_subtotal,
        ValorIva = x.rev_valor_iva,
        Total = x.rev_total,
        Moneda = x.rev_moneda,
        OrigenCanal = x.rev_origen_canal,
        Estado = x.rev_estado,
        Detalles = x.Detalles.Select(d => new ReservaDetalleDataModel { TicketGuid = d.tck_guid, TicketTitulo = d.rdet_ticket_titulo, TipoParticipante = d.rdet_tipo_participante, Cantidad = d.rdet_cantidad, PrecioUnitario = d.rdet_precio_unit, Subtotal = d.rdet_subtotal }).ToList()
    };
}
