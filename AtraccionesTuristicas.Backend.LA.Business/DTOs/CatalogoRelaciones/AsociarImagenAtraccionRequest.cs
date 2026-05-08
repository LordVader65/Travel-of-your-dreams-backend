namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.CatalogoRelaciones;

public sealed class AsociarImagenAtraccionRequest { public int ImagenId { get; set; } public int AtraccionId { get; set; } public bool EsPrincipal { get; set; } public int Orden { get; set; } public string UsuarioIngreso { get; set; } = string.Empty; }

