namespace AtraccionesTuristicas.Backend.LA.Api.Models.Requests;

public sealed class GenerarFacturaApiRequest
{
    public Guid ReservaGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public string? Observacion { get; set; }
    public string? OrigenCanal { get; set; }
}
