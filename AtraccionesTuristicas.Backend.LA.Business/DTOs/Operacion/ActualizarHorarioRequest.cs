namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class ActualizarHorarioRequest : CrearHorarioRequest { public Guid Guid { get; set; } public string Estado { get; set; } = "A"; public string? UsuarioModificacion { get; set; } public string? IpModificacion { get; set; } }

