namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class ImagenResponse { public int Id { get; set; } public Guid Guid { get; set; } public string Url { get; set; } = string.Empty; public string? Descripcion { get; set; } public string Estado { get; set; } = "A"; }

