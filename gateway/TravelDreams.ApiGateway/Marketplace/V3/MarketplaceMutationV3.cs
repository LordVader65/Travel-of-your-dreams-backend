namespace TravelDreams.ApiGateway.Marketplace.V3;

public sealed class MarketplaceMutationV3
{
    public async Task<SolicitudProcesoResponse> SolicitarReserva(
        SolicitarReservaInput input,
        [Service] MarketplaceDownstreamClientV3 downstream,
        [Service] IMarketplaceEventPublisherV3 publisher,
        CancellationToken ct)
    {
        if (input.AtraccionGuid == Guid.Empty || input.HorarioGuid == Guid.Empty)
            throw Error("INVALID_INPUT", "AtraccionGuid y HorarioGuid son obligatorios.");
        if (input.Lineas.Count == 0 || input.Lineas.Any(x => x.TicketGuid == Guid.Empty || x.Cantidad <= 0))
            throw Error("INVALID_INPUT", "Debe enviar lineas con ticket y cantidad positiva.");

        var user = await downstream.RequireClientAsync(ct);
        var correlationId = Guid.NewGuid();
        await publisher.PublishAsync(new MarketplaceIntegrationEventV3
        {
            EventType = "marketplace.v3.reserva.solicitada",
            CorrelationId = correlationId,
            Payload = new
            {
                clienteGuid = user.ClienteGuid,
                input.AtraccionGuid,
                input.HorarioGuid,
                lineas = input.Lineas.Select(x => new { x.TicketGuid, x.Cantidad }),
                expiracionMinutos = input.ExpiracionMinutos,
                porcentajeIva = input.PorcentajeIva
            }
        }, "marketplace.v3.reserva.solicitada", ct);

        return new(correlationId, "RECIBIDA", "La solicitud de reserva fue recibida para procesamiento.");
    }

    public async Task<SolicitudProcesoResponse> ConfirmarPago(
        ConfirmarPagoInput input,
        [Service] MarketplaceDownstreamClientV3 downstream,
        [Service] IMarketplaceEventPublisherV3 publisher,
        CancellationToken ct)
    {
        if (input.ReservaGuid == Guid.Empty || input.Monto <= 0)
            throw Error("INVALID_INPUT", "ReservaGuid y un monto positivo son obligatorios.");

        var user = await downstream.RequireClientAsync(ct);
        var correlationId = Guid.NewGuid();
        await publisher.PublishAsync(new MarketplaceIntegrationEventV3
        {
            EventType = "marketplace.v3.pago.solicitado",
            CorrelationId = correlationId,
            Payload = new
            {
                clienteGuid = user.ClienteGuid,
                input.ReservaGuid,
                input.DatosFacturacionGuid,
                input.Metodo,
                input.Monto,
                input.Moneda,
                input.Referencia,
                input.Observacion
            }
        }, "marketplace.v3.pago.solicitado", ct);

        return new(correlationId, "RECIBIDA", "La solicitud de pago fue recibida para procesamiento.");
    }

    private static GraphQLException Error(string code, string message) =>
        new(ErrorBuilder.New().SetCode(code).SetMessage(message).Build());
}
