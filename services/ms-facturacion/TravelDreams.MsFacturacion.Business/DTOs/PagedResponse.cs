namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
}
