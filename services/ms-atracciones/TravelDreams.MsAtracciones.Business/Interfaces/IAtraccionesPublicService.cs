using TravelDreams.MsAtracciones.Business.DTOs;

namespace TravelDreams.MsAtracciones.Business.Interfaces;

public interface IAtraccionesPublicService
{
    Task<IReadOnlyList<AtraccionResponse>> ListarAtraccionesAsync(CancellationToken cancellationToken = default);
    Task<AtraccionResponse?> ObtenerAtraccionAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketResponse>> ListarTicketsAsync(Guid atraccionGuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HorarioResponse>> ListarHorariosPorAtraccionAsync(Guid atraccionGuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HorarioResponse>> ListarHorariosPorTicketAsync(Guid ticketGuid, CancellationToken cancellationToken = default);
}
