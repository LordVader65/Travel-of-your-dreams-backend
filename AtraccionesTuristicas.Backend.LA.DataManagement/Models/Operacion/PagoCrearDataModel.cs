namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class PagoCrearDataModel
{
    public Guid ReservaGuid { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string? OrigenCanal { get; set; }
}
