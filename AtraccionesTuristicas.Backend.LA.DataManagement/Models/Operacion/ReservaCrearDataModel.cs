namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class ReservaCrearDataModel
{
    public Guid ClienteGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public IReadOnlyList<ReservaCrearDetalleDataModel> Tickets { get; set; } = [];
    public string Usuario { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string? OrigenCanal { get; set; }
    public int ExpiracionMinutos { get; set; } = 15;
    public decimal PorcentajeIva { get; set; }
}
