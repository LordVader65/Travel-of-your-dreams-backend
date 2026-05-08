namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class DestinoResponse { public int Id { get; set; } public Guid Guid { get; set; } public string Nombre { get; set; } = string.Empty; public string Pais { get; set; } = string.Empty; public string? ImagenUrl { get; set; } public string Estado { get; set; } = "A"; }

