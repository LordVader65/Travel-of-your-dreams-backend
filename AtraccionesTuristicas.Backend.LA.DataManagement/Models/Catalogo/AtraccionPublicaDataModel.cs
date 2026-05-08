namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class AtraccionPublicaDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Direccion { get; set; }
    public int? DuracionMinutos { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public bool Disponible { get; set; }
    public bool FreeCancellation { get; set; }
    public bool SkipTheLine { get; set; }
    public int TotalResenias { get; set; }
}
