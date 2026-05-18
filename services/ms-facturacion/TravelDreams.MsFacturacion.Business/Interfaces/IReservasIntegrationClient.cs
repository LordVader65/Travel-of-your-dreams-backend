using TravelDreams.MsFacturacion.Business.DTOs;

namespace TravelDreams.MsFacturacion.Business.Interfaces;

public interface IReservasIntegrationClient
{
    Task<ReservaPagoSnapshot?> GetPaymentSnapshotAsync(Guid reservaGuid, CancellationToken ct = default);
    Task<Guid?> GetClienteGuidByUsuarioGuidAsync(Guid usuarioGuid, CancellationToken ct = default);
    Task MarkAsPaidAsync(Guid reservaGuid, Guid pagoGuid, Guid facturaGuid, CancellationToken ct = default);
}
