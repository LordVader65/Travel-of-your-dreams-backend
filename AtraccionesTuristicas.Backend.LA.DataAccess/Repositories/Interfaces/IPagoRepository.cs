using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IPagoRepository : IRepositoryBase<PagoEntity>
{
    Task<PagoEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PagoEntity>> ObtenerPorReservaIdAsync(int reservaId, CancellationToken cancellationToken = default);
    Task<Guid> ConfirmarPagoAsync(
        Guid reservaGuid,
        string metodo,
        decimal monto,
        string referencia,
        string usuario,
        string ip,
        string? origenCanal = null,
        CancellationToken cancellationToken = default);
}
