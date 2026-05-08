namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class TicketResponse { public int Id { get; set; } public Guid Guid { get; set; } public int AtraccionId { get; set; } public string Titulo { get; set; } = string.Empty; public decimal Precio { get; set; } public string Moneda { get; set; } = "USD"; public string TipoParticipante { get; set; } = string.Empty; public int CapacidadMaxima { get; set; } public string Estado { get; set; } = "A"; }

