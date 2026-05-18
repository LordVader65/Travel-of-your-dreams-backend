namespace TravelDreams.MsAtracciones.DataManagement.Models.Admin;

public sealed class CrearReseniaDataModel
{
    public Guid AtraccionGuid { get; set; }
    public Guid ReservaGuid { get; set; }
    public string? Comentario { get; set; }
    public short Rating { get; set; }
    public string Usuario { get; set; } = "system";
    public string Ip { get; set; } = "127.0.0.1";
}
