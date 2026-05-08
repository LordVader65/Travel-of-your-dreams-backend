namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public sealed class ActualizarClienteRequest { public Guid Guid { get; set; } public string TipoIdentificacion { get; set; } = string.Empty; public string NumeroIdentificacion { get; set; } = string.Empty; public string? Nombres { get; set; } public string? Apellidos { get; set; } public string? RazonSocial { get; set; } public string Correo { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } public string Estado { get; set; } = "A"; }

