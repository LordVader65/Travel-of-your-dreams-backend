using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.DataAccess.Common;
using TravelDreams.MsFacturacion.DataAccess.Context;
using TravelDreams.MsFacturacion.DataAccess.Entities;
using TravelDreams.MsFacturacion.DataManagement.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.DataManagement.Services;

public sealed class FacturacionDataService : IFacturacionDataService
{
    private readonly FacturacionDbContext _db;

    public FacturacionDataService(FacturacionDbContext db) => _db = db;

    public async Task<FacturaDataModel?> ObtenerFacturaPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        var entity = await FacturasBase().FirstOrDefaultAsync(x => x.fac_guid == guid, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<FacturaDataModel?> ObtenerFacturaPorNumeroAsync(string numero, CancellationToken ct = default)
    {
        var entity = await FacturasBase().FirstOrDefaultAsync(x => x.fac_numero == numero, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<PagedResult<FacturaDataModel>> ListarFacturasAsync(FacturaFiltroDataModel filtro, CancellationToken ct = default)
    {
        var page = filtro.Page <= 0 ? 1 : filtro.Page;
        var limit = filtro.Limit <= 0 ? 20 : Math.Min(filtro.Limit, 100);
        var query = FacturasBase();

        if (filtro.ClienteGuid.HasValue) query = query.Where(x => x.cli_guid == filtro.ClienteGuid);
        if (filtro.ReservaGuid.HasValue) query = query.Where(x => x.rev_guid == filtro.ReservaGuid);
        if (!string.IsNullOrWhiteSpace(filtro.Numero)) query = query.Where(x => x.fac_numero.Contains(filtro.Numero));
        if (!string.IsNullOrWhiteSpace(filtro.Estado)) query = query.Where(x => x.fac_estado == filtro.Estado);
        if (filtro.FechaDesdeUtc.HasValue) query = query.Where(x => x.fac_fecha_emision_utc >= filtro.FechaDesdeUtc);
        if (filtro.FechaHastaUtc.HasValue) query = query.Where(x => x.fac_fecha_emision_utc <= filtro.FechaHastaUtc);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.fac_fecha_emision_utc)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return new PagedResult<FacturaDataModel> { Items = items, Page = page, Limit = limit, Total = total };
    }

    public async Task<FacturaDataModel> RegistrarPagoYFacturaAsync(CrearPagoDataModel pago, decimal subtotal, decimal valorIva, CancellationToken ct = default)
    {
        var existing = await FacturasBase().FirstOrDefaultAsync(x => x.rev_guid == pago.ReservaGuid && x.fac_estado == DatabaseConstants.EstadoActivo, ct);
        if (existing is not null) return Map(existing);

        DatosFacturacionEntity? datos = null;
        if (pago.DatosFacturacionGuid.HasValue)
        {
            datos = await _db.DatosFacturacion.FirstOrDefaultAsync(x => x.dfac_guid == pago.DatosFacturacionGuid && x.dfac_estado == DatabaseConstants.EstadoActivo, ct)
                ?? throw new InvalidOperationException("Datos de facturacion no encontrados o inactivos.");
            if (datos.cli_guid != pago.ClienteGuid) throw new InvalidOperationException("Los datos de facturacion no pertenecen al cliente de la reserva.");
        }

        var pagoEntity = new PagoEntity
        {
            rev_guid = pago.ReservaGuid,
            cli_guid = pago.ClienteGuid,
            dfac_id = datos?.dfac_id,
            pag_monto = pago.Monto,
            pag_moneda = pago.Moneda,
            pag_metodo = pago.Metodo,
            pag_referencia = pago.Referencia,
            pag_fecha_utc = DateTime.UtcNow,
            pag_origen_canal = pago.OrigenCanal,
            pag_estado = DatabaseConstants.PagoAprobado,
            pag_usuario_ingreso = pago.Usuario,
            pag_ip_ingreso = pago.Ip,
            pag_observacion = pago.Observacion
        };

        var factura = new FacturaEntity
        {
            rev_guid = pago.ReservaGuid,
            cli_guid = pago.ClienteGuid,
            dfac_id = datos?.dfac_id,
            fac_numero = await NextNumeroAsync(ct),
            fac_fecha_emision_utc = DateTime.UtcNow,
            fac_subtotal = subtotal,
            fac_valor_iva = valorIva,
            fac_total = pago.Monto,
            fac_moneda = pago.Moneda,
            fac_observacion = "Factura generada automaticamente despues de pago aprobado.",
            fac_usuario_ingreso = pago.Usuario,
            fac_ip_ingreso = pago.Ip,
            Pago = pagoEntity
        };

        _db.Pagos.Add(pagoEntity);
        _db.Facturas.Add(factura);
        await _db.SaveChangesAsync(ct);

        return Map(await FacturasBase().FirstAsync(x => x.fac_id == factura.fac_id, ct));
    }

    public async Task<FacturaDataModel> GenerarFacturaDesdePagoAprobadoAsync(Guid reservaGuid, Guid clienteGuid, Guid? datosFacturacionGuid, decimal subtotal, decimal valorIva, decimal total, string moneda, string usuario, string ip, string? observacion, CancellationToken ct = default)
    {
        var existing = await FacturasBase().FirstOrDefaultAsync(x => x.rev_guid == reservaGuid && x.fac_estado == DatabaseConstants.EstadoActivo, ct);
        if (existing is not null) return Map(existing);

        var pago = await _db.Pagos
            .Where(x => x.rev_guid == reservaGuid && x.cli_guid == clienteGuid && x.pag_estado == DatabaseConstants.PagoAprobado)
            .OrderByDescending(x => x.pag_fecha_utc)
            .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException("No existe un pago aprobado para facturar esta reserva.");

        DatosFacturacionEntity? datos = null;
        if (datosFacturacionGuid.HasValue)
        {
            datos = await _db.DatosFacturacion.FirstOrDefaultAsync(x => x.dfac_guid == datosFacturacionGuid && x.dfac_estado == DatabaseConstants.EstadoActivo, ct)
                ?? throw new InvalidOperationException("Datos de facturacion no encontrados o inactivos.");
            if (datos.cli_guid != clienteGuid) throw new InvalidOperationException("Los datos de facturacion no pertenecen al cliente de la reserva.");
        }

        var factura = new FacturaEntity
        {
            rev_guid = reservaGuid,
            cli_guid = clienteGuid,
            pag_id = pago.pag_id,
            dfac_id = datos?.dfac_id,
            fac_numero = await NextNumeroAsync(ct),
            fac_fecha_emision_utc = DateTime.UtcNow,
            fac_subtotal = subtotal,
            fac_valor_iva = valorIva,
            fac_total = total,
            fac_moneda = moneda,
            fac_observacion = observacion,
            fac_usuario_ingreso = usuario,
            fac_ip_ingreso = ip
        };

        _db.Facturas.Add(factura);
        await _db.SaveChangesAsync(ct);

        return Map(await FacturasBase().FirstAsync(x => x.fac_id == factura.fac_id, ct));
    }

    private IQueryable<FacturaEntity> FacturasBase() =>
        _db.Facturas.AsNoTracking()
            .Include(x => x.Pago)
            .Include(x => x.DatosFacturacion);

    private async Task<string> NextNumeroAsync(CancellationToken ct)
    {
        var prefix = $"FAC-{DateTime.UtcNow:yyyyMMdd}-";
        var count = await _db.Facturas.CountAsync(x => x.fac_numero.StartsWith(prefix), ct) + 1;
        return $"{prefix}{count:D6}";
    }

    private static FacturaDataModel Map(FacturaEntity entity) => new()
    {
        Id = entity.fac_id,
        Guid = entity.fac_guid,
        Numero = entity.fac_numero,
        ReservaGuid = entity.rev_guid,
        ClienteGuid = entity.cli_guid,
        PagoGuid = entity.Pago?.pag_guid ?? Guid.Empty,
        DatosFacturacionGuid = entity.DatosFacturacion?.dfac_guid,
        FechaEmisionUtc = entity.fac_fecha_emision_utc,
        Subtotal = entity.fac_subtotal,
        ValorIva = entity.fac_valor_iva,
        Total = entity.fac_total,
        Moneda = entity.fac_moneda,
        Observacion = entity.fac_observacion,
        Estado = entity.fac_estado,
        Pago = entity.Pago is null ? null : PagoDataService.Map(entity.Pago),
        DatosFacturacion = entity.DatosFacturacion is null ? null : DatosFacturacionDataService.Map(entity.DatosFacturacion)
    };
}
