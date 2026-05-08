namespace AtraccionesTuristicas.Backend.LA.Api.Models.Requests;

public sealed class CrearPagoApiRequest
{
    public Guid ReservaGuid { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public Guid? DatosFacturacionGuid { get; set; }
    public string? OrigenCanal { get; set; }
}
