namespace TravelDreams.MsAtracciones.DataManagement.Models.Admin;

public sealed class ReseniaDataModel
{
    public Guid Guid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid ReservaGuid { get; set; }
    public string? Comentario { get; set; }
    public short Rating { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string Estado { get; set; } = "A";
}
