namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.CatalogoRelaciones;

public sealed class AsociarIdiomaAtraccionRequest { public int IdiomaId { get; set; } public int AtraccionId { get; set; } public string UsuarioIngreso { get; set; } = string.Empty; }

