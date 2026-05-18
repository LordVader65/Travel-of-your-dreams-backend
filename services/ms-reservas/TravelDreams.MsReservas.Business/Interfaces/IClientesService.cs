using TravelDreams.MsReservas.Business.DTOs;

namespace TravelDreams.MsReservas.Business.Interfaces;

public interface IClientesService
{
    Task<IReadOnlyList<ClienteResponse>> ListarAsync(string? numeroIdentificacion, string? correo, string? estado, CancellationToken ct = default);
    Task<ClienteResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default);
    Task<ClienteResponse?> ObtenerPorUsuarioGuidAsync(Guid usuarioGuid, CancellationToken ct = default);
    Task<ClienteResponse?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken ct = default);
    Task<ClienteResponse> GuardarAsync(ClienteRequest request, CancellationToken ct = default);
    Task<bool> CambiarEstadoAsync(Guid guid, CambiarEstadoClienteRequest request, CancellationToken ct = default);
}
