namespace TravelDreams.MsReservas.Business.DTOs;

public sealed class CambiarEstadoReservaRequest
{
    public string Estado { get; set; } = string.Empty;
    public string? Observacion { get; set; }
}
