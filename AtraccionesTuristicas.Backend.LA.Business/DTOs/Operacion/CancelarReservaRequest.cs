namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class CancelarReservaRequest { public Guid ReservaGuid { get; set; } public string Usuario { get; set; } = string.Empty; public string Ip { get; set; } = "127.0.0.1"; public string Motivo { get; set; } = string.Empty; }

