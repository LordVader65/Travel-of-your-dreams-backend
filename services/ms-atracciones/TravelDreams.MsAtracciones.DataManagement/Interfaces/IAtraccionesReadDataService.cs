using TravelDreams.MsAtracciones.DataManagement.Models.Public;

namespace TravelDreams.MsAtracciones.DataManagement.Interfaces;

public interface IAtraccionesReadDataService
{
    Task<IReadOnlyList<AtraccionPublicaDataModel>> ListarAtraccionesAsync(CancellationToken cancellationToken = default);
    Task<AtraccionPublicaDataModel?> ObtenerAtraccionAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketPublicoDataModel>> ListarTicketsAsync(Guid atraccionGuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HorarioPublicoDataModel>> ListarHorariosPorAtraccionAsync(Guid atraccionGuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HorarioPublicoDataModel>> ListarHorariosPorTicketAsync(Guid ticketGuid, CancellationToken cancellationToken = default);
}
