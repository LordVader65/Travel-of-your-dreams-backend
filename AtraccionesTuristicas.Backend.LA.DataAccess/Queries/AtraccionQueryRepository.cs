using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries;

public sealed class AtraccionQueryRepository : IAtraccionQueryRepository
{
    private readonly AtraccionesDbContext _context;

    public AtraccionQueryRepository(AtraccionesDbContext context) => _context = context;

    public async Task<PagedResult<AtraccionEntity>> ListarPublicasAsync(
        int page,
        int limit,
        string? pais = null,
        string? tipo = null,
        string? subtipo = null,
        string? etiqueta = null,
        string? idioma = null,
        decimal? precioMinimo = null,
        decimal? precioMaximo = null,
        short? ratingMinimo = null,
        string? horario = null,
        string? ordenarPor = null,
        bool soloDisponibles = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Atracciones
            .AsNoTracking()
            .Include(x => x.Destino)
            .Include(x => x.CategoriaAtracciones).ThenInclude(x => x.Categoria)
            .Include(x => x.IdiomaAtracciones).ThenInclude(x => x.Idioma)
            .Include(x => x.AtraccionIncluyes).ThenInclude(x => x.Incluye)
            .Where(x => x.at_estado == DatabaseConstants.EstadoActivo);

        if (soloDisponibles)
            query = query.Where(x => x.at_disponible);
        if (!string.IsNullOrWhiteSpace(pais))
            query = query.Where(x => x.Destino != null && x.Destino.des_pais == pais);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(x => x.CategoriaAtracciones.Any(c => c.Categoria != null && c.Categoria.cat_tagname == tipo));
        if (!string.IsNullOrWhiteSpace(subtipo))
            query = query.Where(x => x.CategoriaAtracciones.Any(c => c.Categoria != null && c.Categoria.cat_tagname == subtipo));
        if (!string.IsNullOrWhiteSpace(etiqueta))
            query = query.Where(x => x.AtraccionIncluyes.Any(i => i.Incluye != null && i.Incluye.inc_tipo == DatabaseConstants.Etiqueta && i.Incluye.inc_descripcion == etiqueta));
        if (!string.IsNullOrWhiteSpace(idioma))
            query = query.Where(x => x.IdiomaAtracciones.Any(i => i.Idioma != null && i.Idioma.id_codigo == idioma));
        if (precioMinimo.HasValue)
            query = query.Where(x => x.at_precio_referencia >= precioMinimo.Value || x.Tickets.Any(t => t.tck_estado == DatabaseConstants.EstadoActivo && t.tck_precio >= precioMinimo.Value));
        if (precioMaximo.HasValue)
            query = query.Where(x => x.at_precio_referencia <= precioMaximo.Value || x.Tickets.Any(t => t.tck_estado == DatabaseConstants.EstadoActivo && t.tck_precio <= precioMaximo.Value));
        if (ratingMinimo.HasValue)
            query = query.Where(x => x.Resenias.Where(r => r.rsn_estado == DatabaseConstants.EstadoActivo).Average(r => (double?)r.rsn_rating) >= ratingMinimo.Value);
        if (!string.IsNullOrWhiteSpace(horario))
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            query = horario switch
            {
                "05:00-12:00" => query.Where(x => x.Horarios.Any(h => h.hor_estado == DatabaseConstants.EstadoActivo && h.hor_cupos_disponibles > 0 && h.hor_fecha >= today && h.hor_hora_inicio >= new TimeOnly(5, 0) && h.hor_hora_inicio < new TimeOnly(12, 0))),
                "12:00-18:00" => query.Where(x => x.Horarios.Any(h => h.hor_estado == DatabaseConstants.EstadoActivo && h.hor_cupos_disponibles > 0 && h.hor_fecha >= today && h.hor_hora_inicio >= new TimeOnly(12, 0) && h.hor_hora_inicio < new TimeOnly(18, 0))),
                "18:00-05:00" => query.Where(x => x.Horarios.Any(h => h.hor_estado == DatabaseConstants.EstadoActivo && h.hor_cupos_disponibles > 0 && h.hor_fecha >= today && (h.hor_hora_inicio >= new TimeOnly(18, 0) || h.hor_hora_inicio < new TimeOnly(5, 0)))),
                _ => query
            };
        }

        query = ordenarPor switch
        {
            "lowest_price" => query.OrderBy(x => x.at_precio_referencia ?? x.Tickets.Where(t => t.tck_estado == DatabaseConstants.EstadoActivo).Min(t => (decimal?)t.tck_precio) ?? 0),
            "highest_weighted_rating" => query.OrderByDescending(x => x.Resenias.Where(r => r.rsn_estado == DatabaseConstants.EstadoActivo).Average(r => (double?)r.rsn_rating) ?? 0).ThenByDescending(x => x.at_total_resenias),
            "trending" => query.OrderByDescending(x => x.at_total_resenias).ThenBy(x => x.at_nombre),
            _ => query.OrderBy(x => x.at_nombre)
        };
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<AtraccionEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }

    public Task<AtraccionEntity?> ObtenerDetallePublicoAsync(Guid guid, CancellationToken cancellationToken = default) =>
        _context.Atracciones.AsNoTracking()
            .Include(x => x.Destino)
            .Include(x => x.ImagenAtracciones).ThenInclude(x => x.Imagen)
            .Include(x => x.Tickets)
            .Include(x => x.Horarios)
            .FirstOrDefaultAsync(x => x.at_guid == guid && x.at_estado == DatabaseConstants.EstadoActivo, cancellationToken);
}
