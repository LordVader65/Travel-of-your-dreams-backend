namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class CategoriaResponse { public int Id { get; set; } public Guid Guid { get; set; } public int? ParentId { get; set; } public string Nombre { get; set; } = string.Empty; public string? TagName { get; set; } public string Estado { get; set; } = "A"; }

