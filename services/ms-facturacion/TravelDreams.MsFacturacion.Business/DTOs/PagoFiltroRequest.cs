namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class PagoFiltroRequest
{
    public Guid? ReservaGuid { get; set; }
    public Guid? ClienteGuid { get; set; }
    public string? Metodo { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesdeUtc { get; set; }
    public DateTime? FechaHastaUtc { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
