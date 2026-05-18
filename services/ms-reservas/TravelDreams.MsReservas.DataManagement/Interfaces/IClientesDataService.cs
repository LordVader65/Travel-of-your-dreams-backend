using TravelDreams.MsReservas.DataManagement.Models;

namespace TravelDreams.MsReservas.DataManagement.Interfaces;

public interface IClientesDataService
{
    Task<IReadOnlyList<ClienteDataModel>> ListarAsync(string? numeroIdentificacion, string? correo, string? estado, CancellationToken ct = default);
    Task<ClienteDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default);
    Task<ClienteDataModel?> ObtenerPorUsuarioGuidAsync(Guid usuarioGuid, CancellationToken ct = default);
    Task<ClienteDataModel?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken ct = default);
    Task<ClienteDataModel> GuardarAsync(ClienteDataModel model, CancellationToken ct = default);
    Task<bool> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken ct = default);
}
