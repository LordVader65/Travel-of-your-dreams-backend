namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class ReservaDetalleResponse { public int Id { get; set; } public Guid Guid { get; set; } public int ReservaId { get; set; } public int TicketId { get; set; } public int Cantidad { get; set; } public decimal PrecioUnitario { get; set; } public decimal Subtotal { get; set; } public string Estado { get; set; } = "A"; }

