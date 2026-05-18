namespace TravelDreams.MsAtracciones.DataManagement.Models.Public;

public sealed class AtraccionPublicaDataModel
{
    public Guid Guid { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public bool Disponible { get; set; }
    public int TotalResenias { get; set; }
}
