using Grpc.Core;
using TravelDreams.Grpc.Reservas;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Grpc;

public sealed class ReservasInternalGrpcService : ReservasInternal.ReservasInternalBase
{
    private readonly IReservasService _reservas;
    private readonly IClientesService _clientes;

    public ReservasInternalGrpcService(IReservasService reservas, IClientesService clientes)
    {
        _reservas = reservas;
        _clientes = clientes;
    }

    public override async Task<PaymentSnapshotResponse> GetPaymentSnapshot(ReservaGuidRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ReservaGuid, out var reservaGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ReservaGuid invalido."));
        }

        var reserva = await _reservas.ObtenerAsync(reservaGuid, context.CancellationToken);
        if (reserva is null)
        {
            return new PaymentSnapshotResponse { Found = false };
        }

        return new PaymentSnapshotResponse
        {
            Found = true,
            ReservaGuid = reserva.Guid.ToString(),
            RevCodigo = reserva.Codigo,
            ClienteGuid = reserva.ClienteGuid.ToString(),
            Estado = reserva.Estado,
            FechaExpiracionUtc = reserva.FechaExpiracionUtc.ToString("O"),
            Subtotal = reserva.Subtotal.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ValorIva = reserva.ValorIva.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Total = reserva.Total.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Moneda = reserva.Moneda
        };
    }

    public override async Task<ClienteLookupResponse> GetClienteByUsuario(UsuarioGuidRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UsuarioGuid, out var usuarioGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UsuarioGuid invalido."));
        }

        var cliente = await _clientes.ObtenerPorUsuarioGuidAsync(usuarioGuid, context.CancellationToken);
        return cliente is null
            ? new ClienteLookupResponse { Found = false }
            : new ClienteLookupResponse { Found = true, ClienteGuid = cliente.Guid.ToString() };
    }

    public override async Task<OperationResult> MarkAsPaid(MarkAsPaidRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ReservaGuid, out var reservaGuid) ||
            !Guid.TryParse(request.PagoGuid, out var pagoGuid) ||
            !Guid.TryParse(request.FacturaGuid, out var facturaGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ReservaGuid, PagoGuid y FacturaGuid son obligatorios."));
        }

        var ok = await _reservas.CambiarEstadoAsync(reservaGuid, new CambiarEstadoReservaRequest
        {
            Estado = "PAGADA",
            Observacion = $"Pago {pagoGuid} / factura {facturaGuid}"
        }, context.CancellationToken);

        return ok
            ? new OperationResult { Success = true }
            : new OperationResult { Success = false, Error = "Reserva no encontrada." };
    }
}
