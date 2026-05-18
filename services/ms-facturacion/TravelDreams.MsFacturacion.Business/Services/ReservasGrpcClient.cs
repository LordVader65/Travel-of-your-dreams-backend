using Grpc.Core;
using TravelDreams.Grpc.Reservas;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Business.Services;

public sealed class ReservasGrpcClient : IReservasIntegrationClient
{
    private readonly ReservasInternal.ReservasInternalClient _client;

    public ReservasGrpcClient(ReservasInternal.ReservasInternalClient client)
    {
        _client = client;
    }

    public async Task<ReservaPagoSnapshot?> GetPaymentSnapshotAsync(Guid reservaGuid, CancellationToken ct = default)
    {
        var response = await _client.GetPaymentSnapshotAsync(new ReservaGuidRequest
        {
            ReservaGuid = reservaGuid.ToString()
        }, cancellationToken: ct);

        if (!response.Found) return null;

        return new ReservaPagoSnapshot
        {
            ReservaGuid = Guid.Parse(response.ReservaGuid),
            ClienteGuid = Guid.Parse(response.ClienteGuid),
            Estado = response.Estado,
            FechaExpiracionUtc = DateTime.Parse(response.FechaExpiracionUtc, null, System.Globalization.DateTimeStyles.RoundtripKind),
            Subtotal = decimal.Parse(response.Subtotal, System.Globalization.CultureInfo.InvariantCulture),
            ValorIva = decimal.Parse(response.ValorIva, System.Globalization.CultureInfo.InvariantCulture),
            Total = decimal.Parse(response.Total, System.Globalization.CultureInfo.InvariantCulture),
            Moneda = response.Moneda
        };
    }

    public async Task<Guid?> GetClienteGuidByUsuarioGuidAsync(Guid usuarioGuid, CancellationToken ct = default)
    {
        var response = await _client.GetClienteByUsuarioAsync(new UsuarioGuidRequest
        {
            UsuarioGuid = usuarioGuid.ToString()
        }, cancellationToken: ct);

        return response.Found ? Guid.Parse(response.ClienteGuid) : null;
    }

    public async Task MarkAsPaidAsync(Guid reservaGuid, Guid pagoGuid, Guid facturaGuid, CancellationToken ct = default)
    {
        var response = await _client.MarkAsPaidAsync(new MarkAsPaidRequest
        {
            ReservaGuid = reservaGuid.ToString(),
            PagoGuid = pagoGuid.ToString(),
            FacturaGuid = facturaGuid.ToString()
        }, cancellationToken: ct);

        if (!response.Success)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, response.Error));
        }
    }
}
