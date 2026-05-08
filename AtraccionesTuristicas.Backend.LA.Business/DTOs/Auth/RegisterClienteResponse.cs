namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Auth;

public sealed class RegisterClienteResponse { public Guid ClienteGuid { get; set; } public Guid UsuarioGuid { get; set; } public string Correo { get; set; } = string.Empty; }

