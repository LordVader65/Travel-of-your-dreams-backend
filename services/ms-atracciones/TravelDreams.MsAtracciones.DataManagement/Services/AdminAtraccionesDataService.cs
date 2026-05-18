using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAtracciones.DataAccess.Context;
using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;
using TravelDreams.MsAtracciones.DataAccess.Entities.Operacion;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Admin;

namespace TravelDreams.MsAtracciones.DataManagement.Services;

public sealed class AdminAtraccionesDataService : IAdminAtraccionesDataService
{
    private readonly AtraccionesDbContext _db;

    public AdminAtraccionesDataService(AtraccionesDbContext db) => _db = db;

    public async Task<IReadOnlyList<CatalogoItemDataModel>> ListarDestinosAsync(CancellationToken ct = default) =>
        await _db.Destinos.AsNoTracking().Where(x => x.des_estado == "A").OrderBy(x => x.des_nombre)
            .Select(x => new CatalogoItemDataModel { Id = x.des_id, Guid = x.des_guid, Nombre = x.des_nombre, Pais = x.des_pais, ImagenUrl = x.des_imagen_url, Estado = x.des_estado }).ToListAsync(ct);

    public async Task<CatalogoItemDataModel> GuardarDestinoAsync(CatalogoUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Id.HasValue ? await _db.Destinos.FirstOrDefaultAsync(x => x.des_id == model.Id, ct) : null;
        if (entity is null)
        {
            entity = new DestinoEntity { des_nombre = model.Nombre, des_pais = model.Pais ?? string.Empty, des_imagen_url = model.ImagenUrl, des_usuario_ingreso = model.Usuario, des_ip_ingreso = model.Ip };
            _db.Destinos.Add(entity);
        }
        else
        {
            entity.des_nombre = model.Nombre; entity.des_pais = model.Pais ?? entity.des_pais; entity.des_imagen_url = model.ImagenUrl; entity.des_fecha_mod = DateTime.UtcNow; entity.des_usuario_mod = model.Usuario; entity.des_ip_mod = model.Ip;
        }
        await _db.SaveChangesAsync(ct);
        return new CatalogoItemDataModel { Id = entity.des_id, Guid = entity.des_guid, Nombre = entity.des_nombre, Pais = entity.des_pais, ImagenUrl = entity.des_imagen_url, Estado = entity.des_estado };
    }

    public async Task<IReadOnlyList<CatalogoItemDataModel>> ListarCategoriasAsync(CancellationToken ct = default) =>
        await _db.Categorias.AsNoTracking().Where(x => x.cat_estado == "A").OrderBy(x => x.cat_nombre)
            .Select(x => new CatalogoItemDataModel { Id = x.cat_id, Guid = x.cat_guid, Nombre = x.cat_nombre, Codigo = x.cat_tagname, ParentId = x.cat_parent_id, Estado = x.cat_estado }).ToListAsync(ct);

    public async Task<CatalogoItemDataModel> GuardarCategoriaAsync(CatalogoUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Id.HasValue ? await _db.Categorias.FirstOrDefaultAsync(x => x.cat_id == model.Id, ct) : null;
        if (entity is null)
        {
            entity = new CategoriaEntity { cat_nombre = model.Nombre, cat_tagname = model.Codigo, cat_parent_id = model.ParentId, cat_usuario_ingreso = model.Usuario, cat_ip_ingreso = model.Ip };
            _db.Categorias.Add(entity);
        }
        else
        {
            entity.cat_nombre = model.Nombre; entity.cat_tagname = model.Codigo; entity.cat_parent_id = model.ParentId; entity.cat_fecha_mod = DateTime.UtcNow; entity.cat_usuario_mod = model.Usuario; entity.cat_ip_mod = model.Ip;
        }
        await _db.SaveChangesAsync(ct);
        return new CatalogoItemDataModel { Id = entity.cat_id, Guid = entity.cat_guid, Nombre = entity.cat_nombre, Codigo = entity.cat_tagname, ParentId = entity.cat_parent_id, Estado = entity.cat_estado };
    }

    public async Task<IReadOnlyList<CatalogoItemDataModel>> ListarIdiomasAsync(CancellationToken ct = default) =>
        await _db.Idiomas.AsNoTracking().Where(x => x.id_estado == "A").OrderBy(x => x.id_descripcion)
            .Select(x => new CatalogoItemDataModel { Id = x.id_id, Guid = x.id_guid, Nombre = x.id_descripcion, Codigo = x.id_codigo, Estado = x.id_estado }).ToListAsync(ct);

    public async Task<CatalogoItemDataModel> GuardarIdiomaAsync(CatalogoUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Id.HasValue ? await _db.Idiomas.FirstOrDefaultAsync(x => x.id_id == model.Id, ct) : null;
        if (entity is null)
        {
            entity = new IdiomaEntity { id_codigo = model.Codigo ?? model.Nombre, id_descripcion = model.Nombre, id_usuario_ingreso = model.Usuario, id_ip_ingreso = model.Ip };
            _db.Idiomas.Add(entity);
        }
        else
        {
            entity.id_codigo = model.Codigo ?? entity.id_codigo; entity.id_descripcion = model.Nombre; entity.id_fecha_mod = DateTime.UtcNow; entity.id_usuario_mod = model.Usuario; entity.id_ip_mod = model.Ip;
        }
        await _db.SaveChangesAsync(ct);
        return new CatalogoItemDataModel { Id = entity.id_id, Guid = entity.id_guid, Nombre = entity.id_descripcion, Codigo = entity.id_codigo, Estado = entity.id_estado };
    }

    public async Task<IReadOnlyList<CatalogoItemDataModel>> ListarImagenesAsync(CancellationToken ct = default) =>
        await _db.Imagenes.AsNoTracking().Where(x => x.img_estado == "A").OrderBy(x => x.img_id)
            .Select(x => new CatalogoItemDataModel { Id = x.img_id, Guid = x.img_guid, Nombre = x.img_descripcion ?? x.img_url, ImagenUrl = x.img_url, Descripcion = x.img_descripcion, Estado = x.img_estado }).ToListAsync(ct);

    public async Task<CatalogoItemDataModel> GuardarImagenAsync(CatalogoUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Id.HasValue ? await _db.Imagenes.FirstOrDefaultAsync(x => x.img_id == model.Id, ct) : null;
        if (entity is null)
        {
            entity = new ImagenEntity { img_url = model.ImagenUrl ?? model.Nombre, img_descripcion = model.Descripcion, img_usuario_ingreso = model.Usuario, img_ip_ingreso = model.Ip };
            _db.Imagenes.Add(entity);
        }
        else
        {
            entity.img_url = model.ImagenUrl ?? entity.img_url; entity.img_descripcion = model.Descripcion; entity.img_fecha_mod = DateTime.UtcNow; entity.img_usuario_mod = model.Usuario; entity.img_ip_mod = model.Ip;
        }
        await _db.SaveChangesAsync(ct);
        return new CatalogoItemDataModel { Id = entity.img_id, Guid = entity.img_guid, Nombre = entity.img_descripcion ?? entity.img_url, ImagenUrl = entity.img_url, Descripcion = entity.img_descripcion, Estado = entity.img_estado };
    }

    public async Task<IReadOnlyList<CatalogoItemDataModel>> ListarIncluyeAsync(CancellationToken ct = default) =>
        await _db.Incluyes.AsNoTracking().Where(x => x.inc_estado == "A").OrderBy(x => x.inc_descripcion)
            .Select(x => new CatalogoItemDataModel { Id = x.inc_id, Guid = x.inc_guid, Nombre = x.inc_descripcion, Tipo = x.inc_tipo, Estado = x.inc_estado }).ToListAsync(ct);

    public async Task<CatalogoItemDataModel> GuardarIncluyeAsync(CatalogoUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Id.HasValue ? await _db.Incluyes.FirstOrDefaultAsync(x => x.inc_id == model.Id, ct) : null;
        if (entity is null)
        {
            entity = new IncluyeEntity { inc_descripcion = model.Nombre, inc_tipo = model.Tipo ?? "INCLUYE" };
            _db.Incluyes.Add(entity);
        }
        else
        {
            entity.inc_descripcion = model.Nombre; entity.inc_tipo = model.Tipo ?? entity.inc_tipo;
        }
        await _db.SaveChangesAsync(ct);
        return new CatalogoItemDataModel { Id = entity.inc_id, Guid = entity.inc_guid, Nombre = entity.inc_descripcion, Tipo = entity.inc_tipo, Estado = entity.inc_estado };
    }

    public async Task<bool> DesactivarCatalogoAsync(string tipo, int id, string usuario, CancellationToken ct = default)
    {
        switch (tipo.ToLowerInvariant())
        {
            case "destinos": var d = await _db.Destinos.FindAsync([id], ct); if (d is null) return false; d.des_estado = "I"; d.des_fecha_eliminacion = DateTime.UtcNow; d.des_usuario_eliminacion = usuario; break;
            case "categorias": var c = await _db.Categorias.FindAsync([id], ct); if (c is null) return false; c.cat_estado = "I"; c.cat_fecha_eliminacion = DateTime.UtcNow; c.cat_usuario_eliminacion = usuario; break;
            case "idiomas": var i = await _db.Idiomas.FindAsync([id], ct); if (i is null) return false; i.id_estado = "I"; i.id_fecha_eliminacion = DateTime.UtcNow; i.id_usuario_eliminacion = usuario; break;
            case "imagenes": var im = await _db.Imagenes.FindAsync([id], ct); if (im is null) return false; im.img_estado = "I"; im.img_fecha_eliminacion = DateTime.UtcNow; im.img_usuario_eliminacion = usuario; break;
            case "incluye": var inc = await _db.Incluyes.FindAsync([id], ct); if (inc is null) return false; inc.inc_estado = "I"; break;
            default: return false;
        }
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<AtraccionAdminDataModel>> ListarAtraccionesAsync(CancellationToken ct = default) =>
        await _db.Atracciones.AsNoTracking().Where(x => x.at_estado == "A").OrderBy(x => x.at_nombre).Select(MapAtraccion).ToListAsync(ct);

    public async Task<AtraccionAdminDataModel?> ObtenerAtraccionAsync(Guid guid, CancellationToken ct = default) =>
        await _db.Atracciones.AsNoTracking().Where(x => x.at_guid == guid).Select(MapAtraccion).FirstOrDefaultAsync(ct);

    public async Task<AtraccionAdminDataModel> GuardarAtraccionAsync(AtraccionUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Guid.HasValue ? await _db.Atracciones.FirstOrDefaultAsync(x => x.at_guid == model.Guid, ct) : null;
        if (entity is null)
        {
            entity = new AtraccionEntity { des_id = model.DestinoId, at_nombre = model.Nombre, at_usuario_ingreso = model.Usuario, at_ip_ingreso = model.Ip };
            _db.Atracciones.Add(entity);
        }
        entity.des_id = model.DestinoId; entity.at_nombre = model.Nombre; entity.at_descripcion = model.Descripcion; entity.at_direccion = model.Direccion; entity.at_duracion_minutos = model.DuracionMinutos; entity.at_punto_encuentro = model.PuntoEncuentro; entity.at_precio_referencia = model.PrecioReferencia; entity.at_incluye_acompaniante = model.IncluyeAcompaniante; entity.at_incluye_transporte = model.IncluyeTransporte; entity.at_disponible = model.Disponible; entity.at_free_cancellation = model.FreeCancellation; entity.at_skip_the_line = model.SkipTheLine; entity.at_fecha_mod = DateTime.UtcNow; entity.at_usuario_mod = model.Usuario; entity.at_ip_mod = model.Ip;
        await _db.SaveChangesAsync(ct);
        return MapAtraccion.Compile().Invoke(entity);
    }

    public async Task<bool> DesactivarAtraccionAsync(Guid guid, string usuario, CancellationToken ct = default)
    {
        var entity = await _db.Atracciones.FirstOrDefaultAsync(x => x.at_guid == guid, ct);
        if (entity is null) return false;
        entity.at_estado = "I"; entity.at_disponible = false; entity.at_fecha_eliminacion = DateTime.UtcNow; entity.at_usuario_eliminacion = usuario;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<TicketAdminDataModel>> ListarTicketsAsync(CancellationToken ct = default) =>
        await _db.Tickets.AsNoTracking().Where(x => x.tck_estado == "A").OrderBy(x => x.tck_titulo).Select(x => new TicketAdminDataModel { Id = x.tck_id, Guid = x.tck_guid, AtraccionId = x.at_id, Titulo = x.tck_titulo, Precio = x.tck_precio, Moneda = x.tck_moneda, TipoParticipante = x.tck_tipo_participante, CapacidadMaxima = x.tck_capacidad_maxima, Estado = x.tck_estado }).ToListAsync(ct);

    public async Task<TicketAdminDataModel> GuardarTicketAsync(TicketUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Guid.HasValue ? await _db.Tickets.FirstOrDefaultAsync(x => x.tck_guid == model.Guid, ct) : null;
        if (entity is null) { entity = new TicketEntity { at_id = model.AtraccionId, tck_usuario_ingreso = model.Usuario, tck_ip_ingreso = model.Ip }; _db.Tickets.Add(entity); }
        entity.at_id = model.AtraccionId; entity.tck_titulo = model.Titulo; entity.tck_precio = model.Precio; entity.tck_moneda = model.Moneda; entity.tck_tipo_participante = model.TipoParticipante; entity.tck_capacidad_maxima = model.CapacidadMaxima; entity.tck_fecha_mod = DateTime.UtcNow; entity.tck_usuario_mod = model.Usuario; entity.tck_ip_mod = model.Ip;
        await _db.SaveChangesAsync(ct);
        return new TicketAdminDataModel { Id = entity.tck_id, Guid = entity.tck_guid, AtraccionId = entity.at_id, Titulo = entity.tck_titulo, Precio = entity.tck_precio, Moneda = entity.tck_moneda, TipoParticipante = entity.tck_tipo_participante, CapacidadMaxima = entity.tck_capacidad_maxima, Estado = entity.tck_estado };
    }

    public async Task<bool> DesactivarTicketAsync(Guid guid, string usuario, CancellationToken ct = default)
    {
        var entity = await _db.Tickets.FirstOrDefaultAsync(x => x.tck_guid == guid, ct); if (entity is null) return false;
        entity.tck_estado = "I"; entity.tck_fecha_eliminacion = DateTime.UtcNow; entity.tck_usuario_eliminacion = usuario; await _db.SaveChangesAsync(ct); return true;
    }

    public async Task<IReadOnlyList<HorarioAdminDataModel>> ListarHorariosAsync(CancellationToken ct = default) =>
        await _db.Horarios.AsNoTracking().Where(x => x.hor_estado == "A").OrderBy(x => x.hor_fecha).ThenBy(x => x.hor_hora_inicio).Select(x => new HorarioAdminDataModel { Id = x.hor_id, Guid = x.hor_guid, AtraccionId = x.at_id, Fecha = x.hor_fecha, HoraInicio = x.hor_hora_inicio, HoraFin = x.hor_hora_fin, CuposDisponibles = x.hor_cupos_disponibles, DiasSemana = x.hor_dias_semana, Estado = x.hor_estado }).ToListAsync(ct);

    public async Task<HorarioAdminDataModel> GuardarHorarioAsync(HorarioUpsertDataModel model, CancellationToken ct = default)
    {
        var entity = model.Guid.HasValue ? await _db.Horarios.FirstOrDefaultAsync(x => x.hor_guid == model.Guid, ct) : null;
        if (entity is null) { entity = new HorarioEntity { at_id = model.AtraccionId, hor_usuario_ingreso = model.Usuario, hor_ip_ingreso = model.Ip }; _db.Horarios.Add(entity); }
        entity.at_id = model.AtraccionId; entity.hor_fecha = model.Fecha; entity.hor_hora_inicio = model.HoraInicio; entity.hor_hora_fin = model.HoraFin; entity.hor_cupos_disponibles = model.CuposDisponibles; entity.hor_dias_semana = model.DiasSemana; entity.hor_fecha_mod = DateTime.UtcNow; entity.hor_usuario_mod = model.Usuario; entity.hor_ip_mod = model.Ip;
        await _db.SaveChangesAsync(ct);
        return new HorarioAdminDataModel { Id = entity.hor_id, Guid = entity.hor_guid, AtraccionId = entity.at_id, Fecha = entity.hor_fecha, HoraInicio = entity.hor_hora_inicio, HoraFin = entity.hor_hora_fin, CuposDisponibles = entity.hor_cupos_disponibles, DiasSemana = entity.hor_dias_semana, Estado = entity.hor_estado };
    }

    public async Task<bool> DesactivarHorarioAsync(Guid guid, string usuario, CancellationToken ct = default)
    {
        var entity = await _db.Horarios.FirstOrDefaultAsync(x => x.hor_guid == guid, ct); if (entity is null) return false;
        entity.hor_estado = "I"; entity.hor_fecha_eliminacion = DateTime.UtcNow; entity.hor_usuario_eliminacion = usuario; await _db.SaveChangesAsync(ct); return true;
    }

    public async Task<IReadOnlyList<ReseniaDataModel>> ListarReseniasAsync(CancellationToken ct = default) =>
        await _db.Resenias.AsNoTracking().Where(x => x.rsn_estado == "A").OrderByDescending(x => x.rsn_fecha_creacion)
            .Select(x => new ReseniaDataModel { Guid = x.rsn_guid, AtraccionGuid = x.Atraccion!.at_guid, ReservaGuid = x.rev_guid, Comentario = x.rsn_comentario, Rating = x.rsn_rating, FechaCreacion = x.rsn_fecha_creacion, Estado = x.rsn_estado }).ToListAsync(ct);

    public async Task<IReadOnlyList<ReseniaDataModel>> ListarReseniasPorAtraccionAsync(Guid atraccionGuid, CancellationToken ct = default) =>
        await _db.Resenias.AsNoTracking()
            .Where(x => x.rsn_estado == "A" && x.Atraccion != null && x.Atraccion.at_guid == atraccionGuid)
            .OrderByDescending(x => x.rsn_fecha_creacion)
            .Select(x => new ReseniaDataModel { Guid = x.rsn_guid, AtraccionGuid = x.Atraccion!.at_guid, ReservaGuid = x.rev_guid, Comentario = x.rsn_comentario, Rating = x.rsn_rating, FechaCreacion = x.rsn_fecha_creacion, Estado = x.rsn_estado })
            .ToListAsync(ct);

    public async Task<ReseniaDataModel> CrearReseniaAsync(CrearReseniaDataModel model, CancellationToken ct = default)
    {
        var atraccion = await _db.Atracciones.FirstOrDefaultAsync(x => x.at_guid == model.AtraccionGuid && x.at_estado == "A", ct)
            ?? throw new InvalidOperationException("Atraccion no encontrada.");
        var entity = new ReseniaEntity { at_id = atraccion.at_id, rev_guid = model.ReservaGuid, rsn_comentario = model.Comentario, rsn_rating = model.Rating, rsn_usuario_creacion = model.Usuario, rsn_ip_creacion = model.Ip };
        _db.Resenias.Add(entity); atraccion.at_total_resenias += 1;
        await _db.SaveChangesAsync(ct);
        return new ReseniaDataModel { Guid = entity.rsn_guid, AtraccionGuid = atraccion.at_guid, ReservaGuid = entity.rev_guid, Comentario = entity.rsn_comentario, Rating = entity.rsn_rating, FechaCreacion = entity.rsn_fecha_creacion, Estado = entity.rsn_estado };
    }

    public async Task<bool> CambiarEstadoReseniaAsync(Guid guid, string estado, string usuario, CancellationToken ct = default)
    {
        var entity = await _db.Resenias.FirstOrDefaultAsync(x => x.rsn_guid == guid, ct); if (entity is null) return false;
        entity.rsn_estado = estado; entity.rsn_fecha_mod = DateTime.UtcNow; entity.rsn_usuario_mod = usuario; await _db.SaveChangesAsync(ct); return true;
    }

    private static readonly System.Linq.Expressions.Expression<Func<AtraccionEntity, AtraccionAdminDataModel>> MapAtraccion = x => new AtraccionAdminDataModel
    {
        Id = x.at_id, Guid = x.at_guid, DestinoId = x.des_id, Nombre = x.at_nombre, Descripcion = x.at_descripcion, Direccion = x.at_direccion, DuracionMinutos = x.at_duracion_minutos, PuntoEncuentro = x.at_punto_encuentro, PrecioReferencia = x.at_precio_referencia, IncluyeAcompaniante = x.at_incluye_acompaniante, IncluyeTransporte = x.at_incluye_transporte, Disponible = x.at_disponible, FreeCancellation = x.at_free_cancellation, SkipTheLine = x.at_skip_the_line, Estado = x.at_estado
    };
}
