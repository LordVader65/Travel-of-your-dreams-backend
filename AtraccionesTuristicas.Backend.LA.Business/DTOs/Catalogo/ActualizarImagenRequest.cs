namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class ActualizarImagenRequest : CrearImagenRequest { public Guid Guid { get; set; } public int Id { get; set; } public string Estado { get; set; } = "A"; }

