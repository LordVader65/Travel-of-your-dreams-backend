using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("internal/v1/availability")]
public sealed class InternalAvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availability;

    public InternalAvailabilityController(IAvailabilityService availability)
    {
        _availability = availability;
    }

    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve(ReserveAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await _availability.ReserveAvailabilityAsync(request, cancellationToken);
        return result.Success
            ? Ok(result)
            : Conflict(result);
    }

    [HttpPost("release")]
    public async Task<IActionResult> Release(ReleaseAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await _availability.ReleaseAvailabilityAsync(request, cancellationToken);
        return result.Success
            ? Ok(result)
            : Conflict(result);
    }
}
