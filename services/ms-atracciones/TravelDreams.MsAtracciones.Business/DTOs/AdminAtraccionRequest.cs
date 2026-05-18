namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminAtraccionRequest
{
    public int DestinoId { get; set; }
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
}
