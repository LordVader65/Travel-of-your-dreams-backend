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
    public async Task<object> GuardarAtraccionAsync(Guid? guid, AdminAtraccionRequest request, CancellationToken ct = default) =>
        await _data.GuardarAtraccionAsync(new AtraccionUpsertDataModel { Guid = guid, DestinoId = request.DestinoId, Nombre = request.Nombre, Descripcion = request.Descripcion, Direccion = request.Direccion, DuracionMinutos = request.DuracionMinutos, PuntoEncuentro = request.PuntoEncuentro, PrecioReferencia = request.PrecioReferencia, IncluyeAcompaniante = request.IncluyeAcompaniante, IncluyeTransporte = request.IncluyeTransporte, Disponible = request.Disponible, FreeCancellation = request.FreeCancellation, SkipTheLine = request.SkipTheLine, Usuario = "admin" }, ct);

    public Task<bool> DesactivarAtraccionAsync(Guid guid, CancellationToken ct = default) => _data.DesactivarAtraccionAsync(guid, "admin", ct);

    public async Task<object> ListarTicketsAsync(CancellationToken ct = default) => await _data.ListarTicketsAsync(ct);
    public async Task<object> GuardarTicketAsync(Guid? guid, AdminTicketRequest request, CancellationToken ct = default) =>
        await _data.GuardarTicketAsync(new TicketUpsertDataModel { Guid = guid, AtraccionId = request.AtraccionId, Titulo = request.Titulo, Precio = request.Precio, Moneda = request.Moneda, TipoParticipante = request.TipoParticipante, CapacidadMaxima = request.CapacidadMaxima, Usuario = "admin" }, ct);

    public Task<bool> DesactivarTicketAsync(Guid guid, CancellationToken ct = default) => _data.DesactivarTicketAsync(guid, "admin", ct);

    public async Task<object> ListarHorariosAsync(CancellationToken ct = default) => await _data.ListarHorariosAsync(ct);
    public async Task<object> GuardarHorarioAsync(Guid? guid, AdminHorarioRequest request, CancellationToken ct = default) =>
        await _data.GuardarHorarioAsync(new HorarioUpsertDataModel { Guid = guid, AtraccionId = request.AtraccionId, Fecha = request.Fecha, HoraInicio = request.HoraInicio, HoraFin = request.HoraFin, CuposDisponibles = request.CuposDisponibles, DiasSemana = request.DiasSemana, Usuario = "admin" }, ct);

    public Task<bool> DesactivarHorarioAsync(Guid guid, CancellationToken ct = default) => _data.DesactivarHorarioAsync(guid, "admin", ct);

    public async Task<object> ListarReseniasAsync(CancellationToken ct = default) => await _data.ListarReseniasAsync(ct);
    public async Task<object> CrearReseniaAsync(CrearReseniaRequest request, CancellationToken ct = default)
    {
        if (request.Rating is < 1 or > 5) throw new InvalidOperationException("La calificacion debe estar entre 1 y 5.");
        return await _data.CrearReseniaAsync(new CrearReseniaDataModel { AtraccionGuid = request.AtraccionGuid, ReservaGuid = request.ReservaGuid, Comentario = request.Comentario, Rating = request.Rating, Usuario = "cliente" }, ct);
    }

    public Task<bool> CambiarEstadoReseniaAsync(Guid guid, string estado, CancellationToken ct = default) =>
        _data.CambiarEstadoReseniaAsync(guid, estado, "admin", ct);

    private static async Task<object> Wrap<T>(Task<T> task) where T : notnull => await task;
}
