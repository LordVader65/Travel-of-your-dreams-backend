namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class IncluyeDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = "INCLUYE";
    public string Estado { get; set; } = "A";
}
