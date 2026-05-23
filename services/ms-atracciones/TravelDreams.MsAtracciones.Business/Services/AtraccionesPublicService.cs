using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Public;

namespace TravelDreams.MsAtracciones.Business.Services;

public sealed class AtraccionesPublicService : IAtraccionesPublicService
{
    private readonly IAtraccionesReadDataService _data;

    public AtraccionesPublicService(IAtraccionesReadDataService data)
    {
        _data = data;
    }

    public async Task<IReadOnlyList<AtraccionResponse>> ListarAtraccionesAsync(CancellationToken cancellationToken = default)
    {
        var atracciones = await _data.ListarAtraccionesAsync(cancellationToken);
        return atracciones.Select(Map).ToList();
    }

    public async Task<AtraccionResponse?> ObtenerAtraccionAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var atraccion = await _data.ObtenerAtraccionAsync(guid, cancellationToken);
        return atraccion is null ? null : Map(atraccion);
    }

    public async Task<IReadOnlyList<TicketResponse>> ListarTicketsAsync(Guid atraccionGuid, CancellationToken cancellationToken = default)
    {
        var tickets = await _data.ListarTicketsAsync(atraccionGuid, cancellationToken);
        return tickets.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<HorarioResponse>> ListarHorariosPorAtraccionAsync(Guid atraccionGuid, DateOnly? fecha = null, CancellationToken cancellationToken = default)
    {
        var horarios = await _data.ListarHorariosPorAtraccionAsync(atraccionGuid, fecha, cancellationToken);
        return horarios.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<HorarioResponse>> ListarHorariosPorTicketAsync(Guid ticketGuid, CancellationToken cancellationToken = default)
    {
        var horarios = await _data.ListarHorariosPorTicketAsync(ticketGuid, cancellationToken);
        return horarios.Select(Map).ToList();
    }

    private static AtraccionResponse Map(AtraccionPublicaDataModel model) => new()
    {
        Guid = model.Guid,
        Nombre = model.Nombre,
        Descripcion = model.Descripcion,
        Ciudad = model.Ciudad,
        Pais = model.Pais,
        PrecioReferencia = model.PrecioReferencia,
        Disponible = model.Disponible,
        TotalResenias = model.TotalResenias
    };

    private static TicketResponse Map(TicketPublicoDataModel model) => new()
    {
        Guid = model.Guid,
        Titulo = model.Titulo,
        TipoParticipante = model.TipoParticipante,
        Precio = model.Precio,
        Moneda = model.Moneda,
        CapacidadMaxima = model.CapacidadMaxima
    };

    private static HorarioResponse Map(HorarioPublicoDataModel model) => new()
    {
        Guid = model.Guid,
        AtraccionGuid = model.AtraccionGuid,
        Fecha = model.Fecha,
        HoraInicio = model.HoraInicio,
        HoraFin = model.HoraFin,
        CuposDisponibles = model.CuposDisponibles
    };
}
