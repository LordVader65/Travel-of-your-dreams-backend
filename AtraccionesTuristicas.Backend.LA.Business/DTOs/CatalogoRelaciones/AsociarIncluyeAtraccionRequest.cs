namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.CatalogoRelaciones;

public sealed class AsociarIncluyeAtraccionRequest { public int IncluyeId { get; set; } public int AtraccionId { get; set; } public string UsuarioIngreso { get; set; } = string.Empty; }

