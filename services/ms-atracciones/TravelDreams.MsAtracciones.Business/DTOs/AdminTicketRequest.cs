namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminTicketRequest
{
    public int AtraccionId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public string TipoParticipante { get; set; } = "Adulto";
    public int CapacidadMaxima { get; set; }
}
