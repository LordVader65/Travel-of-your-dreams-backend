namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public class CrearDatosFacturacionRequest { public int ClienteId { get; set; } public string TipoIdentificacion { get; set; } = string.Empty; public string NumeroIdentificacion { get; set; } = string.Empty; public string? RazonSocial { get; set; } public string Nombre { get; set; } = string.Empty; public string? Apellido { get; set; } public string Correo { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } public string UsuarioIngreso { get; set; } = string.Empty; public string IpIngreso { get; set; } = "127.0.0.1"; }

