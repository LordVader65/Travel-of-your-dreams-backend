namespace AtraccionesTuristicas.Backend.LA.Api.Models.Common;

public sealed class ApiErrorResponse
{
    public int Status { get; set; }
    public string Error { get; set; } = string.Empty;
    public IReadOnlyList<string> Details { get; set; } = [];
    public DateTime Timestamp { get; set; }
    public string Path { get; set; } = string.Empty;

    public static ApiErrorResponse Create(int status, string error, string path, IEnumerable<string>? details = null) =>
        new() { Status = status, Error = error, Details = details?.ToList() ?? [], Timestamp = DateTime.UtcNow, Path = path };
}
