using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface ITicketDataService
{
    Task<IReadOnlyList<TicketDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<TicketDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketDataModel>> ListarActivosPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default);
    Task<TicketDataModel> CrearAsync(TicketDataModel model, CancellationToken cancellationToken = default);
    Task<TicketDataModel> ActualizarAsync(TicketDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
