using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface ITicketRepository : IRepositoryBase<TicketEntity>
{
    Task<TicketEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketEntity>> ListarActivosPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default);
}
