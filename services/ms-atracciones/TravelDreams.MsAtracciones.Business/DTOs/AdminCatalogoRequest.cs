using System.Text.Json.Serialization;

namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminCatalogoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    [JsonPropertyName("tag_name")]
    public string? TagName { get; set; }
    public string? Descripcion { get; set; }
    public string? Tipo { get; set; }
    public string? Pais { get; set; }
    [JsonPropertyName("imagen_url")]
    public string? ImagenUrl { get; set; }
    public string? Url { get; set; }
    [JsonPropertyName("parent_id")]
    public int? ParentId { get; set; }
}
