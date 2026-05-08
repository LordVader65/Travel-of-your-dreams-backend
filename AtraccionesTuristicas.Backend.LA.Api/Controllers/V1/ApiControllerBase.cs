using AtraccionesTuristicas.Backend.LA.Api.Models.Common;
using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.Common;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
public abstract class ApiControllerBase : ControllerBase
{
    private readonly ICurrentUserFactory _currentUserFactory;

    protected ApiControllerBase(ICurrentUserFactory currentUserFactory) => _currentUserFactory = currentUserFactory;

    protected CurrentUserData CurrentUser => _currentUserFactory.Create(HttpContext);

    protected OkObjectResult OkEnvelope<T>(T data, string message = "Operacion exitosa") =>
        Ok(ApiResponse<T>.Ok(data, message));

    protected ObjectResult CreatedEnvelope<T>(T data, string message = "Operacion exitosa") =>
        StatusCode(StatusCodes.Status201Created, ApiResponse<T>.Created(data, message));

    protected OkObjectResult ListEnvelope<T>(IReadOnlyList<T> data, int page, int limit, int total, string message = "Consulta exitosa") =>
        Ok(new ApiListResponse<T>
        {
            Status = StatusCodes.Status200OK,
            Message = message,
            Data = data,
            Pagination = PaginationResponse.From(page, limit, total)
        });
}
