namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class HorarioFiltroDataModel
{
    public Guid AtraccionGuid { get; set; }
    public DateOnly? Fecha { get; set; }
}
