using System.Text.Json.Serialization;

namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminAtraccionRequest
{
    [JsonPropertyName("destino_id")]
    public int DestinoId { get; set; }

    [JsonPropertyName("numero_establecimiento")]
    public string? NumeroEstablecimiento { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Direccion { get; set; }

    [JsonPropertyName("duracion_minutos")]
    public int? DuracionMinutos { get; set; }

    [JsonPropertyName("punto_encuentro")]
    public string? PuntoEncuentro { get; set; }

    [JsonPropertyName("precio_referencia")]
    public decimal? PrecioReferencia { get; set; }

    [JsonPropertyName("incluye_acompaniante")]
    public bool IncluyeAcompaniante { get; set; }

    [JsonPropertyName("incluye_transporte")]
    public bool IncluyeTransporte { get; set; }

    public bool Disponible { get; set; } = true;

    [JsonPropertyName("free_cancellation")]
    public bool FreeCancellation { get; set; }

    [JsonPropertyName("skip_the_line")]
    public bool SkipTheLine { get; set; }
}
