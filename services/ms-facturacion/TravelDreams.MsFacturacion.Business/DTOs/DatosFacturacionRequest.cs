using System.Text.Json.Serialization;

namespace TravelDreams.MsFacturacion.Business.DTOs;

public class DatosFacturacionRequest
{
    public Guid? Guid { get; set; }

    [JsonPropertyName("cliente_guid")]
    public Guid ClienteGuid { get; set; }

    [JsonPropertyName("tipo_identificacion")]
    public string TipoIdentificacion { get; set; } = "OTRO";

    [JsonPropertyName("numero_identificacion")]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [JsonPropertyName("razon_social")]
    public string? RazonSocial { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string? Apellido { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
