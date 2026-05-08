namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class AtraccionDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int DestinoId { get; set; }
    public string? NumeroEstablecimiento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Direccion { get; set; }
    public int? DuracionMinutos { get; set; }
    public string? PuntoEncuentro { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public bool IncluyeAcompaniante { get; set; }
    public bool IncluyeTransporte { get; set; }
    public bool Disponible { get; set; } = true;
    public bool FreeCancellation { get; set; }
    public bool SkipTheLine { get; set; }
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
    public string IpIngreso { get; set; } = string.Empty;
}
