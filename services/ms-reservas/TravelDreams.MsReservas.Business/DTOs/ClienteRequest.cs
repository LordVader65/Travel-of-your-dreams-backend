using System.Text.Json.Serialization;

namespace TravelDreams.MsReservas.Business.DTOs;

public sealed class ClienteRequest
{
    [JsonPropertyName("cliente_guid")]
    public Guid? ClienteGuid { get; set; }

    [JsonPropertyName("usuario_guid")]
    public Guid? UsuarioGuid { get; set; }

    [JsonPropertyName("tipo_identificacion")]
    public string TipoIdentificacion { get; set; } = "OTRO";

    [JsonPropertyName("numero_identificacion")]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }

    [JsonPropertyName("razon_social")]
    public string? RazonSocial { get; set; }

    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
