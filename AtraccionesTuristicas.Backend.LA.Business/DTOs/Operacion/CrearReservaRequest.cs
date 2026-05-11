namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class CrearReservaRequest { public Guid ClienteGuid { get; set; } public Guid HorarioGuid { get; set; } public DateOnly? Fecha { get; set; } public IReadOnlyList<CrearReservaDetalleRequest> Tickets { get; set; } = []; public string Usuario { get; set; } = string.Empty; public string Ip { get; set; } = "127.0.0.1"; public string? OrigenCanal { get; set; } public int ExpiracionMinutos { get; set; } = 15; public decimal PorcentajeIva { get; set; } }

