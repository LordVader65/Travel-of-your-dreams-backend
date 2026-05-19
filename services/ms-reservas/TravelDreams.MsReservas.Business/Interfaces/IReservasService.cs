using TravelDreams.MsReservas.Business.DTOs;

namespace TravelDreams.MsReservas.Business.Interfaces;

public interface IReservasService
{
    Task<IReadOnlyList<ReservaResponse>> ListarAsync(Guid? clienteGuid, string? estado, CancellationToken ct = default);
    Task<IReadOnlyList<ReservaResponse>> ListarPorCanalAsync(string origenCanal, string? estado, CancellationToken ct = default);
    Task<ReservaResponse?> ObtenerAsync(Guid reservaGuid, CancellationToken ct = default);
    Task<ReservaResponse> CrearAsync(CrearReservaRequest request, CancellationToken ct = default);
    Task<bool> CancelarAsync(Guid reservaGuid, CancelarReservaRequest request, CancellationToken ct = default);
    Task<int> ExpirarPendientesAsync(CancellationToken ct = default);
    Task<bool> CambiarEstadoAsync(Guid reservaGuid, CambiarEstadoReservaRequest request, CancellationToken ct = default);
}
