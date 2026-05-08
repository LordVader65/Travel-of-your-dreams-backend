namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class CrearReseniaRequest { public int AtraccionId { get; set; } public int ReservaId { get; set; } public string? Comentario { get; set; } public short Rating { get; set; } public string UsuarioCreacion { get; set; } = string.Empty; public string IpCreacion { get; set; } = "127.0.0.1"; }

