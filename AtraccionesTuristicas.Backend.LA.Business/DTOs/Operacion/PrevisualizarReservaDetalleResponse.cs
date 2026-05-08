namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class PrevisualizarReservaDetalleResponse
{
    public Guid TicketGuid { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
