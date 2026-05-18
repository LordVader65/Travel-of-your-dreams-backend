namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class FacturaFiltroRequest
{
    public Guid? ClienteGuid { get; set; }
    public Guid? ReservaGuid { get; set; }
    public string? Numero { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesdeUtc { get; set; }
    public DateTime? FechaHastaUtc { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
