using TravelDreams.MsReservas.DataManagement.Models;

namespace TravelDreams.MsReservas.DataManagement.Interfaces;

public interface IReservasDataService
{
    Task<ClienteDataModel> UpsertClienteAsync(ClienteDataModel model, CancellationToken ct = default);
    Task<IReadOnlyList<ReservaDataModel>> ListarAsync(Guid? clienteGuid, string? estado, CancellationToken ct = default);
    Task<IReadOnlyList<ReservaDataModel>> ListarPorCanalAsync(string origenCanal, string? estado, CancellationToken ct = default);
    Task<ReservaDataModel?> ObtenerAsync(Guid reservaGuid, CancellationToken ct = default);
    Task<Guid> CrearAsync(CrearReservaDataModel model, CancellationToken ct = default);
    Task<bool> CancelarAsync(Guid reservaGuid, string motivo, string usuario, string ip, CancellationToken ct = default);
    Task<int> ExpirarPendientesAsync(string usuario, string ip, Func<Guid, int, CancellationToken, Task> releaseAvailability, CancellationToken ct = default);
    Task<bool> CambiarEstadoAsync(Guid reservaGuid, string estado, string usuario, string ip, string? observacion, CancellationToken ct = default);
}
