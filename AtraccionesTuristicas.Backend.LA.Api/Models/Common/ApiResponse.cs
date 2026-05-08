namespace AtraccionesTuristicas.Backend.LA.Api.Models.Common;

public class ApiResponse<T>
{
    public int Status { get; set; }
    public string Message { get; set; } = "Operacion exitosa";
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T? data, string message = "Operacion exitosa") =>
        new() { Status = StatusCodes.Status200OK, Message = message, Data = data };

    public static ApiResponse<T> Created(T? data, string message = "Operacion exitosa") =>
        new() { Status = StatusCodes.Status201Created, Message = message, Data = data };
}
