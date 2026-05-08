using AtraccionesTuristicas.Backend.LA.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AtraccionesTuristicas.Backend.LA.Api.Filters;

public sealed class ValidateModelFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        var errors = context.ModelState.Values
            .SelectMany(x => x.Errors)
            .Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage) ? "Payload invalido." : x.ErrorMessage)
            .ToList();

        context.Result = new BadRequestObjectResult(ApiErrorResponse.Create(
            StatusCodes.Status400BadRequest,
            "La solicitud es invalida.",
            context.HttpContext.Request.Path,
            errors));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
