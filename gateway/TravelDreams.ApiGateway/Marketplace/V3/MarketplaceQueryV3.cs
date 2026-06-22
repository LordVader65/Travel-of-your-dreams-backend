using HotChocolate.Types;

namespace TravelDreams.ApiGateway.Marketplace.V3;

public sealed class MarketplaceQueryV3
{
    [GraphQLType(typeof(AnyType))]
    public Task<object?> Atracciones([Service] MarketplaceDownstreamClientV3 downstream, CancellationToken ct) =>
        downstream.GetPublicAsync("AtraccionesUrl", "/api/v1/atracciones", ct);

    [GraphQLType(typeof(AnyType))]
    public Task<object?> Atraccion(Guid guid, [Service] MarketplaceDownstreamClientV3 downstream, CancellationToken ct) =>
        downstream.GetPublicAsync("AtraccionesUrl", $"/api/v1/atracciones/{guid}", ct);

    [GraphQLType(typeof(AnyType))]
    public Task<object?> Tickets(Guid atraccionGuid, [Service] MarketplaceDownstreamClientV3 downstream, CancellationToken ct) =>
        downstream.GetPublicAsync("AtraccionesUrl", $"/api/v1/atracciones/{atraccionGuid}/tickets", ct);

    [GraphQLType(typeof(AnyType))]
    public Task<object?> Horarios(Guid atraccionGuid, DateOnly? fecha, [Service] MarketplaceDownstreamClientV3 downstream, CancellationToken ct)
    {
        var query = fecha.HasValue ? $"?fecha={fecha:yyyy-MM-dd}" : string.Empty;
        return downstream.GetPublicAsync("AtraccionesUrl", $"/api/v1/atracciones/{atraccionGuid}/horarios{query}", ct);
    }

    [GraphQLType(typeof(AnyType))]
    public async Task<object?> MisReservas(string? estado, [Service] MarketplaceDownstreamClientV3 downstream, CancellationToken ct)
    {
        var user = await downstream.RequireClientAsync(ct);
        var query = string.IsNullOrWhiteSpace(estado) ? string.Empty : $"?estado={Uri.EscapeDataString(estado)}";
        return await downstream.GetClientAsync("ReservasUrl", $"/api/v1/reservas{query}", user, ct);
    }

    [GraphQLType(typeof(AnyType))]
    public async Task<object?> Reserva(Guid guid, [Service] MarketplaceDownstreamClientV3 downstream, CancellationToken ct)
    {
        var user = await downstream.RequireClientAsync(ct);
        return await downstream.GetClientAsync("ReservasUrl", $"/api/v1/reservas/{guid}", user, ct);
    }

    [GraphQLType(typeof(AnyType))]
    public async Task<object?> MisFacturas(int page, int limit, [Service] MarketplaceDownstreamClientV3 downstream, CancellationToken ct)
    {
        var user = await downstream.RequireClientAsync(ct);
        return await downstream.GetClientAsync("FacturacionUrl", $"/api/v1/facturas/mis-facturas?page={Math.Max(page, 1)}&limit={Math.Clamp(limit, 1, 50)}", user, ct);
    }

    public async Task<EstadoReservaProcesoResponse> EstadoReserva(
        Guid correlationId,
        [Service] MarketplaceDownstreamClientV3 downstream,
        CancellationToken ct)
    {
        var user = await downstream.RequireClientAsync(ct);
        return await downstream.GetReservaStatusAsync(correlationId, user, ct);
    }

    public async Task<EstadoPagoProcesoResponse> EstadoPago(
        Guid correlationId,
        [Service] MarketplaceDownstreamClientV3 downstream,
        CancellationToken ct)
    {
        var user = await downstream.RequireClientAsync(ct);
        return await downstream.GetPagoStatusAsync(correlationId, user, ct);
    }
}
