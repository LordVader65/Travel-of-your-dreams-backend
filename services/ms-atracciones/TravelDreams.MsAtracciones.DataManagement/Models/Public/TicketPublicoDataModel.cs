namespace TravelDreams.MsAtracciones.DataManagement.Models.Public;

public sealed class TicketPublicoDataModel
{
    public Guid Guid { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public int CapacidadMaxima { get; set; }
}
