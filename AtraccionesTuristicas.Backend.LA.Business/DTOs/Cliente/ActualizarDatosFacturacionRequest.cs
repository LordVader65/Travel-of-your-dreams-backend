namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public sealed class ActualizarDatosFacturacionRequest : CrearDatosFacturacionRequest { public Guid Guid { get; set; } public string Estado { get; set; } = "A"; }

