namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class ActualizarCategoriaRequest : CrearCategoriaRequest { public Guid Guid { get; set; } public int Id { get; set; } public string Estado { get; set; } = "A"; }

