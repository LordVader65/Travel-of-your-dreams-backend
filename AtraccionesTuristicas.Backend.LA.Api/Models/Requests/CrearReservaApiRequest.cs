namespace AtraccionesTuristicas.Backend.LA.Api.Models.Requests;

public sealed class CrearReservaApiRequest
{
    public Guid HorGuid { get; set; }
    public DateOnly? Fecha { get; set; }
    public IReadOnlyList<ReservaLineaApiRequest> Lineas { get; set; } = [];
    public string? OrigenCanal { get; set; }
    public int ExpiracionMinutos { get; set; } = 15;
    public decimal PorcentajeIva { get; set; }
}

public sealed class ReservaLineaApiRequest
{
    public Guid TckGuid { get; set; }
    public int Cantidad { get; set; }
}
