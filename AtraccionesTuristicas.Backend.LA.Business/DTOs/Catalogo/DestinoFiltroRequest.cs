namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class DestinoFiltroRequest { public string? Pais { get; set; } public string? Estado { get; set; } public int Page { get; set; } = 1; public int Limit { get; set; } = 20; }

