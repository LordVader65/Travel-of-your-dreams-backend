namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

public sealed class ImagenAtraccionDataModel
{
    public int ImagenId { get; set; }
    public int AtraccionId { get; set; }
    public bool EsPrincipal { get; set; }
    public int Orden { get; set; }
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
}
