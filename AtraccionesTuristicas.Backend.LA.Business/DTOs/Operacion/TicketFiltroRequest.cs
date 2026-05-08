namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class TicketFiltroRequest { public int? AtraccionId { get; set; } public int Page { get; set; } = 1; public int Limit { get; set; } = 20; }

