namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Auth;

public sealed class RegisterClienteRequest { public string TipoIdentificacion { get; set; } = string.Empty; public string NumeroIdentificacion { get; set; } = string.Empty; public string? Nombres { get; set; } public string? Apellidos { get; set; } public string? RazonSocial { get; set; } public string Correo { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } public string Usuario { get; set; } = "self-register"; public string Ip { get; set; } = "127.0.0.1"; }

