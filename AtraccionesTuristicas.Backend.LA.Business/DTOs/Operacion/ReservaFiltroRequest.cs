namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class ReservaFiltroRequest { public Guid? ClienteGuid { get; set; } public Guid? AtraccionGuid { get; set; } public string? Codigo { get; set; } public string? Estado { get; set; } public DateOnly? FechaDesde { get; set; } public DateOnly? FechaHasta { get; set; } public int Page { get; set; } = 1; public int Limit { get; set; } = 20; }

