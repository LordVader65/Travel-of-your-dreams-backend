namespace AtraccionesTuristicas.Backend.LA.Api.Models.Requests;

public sealed class CrearReservaAdminApiRequest
{
    public Guid ClienteGuid { get; set; }
    public Guid HorGuid { get; set; }
    public IReadOnlyList<ReservaLineaApiRequest> Lineas { get; set; } = [];
    public string? OrigenCanal { get; set; }
    public int ExpiracionMinutos { get; set; } = 15;
    public decimal PorcentajeIva { get; set; }
}
