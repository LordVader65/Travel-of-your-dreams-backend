using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Public;

[Route("api/v{version:apiVersion}/atracciones")]
[AllowAnonymous]
public sealed class AtraccionesController : ApiControllerBase
{
    private readonly IAtraccionService _atracciones;
    private readonly ICategoriaService _categorias;
    private readonly IDestinoService _destinos;
    private readonly IIdiomaService _idiomas;

    public AtraccionesController(IAtraccionService atracciones, ICategoriaService categorias, IDestinoService destinos, IIdiomaService idiomas, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _atracciones = atracciones;
        _categorias = categorias;
        _destinos = destinos;
        _idiomas = idiomas;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? destino,
        [FromQuery(Name = "fecha_desde")] DateOnly? fechaDesde,
        [FromQuery(Name = "fecha_hasta")] DateOnly? fechaHasta,
        [FromQuery] string? tipo,
        [FromQuery] string? subtipo,
        [FromQuery] string? etiqueta,
        [FromQuery] string? idioma,
        [FromQuery(Name = "calificacion_min")] short? calificacionMin,
        [FromQuery] string? horario,
        [FromQuery] bool disponible = true,
        [FromQuery(Name = "ordenar_por")] string? ordenarPor = "trending",
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var filtro = new AtraccionFiltroRequest
        {
            Pais = destino,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Tipo = tipo,
            Subtipo = subtipo,
            Etiqueta = etiqueta,
            Idioma = idioma,
            RatingMinimo = calificacionMin,
            Horario = horario,
            SoloDisponibles = disponible,
            OrdenarPor = ordenarPor,
            Page = page,
            Limit = Math.Min(limit, 50)
        };

        var result = await _atracciones.ListarPublicasAsync(filtro, cancellationToken);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }

    [HttpGet("filtros")]
    public async Task<IActionResult> Filtros([FromQuery] string? destino, CancellationToken cancellationToken)
    {
        var data = new
        {
            destination_filters = (await _destinos.ListarAsync(cancellationToken)).Select(x => new { name = x.Nombre, tagname = x.Guid, product_count = 0, image = new { url = x.ImagenUrl } }),
            type_filters = (await _categorias.ListarAsync(cancellationToken)).Select(x => new { name = x.Nombre, tagname = x.TagName ?? x.Guid.ToString(), product_count = 0, child_filter_options = Array.Empty<object>() }),
            label_filters = new[] { new { name = "Cancelacion gratis", tagname = "free_cancellation", product_count = 0 }, new { name = "Sin fila", tagname = "skip_the_line", product_count = 0 } },
            min_rating_filter = new[] { 3.0m, 3.5m, 4.0m, 4.5m }.Select(x => new { name = x.ToString("0.0"), tagname = x.ToString("0.0"), product_count = 0 }),
            time_of_day_filters = new[] { "05:00-12:00", "12:00-18:00", "18:00-05:00" }.Select(x => new { name = x, tagname = x, product_count = 0 }),
            supported_language_filters = (await _idiomas.ListarAsync(cancellationToken)).Select(x => new { name = x.Descripcion, tagname = x.Codigo, product_count = 0 }),
            destino
        };

        return OkEnvelope(data);
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken cancellationToken)
    {
        var response = await _atracciones.ObtenerDetalleCompletoAsync(guid, cancellationToken);
        return OkEnvelope(response);
    }
}
