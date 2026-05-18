using TravelDreams.MsFacturacion.Business.DTOs;

namespace TravelDreams.MsFacturacion.Business.Interfaces;

public interface IDatosFacturacionService
{
    Task<IReadOnlyList<DatosFacturacionResponse>> ListarPorClienteAsync(Guid clienteGuid, CancellationToken ct = default);
    Task<DatosFacturacionResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default);
    Task<DatosFacturacionResponse> GuardarAsync(DatosFacturacionRequest request, CancellationToken ct = default);
    Task<bool> InactivarAsync(Guid guid, CancellationToken ct = default);
}
