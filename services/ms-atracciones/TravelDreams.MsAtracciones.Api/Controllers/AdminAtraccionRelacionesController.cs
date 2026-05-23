using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAtracciones.DataAccess.Context;
using TravelDreams.MsAtracciones.DataAccess.Entities.CatalogoRelaciones;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/admin/atracciones/{atraccionId:int}")]
public sealed class AdminAtraccionRelacionesController : ControllerBase
{
    private readonly AtraccionesDbContext _db;

    public AdminAtraccionRelacionesController(AtraccionesDbContext db) => _db = db;

    [HttpGet("caracteristicas")]
    public async Task<IActionResult> Caracteristicas(int atraccionId, CancellationToken ct)
    {
        await EnsureAtraccionAsync(atraccionId, ct);

        var categorias = await _db.CategoriaAtracciones.AsNoTracking()
            .Where(x => x.at_id == atraccionId && x.ca_estado == "A")
            .Select(x => new { id = x.cat_id, es_principal = x.ca_es_principal })
            .ToListAsync(ct);
        var idiomas = await _db.IdiomaAtracciones.AsNoTracking()
            .Where(x => x.at_id == atraccionId && x.ia_estado == "A")
            .Select(x => new { id = x.id_id })
            .ToListAsync(ct);
        var incluye = await _db.AtraccionIncluyes.AsNoTracking()
            .Where(x => x.at_id == atraccionId && x.ai_estado == "A")
            .Select(x => new { id = x.inc_id })
            .ToListAsync(ct);
        var imagenes = await _db.ImagenAtracciones.AsNoTracking()
            .Where(x => x.at_id == atraccionId && x.ima_estado == "A")
            .OrderByDescending(x => x.ima_es_principal)
            .ThenBy(x => x.ima_orden)
            .Select(x => new { id = x.img_id, es_principal = x.ima_es_principal, orden = x.ima_orden })
            .ToListAsync(ct);

        return Ok(new { status = 200, data = new { categorias, idiomas, imagenes, incluye } });
    }

    [HttpPost("categorias/{categoriaId:int}")]
    public async Task<IActionResult> AsociarCategoria(int atraccionId, int categoriaId, [FromQuery] bool esPrincipal = false, CancellationToken ct = default)
    {
        await EnsureAtraccionAsync(atraccionId, ct);
        if (!await _db.Categorias.AnyAsync(x => x.cat_id == categoriaId && x.cat_estado == "A", ct))
        {
            return NotFound(new { status = 404, error = "Categoria no encontrada." });
        }

        if (esPrincipal)
        {
            await _db.CategoriaAtracciones
                .Where(x => x.at_id == atraccionId && x.ca_estado == "A")
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.ca_es_principal, false), ct);
        }

        var relation = await _db.CategoriaAtracciones.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.cat_id == categoriaId, ct);
        if (relation is null)
        {
            relation = new CategoriaAtraccionEntity { at_id = atraccionId, cat_id = categoriaId, ca_usuario_ingreso = "admin" };
            _db.CategoriaAtracciones.Add(relation);
        }

        relation.ca_estado = "A";
        relation.ca_es_principal = esPrincipal || relation.ca_es_principal;
        relation.ca_fecha_eliminacion = null;
        relation.ca_usuario_eliminacion = null;
        await _db.SaveChangesAsync(ct);
        return Ok(new { status = 200, message = "Categoria asociada correctamente." });
    }

    [HttpDelete("categorias/{categoriaId:int}")]
    public async Task<IActionResult> DesasociarCategoria(int atraccionId, int categoriaId, CancellationToken ct)
    {
        var relation = await _db.CategoriaAtracciones.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.cat_id == categoriaId, ct);
        if (relation is null) return NoContent();
        relation.ca_estado = "I";
        relation.ca_es_principal = false;
        relation.ca_fecha_eliminacion = DateTime.UtcNow;
        relation.ca_usuario_eliminacion = "admin";
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("idiomas/{idiomaId:int}")]
    public async Task<IActionResult> AsociarIdioma(int atraccionId, int idiomaId, CancellationToken ct)
    {
        await EnsureAtraccionAsync(atraccionId, ct);
        if (!await _db.Idiomas.AnyAsync(x => x.id_id == idiomaId && x.id_estado == "A", ct))
        {
            return NotFound(new { status = 404, error = "Idioma no encontrado." });
        }

        var relation = await _db.IdiomaAtracciones.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.id_id == idiomaId, ct);
        if (relation is null)
        {
            relation = new IdiomaAtraccionEntity { at_id = atraccionId, id_id = idiomaId, ia_usuario_ingreso = "admin" };
            _db.IdiomaAtracciones.Add(relation);
        }

        relation.ia_estado = "A";
        relation.ia_fecha_eliminacion = null;
        relation.ia_usuario_eliminacion = null;
        await _db.SaveChangesAsync(ct);
        return Ok(new { status = 200, message = "Idioma asociado correctamente." });
    }

    [HttpDelete("idiomas/{idiomaId:int}")]
    public async Task<IActionResult> DesasociarIdioma(int atraccionId, int idiomaId, CancellationToken ct)
    {
        var relation = await _db.IdiomaAtracciones.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.id_id == idiomaId, ct);
        if (relation is null) return NoContent();
        relation.ia_estado = "I";
        relation.ia_fecha_eliminacion = DateTime.UtcNow;
        relation.ia_usuario_eliminacion = "admin";
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("incluye/{incluyeId:int}")]
    public async Task<IActionResult> AsociarIncluye(int atraccionId, int incluyeId, CancellationToken ct)
    {
        await EnsureAtraccionAsync(atraccionId, ct);
        if (!await _db.Incluyes.AnyAsync(x => x.inc_id == incluyeId && x.inc_estado == "A", ct))
        {
            return NotFound(new { status = 404, error = "Incluye/no incluye no encontrado." });
        }

        var relation = await _db.AtraccionIncluyes.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.inc_id == incluyeId, ct);
        if (relation is null)
        {
            relation = new AtraccionIncluyeEntity { at_id = atraccionId, inc_id = incluyeId, ai_usuario_ingreso = "admin" };
            _db.AtraccionIncluyes.Add(relation);
        }

        relation.ai_estado = "A";
        relation.ai_fecha_eliminacion = null;
        relation.ai_usuario_eliminacion = null;
        await _db.SaveChangesAsync(ct);
        return Ok(new { status = 200, message = "Incluye/no incluye asociado correctamente." });
    }

    [HttpDelete("incluye/{incluyeId:int}")]
    public async Task<IActionResult> DesasociarIncluye(int atraccionId, int incluyeId, CancellationToken ct)
    {
        var relation = await _db.AtraccionIncluyes.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.inc_id == incluyeId, ct);
        if (relation is null) return NoContent();
        relation.ai_estado = "I";
        relation.ai_fecha_eliminacion = DateTime.UtcNow;
        relation.ai_usuario_eliminacion = "admin";
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("imagenes/{imagenId:int}")]
    public async Task<IActionResult> AsociarImagen(int atraccionId, int imagenId, [FromQuery] bool esPrincipal = false, [FromQuery] int orden = 1, CancellationToken ct = default)
    {
        await EnsureAtraccionAsync(atraccionId, ct);
        if (!await _db.Imagenes.AnyAsync(x => x.img_id == imagenId && x.img_estado == "A", ct))
        {
            return NotFound(new { status = 404, error = "Imagen no encontrada." });
        }

        if (esPrincipal)
        {
            await _db.ImagenAtracciones
                .Where(x => x.at_id == atraccionId && x.ima_estado == "A")
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.ima_es_principal, false), ct);
        }

        var relation = await _db.ImagenAtracciones.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.img_id == imagenId, ct);
        if (relation is null)
        {
            relation = new ImagenAtraccionEntity { at_id = atraccionId, img_id = imagenId, ima_usuario_ingreso = "admin" };
            _db.ImagenAtracciones.Add(relation);
        }

        relation.ima_es_principal = esPrincipal;
        relation.ima_orden = orden;
        relation.ima_estado = "A";
        relation.ima_fecha_eliminacion = null;
        relation.ima_usuario_eliminacion = null;
        await _db.SaveChangesAsync(ct);
        return Ok(new { status = 200, message = "Imagen asociada correctamente." });
    }

    [HttpDelete("imagenes/{imagenId:int}")]
    public async Task<IActionResult> DesasociarImagen(int atraccionId, int imagenId, CancellationToken ct)
    {
        var relation = await _db.ImagenAtracciones.FirstOrDefaultAsync(x => x.at_id == atraccionId && x.img_id == imagenId, ct);
        if (relation is null) return NoContent();
        relation.ima_estado = "I";
        relation.ima_es_principal = false;
        relation.ima_fecha_eliminacion = DateTime.UtcNow;
        relation.ima_usuario_eliminacion = "admin";
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private async Task EnsureAtraccionAsync(int atraccionId, CancellationToken ct)
    {
        if (!await _db.Atracciones.AnyAsync(x => x.at_id == atraccionId && x.at_estado == "A", ct))
        {
            throw new InvalidOperationException("Atraccion no encontrada.");
        }
    }
}
