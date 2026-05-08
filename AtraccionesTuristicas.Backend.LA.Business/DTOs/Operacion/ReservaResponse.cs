namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class ReservaResponse { public int Id { get; set; } public Guid Guid { get; set; } public string Codigo { get; set; } = string.Empty; public int ClienteId { get; set; } public int HorarioId { get; set; } public DateTime FechaReservaUtc { get; set; } public DateTime FechaExpiracionUtc { get; set; } public decimal Subtotal { get; set; } public decimal ValorIva { get; set; } public decimal Total { get; set; } public string Moneda { get; set; } = "USD"; public string Estado { get; set; } = string.Empty; public IReadOnlyList<ReservaDetalleResponse> Detalles { get; set; } = []; }

