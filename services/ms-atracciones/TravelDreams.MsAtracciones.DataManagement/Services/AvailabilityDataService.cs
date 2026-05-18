using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAtracciones.DataAccess.Context;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Availability;

namespace TravelDreams.MsAtracciones.DataManagement.Services;

public sealed class AvailabilityDataService : IAvailabilityDataService
{
    private readonly AtraccionesDbContext _db;

    public AvailabilityDataService(AtraccionesDbContext db)
    {
        _db = db;
    }

    public async Task<AvailabilityResultDataModel> ReserveAvailabilityAsync(Guid horarioGuid, IReadOnlyList<AvailabilityLineDataModel> lines, CancellationToken cancellationToken = default)
    {
        if (lines.Count == 0 || lines.Any(x => x.Cantidad <= 0))
        {
            return Failure("La reserva debe incluir tickets con cantidades positivas.");
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var horario = await _db.Horarios
            .Include(x => x.Atraccion)
            .FirstOrDefaultAsync(x => x.hor_guid == horarioGuid && x.hor_estado == "A", cancellationToken);

        if (horario is null || horario.Atraccion is null || horario.Atraccion.at_estado != "A" || !horario.Atraccion.at_disponible)
        {
            return Failure("Horario activo no encontrado.");
        }

        if (horario.hor_fecha < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return Failure("El horario ya no esta vigente.");
        }

        var ticketGuids = lines.Select(x => x.TicketGuid).ToArray();
        var tickets = await _db.Tickets
            .Where(x => ticketGuids.Contains(x.tck_guid) && x.tck_estado == "A")
            .ToListAsync(cancellationToken);

        if (tickets.Count != ticketGuids.Distinct().Count())
        {
            return Failure("Todos los tickets deben existir y estar activos.");
        }

        if (tickets.Any(x => x.at_id != horario.at_id))
        {
            return Failure("Todos los tickets deben pertenecer a la atraccion del horario.");
        }

        foreach (var line in lines)
        {
            var ticket = tickets.First(x => x.tck_guid == line.TicketGuid);
            if (line.Cantidad > ticket.tck_capacidad_maxima)
            {
                return Failure($"La cantidad excede la capacidad maxima del ticket {ticket.tck_titulo}.");
            }
        }

        var cantidadTotal = lines.Sum(x => x.Cantidad);
        if (horario.hor_cupos_disponibles < cantidadTotal)
        {
            return Failure($"Cupos insuficientes. Disponibles: {horario.hor_cupos_disponibles}, solicitados: {cantidadTotal}.");
        }

        horario.hor_cupos_disponibles -= cantidadTotal;
        horario.hor_fecha_mod = DateTime.UtcNow;
        horario.hor_usuario_mod = "ms-reservas";
        horario.hor_ip_mod = "internal";

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new AvailabilityResultDataModel
        {
            Success = true,
            AtraccionGuid = horario.Atraccion.at_guid,
            HorarioGuid = horario.hor_guid,
            CuposRestantes = horario.hor_cupos_disponibles
        };
    }

    public async Task<AvailabilityResultDataModel> ReleaseAvailabilityAsync(Guid horarioGuid, int cantidad, CancellationToken cancellationToken = default)
    {
        if (cantidad <= 0)
        {
            return Failure("La cantidad a liberar debe ser positiva.");
        }

        var horario = await _db.Horarios
            .Include(x => x.Atraccion)
            .FirstOrDefaultAsync(x => x.hor_guid == horarioGuid, cancellationToken);

        if (horario is null)
        {
            return Failure("Horario no encontrado.");
        }

        horario.hor_cupos_disponibles += cantidad;
        horario.hor_fecha_mod = DateTime.UtcNow;
        horario.hor_usuario_mod = "ms-reservas";
        horario.hor_ip_mod = "internal";

        await _db.SaveChangesAsync(cancellationToken);

        return new AvailabilityResultDataModel
        {
            Success = true,
            AtraccionGuid = horario.Atraccion?.at_guid,
            HorarioGuid = horario.hor_guid,
            CuposRestantes = horario.hor_cupos_disponibles
        };
    }

    private static AvailabilityResultDataModel Failure(string error) => new()
    {
        Success = false,
        Error = error
    };
}
