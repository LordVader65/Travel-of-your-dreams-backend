namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public sealed class CrearClienteRequest { public int UsuarioId { get; set; } public string TipoIdentificacion { get; set; } = string.Empty; public string NumeroIdentificacion { get; set; } = string.Empty; public string? Nombres { get; set; } public string? Apellidos { get; set; } public string? RazonSocial { get; set; } public string Correo { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } public string UsuarioIngreso { get; set; } = string.Empty; public string IpIngreso { get; set; } = string.Empty; }

