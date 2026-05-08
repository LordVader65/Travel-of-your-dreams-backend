namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public class CrearTicketRequest { public int AtraccionId { get; set; } public string Titulo { get; set; } = string.Empty; public decimal Precio { get; set; } public string Moneda { get; set; } = "USD"; public string TipoParticipante { get; set; } = "Adulto"; public int CapacidadMaxima { get; set; } public string UsuarioIngreso { get; set; } = string.Empty; public string IpIngreso { get; set; } = "127.0.0.1"; }

