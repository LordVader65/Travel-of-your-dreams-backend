namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class CategoriaDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int? ParentId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? TagName { get; set; }
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
    public string IpIngreso { get; set; } = string.Empty;
}
