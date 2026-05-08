namespace AtraccionesTuristicas.Backend.LA.Business.Validators.CatalogoRelaciones;

public static class ImagenAtraccionValidator
{
    public static void Validate(object? request)
    {
        if (request is null)
        {
            throw new ValidationException(["La solicitud es obligatoria."]);
        }
    }
}
