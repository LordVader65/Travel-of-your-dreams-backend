namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public sealed class DatosFacturacionFiltroRequest { public Guid? ClienteGuid { get; set; } public string? NumeroIdentificacion { get; set; } public string? Correo { get; set; } public string? Estado { get; set; } public int Page { get; set; } = 1; public int Limit { get; set; } = 20; }

