namespace AtraccionesTuristicas.Backend.LA.Business.Exceptions;

public sealed class ValidationException : BusinessException
    {
        public IReadOnlyList<string> Errors { get; }
        public ValidationException(IEnumerable<string> errors) : base("La solicitud contiene errores de validacion.") => Errors = errors.ToList();
    }

