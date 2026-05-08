namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class TicketDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int AtraccionId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public string TipoParticipante { get; set; } = "Adulto";
    public int CapacidadMaxima { get; set; }
    public string Estado { get; set; } = "A";
    public string UsuarioIngreso { get; set; } = string.Empty;
    public string IpIngreso { get; set; } = string.Empty;
}
