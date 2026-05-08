namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

public sealed class CategoriaAtraccionDataModel
{
    public int CategoriaId { get; set; }
    public int AtraccionId { get; set; }
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
}
