namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Identity;

public sealed class UsuarioFiltroRequest { public string? Login { get; set; } public string? Estado { get; set; } public int Page { get; set; } = 1; public int Limit { get; set; } = 20; }

