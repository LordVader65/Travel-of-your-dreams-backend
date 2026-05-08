namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class CambiarEstadoReservaAdminRequest { public Guid ReservaGuid { get; set; } public string Estado { get; set; } = string.Empty; public string Usuario { get; set; } = string.Empty; public string Ip { get; set; } = "127.0.0.1"; public string? Observacion { get; set; } }

