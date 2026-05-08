namespace AtraccionesTuristicas.Backend.LA.Business.Validators.Catalogo;

public static class DestinoValidator
{
    public static void Validate(object? request)
    {
        if (request is null)
        {
            throw new ValidationException(["La solicitud es obligatoria."]);
        }
    }
}
