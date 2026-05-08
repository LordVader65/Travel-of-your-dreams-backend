namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class ReservaEstadoHistorialResponse { public int Id { get; set; } public Guid Guid { get; set; } public int ReservaId { get; set; } public string? EstadoAnterior { get; set; } public string EstadoNuevo { get; set; } = string.Empty; public DateTime FechaUtc { get; set; } public string Usuario { get; set; } = string.Empty; public string Ip { get; set; } = string.Empty; public string? Observacion { get; set; } }

