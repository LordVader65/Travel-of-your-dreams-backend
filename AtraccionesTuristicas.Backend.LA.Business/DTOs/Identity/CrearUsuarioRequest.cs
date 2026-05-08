namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Identity;

public sealed class CrearUsuarioRequest { public string Login { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; public string UsuarioRegistro { get; set; } = string.Empty; public string IpRegistro { get; set; } = "127.0.0.1"; }

