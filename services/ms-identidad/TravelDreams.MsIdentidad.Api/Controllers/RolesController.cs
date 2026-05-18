using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsIdentidad.Business.Interfaces;

namespace TravelDreams.MsIdentidad.Api.Controllers;

[ApiController]
[Route("api/v1/admin/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly IUsuarioService _usuarios;

    public RolesController(IUsuarioService usuarios) => _usuarios = usuarios;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _usuarios.ListarRolesAsync(ct) });
}
