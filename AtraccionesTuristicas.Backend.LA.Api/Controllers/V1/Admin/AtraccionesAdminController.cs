using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.CatalogoRelaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/atracciones")]
[Authorize(Policy = "AdminOnly")]
public sealed class AtraccionesAdminController : ApiControllerBase
{
    private readonly IAtraccionService _atracciones;
    private readonly ICategoriaAtraccionService _categorias;
    private readonly IIdiomaAtraccionService _idiomas;
    private readonly IImagenAtraccionService _imagenes;
    private readonly IAtraccionIncluyeService _incluyes;

    public AtraccionesAdminController(IAtraccionService atracciones, ICategoriaAtraccionService categorias, IIdiomaAtraccionService idiomas, IImagenAtraccionService imagenes, IAtraccionIncluyeService incluyes, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _atracciones = atracciones; _categorias = categorias; _idiomas = idiomas; _imagenes = imagenes; _incluyes = incluyes;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken) => OkEnvelope(await _atracciones.ListarAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Crear(CrearAtraccionRequest request, CancellationToken cancellationToken)
    {
        request.UsuarioIngreso = CurrentUser.Login; request.IpIngreso = CurrentUser.Ip;
        return CreatedEnvelope(await _atracciones.CrearAsync(request, CurrentUser, cancellationToken));
    }

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, ActualizarAtraccionRequest request, CancellationToken cancellationToken)
    {
        request.Guid = guid;
        return OkEnvelope(await _atracciones.ActualizarAsync(request, CurrentUser, cancellationToken));
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken cancellationToken)
    {
        await _atracciones.EliminarAsync(guid, CurrentUser, cancellationToken);
        return NoContent();
    }

    [HttpPost("{atraccionId:int}/categorias")]
    public async Task<IActionResult> AsociarCategoria(int atraccionId, AsociarCategoriaAtraccionRequest request, CancellationToken cancellationToken)
    {
        request.AtraccionId = atraccionId; request.UsuarioIngreso = CurrentUser.Login;
        return CreatedEnvelope(await _categorias.AsociarAsync(request, CurrentUser, cancellationToken));
    }

    [HttpPost("{atraccionId:int}/idiomas")]
    public async Task<IActionResult> AsociarIdioma(int atraccionId, AsociarIdiomaAtraccionRequest request, CancellationToken cancellationToken)
    {
        request.AtraccionId = atraccionId; request.UsuarioIngreso = CurrentUser.Login;
        return CreatedEnvelope(await _idiomas.AsociarAsync(request, CurrentUser, cancellationToken));
    }

    [HttpPost("{atraccionId:int}/imagenes")]
    public async Task<IActionResult> AsociarImagen(int atraccionId, AsociarImagenAtraccionRequest request, CancellationToken cancellationToken)
    {
        request.AtraccionId = atraccionId; request.UsuarioIngreso = CurrentUser.Login;
        return CreatedEnvelope(await _imagenes.AsociarAsync(request, CurrentUser, cancellationToken));
    }

    [HttpPost("{atraccionId:int}/incluye")]
    public async Task<IActionResult> AsociarIncluye(int atraccionId, AsociarIncluyeAtraccionRequest request, CancellationToken cancellationToken)
    {
        request.AtraccionId = atraccionId; request.UsuarioIngreso = CurrentUser.Login;
        return CreatedEnvelope(await _incluyes.AsociarAsync(request, CurrentUser, cancellationToken));
    }
}
