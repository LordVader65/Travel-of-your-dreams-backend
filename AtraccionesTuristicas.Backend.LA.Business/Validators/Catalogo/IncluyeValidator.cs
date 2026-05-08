namespace AtraccionesTuristicas.Backend.LA.Business.Validators.Catalogo;

public static class IncluyeValidator
{
    public static void Validate(object? request)
    {
        if (request is null)
        {
            throw new ValidationException(["La solicitud es obligatoria."]);
        }
    }
}
