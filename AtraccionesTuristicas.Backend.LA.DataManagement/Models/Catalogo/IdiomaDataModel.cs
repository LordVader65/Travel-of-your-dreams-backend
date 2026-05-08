namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class IdiomaDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
    public string IpIngreso { get; set; } = string.Empty;
}
