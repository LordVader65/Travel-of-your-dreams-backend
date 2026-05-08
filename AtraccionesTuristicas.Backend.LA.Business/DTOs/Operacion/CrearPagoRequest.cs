namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class CrearPagoRequest { public Guid ReservaGuid { get; set; } public string Metodo { get; set; } = string.Empty; public decimal Monto { get; set; } public string Referencia { get; set; } = string.Empty; public string Usuario { get; set; } = string.Empty; public string Ip { get; set; } = "127.0.0.1"; public string? OrigenCanal { get; set; } }

