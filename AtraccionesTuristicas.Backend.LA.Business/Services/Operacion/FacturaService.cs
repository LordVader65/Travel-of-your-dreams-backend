namespace AtraccionesTuristicas.Backend.LA.Business.Services.Operacion;

public sealed class FacturaService : IFacturaService
    {
        private readonly IFacturaDataService _data; public FacturaService(IFacturaDataService data) => _data = data;
        public async Task<FacturaResponse> ObtenerPorGuidAsync(Guid guid, CurrentUserData user, CancellationToken cancellationToken = default) => Map.Factura(await _data.ObtenerPorGuidAsync(guid, cancellationToken) ?? throw new NotFoundException("Factura no encontrada."));
        public async Task<FacturaResponse?> ObtenerPorNumeroAsync(string numero, CancellationToken cancellationToken = default) => (await _data.ObtenerPorNumeroAsync(numero, cancellationToken)) is { } f ? Map.Factura(f) : null;
        public async Task<BusinessPagedResult<FacturaResponse>> ListarAsync(FacturaFiltroRequest filtro, CurrentUserData user, CancellationToken cancellationToken = default) { if (!user.EsAdmin) filtro.ClienteGuid = user.ClienteGuid ?? filtro.ClienteGuid; var result = await _data.ListarAsync(new FacturaFiltroDataModel { ClienteGuid = filtro.ClienteGuid, ReservaGuid = filtro.ReservaGuid, Numero = filtro.Numero, Estado = filtro.Estado, FechaDesdeUtc = filtro.FechaDesdeUtc, FechaHastaUtc = filtro.FechaHastaUtc, Page = filtro.Page, Limit = filtro.Limit }, cancellationToken); return Support.Paging.Map(result, Map.Factura); }
        public Task<Guid> GenerarAsync(Guid reservaGuid, Guid? datosFacturacionGuid, CurrentUserData user, string? observacion = null, string? origenCanal = null, CancellationToken cancellationToken = default) => _data.GenerarFacturaAsync(reservaGuid, datosFacturacionGuid, user.Login, user.Ip, observacion, origenCanal, cancellationToken);
    }

