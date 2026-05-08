namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Identity;

public sealed class CambiarRolUsuarioRequest { public int UsuarioId { get; set; } public int RolId { get; set; } public IReadOnlyList<int> RolIds { get; set; } = []; }

