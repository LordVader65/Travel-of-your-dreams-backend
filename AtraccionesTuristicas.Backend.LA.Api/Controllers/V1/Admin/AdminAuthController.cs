using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Auth;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/auth")]
public sealed class AdminAuthController : ApiControllerBase
{
    private readonly IAuthService _auth;

    public AdminAuthController(IAuthService auth, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _auth = auth;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken) =>
        OkEnvelope(await _auth.LoginAdminAsync(request, cancellationToken));
}
