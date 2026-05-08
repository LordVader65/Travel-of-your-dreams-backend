namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class AtraccionResponse : CrearAtraccionRequest { public int Id { get; set; } public Guid Guid { get; set; } public string Estado { get; set; } = "A"; }

