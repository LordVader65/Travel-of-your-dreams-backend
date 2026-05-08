using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IFacturaRepository : IRepositoryBase<FacturaEntity>
{
    Task<FacturaEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<FacturaEntity?> ObtenerPorReservaIdAsync(int reservaId, CancellationToken cancellationToken = default);
    Task<FacturaEntity?> ObtenerPorPagoIdAsync(int pagoId, CancellationToken cancellationToken = default);
    Task<Guid> GenerarFacturaAsync(
        Guid reservaGuid,
        Guid? datosFacturacionGuid,
        string usuario,
        string ip,
        string? observacion = null,
        string? origenCanal = null,
        CancellationToken cancellationToken = default);
}
