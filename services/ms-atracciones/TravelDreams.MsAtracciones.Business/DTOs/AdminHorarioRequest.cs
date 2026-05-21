using System.Text.Json.Serialization;

namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminHorarioRequest
{
    [JsonPropertyName("atraccion_id")]
    public int AtraccionId { get; set; }

    public DateOnly? Fecha { get; set; }

    [JsonPropertyName("hora_inicio")]
    public TimeOnly HoraInicio { get; set; }

    [JsonPropertyName("hora_fin")]
    public TimeOnly? HoraFin { get; set; }

    [JsonPropertyName("cupos_disponibles")]
    public int CuposDisponibles { get; set; }

    [JsonPropertyName("dias_semana")]
    public string DiasSemana { get; set; } = "0,1,2,3,4,5,6";

    [JsonPropertyName("fecha_inicio")]
    public DateOnly? FechaInicio { get; set; }

    [JsonPropertyName("fecha_fin")]
    public DateOnly? FechaFin { get; set; }
}
