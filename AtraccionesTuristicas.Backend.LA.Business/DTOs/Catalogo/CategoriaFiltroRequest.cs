namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class CategoriaFiltroRequest { public string? Nombre { get; set; } public string? Estado { get; set; } public int Page { get; set; } = 1; public int Limit { get; set; } = 20; }

