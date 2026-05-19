namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class CrearReseniaRequest
{
    public int? ClienteId { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid ReservaGuid { get; set; }
    public string? Comentario { get; set; }
    public short Rating { get; set; }
    public int? Calificacion { get; set; }
}
