namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class ImagenDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
    public string IpIngreso { get; set; } = string.Empty;
}
