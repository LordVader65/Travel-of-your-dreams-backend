namespace TravelDreams.MsAtracciones.DataManagement.Models.Admin;

public sealed class ReseniaDataModel
{
    public Guid Guid { get; set; }
    public int ClienteId { get; set; }
    public int AtraccionId { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid ReservaGuid { get; set; }
    public int Calificacion { get; set; }
    public string? Comentario { get; set; }
    public short Rating { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? UsuarioCreacion { get; set; }
    public string Estado { get; set; } = "A";
}
