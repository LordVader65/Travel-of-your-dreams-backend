namespace AtraccionesTuristicas.Backend.LA.Business.Validators.Operacion;

public static class PagoValidator
{
    public static void Validate(object? request)
    {
        if (request is null)
        {
            throw new ValidationException(["La solicitud es obligatoria."]);
        }
    }
}
