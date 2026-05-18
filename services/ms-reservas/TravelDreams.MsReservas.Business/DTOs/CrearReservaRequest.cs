namespace TravelDreams.MsReservas.Business.DTOs;

public sealed class CrearReservaRequest
{
    public Guid? ClienteGuid { get; set; }
    public ClienteRequest? ClienteInvitado { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public IReadOnlyList<CrearReservaLineaRequest> Lineas { get; set; } = [];
    public string? OrigenCanal { get; set; }
    public int ExpiracionMinutos { get; set; } = 15;
    public decimal PorcentajeIva { get; set; }
}

public sealed class CrearReservaLineaRequest
{
    public Guid TicketGuid { get; set; }
    public int Cantidad { get; set; }
}
