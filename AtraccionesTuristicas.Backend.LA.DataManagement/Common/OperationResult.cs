namespace AtraccionesTuristicas.Backend.LA.DataManagement.Common;

public sealed class OperationResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }

    public static OperationResult Ok(string? message = null) => new() { Success = true, Message = message };
    public static OperationResult Fail(string message) => new() { Success = false, Message = message };
}

public sealed class OperationResult<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static OperationResult<T> Ok(T data, string? message = null) => new() { Success = true, Data = data, Message = message };
    public static OperationResult<T> Fail(string message) => new() { Success = false, Message = message };
}
