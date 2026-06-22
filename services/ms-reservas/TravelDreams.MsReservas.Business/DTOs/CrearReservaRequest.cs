using System.Text.Json.Serialization;

namespace TravelDreams.MsReservas.Business.DTOs;

public sealed class CrearReservaRequest
{
    [JsonIgnore]
    public Guid? CorrelationId { get; set; }

    [JsonPropertyName("cliente_guid")]
    public Guid? ClienteGuid { get; set; }

    [JsonPropertyName("cliente_invitado")]
    public ClienteRequest? ClienteInvitado { get; set; }

    [JsonPropertyName("atraccion_guid")]
    public Guid AtraccionGuid { get; set; }

    [JsonPropertyName("horario_guid")]
    public Guid HorarioGuid { get; set; }

    public IReadOnlyList<CrearReservaLineaRequest> Lineas { get; set; } = [];

    [JsonPropertyName("origen_canal")]
    public string? OrigenCanal { get; set; }

    [JsonPropertyName("expiracion_minutos")]
    public int ExpiracionMinutos { get; set; } = 15;

    [JsonPropertyName("porcentaje_iva")]
    public decimal PorcentajeIva { get; set; }
}

public sealed class CrearReservaLineaRequest
{
    [JsonPropertyName("ticket_guid")]
    public Guid TicketGuid { get; set; }
    public int Cantidad { get; set; }
}
