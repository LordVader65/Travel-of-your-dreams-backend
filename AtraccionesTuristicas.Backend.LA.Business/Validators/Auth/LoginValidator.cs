namespace AtraccionesTuristicas.Backend.LA.Business.Validators.Auth;

public static class LoginValidator
{
    public static void Validate(object? request)
    {
        if (request is null)
        {
            throw new ValidationException(["La solicitud es obligatoria."]);
        }
    }
}
