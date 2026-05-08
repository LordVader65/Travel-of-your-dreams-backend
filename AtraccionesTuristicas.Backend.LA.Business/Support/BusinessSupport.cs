namespace AtraccionesTuristicas.Backend.LA.Business.Support
{
    using AtraccionesTuristicas.Backend.LA.Business.Common;
    using AtraccionesTuristicas.Backend.LA.Business.Exceptions;

    internal static class Guard
    {
        public static void Required(string? value, string name, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(value)) errors.Add($"{name} es obligatorio.");
        }

        public static void Positive(decimal value, string name, List<string> errors)
        {
            if (value <= 0) errors.Add($"{name} debe ser mayor a cero.");
        }

        public static void Positive(int value, string name, List<string> errors)
        {
            if (value <= 0) errors.Add($"{name} debe ser mayor a cero.");
        }

        public static void Email(string? value, string name, List<string> errors)
        {
            if (!string.IsNullOrWhiteSpace(value) && !Regex.IsMatch(value, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")) errors.Add($"{name} no tiene formato valido.");
        }

        public static void ThrowIfAny(List<string> errors)
        {
            if (errors.Count > 0) throw new ValidationException(errors);
        }

        public static void EnsureAdmin(CurrentUserData user)
        {
            if (!user.EsAdmin) throw new ForbiddenBusinessException("La operacion requiere rol ADMIN.");
        }

        public static void EnsureOwnerOrAdmin(CurrentUserData user, Guid ownerGuid)
        {
            if (!user.EsAdmin && user.ClienteGuid != ownerGuid) throw new ForbiddenBusinessException("No puede operar informacion de otro cliente.");
        }
    }

    internal static class Paging
    {
        public static BusinessPagedResult<TOut> Map<TIn, TOut>(DataPagedResult<TIn> source, Func<TIn, TOut> map) => new()
        {
            Items = source.Items.Select(map).ToList(),
            Page = source.Page,
            Limit = source.Limit,
            Total = source.Total
        };
    }
}

