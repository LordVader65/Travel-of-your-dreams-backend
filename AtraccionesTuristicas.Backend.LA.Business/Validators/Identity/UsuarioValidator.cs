namespace AtraccionesTuristicas.Backend.LA.Business.Validators.Identity;

public static class UsuarioValidator
{
    public static void Validate(object? request)
    {
        if (request is null)
        {
            throw new ValidationException(["La solicitud es obligatoria."]);
        }
    }
}
