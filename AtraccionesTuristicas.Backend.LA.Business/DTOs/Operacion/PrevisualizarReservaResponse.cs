namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class PrevisualizarReservaResponse
{
    public Guid ClienteGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public IReadOnlyList<PrevisualizarReservaDetalleResponse> Detalles { get; set; } = [];
}
