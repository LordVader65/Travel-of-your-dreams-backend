namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public sealed class CambiarEstadoClienteRequest { public Guid ClienteGuid { get; set; } public string Estado { get; set; } = "A"; public string Usuario { get; set; } = string.Empty; public string Ip { get; set; } = "127.0.0.1"; }

