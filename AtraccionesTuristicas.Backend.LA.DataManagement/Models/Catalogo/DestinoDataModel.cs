namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class DestinoDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
    public string IpIngreso { get; set; } = string.Empty;
}
