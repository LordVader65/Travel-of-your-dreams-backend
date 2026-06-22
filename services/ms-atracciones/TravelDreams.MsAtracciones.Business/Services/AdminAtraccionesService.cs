using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Events.V3;
using TravelDreams.MsAtracciones.Business.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Admin;

namespace TravelDreams.MsAtracciones.Business.Services;

public sealed class AdminAtraccionesService : IAdminAtraccionesService
{
    private readonly IAdminAtraccionesDataService _data;
    private readonly IAtraccionesEventPublisherV3 _events;

    public AdminAtraccionesService(IAdminAtraccionesDataService data, IAtraccionesEventPublisherV3 events)
    {
        _data = data;
        _events = events;
    }

    public Task<object> ListarCatalogoAsync(string tipo, CancellationToken ct = default) => tipo.ToLowerInvariant() switch
    {
        "destinos" => Wrap(_data.ListarDestinosAsync(ct)),
        "categorias" => Wrap(_data.ListarCategoriasAsync(ct)),
        "idiomas" => Wrap(_data.ListarIdiomasAsync(ct)),
        "imagenes" => Wrap(_data.ListarImagenesAsync(ct)),
        "incluye" => Wrap(_data.ListarIncluyeAsync(ct)),
        _ => throw new InvalidOperationException("Catalogo no soportado.")
    };

    public async Task<object> GuardarCatalogoAsync(string tipo, int? id, AdminCatalogoRequest request, CancellationToken ct = default)
    {
        var model = new CatalogoUpsertDataModel
        {
            Id = id,
            Nombre = request.Nombre,
            Codigo = FirstNotBlank(request.Codigo, request.TagName),
            Descripcion = request.Descripcion,
            Tipo = request.Tipo,
            Pais = request.Pais,
            ImagenUrl = FirstNotBlank(request.ImagenUrl, request.Url),
            ParentId = request.ParentId
        };

        var normalized = tipo.ToLowerInvariant();
        var result = normalized switch
        {
            "destinos" => Wrap(_data.GuardarDestinoAsync(model, ct)),
            "categorias" => Wrap(_data.GuardarCategoriaAsync(model, ct)),
            "idiomas" => Wrap(_data.GuardarIdiomaAsync(model, ct)),
            "imagenes" => Wrap(_data.GuardarImagenAsync(model, ct)),
            "incluye" => Wrap(_data.GuardarIncluyeAsync(model, ct)),
            _ => throw new InvalidOperationException("Catalogo no soportado.")
        };

        var data = await result;
        await PublishCatalogoEventAsync(data, normalized, id.HasValue ? "actualizado" : "creado", ct);
        return data;
    }

    public async Task<bool> DesactivarCatalogoAsync(string tipo, int id, CancellationToken ct = default)
    {
        var result = await _data.DesactivarCatalogoAsync(tipo, id, "admin", ct);
        if (result)
        {
            await PublishAsync("catalogo.v3.desactivado", "catalogo.v3.desactivado", new AtraccionesEventPayloadV3
            {
                Entidad = "catalogo",
                Accion = "desactivado",
                CatalogoTipo = tipo.ToLowerInvariant(),
                Id = id,
                Estado = "I"
            }, ct);
        }

        return result;
    }

    public async Task<object> ListarAtraccionesAsync(CancellationToken ct = default) => await _data.ListarAtraccionesAsync(ct);
    public async Task<object?> ObtenerAtraccionAsync(Guid guid, CancellationToken ct = default) => await _data.ObtenerAtraccionAsync(guid, ct);
    public async Task<object> GuardarAtraccionAsync(Guid? guid, AdminAtraccionRequest request, CancellationToken ct = default)
    {
        ValidateAtraccion(request);

        var result = await _data.GuardarAtraccionAsync(new AtraccionUpsertDataModel { Guid = guid, DestinoId = request.DestinoId, NumeroEstablecimiento = request.NumeroEstablecimiento, Nombre = request.Nombre.Trim(), Descripcion = request.Descripcion, Direccion = request.Direccion, DuracionMinutos = request.DuracionMinutos, PuntoEncuentro = request.PuntoEncuentro, PrecioReferencia = request.PrecioReferencia, IncluyeAcompaniante = request.IncluyeAcompaniante, IncluyeTransporte = request.IncluyeTransporte, Disponible = request.Disponible, FreeCancellation = request.FreeCancellation, SkipTheLine = request.SkipTheLine, Usuario = "admin" }, ct);
        var eventType = guid.HasValue ? "atraccion.v3.actualizada" : "atraccion.v3.creada";
        await PublishAsync(eventType, eventType, new AtraccionesEventPayloadV3
        {
            Entidad = "atraccion",
            Accion = guid.HasValue ? "actualizada" : "creada",
            Guid = result.Guid,
            Id = result.Id,
            AtraccionGuid = result.Guid,
            AtraccionId = result.Id,
            Nombre = result.Nombre,
            Estado = result.Estado,
            Snapshot = result
        }, ct);
        return result;
    }

    public async Task<bool> DesactivarAtraccionAsync(Guid guid, CancellationToken ct = default)
    {
        var result = await _data.DesactivarAtraccionAsync(guid, "admin", ct);
        if (result)
        {
            await PublishAsync("atraccion.v3.desactivada", "atraccion.v3.desactivada", new AtraccionesEventPayloadV3
            {
                Entidad = "atraccion",
                Accion = "desactivada",
                Guid = guid,
                AtraccionGuid = guid,
                Estado = "I"
            }, ct);
        }

        return result;
    }

    public async Task<object> ListarTicketsAsync(CancellationToken ct = default) => await _data.ListarTicketsAsync(ct);
    public async Task<object> GuardarTicketAsync(Guid? guid, AdminTicketRequest request, CancellationToken ct = default)
    {
        var result = await _data.GuardarTicketAsync(new TicketUpsertDataModel { Guid = guid, AtraccionId = request.AtraccionId, Titulo = request.Titulo, Precio = request.Precio, Moneda = request.Moneda, TipoParticipante = request.TipoParticipante, CapacidadMaxima = request.CapacidadMaxima, Usuario = "admin" }, ct);
        var eventType = guid.HasValue ? "ticket.v3.actualizado" : "ticket.v3.creado";
        await PublishAsync(eventType, eventType, new AtraccionesEventPayloadV3
        {
            Entidad = "ticket",
            Accion = guid.HasValue ? "actualizado" : "creado",
            Guid = result.Guid,
            Id = result.Id,
            TicketGuid = result.Guid,
            AtraccionId = result.AtraccionId,
            Nombre = result.Titulo,
            Estado = result.Estado,
            Snapshot = result
        }, ct);
        return result;
    }

    public async Task<bool> DesactivarTicketAsync(Guid guid, CancellationToken ct = default)
    {
        var result = await _data.DesactivarTicketAsync(guid, "admin", ct);
        if (result)
        {
            await PublishAsync("ticket.v3.desactivado", "ticket.v3.desactivado", new AtraccionesEventPayloadV3
            {
                Entidad = "ticket",
                Accion = "desactivado",
                Guid = guid,
                TicketGuid = guid,
                Estado = "I"
            }, ct);
        }

        return result;
    }

    public async Task<object> ListarHorariosAsync(CancellationToken ct = default) => await _data.ListarHorariosAsync(ct);
    public async Task<object> GuardarHorarioAsync(Guid? guid, AdminHorarioRequest request, CancellationToken ct = default)
    {
        ValidateHorario(request, guid.HasValue);

        var model = new HorarioUpsertDataModel
        {
            Guid = guid,
            AtraccionId = request.AtraccionId,
            Fecha = request.Fecha,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin,
            CuposDisponibles = request.CuposDisponibles,
            DiasSemana = NormalizeDiasSemana(request.DiasSemana),
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Usuario = "admin"
        };

        object result = request.Fecha.HasValue
            ? await _data.GuardarHorarioAsync(model, ct)
            : await _data.GuardarReglaYGenerarHorariosAsync(model, ct);

        if (result is HorarioAdminDataModel horario)
        {
            var eventType = guid.HasValue ? "horario.v3.actualizado" : "horario.v3.creado";
            await PublishAsync(eventType, eventType, new AtraccionesEventPayloadV3
            {
                Entidad = "horario",
                Accion = guid.HasValue ? "actualizado" : "creado",
                Guid = horario.Guid,
                Id = horario.Id,
                HorarioGuid = horario.Guid,
                AtraccionId = horario.AtraccionId,
                CuposRestantes = horario.CuposDisponibles,
                Estado = horario.Estado,
                Snapshot = horario
            }, ct);
        }
        else if (result is HorarioGeneracionDataModel generacion)
        {
            await PublishAsync("horario.v3.generado", "horario.v3.generado", new AtraccionesEventPayloadV3
            {
                Entidad = "horario",
                Accion = "generado",
                Guid = generacion.Regla.Guid,
                Id = generacion.Regla.Id,
                AtraccionId = generacion.Regla.AtraccionId,
                CuposRestantes = generacion.Regla.Cupos,
                Cantidad = generacion.Generados,
                Estado = generacion.Regla.Estado,
                Snapshot = generacion
            }, ct);
        }

        return result;
    }

    public async Task<bool> DesactivarHorarioAsync(Guid guid, CancellationToken ct = default)
    {
        var result = await _data.DesactivarHorarioAsync(guid, "admin", ct);
        if (result)
        {
            await PublishAsync("horario.v3.desactivado", "horario.v3.desactivado", new AtraccionesEventPayloadV3
            {
                Entidad = "horario",
                Accion = "desactivado",
                Guid = guid,
                HorarioGuid = guid,
                Estado = "I"
            }, ct);
        }

        return result;
    }

    public async Task<int> DesactivarHorariosVencidosAsync(CancellationToken ct = default)
    {
        var total = await _data.DesactivarHorariosVencidosAsync("admin", ct);
        if (total > 0)
        {
            await PublishAsync("horario.v3.vencidos_desactivados", "horario.v3.vencidos_desactivados", new AtraccionesEventPayloadV3
            {
                Entidad = "horario",
                Accion = "vencidos_desactivados",
                Cantidad = total,
                Estado = "I"
            }, ct);
        }

        return total;
    }

    public async Task<object> ListarReseniasAsync(CancellationToken ct = default) => await _data.ListarReseniasAsync(ct);
    public async Task<object> ListarReseniasPorAtraccionAsync(Guid atraccionGuid, CancellationToken ct = default) =>
        await _data.ListarReseniasPorAtraccionAsync(atraccionGuid, ct);

    public async Task<object> CrearReseniaAsync(CrearReseniaRequest request, CancellationToken ct = default)
    {
        var rating = request.Calificacion.HasValue
            ? (short)request.Calificacion.Value
            : request.Rating;

        if (rating is < 1 or > 5) throw new InvalidOperationException("La calificacion debe estar entre 1 y 5.");

        var reservaGuid = request.ReservaGuid == Guid.Empty
            ? Guid.NewGuid()
            : request.ReservaGuid;

        var usuario = request.ClienteId.HasValue
            ? $"booking:{request.ClienteId.Value}"
            : "cliente";

        var result = await _data.CrearReseniaAsync(new CrearReseniaDataModel
        {
            ClienteId = request.ClienteId,
            AtraccionGuid = request.AtraccionGuid,
            ReservaGuid = reservaGuid,
            Comentario = request.Comentario,
            Rating = rating,
            Usuario = usuario
        }, ct);
        await PublishAsync("resenia.v3.creada", "resenia.v3.creada", new AtraccionesEventPayloadV3
        {
            Entidad = "resenia",
            Accion = "creada",
            Guid = result.Guid,
            Id = result.AtraccionId,
            AtraccionGuid = result.AtraccionGuid,
            AtraccionId = result.AtraccionId,
            Estado = result.Estado,
            Snapshot = result
        }, ct);
        return result;
    }

    public async Task<bool> CambiarEstadoReseniaAsync(Guid guid, string estado, CancellationToken ct = default)
    {
        var result = await _data.CambiarEstadoReseniaAsync(guid, estado, "admin", ct);
        if (result)
        {
            await PublishAsync("resenia.v3.estado_actualizado", "resenia.v3.estado_actualizado", new AtraccionesEventPayloadV3
            {
                Entidad = "resenia",
                Accion = "estado_actualizado",
                Guid = guid,
                Estado = estado
            }, ct);
        }

        return result;
    }

    private static async Task<object> Wrap<T>(Task<T> task) where T : notnull => await task;

    private Task PublishCatalogoEventAsync(object data, string tipo, string accion, CancellationToken ct)
    {
        var item = data as CatalogoItemDataModel;
        return PublishAsync($"catalogo.v3.{accion}", $"catalogo.v3.{accion}", new AtraccionesEventPayloadV3
        {
            Entidad = "catalogo",
            Accion = accion,
            CatalogoTipo = tipo,
            Guid = item?.Guid,
            Id = item?.Id,
            Nombre = item?.Nombre,
            Estado = item?.Estado,
            Snapshot = data
        }, ct);
    }

    private Task PublishAsync(string eventType, string routingKey, AtraccionesEventPayloadV3 payload, CancellationToken ct) =>
        _events.PublishAsync(new AtraccionesV3Event { EventType = eventType, Payload = payload }, routingKey, ct);

    private static string? FirstNotBlank(params string?[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim();

    private static void ValidateAtraccion(AdminAtraccionRequest request)
    {
        if (request.DestinoId <= 0) throw new InvalidOperationException("Destino es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Nombre)) throw new InvalidOperationException("Nombre de atraccion es obligatorio.");
        if (request.DuracionMinutos.HasValue && request.DuracionMinutos <= 0) throw new InvalidOperationException("Duracion minutos debe ser mayor a cero.");
        if (request.PrecioReferencia.HasValue && request.PrecioReferencia < 0) throw new InvalidOperationException("Precio referencia no puede ser negativo.");
    }

    private static void ValidateHorario(AdminHorarioRequest request, bool isUpdate)
    {
        if (request.AtraccionId <= 0) throw new InvalidOperationException("Atraccion es obligatoria.");
        if (request.CuposDisponibles <= 0) throw new InvalidOperationException("Cupos debe ser mayor a cero.");
        if (request.HoraFin.HasValue && request.HoraFin.Value <= request.HoraInicio)
        {
            throw new InvalidOperationException("Hora fin debe ser mayor a hora inicio.");
        }

        if (request.Fecha.HasValue)
        {
            return;
        }

        if (isUpdate) throw new InvalidOperationException("Fecha es obligatoria para actualizar un horario puntual.");

        var fechaInicio = request.FechaInicio ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var fechaFin = request.FechaFin ?? fechaInicio.AddDays(89);
        if (fechaFin < fechaInicio) throw new InvalidOperationException("Fecha fin debe ser mayor o igual a fecha inicio.");
        if (fechaFin.DayNumber - fechaInicio.DayNumber > 179) throw new InvalidOperationException("El rango maximo de generacion es 180 dias.");
        NormalizeDiasSemana(request.DiasSemana);
        request.FechaInicio = fechaInicio;
        request.FechaFin = fechaFin;
    }

    private static string NormalizeDiasSemana(string? value)
    {
        var days = (value ?? string.Empty)
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(day => int.TryParse(day, out var parsed) ? parsed : -1)
            .Distinct()
            .Order()
            .ToArray();

        if (days.Length == 0) throw new InvalidOperationException("Debe seleccionar al menos un dia de atencion.");
        if (days.Any(day => day is < 0 or > 6)) throw new InvalidOperationException("Dias de semana debe contener valores entre 0 y 6.");
        return string.Join(',', days);
    }
}
