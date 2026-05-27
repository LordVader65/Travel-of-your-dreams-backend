using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAtracciones.DataAccess.Context;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Public;

namespace TravelDreams.MsAtracciones.DataManagement.Services;

public sealed class AtraccionesReadDataService : IAtraccionesReadDataService
{
    private readonly AtraccionesDbContext _db;

    public AtraccionesReadDataService(AtraccionesDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<AtraccionPublicaDataModel>> ListarAtraccionesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Atracciones
            .AsNoTracking()
            .Where(x => x.at_estado == "A" && x.at_disponible)
            .OrderBy(x => x.at_nombre)
            .Select(x => new AtraccionPublicaDataModel
            {
                Guid = x.at_guid,
                Nombre = x.at_nombre,
                Descripcion = x.at_descripcion,
                Ciudad = x.Destino != null ? x.Destino.des_nombre : null,
                Pais = x.Destino != null ? x.Destino.des_pais : null,
                PrecioReferencia = x.at_precio_referencia,
                Disponible = x.at_disponible,
                TotalResenias = x.at_total_resenias
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AtraccionPublicaDataModel?> ObtenerAtraccionAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _db.Atracciones
            .AsNoTracking()
            .Where(x => x.at_guid == guid && x.at_estado == "A")
            .Select(x => new AtraccionPublicaDataModel
            {
                Guid = x.at_guid,
                Nombre = x.at_nombre,
                Descripcion = x.at_descripcion,
                Ciudad = x.Destino != null ? x.Destino.des_nombre : null,
                Pais = x.Destino != null ? x.Destino.des_pais : null,
                PrecioReferencia = x.at_precio_referencia,
                Disponible = x.at_disponible,
                TotalResenias = x.at_total_resenias
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TicketPublicoDataModel>> ListarTicketsAsync(Guid atraccionGuid, CancellationToken cancellationToken = default)
    {
        return await _db.Tickets
            .AsNoTracking()
            .Where(x => x.tck_estado == "A" && x.Atraccion != null && x.Atraccion.at_guid == atraccionGuid && x.Atraccion.at_estado == "A")
            .OrderBy(x => x.tck_precio)
            .Select(x => new TicketPublicoDataModel
            {
                Guid = x.tck_guid,
                Titulo = x.tck_titulo,
                TipoParticipante = x.tck_tipo_participante,
                Precio = x.tck_precio,
                Moneda = x.tck_moneda,
                CapacidadMaxima = x.tck_capacidad_maxima
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HorarioPublicoDataModel>> ListarHorariosPorAtraccionAsync(Guid atraccionGuid, DateOnly? fecha = null, CancellationToken cancellationToken = default)
    {
        var now = EcuadorNow();
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        var query = _db.Horarios
            .AsNoTracking()
            .Where(x => x.hor_estado == "A"
                && x.hor_cupos_disponibles > 0
                && (x.hor_fecha > today || x.hor_fecha == today && x.hor_hora_inicio > currentTime)
                && x.Atraccion != null
                && x.Atraccion.at_guid == atraccionGuid
                && x.Atraccion.at_estado == "A"
                && x.Atraccion.at_disponible);

        if (fecha.HasValue)
        {
            query = query.Where(x => x.hor_fecha == fecha.Value);
        }

        return await query
            .OrderBy(x => x.hor_fecha)
            .ThenBy(x => x.hor_hora_inicio)
            .Select(x => new HorarioPublicoDataModel
            {
                Guid = x.hor_guid,
                AtraccionGuid = x.Atraccion!.at_guid,
                Fecha = x.hor_fecha,
                HoraInicio = x.hor_hora_inicio,
                HoraFin = x.hor_hora_fin,
                CuposDisponibles = x.hor_cupos_disponibles
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HorarioPublicoDataModel>> ListarHorariosPorTicketAsync(Guid ticketGuid, CancellationToken cancellationToken = default)
    {
        var atraccionGuid = await _db.Tickets
            .AsNoTracking()
            .Where(x => x.tck_guid == ticketGuid && x.tck_estado == "A" && x.Atraccion != null)
            .Select(x => x.Atraccion!.at_guid)
            .FirstOrDefaultAsync(cancellationToken);

        return atraccionGuid == Guid.Empty
            ? []
            : await ListarHorariosPorAtraccionAsync(atraccionGuid, null, cancellationToken);
    }

    private static DateTime EcuadorNow()
    {
        try
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
        }
    }
}
