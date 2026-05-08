namespace AtraccionesTuristicas.Backend.LA.Api.Models.Common;

public sealed class PaginationResponse
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }

    public static PaginationResponse From(int page, int limit, int total) =>
        new() { Page = page, Limit = limit, Total = total, TotalPages = limit <= 0 ? 0 : (int)Math.Ceiling(total / (double)limit) };
}
