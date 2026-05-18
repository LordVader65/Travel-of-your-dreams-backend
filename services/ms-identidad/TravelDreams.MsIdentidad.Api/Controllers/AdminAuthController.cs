using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.Business.Interfaces;

namespace TravelDreams.MsIdentidad.Api.Controllers;

[ApiController]
[Route("api/v1/admin/auth")]
public sealed class AdminAuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AdminAuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var data = await _auth.LoginAdminAsync(request, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }
}
