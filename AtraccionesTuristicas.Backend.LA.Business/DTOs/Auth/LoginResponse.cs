namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Auth;

public sealed class LoginResponse { public Guid UsuarioGuid { get; set; } public Guid? ClienteGuid { get; set; } public string Login { get; set; } = string.Empty; public IReadOnlyList<string> Roles { get; set; } = []; public string Token { get; set; } = string.Empty; public DateTime ExpiraEnUtc { get; set; } }

