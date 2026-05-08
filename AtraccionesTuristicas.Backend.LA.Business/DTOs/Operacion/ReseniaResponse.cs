namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class ReseniaResponse { public int Id { get; set; } public Guid Guid { get; set; } public int AtraccionId { get; set; } public int ReservaId { get; set; } public string? Comentario { get; set; } public short Rating { get; set; } public DateTime FechaCreacion { get; set; } public string Estado { get; set; } = "A"; }

