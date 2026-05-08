using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Cliente;

[Route("api/v{version:apiVersion}/resenias")]
public sealed class ReseniasController : ApiControllerBase
{
    private readonly IReseniaService _resenias;

    public ReseniasController(IReseniaService resenias, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _resenias = resenias;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken) =>
        OkEnvelope(await _resenias.ListarAsync(cancellationToken));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Crear(CrearReseniaRequest request, CancellationToken cancellationToken)
    {
        request.UsuarioCreacion = CurrentUser.Login;
        request.IpCreacion = CurrentUser.Ip;
        return CreatedEnvelope(await _resenias.CrearAsync(request, CurrentUser, cancellationToken));
    }
}
