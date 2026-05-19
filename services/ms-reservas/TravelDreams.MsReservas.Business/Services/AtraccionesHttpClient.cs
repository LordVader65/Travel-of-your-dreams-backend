using System.Net.Http.Json;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Business.Services;

public sealed class AtraccionesHttpClient : IAtraccionesIntegrationClient
{
    private readonly HttpClient _httpClient;

    public AtraccionesHttpClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IReadOnlyList<AtraccionTicketDto>> GetTicketsAsync(Guid atraccionGuid, CancellationToken ct = default)
    {
        var response = await _httpClient.GetFromJsonAsync<ApiEnvelope<List<AtraccionTicketDto>>>($"/api/v1/atracciones/{atraccionGuid}/tickets", ct);
        return response?.Data ?? [];
    }

    public async Task<AtraccionReservationContextDto> GetReservationContextAsync(Guid atraccionGuid, Guid horarioGuid, CancellationToken ct = default)
    {
        var atraccionResponse = await _httpClient.GetFromJsonAsync<ApiEnvelope<AtraccionDetailResponse>>($"/api/v1/atracciones/{atraccionGuid}", ct);
        var horariosResponse = await _httpClient.GetFromJsonAsync<ApiEnvelope<List<HorarioResponse>>>($"/api/v1/atracciones/{atraccionGuid}/horarios", ct);
        var horario = horariosResponse?.Data?.FirstOrDefault(x => x.Guid == horarioGuid)
            ?? throw new InvalidOperationException("Atraccion u horario no encontrado.");

        return new AtraccionReservationContextDto
        {
            AtraccionGuid = atraccionGuid,
            HorarioGuid = horarioGuid,
            AtraccionNombre = atraccionResponse?.Data?.Nombre ?? string.Empty,
            HorFecha = horario.Fecha,
            HorHoraInicio = horario.HoraInicio,
            HorHoraFin = horario.HoraFin
        };
    }

    public async Task ReserveAsync(Guid horarioGuid, IReadOnlyList<CrearReservaLineaRequest> lineas, CancellationToken ct = default)
    {
        var payload = new
        {
            horarioGuid,
            lines = lineas.Select(x => new { ticketGuid = x.TicketGuid, cantidad = x.Cantidad }).ToList()
        };
        var response = await _httpClient.PostAsJsonAsync("/internal/v1/availability/reserve", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        }
    }

    public async Task ReleaseAsync(Guid horarioGuid, int cantidad, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/internal/v1/availability/release", new { horarioGuid, cantidad }, ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
        }
    }

    private sealed class ApiEnvelope<T>
    {
        public T? Data { get; set; }
    }

    private sealed class AtraccionDetailResponse
    {
        public string Nombre { get; set; } = string.Empty;
    }

    private sealed class HorarioResponse
    {
        public Guid Guid { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly? HoraFin { get; set; }
    }
}
