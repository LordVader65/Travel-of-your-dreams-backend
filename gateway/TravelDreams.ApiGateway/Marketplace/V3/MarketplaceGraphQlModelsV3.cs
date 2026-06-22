namespace TravelDreams.ApiGateway.Marketplace.V3;

public sealed record SolicitudProcesoResponse(Guid CorrelationId, string Estado, string Mensaje);

public sealed record SolicitarReservaInput(
    Guid AtraccionGuid,
    Guid HorarioGuid,
    IReadOnlyList<SolicitarReservaLineaInput> Lineas,
    int ExpiracionMinutos = 15,
    decimal PorcentajeIva = 15);

public sealed record SolicitarReservaLineaInput(Guid TicketGuid, int Cantidad);

public sealed record ConfirmarPagoInput(
    Guid ReservaGuid,
    decimal Monto,
    string Metodo = "TARJETA",
    string Moneda = "USD",
    Guid? DatosFacturacionGuid = null,
    string? Referencia = null,
    string? Observacion = null);

public sealed record EstadoReservaProcesoResponse(
    Guid CorrelationId,
    string Estado,
    Guid? ReservaGuid,
    string? ReservaCodigo,
    string? Error,
    DateTime? UpdatedAtUtc);

public sealed record EstadoPagoProcesoResponse(
    Guid CorrelationId,
    string Estado,
    Guid? ReservaGuid,
    Guid? FacturaGuid,
    string? FacturaNumero,
    string? Error,
    DateTime? UpdatedAtUtc);

public sealed record MarketplaceUserContext(Guid UserGuid, Guid ClienteGuid, IReadOnlyList<string> Roles);
