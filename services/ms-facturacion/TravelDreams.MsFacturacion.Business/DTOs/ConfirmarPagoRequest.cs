using System.Text.Json.Serialization;

namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class ConfirmarPagoRequest
{
    [JsonIgnore]
    public Guid? CorrelationId { get; set; }

    [JsonPropertyName("cliente_guid")]
    public Guid? ClienteGuid { get; set; }

    [JsonPropertyName("datos_facturacion_guid")]
    public Guid? DatosFacturacionGuid { get; set; }

    public string Metodo { get; set; } = "TARJETA";
    public decimal Monto { get; set; }
    public string? Moneda { get; set; }
    public string? Referencia { get; set; }

    [JsonPropertyName("origen_canal")]
    public string? OrigenCanal { get; set; }

    public string? Observacion { get; set; }
}
