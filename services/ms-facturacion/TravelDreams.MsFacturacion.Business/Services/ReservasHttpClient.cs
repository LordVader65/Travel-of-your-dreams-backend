using System.Net.Http.Json;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Business.Services;

public sealed class ReservasHttpClient : IReservasIntegrationClient
{
    private readonly HttpClient _http;

    public ReservasHttpClient(HttpClient http) => _http = http;

    public async Task<ReservaPagoSnapshot?> GetPaymentSnapshotAsync(Guid reservaGuid, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/internal/v1/reservas/{reservaGuid}/payment-snapshot", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<ReservaPagoSnapshot>>(cancellationToken: ct);
        return envelope?.Data;
    }

    public async Task<Guid?> GetClienteGuidByUsuarioGuidAsync(Guid usuarioGuid, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/internal/v1/clientes/by-user/{usuarioGuid}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<ClienteLookupResponse>>(cancellationToken: ct);
        return envelope?.Data?.Guid;
    }

    public async Task MarkAsPaidAsync(Guid reservaGuid, Guid pagoGuid, Guid facturaGuid, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync($"/internal/v1/reservas/{reservaGuid}/mark-paid", new { pagoGuid, facturaGuid }, ct);
        response.EnsureSuccessStatusCode();
    }

    private sealed class ResponseEnvelope<T>
    {
        public T? Data { get; set; }
    }

    private sealed class ClienteLookupResponse
    {
        public Guid Guid { get; set; }
    }
}
