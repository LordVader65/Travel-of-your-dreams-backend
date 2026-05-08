namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public class CrearCategoriaRequest { public int? ParentId { get; set; } public string Nombre { get; set; } = string.Empty; public string? TagName { get; set; } public string UsuarioIngreso { get; set; } = string.Empty; public string IpIngreso { get; set; } = "127.0.0.1"; }

