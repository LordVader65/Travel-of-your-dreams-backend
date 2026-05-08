namespace AtraccionesTuristicas.Backend.LA.Api.Models.Common;

public sealed class ApiListResponse<T> : ApiResponse<IReadOnlyList<T>>
{
    public PaginationResponse? Pagination { get; set; }
}
