namespace TravelDreams.MsAtracciones.DataManagement.Models.Admin;

public sealed class HorarioGeneracionDataModel
{
    public HorarioReglaAdminDataModel Regla { get; set; } = new();
    public int Generados { get; set; }
    public int Existentes { get; set; }
}
