namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminCatalogoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    public string? Tipo { get; set; }
    public string? Pais { get; set; }
    public string? ImagenUrl { get; set; }
    public int? ParentId { get; set; }
}
