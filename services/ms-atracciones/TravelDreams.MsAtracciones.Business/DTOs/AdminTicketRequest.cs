using System.Text.Json.Serialization;

namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminTicketRequest
{
    [JsonPropertyName("atraccion_id")]
    public int AtraccionId { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";

    [JsonPropertyName("tipo_participante")]
    public string TipoParticipante { get; set; } = "Adulto";

    [JsonPropertyName("capacidad_maxima")]
    public int CapacidadMaxima { get; set; }
}
