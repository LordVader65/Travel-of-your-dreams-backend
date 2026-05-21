using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Admin;

namespace TravelDreams.MsAtracciones.Business.Services;

public sealed class AdminAtraccionesService : IAdminAtraccionesService
{
    private readonly IAdminAtraccionesDataService _data;

    public AdminAtraccionesService(IAdminAtraccionesDataService data) => _data = data;

    public Task<object> ListarCatalogoAsync(string tipo, CancellationToken ct = default) => tipo.ToLowerInvariant() switch
    {
        "destinos" => Wrap(_data.ListarDestinosAsync(ct)),
        "categorias" => Wrap(_data.ListarCategoriasAsync(ct)),
        "idiomas" => Wrap(_data.ListarIdiomasAsync(ct)),
        "imagenes" => Wrap(_data.ListarImagenesAsync(ct)),
        "incluye" => Wrap(_data.ListarIncluyeAsync(ct)),
        _ => throw new InvalidOperationException("Catalogo no soportado.")
    };

    public Task<object> GuardarCatalogoAsync(string tipo, int? id, AdminCatalogoRequest request, CancellationToken ct = default)
    {
        var model = new CatalogoUpsertDataModel
        {
            Id = id,
            Nombre = request.Nombre,
            Codigo = request.Codigo,
            Descripcion = request.Descripcion,
            Tipo = request.Tipo,
            Pais = request.Pais,
            ImagenUrl = request.ImagenUrl,
            ParentId = request.ParentId
        };

        return tipo.ToLowerInvariant() switch
        {
            "destinos" => Wrap(_data.GuardarDestinoAsync(model, ct)),
            "categorias" => Wrap(_data.GuardarCategoriaAsync(model, ct)),
            "idiomas" => Wrap(_data.GuardarIdiomaAsync(model, ct)),
            "imagenes" => Wrap(_data.GuardarImagenAsync(model, ct)),
            "incluye" => Wrap(_data.GuardarIncluyeAsync(model, ct)),
            _ => throw new InvalidOperationException("Catalogo no soportado.")
        };
    }

    public Task<bool> DesactivarCatalogoAsync(string tipo, int id, CancellationToken ct = default) =>
        _data.DesactivarCatalogoAsync(tipo, id, "admin", ct);

    public async Task<object> ListarAtraccionesAsync(CancellationToken ct = default) => await _data.ListarAtraccionesAsync(ct);
    public async Task<object?> ObtenerAtraccionAsync(Guid guid, CancellationToken ct = default) => await _data.ObtenerAtraccionAsync(guid, ct);
    public async Task<object> GuardarAtraccionAsync(Guid? guid, AdminAtraccionRequest request, CancellationToken ct = default)
    {
        ValidateAtraccion(request);

        return await _data.GuardarAtraccionAsync(new AtraccionUpsertDataModel { Guid = guid, DestinoId = request.DestinoId, NumeroEstablecimiento = request.NumeroEstablecimiento, Nombre = request.Nombre.Trim(), Descripcion = request.Descripcion, Direccion = request.Direccion, DuracionMinutos = request.DuracionMinutos, PuntoEncuentro = request.PuntoEncuentro, PrecioReferencia = request.PrecioReferencia, IncluyeAcompaniante = request.IncluyeAcompaniante, IncluyeTransporte = request.IncluyeTransporte, Disponible = request.Disponible, FreeCancellation = request.FreeCancellation, SkipTheLine = request.SkipTheLine, Usuario = "admin" }, ct);
    }

    public Task<bool> DesactivarAtraccionAsync(Guid guid, CancellationToken ct = default) => _data.DesactivarAtraccionAsync(guid, "admin", ct);

    public async Task<object> ListarTicketsAsync(CancellationToken ct = default) => await _data.ListarTicketsAsync(ct);
    public async Task<object> GuardarTicketAsync(Guid? guid, AdminTicketRequest request, CancellationToken ct = default) =>
        await _data.GuardarTicketAsync(new TicketUpsertDataModel { Guid = guid, AtraccionId = request.AtraccionId, Titulo = request.Titulo, Precio = request.Precio, Moneda = request.Moneda, TipoParticipante = request.TipoParticipante, CapacidadMaxima = request.CapacidadMaxima, Usuario = "admin" }, ct);

    public Task<bool> DesactivarTicketAsync(Guid guid, CancellationToken ct = default) => _data.DesactivarTicketAsync(guid, "admin", ct);

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

        return request.Fecha.HasValue
            ? await _data.GuardarHorarioAsync(model, ct)
            : await _data.GuardarReglaYGenerarHorariosAsync(model, ct);
    }

    public Task<bool> DesactivarHorarioAsync(Guid guid, CancellationToken ct = default) => _data.DesactivarHorarioAsync(guid, "admin", ct);
    public Task<int> DesactivarHorariosVencidosAsync(CancellationToken ct = default) => _data.DesactivarHorariosVencidosAsync("admin", ct);

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

        return await _data.CrearReseniaAsync(new CrearReseniaDataModel
        {
            ClienteId = request.ClienteId,
            AtraccionGuid = request.AtraccionGuid,
            ReservaGuid = reservaGuid,
            Comentario = request.Comentario,
            Rating = rating,
            Usuario = usuario
        }, ct);
    }

    public Task<bool> CambiarEstadoReseniaAsync(Guid guid, string estado, CancellationToken ct = default) =>
        _data.CambiarEstadoReseniaAsync(guid, estado, "admin", ct);

    private static async Task<object> Wrap<T>(Task<T> task) where T : notnull => await task;

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
