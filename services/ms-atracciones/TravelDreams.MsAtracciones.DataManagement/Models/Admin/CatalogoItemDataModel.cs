namespace TravelDreams.MsAtracciones.DataManagement.Models.Admin;

public sealed class CatalogoItemDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public string? Descripcion { get; set; }
    public string? Tipo { get; set; }
    public string? Pais { get; set; }
    public string? ImagenUrl { get; set; }
    public int? ParentId { get; set; }
    public string Estado { get; set; } = "A";
}
