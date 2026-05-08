using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/catalogos")]
[Authorize(Policy = "AdminOnly")]
public sealed class CatalogosController : ApiControllerBase
{
    private readonly IDestinoService _destinos;
    private readonly ICategoriaService _categorias;
    private readonly IIdiomaService _idiomas;
    private readonly IImagenService _imagenes;
    private readonly IIncluyeService _incluyes;

    public CatalogosController(IDestinoService destinos, ICategoriaService categorias, IIdiomaService idiomas, IImagenService imagenes, IIncluyeService incluyes, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _destinos = destinos; _categorias = categorias; _idiomas = idiomas; _imagenes = imagenes; _incluyes = incluyes;
    }

    [HttpGet("destinos")] public async Task<IActionResult> Destinos(CancellationToken ct) => OkEnvelope(await _destinos.ListarAsync(ct));
    [HttpPost("destinos")] public async Task<IActionResult> CrearDestino(CrearDestinoRequest request, CancellationToken ct) { request.UsuarioIngreso = CurrentUser.Login; request.IpIngreso = CurrentUser.Ip; return CreatedEnvelope(await _destinos.CrearAsync(request, CurrentUser, ct)); }
    [HttpPut("destinos/{id:int}")] public async Task<IActionResult> ActualizarDestino(int id, ActualizarDestinoRequest request, CancellationToken ct) { request.Id = id; return OkEnvelope(await _destinos.ActualizarAsync(request, CurrentUser, ct)); }
    [HttpDelete("destinos/{id:int}")] public async Task<IActionResult> EliminarDestino(int id, CancellationToken ct) { await _destinos.EliminarAsync(id, CurrentUser, ct); return NoContent(); }

    [HttpGet("categorias")] public async Task<IActionResult> Categorias(CancellationToken ct) => OkEnvelope(await _categorias.ListarAsync(ct));
    [HttpPost("categorias")] public async Task<IActionResult> CrearCategoria(CrearCategoriaRequest request, CancellationToken ct) { request.UsuarioIngreso = CurrentUser.Login; request.IpIngreso = CurrentUser.Ip; return CreatedEnvelope(await _categorias.CrearAsync(request, CurrentUser, ct)); }
    [HttpPut("categorias/{id:int}")] public async Task<IActionResult> ActualizarCategoria(int id, ActualizarCategoriaRequest request, CancellationToken ct) { request.Id = id; return OkEnvelope(await _categorias.ActualizarAsync(request, CurrentUser, ct)); }
    [HttpDelete("categorias/{id:int}")] public async Task<IActionResult> EliminarCategoria(int id, CancellationToken ct) { await _categorias.EliminarAsync(id, CurrentUser, ct); return NoContent(); }

    [HttpGet("idiomas")] public async Task<IActionResult> Idiomas(CancellationToken ct) => OkEnvelope(await _idiomas.ListarAsync(ct));
    [HttpPost("idiomas")] public async Task<IActionResult> CrearIdioma(CrearIdiomaRequest request, CancellationToken ct) { request.UsuarioIngreso = CurrentUser.Login; request.IpIngreso = CurrentUser.Ip; return CreatedEnvelope(await _idiomas.CrearAsync(request, CurrentUser, ct)); }
    [HttpPut("idiomas/{id:int}")] public async Task<IActionResult> ActualizarIdioma(int id, ActualizarIdiomaRequest request, CancellationToken ct) { request.Id = id; return OkEnvelope(await _idiomas.ActualizarAsync(request, CurrentUser, ct)); }
    [HttpDelete("idiomas/{id:int}")] public async Task<IActionResult> EliminarIdioma(int id, CancellationToken ct) { await _idiomas.EliminarAsync(id, CurrentUser, ct); return NoContent(); }

    [HttpGet("imagenes")] public async Task<IActionResult> Imagenes(CancellationToken ct) => OkEnvelope(await _imagenes.ListarAsync(ct));
    [HttpPost("imagenes")] public async Task<IActionResult> CrearImagen(CrearImagenRequest request, CancellationToken ct) { request.UsuarioIngreso = CurrentUser.Login; request.IpIngreso = CurrentUser.Ip; return CreatedEnvelope(await _imagenes.CrearAsync(request, CurrentUser, ct)); }
    [HttpPut("imagenes/{id:int}")] public async Task<IActionResult> ActualizarImagen(int id, ActualizarImagenRequest request, CancellationToken ct) { request.Id = id; return OkEnvelope(await _imagenes.ActualizarAsync(request, CurrentUser, ct)); }
    [HttpDelete("imagenes/{id:int}")] public async Task<IActionResult> EliminarImagen(int id, CancellationToken ct) { await _imagenes.EliminarAsync(id, CurrentUser, ct); return NoContent(); }

    [HttpGet("incluye")] public async Task<IActionResult> Incluye(CancellationToken ct) => OkEnvelope(await _incluyes.ListarAsync(ct));
    [HttpPost("incluye")] public async Task<IActionResult> CrearIncluye(CrearIncluyeRequest request, CancellationToken ct) => CreatedEnvelope(await _incluyes.CrearAsync(request, CurrentUser, ct));
    [HttpPut("incluye/{id:int}")] public async Task<IActionResult> ActualizarIncluye(int id, ActualizarIncluyeRequest request, CancellationToken ct) { request.Id = id; return OkEnvelope(await _incluyes.ActualizarAsync(request, CurrentUser, ct)); }
    [HttpDelete("incluye/{id:int}")] public async Task<IActionResult> EliminarIncluye(int id, CancellationToken ct) { await _incluyes.EliminarAsync(id, CurrentUser, ct); return NoContent(); }
}
