namespace AtraccionesTuristicas.Backend.LA.Business.Validators.Cliente;

public static class ClienteValidator
{
    public static void Validate(object? request)
    {
        if (request is null)
        {
            throw new ValidationException(["La solicitud es obligatoria."]);
        }
    }
}
