namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class TicketResponse
{
    public Guid Guid { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public int CapacidadMaxima { get; set; }
}

public sealed class TicketHorarioResponse
{
    public Guid Guid { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public int CapacidadMaxima { get; set; }
    public Guid HorarioGuid { get; set; }
    public int CuposDisponibles { get; set; }
}
