namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

public sealed class AtraccionFiltroDataModel
{
    public string? Pais { get; set; }
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public string? Tipo { get; set; }
    public string? Subtipo { get; set; }
    public string? Etiqueta { get; set; }
    public string? Idioma { get; set; }
    public decimal? PrecioMinimo { get; set; }
    public decimal? PrecioMaximo { get; set; }
    public short? RatingMinimo { get; set; }
    public string? Horario { get; set; }
    public string? OrdenarPor { get; set; }
    public bool SoloDisponibles { get; set; } = true;
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
