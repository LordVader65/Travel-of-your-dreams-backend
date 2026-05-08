namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class IdiomaResponse { public int Id { get; set; } public Guid Guid { get; set; } public string Codigo { get; set; } = string.Empty; public string Descripcion { get; set; } = string.Empty; public string Estado { get; set; } = "A"; }

