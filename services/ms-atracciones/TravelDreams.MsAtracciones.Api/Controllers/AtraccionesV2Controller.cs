using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAtracciones.DataAccess.Context;
using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v2/atracciones")]
public sealed class AtraccionesV2Controller : ControllerBase
{
    private static readonly object[] Sorters =
    [
        new { name = "Mas populares", value = "trending" },
        new { name = "Menor precio", value = "lowest_price" },
        new { name = "Mejor calificacion", value = "highest_weighted_rating" }
    ];

    private static readonly object DefaultSorter = new { name = "Mas populares", value = "trending" };
    private readonly AtraccionesDbContext _db;

    public AtraccionesV2Controller(AtraccionesDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? ciudad,
        [FromQuery] string? tipo,
        [FromQuery] string? subtipo,
        [FromQuery] string? etiqueta,
        [FromQuery] string? idioma,
        [FromQuery(Name = "calificacion_min")] decimal? calificacionMin,
        [FromQuery] string? horario,
        [FromQuery] bool? disponible,
        [FromQuery(Name = "ordenar_por")] string? ordenarPor = "trending",
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        CancellationToken ct = default)
    {
        if (page < 1) return BadRequest(Error(400, "Parametro invalido", "El campo 'page' debe ser mayor o igual a 1."));
        if (limit is < 1 or > 50) return BadRequest(Error(400, "Parametro invalido", "El campo 'limit' debe estar entre 1 y 50."));
        if (calificacionMin.HasValue && calificacionMin.Value is not (3.0m or 3.5m or 4.0m or 4.5m))
        {
            return BadRequest(Error(400, "Parametro invalido", "El campo 'calificacion_min' debe ser 3.0, 3.5, 4.0 o 4.5."));
        }

        var unfilteredCount = await _db.Atracciones.CountAsync(x => x.at_estado == "A", ct);
        var query = BuildBaseQuery();

        query = ApplyFilters(query, ciudad, tipo, subtipo, etiqueta, idioma, horario, disponible);
        var total = await query.CountAsync(ct);

        query = (ordenarPor ?? "trending") switch
        {
            "lowest_price" => query.OrderBy(x => x.at_precio_referencia ?? decimal.MaxValue).ThenBy(x => x.at_nombre),
            "highest_weighted_rating" => query.OrderByDescending(x => x.at_total_resenias).ThenBy(x => x.at_nombre),
            "trending" => query.OrderByDescending(x => x.at_total_resenias).ThenBy(x => x.at_nombre),
            _ => query.OrderByDescending(x => x.at_total_resenias).ThenBy(x => x.at_nombre)
        };

        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(ct);
        var data = items.Select(BuildListItem).ToList();

        return Ok(new
        {
            status = 200,
            message = "Consulta exitosa",
            data,
            pagination = new
            {
                page,
                limit,
                total,
                total_pages = (int)Math.Ceiling(total / (double)limit)
            },
            filterStats = new
            {
                filteredProductCount = total,
                unfilteredProductCount = unfilteredCount
            },
            sorters = Sorters,
            defaultSorter = DefaultSorter,
            _links = new
            {
                self = BuildSelfLink("/api/v2/atracciones", new Dictionary<string, object?>
                {
                    ["ciudad"] = ciudad,
                    ["tipo"] = tipo,
                    ["subtipo"] = subtipo,
                    ["etiqueta"] = etiqueta,
                    ["idioma"] = idioma,
                    ["calificacion_min"] = calificacionMin,
                    ["horario"] = horario,
                    ["disponible"] = disponible,
                    ["ordenar_por"] = ordenarPor,
                    ["page"] = page,
                    ["limit"] = limit
                })
            }
        });
    }

    [HttpGet("filtros")]
    public async Task<IActionResult> Filtros([FromQuery] string? ciudad, CancellationToken ct)
    {
        var atracciones = BuildBaseQuery();
        if (!string.IsNullOrWhiteSpace(ciudad))
        {
            atracciones = atracciones.Where(x => x.Destino != null && x.Destino.des_nombre.ToLower().Contains(ciudad.ToLower()));
        }

        var atraccionesList = await atracciones.ToListAsync(ct);
        var atraccionIds = atraccionesList.Select(x => x.at_id).ToHashSet();

        var destinos = await _db.Destinos.AsNoTracking()
            .Where(x => x.des_estado == "A")
            .OrderBy(x => x.des_nombre)
            .Select(x => new
            {
                name = x.des_nombre,
                tagname = Slug(x.des_nombre),
                productCount = x.Atracciones.Count(a => atraccionIds.Contains(a.at_id)),
                imageUrl = x.des_imagen_url
            })
            .ToListAsync(ct);

        var destinationFilters = destinos
            .Select(x => new
            {
                x.name,
                x.tagname,
                x.productCount,
                image = new { url = NormalizeImageUrl(x.imageUrl) },
                childFilterOptions = (object?)null
            })
            .ToList();

        var categorias = await _db.Categorias.AsNoTracking()
            .Where(x => x.cat_estado == "A")
            .OrderBy(x => x.cat_nombre)
            .Select(x => new
            {
                x.cat_id,
                x.cat_parent_id,
                x.cat_nombre,
                x.cat_tagname,
                productCount = x.CategoriaAtracciones.Count(ca => ca.ca_estado == "A" && atraccionIds.Contains(ca.at_id))
            })
            .ToListAsync(ct);

        var typeFilters = categorias
            .Where(x => x.cat_parent_id is null)
            .Select(x => new
            {
                name = x.cat_nombre,
                tagname = x.cat_tagname ?? Slug(x.cat_nombre),
                productCount = x.productCount,
                childFilterOptions = categorias
                    .Where(c => c.cat_parent_id == x.cat_id)
                    .Select(c => new { name = c.cat_nombre, tagname = c.cat_tagname ?? Slug(c.cat_nombre), productCount = c.productCount })
                    .ToList()
            })
            .ToList();

        var labelFilters = new[]
        {
            new { name = "Cancelacion gratuita", tagname = "free_cancellation", productCount = atraccionesList.Count(x => x.at_free_cancellation) },
            new { name = "Sin fila", tagname = "skip_the_line", productCount = atraccionesList.Count(x => x.at_skip_the_line) }
        };

        var timeOfDayFilters = new[]
        {
            new { name = "Manana (05:00-12:00)", tagname = "05:00-12:00", productCount = CountByTime(atraccionesList, new TimeOnly(5, 0), new TimeOnly(12, 0)) },
            new { name = "Tarde (12:00-18:00)", tagname = "12:00-18:00", productCount = CountByTime(atraccionesList, new TimeOnly(12, 0), new TimeOnly(18, 0)) },
            new { name = "Noche (18:00-05:00)", tagname = "18:00-05:00", productCount = CountByTime(atraccionesList, new TimeOnly(18, 0), new TimeOnly(5, 0)) }
        };

        var supportedLanguageFilters = await _db.Idiomas.AsNoTracking()
            .Where(x => x.id_estado == "A")
            .OrderBy(x => x.id_descripcion)
            .Select(x => new
            {
                name = x.id_descripcion,
                tagname = x.id_codigo,
                productCount = x.IdiomaAtracciones.Count(ia => ia.ia_estado == "A" && atraccionIds.Contains(ia.at_id))
            })
            .ToListAsync(ct);

        return Ok(new
        {
            status = 200,
            message = "Operacion exitosa",
            data = new
            {
                destinationFilters,
                typeFilters,
                labelFilters,
                minRatingFilter = new[]
                {
                    new { name = "4.5+", tagname = "4.5", productCount = 0 },
                    new { name = "4.0+", tagname = "4.0", productCount = 0 },
                    new { name = "3.5+", tagname = "3.5", productCount = 0 },
                    new { name = "3.0+", tagname = "3.0", productCount = 0 }
                },
                timeOfDayFilters,
                supportedLanguageFilters
            }
        });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var atraccion = await BuildBaseQuery().FirstOrDefaultAsync(x => x.at_guid == guid, ct);
        if (atraccion is null)
        {
            return NotFound(new { status = 404, error = "Atraccion no encontrada o inactiva." });
        }

        var listItem = BuildListItem(atraccion);
        var tickets = atraccion.Tickets
            .Where(x => x.tck_estado == "A")
            .OrderBy(x => x.tck_precio)
            .Select(x => new { tck_guid = x.tck_guid, tipo = x.tck_tipo_participante, precio = x.tck_precio, moneda = x.tck_moneda })
            .ToList();
        var horarios = atraccion.Horarios
            .Where(IsHorarioDisponible)
            .OrderBy(x => x.hor_fecha)
            .ThenBy(x => x.hor_hora_inicio)
            .Take(5)
            .Select(x => new { hor_guid = x.hor_guid, fecha = x.hor_fecha, hora_inicio = FormatTime(x.hor_hora_inicio), hora_fin = FormatTime(x.hor_hora_fin), cupos = x.hor_cupos_disponibles })
            .ToList();

        return Ok(new
        {
            status = 200,
            message = "Operacion exitosa",
            data = new
            {
                listItem.id,
                listItem.nombre,
                listItem.ciudad,
                listItem.pais,
                listItem.tipo_tagname,
                listItem.tipo_nombre,
                listItem.subtipo_tagname,
                listItem.subtipo_nombre,
                listItem.etiquetas,
                listItem.descripcion_corta,
                listItem.imagen_principal,
                listItem.duracion_minutos,
                listItem.precio_desde,
                listItem.moneda,
                listItem.calificacion,
                listItem.total_resenas,
                listItem.idiomas_disponibles,
                listItem.disponibilidad,
                descripcion = atraccion.at_descripcion,
                imagenes = atraccion.ImagenAtracciones
                    .Where(x => x.ima_estado == "A" && x.Imagen != null && x.Imagen.img_estado == "A")
                    .OrderByDescending(x => x.ima_es_principal)
                    .ThenBy(x => x.ima_orden)
                    .Select(x => NormalizeImageUrl(x.Imagen!.img_url))
                    .Where(x => x != null)
                    .ToList(),
                incluye = atraccion.AtraccionIncluyes.Where(x => x.ai_estado == "A" && x.Incluye != null && x.Incluye.inc_estado == "A" && x.Incluye.inc_tipo == "INCLUYE").Select(x => x.Incluye!.inc_descripcion).ToList(),
                no_incluye = atraccion.AtraccionIncluyes.Where(x => x.ai_estado == "A" && x.Incluye != null && x.Incluye.inc_estado == "A" && x.Incluye.inc_tipo != "INCLUYE").Select(x => x.Incluye!.inc_descripcion).ToList(),
                punto_encuentro = atraccion.at_punto_encuentro,
                incluye_transporte = atraccion.at_incluye_transporte,
                incluye_acompaniante = atraccion.at_incluye_acompaniante,
                tickets,
                horarios_proximos = horarios,
                _links = new { self = $"/api/v2/atracciones/{guid}" }
            }
        });
    }

    [HttpGet("{guid:guid}/tickets")]
    public async Task<IActionResult> Tickets(Guid guid, CancellationToken ct)
    {
        var exists = await _db.Atracciones.AnyAsync(x => x.at_guid == guid && x.at_estado == "A", ct);
        if (!exists) return NotFound(new { status = 404, error = "Atraccion no encontrada." });

        var data = await _db.Tickets.AsNoTracking()
            .Where(x => x.tck_estado == "A" && x.Atraccion != null && x.Atraccion.at_guid == guid)
            .OrderBy(x => x.tck_precio)
            .Select(x => new { tck_guid = x.tck_guid, tipo = x.tck_tipo_participante, precio = x.tck_precio, moneda = x.tck_moneda })
            .ToListAsync(ct);

        return Ok(new { status = 200, message = "Tickets disponibles consultados correctamente.", data });
    }

    [HttpGet("{guid:guid}/horarios")]
    public async Task<IActionResult> Horarios(Guid guid, CancellationToken ct)
    {
        var exists = await _db.Atracciones.AnyAsync(x => x.at_guid == guid && x.at_estado == "A", ct);
        if (!exists) return NotFound(new { status = 404, error = "Atraccion no encontrada." });

        var data = await _db.Horarios.AsNoTracking()
            .Where(x => x.hor_estado == "A" && x.hor_cupos_disponibles > 0 && x.hor_fecha >= DateOnly.FromDateTime(DateTime.UtcNow) && x.Atraccion != null && x.Atraccion.at_guid == guid)
            .OrderBy(x => x.hor_fecha)
            .ThenBy(x => x.hor_hora_inicio)
            .Select(x => new { hor_guid = x.hor_guid, fecha = x.hor_fecha, hora_inicio = FormatTime(x.hor_hora_inicio), hora_fin = FormatTime(x.hor_hora_fin), cupos = x.hor_cupos_disponibles })
            .ToListAsync(ct);

        return Ok(new { status = 200, message = "Horarios disponibles consultados correctamente.", data });
    }

    [HttpGet("{guid:guid}/horarios/{horarioGuid:guid}/tickets")]
    public async Task<IActionResult> TicketsPorHorario(Guid guid, Guid horarioGuid, CancellationToken ct)
    {
        var horario = await _db.Horarios.AsNoTracking()
            .FirstOrDefaultAsync(x => x.hor_guid == horarioGuid && x.hor_estado == "A" && x.hor_cupos_disponibles > 0 && x.Atraccion != null && x.Atraccion.at_guid == guid && x.Atraccion.at_estado == "A", ct);
        if (horario is null) return NotFound(new { status = 404, error = "Atraccion o horario no encontrado." });

        var items = await _db.Tickets.AsNoTracking()
            .Where(x => x.tck_estado == "A" && x.Atraccion != null && x.Atraccion.at_guid == guid)
            .OrderBy(x => x.tck_precio)
            .Select(x => new { tck_guid = x.tck_guid, tipo = x.tck_tipo_participante, precio = x.tck_precio, moneda = x.tck_moneda })
            .ToListAsync(ct);

        return Ok(new { status = 200, message = "Tickets disponibles para el horario consultados correctamente.", data = new { items } });
    }

    private IQueryable<AtraccionEntity> BuildBaseQuery() =>
        _db.Atracciones
            .AsNoTracking()
            .Include(x => x.Destino)
            .Include(x => x.CategoriaAtracciones).ThenInclude(x => x.Categoria)
            .Include(x => x.IdiomaAtracciones).ThenInclude(x => x.Idioma)
            .Include(x => x.ImagenAtracciones).ThenInclude(x => x.Imagen)
            .Include(x => x.AtraccionIncluyes).ThenInclude(x => x.Incluye)
            .Include(x => x.Tickets)
            .Include(x => x.Horarios)
            .Where(x => x.at_estado == "A" && x.at_disponible);

    private static IQueryable<AtraccionEntity> ApplyFilters(
        IQueryable<AtraccionEntity> query,
        string? ciudad,
        string? tipo,
        string? subtipo,
        string? etiqueta,
        string? idioma,
        string? horario,
        bool? disponible)
    {
        if (!string.IsNullOrWhiteSpace(ciudad))
        {
            var value = ciudad.ToLower();
            query = query.Where(x => x.Destino != null && x.Destino.des_nombre.ToLower().Contains(value));
        }

        query = ApplyTypeFilter(query, tipo);
        query = ApplySubtypeFilter(query, subtipo);

        if (!string.IsNullOrWhiteSpace(etiqueta))
        {
            query = etiqueta switch
            {
                "free_cancellation" => query.Where(x => x.at_free_cancellation),
                "skip_the_line" => query.Where(x => x.at_skip_the_line),
                _ => query.Where(x => false)
            };
        }

        if (!string.IsNullOrWhiteSpace(idioma))
        {
            query = query.Where(x => x.IdiomaAtracciones.Any(i => i.ia_estado == "A" && i.Idioma != null && i.Idioma.id_codigo == idioma));
        }

        if (!string.IsNullOrWhiteSpace(horario) && TryParseRange(horario, out var start, out var end))
        {
            query = query.Where(x => x.Horarios.Any(h => h.hor_estado == "A" && h.hor_cupos_disponibles > 0 && (start <= end
                ? h.hor_hora_inicio >= start && h.hor_hora_inicio < end
                : h.hor_hora_inicio >= start || h.hor_hora_inicio < end)));
        }

        if (disponible.HasValue)
        {
            query = query.Where(x => x.at_disponible == disponible.Value);
        }

        return query;
    }

    private static IQueryable<AtraccionEntity> ApplyTypeFilter(IQueryable<AtraccionEntity> query, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter)) return query;

        if (Guid.TryParse(filter, out var guid))
        {
            return query.Where(x => x.CategoriaAtracciones.Any(c =>
                c.ca_estado == "A"
                && c.Categoria != null
                && c.Categoria.cat_estado == "A"
                && c.Categoria.cat_parent_id == null
                && c.Categoria.cat_guid == guid));
        }

        return query.Where(x => x.CategoriaAtracciones.Any(c =>
            c.ca_estado == "A"
            && c.Categoria != null
            && c.Categoria.cat_estado == "A"
            && c.Categoria.cat_parent_id == null
            && c.Categoria.cat_tagname == filter));
    }

    private static IQueryable<AtraccionEntity> ApplySubtypeFilter(IQueryable<AtraccionEntity> query, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter)) return query;

        if (Guid.TryParse(filter, out var guid))
        {
            return query.Where(x => x.CategoriaAtracciones.Any(c =>
                c.ca_estado == "A"
                && c.Categoria != null
                && c.Categoria.cat_estado == "A"
                && c.Categoria.cat_parent_id != null
                && c.Categoria.cat_guid == guid));
        }

        return query.Where(x => x.CategoriaAtracciones.Any(c =>
            c.ca_estado == "A"
            && c.Categoria != null
            && c.Categoria.cat_estado == "A"
            && c.Categoria.cat_parent_id != null
            && c.Categoria.cat_tagname == filter));
    }

    private static dynamic BuildListItem(AtraccionEntity atraccion)
    {
        var categorias = atraccion.CategoriaAtracciones.Where(x => x.ca_estado == "A" && x.Categoria != null && x.Categoria.cat_estado == "A").Select(x => x.Categoria!).ToList();
        var tipo = categorias.FirstOrDefault(x => x.cat_parent_id is null) ?? categorias.FirstOrDefault();
        var subtipo = categorias.FirstOrDefault(x => tipo is not null && x.cat_parent_id == tipo.cat_id);
        var horariosDisponibles = atraccion.Horarios.Where(IsHorarioDisponible).OrderBy(x => x.hor_fecha).ThenBy(x => x.hor_hora_inicio).ToList();
        var imagenPrincipal = atraccion.ImagenAtracciones
            .Where(x => x.ima_estado == "A" && x.Imagen != null && x.Imagen.img_estado == "A")
            .OrderByDescending(x => x.ima_es_principal)
            .ThenBy(x => x.ima_orden)
            .Select(x => NormalizeImageUrl(x.Imagen!.img_url))
            .FirstOrDefault(x => x != null);
        var etiquetas = new List<string>();
        if (atraccion.at_free_cancellation) etiquetas.Add("free_cancellation");
        if (atraccion.at_skip_the_line) etiquetas.Add("skip_the_line");

        return new
        {
            id = atraccion.at_guid,
            nombre = atraccion.at_nombre,
            ciudad = atraccion.Destino?.des_nombre,
            pais = atraccion.Destino?.des_pais,
            tipo_tagname = tipo?.cat_tagname,
            tipo_nombre = tipo?.cat_nombre,
            subtipo_tagname = subtipo?.cat_tagname,
            subtipo_nombre = subtipo?.cat_nombre,
            etiquetas,
            descripcion_corta = ShortDescription(atraccion.at_descripcion),
            imagen_principal = imagenPrincipal,
            duracion_minutos = atraccion.at_duracion_minutos,
            precio_desde = atraccion.Tickets.Where(x => x.tck_estado == "A").Select(x => (decimal?)x.tck_precio).Min() ?? atraccion.at_precio_referencia ?? 0,
            moneda = atraccion.Tickets.FirstOrDefault(x => x.tck_estado == "A")?.tck_moneda ?? "USD",
            calificacion = atraccion.at_total_resenias > 0 ? 4.5m : 0m,
            total_resenas = atraccion.at_total_resenias,
            idiomas_disponibles = atraccion.IdiomaAtracciones.Where(x => x.ia_estado == "A" && x.Idioma != null && x.Idioma.id_estado == "A").Select(x => x.Idioma!.id_codigo).Distinct().ToList(),
            disponibilidad = new
            {
                disponible = atraccion.at_disponible && horariosDisponibles.Count > 0,
                disponible_hoy = horariosDisponibles.Any(x => x.hor_fecha == DateOnly.FromDateTime(DateTime.UtcNow)),
                proxima_fecha_disponible = horariosDisponibles.FirstOrDefault()?.hor_fecha,
                cupos_disponibles = horariosDisponibles.Sum(x => x.hor_cupos_disponibles)
            },
            _links = new { self = $"/api/v2/atracciones/{atraccion.at_guid}" }
        };
    }

    private static bool IsHorarioDisponible(TravelDreams.MsAtracciones.DataAccess.Entities.Operacion.HorarioEntity horario) =>
        horario.hor_estado == "A"
        && horario.hor_cupos_disponibles > 0
        && horario.hor_fecha >= DateOnly.FromDateTime(DateTime.UtcNow);

    private static int CountByTime(IEnumerable<AtraccionEntity> atracciones, TimeOnly start, TimeOnly end) =>
        atracciones.Count(a => a.Horarios.Any(h => IsHorarioDisponible(h) && (start <= end
            ? h.hor_hora_inicio >= start && h.hor_hora_inicio < end
            : h.hor_hora_inicio >= start || h.hor_hora_inicio < end)));

    private static bool TryParseRange(string value, out TimeOnly start, out TimeOnly end)
    {
        start = default;
        end = default;
        var parts = value.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 && TimeOnly.TryParse(parts[0], out start) && TimeOnly.TryParse(parts[1], out end);
    }

    private static string? ShortDescription(string? description) =>
        string.IsNullOrWhiteSpace(description)
            ? description
            : description.Length <= 150 ? description : description[..150];

    private static string Slug(string value) => value.Trim().ToLowerInvariant().Replace(' ', '-');

    private static string? FormatTime(TimeOnly? time) => time?.ToString("HH:mm");

    private static string? NormalizeImageUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (value.Contains("example.com", StringComparison.OrdinalIgnoreCase)) return null;
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)) return null;
        if (uri.Scheme is not ("http" or "https")) return null;
        return value;
    }

    private static string BuildSelfLink(string path, IReadOnlyDictionary<string, object?> values)
    {
        var query = values
            .Where(x => x.Value is not null && !string.IsNullOrWhiteSpace(Convert.ToString(x.Value, CultureInfo.InvariantCulture)))
            .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(ToQueryValue(x.Value!))}");

        var queryString = string.Join("&", query);
        return string.IsNullOrWhiteSpace(queryString) ? path : $"{path}?{queryString}";
    }

    private static string ToQueryValue(object value) => value switch
    {
        bool boolean => boolean.ToString().ToLowerInvariant(),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty
    };

    private static object Error(int status, string error, string detail) => new
    {
        status,
        error,
        details = new[] { detail },
        timestamp = DateTime.UtcNow,
        path = string.Empty
    };
}
