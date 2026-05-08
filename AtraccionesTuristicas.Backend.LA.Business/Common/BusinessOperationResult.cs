namespace AtraccionesTuristicas.Backend.LA.Business.Common;

public sealed class BusinessOperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public static BusinessOperationResult Ok(string? message = null) => new() { Success = true, Message = message };
    }

