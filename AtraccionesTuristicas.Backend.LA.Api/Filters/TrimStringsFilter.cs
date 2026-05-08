using Microsoft.AspNetCore.Mvc.Filters;

namespace AtraccionesTuristicas.Backend.LA.Api.Filters;

public sealed class TrimStringsFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values.Where(x => x is not null))
        {
            TrimObject(argument!, new HashSet<object>(ReferenceEqualityComparer.Instance));
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private static void TrimObject(object value, HashSet<object> visited)
    {
        var type = value.GetType();
        if (type == typeof(string) || type.IsValueType || !visited.Add(value)) return;

        foreach (var property in type.GetProperties().Where(x => x.CanRead && x.CanWrite && x.GetIndexParameters().Length == 0))
        {
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(value, ((string?)property.GetValue(value))?.Trim());
            }
            else if (!property.PropertyType.IsValueType)
            {
                var current = property.GetValue(value);
                if (current is not null) TrimObject(current, visited);
            }
        }
    }
}
