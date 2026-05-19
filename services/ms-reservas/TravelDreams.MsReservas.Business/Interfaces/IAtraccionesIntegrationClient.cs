using TravelDreams.MsReservas.Business.DTOs;

namespace TravelDreams.MsReservas.Business.Interfaces;

public interface IAtraccionesIntegrationClient
{
    Task<IReadOnlyList<AtraccionTicketDto>> GetTicketsAsync(Guid atraccionGuid, CancellationToken ct = default);
    Task<AtraccionReservationContextDto> GetReservationContextAsync(Guid atraccionGuid, Guid horarioGuid, CancellationToken ct = default);
    Task ReserveAsync(Guid horarioGuid, IReadOnlyList<CrearReservaLineaRequest> lineas, CancellationToken ct = default);
    Task ReleaseAsync(Guid horarioGuid, int cantidad, CancellationToken ct = default);
}

public sealed class AtraccionTicketDto
{
    public Guid Guid { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public int CapacidadMaxima { get; set; }
}

public sealed class AtraccionReservationContextDto
{
    public Guid AtraccionGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public string AtraccionNombre { get; set; } = string.Empty;
    public DateOnly HorFecha { get; set; }
    public TimeOnly HorHoraInicio { get; set; }
    public TimeOnly? HorHoraFin { get; set; }
}
