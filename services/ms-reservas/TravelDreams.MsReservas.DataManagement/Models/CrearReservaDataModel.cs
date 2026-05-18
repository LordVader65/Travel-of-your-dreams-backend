namespace TravelDreams.MsReservas.DataManagement.Models;

public sealed class CrearReservaDataModel
{
    public Guid ClienteGuid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public IReadOnlyList<CrearReservaLineaDataModel> Lineas { get; set; } = [];
    public string? OrigenCanal { get; set; }
    public int ExpiracionMinutos { get; set; } = 15;
    public decimal PorcentajeIva { get; set; }
    public string Usuario { get; set; } = "system";
    public string Ip { get; set; } = "127.0.0.1";
}

public sealed class CrearReservaLineaDataModel
{
    public Guid TicketGuid { get; set; }
    public string TicketTitulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
