namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class ActualizarAtraccionRequest : CrearAtraccionRequest { public Guid Guid { get; set; } public string Estado { get; set; } = "A"; }

