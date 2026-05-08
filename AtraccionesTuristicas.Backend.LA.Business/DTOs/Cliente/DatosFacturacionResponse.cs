namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public sealed class DatosFacturacionResponse { public int Id { get; set; } public Guid Guid { get; set; } public int ClienteId { get; set; } public string TipoIdentificacion { get; set; } = string.Empty; public string NumeroIdentificacion { get; set; } = string.Empty; public string? RazonSocial { get; set; } public string Nombre { get; set; } = string.Empty; public string? Apellido { get; set; } public string Correo { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } public string Estado { get; set; } = "A"; }

