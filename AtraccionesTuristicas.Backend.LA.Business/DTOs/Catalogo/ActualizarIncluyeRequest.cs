namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class ActualizarIncluyeRequest : CrearIncluyeRequest { public int Id { get; set; } public string Estado { get; set; } = "A"; }

