namespace AtraccionesTuristicas.Backend.LA.Business.Services.Operacion;

public sealed class PagoService : IPagoService
    {
        private readonly IPagoDataService _data;
        private readonly IFacturaDataService _facturas;
        public PagoService(IPagoDataService data, IFacturaDataService facturas) { _data = data; _facturas = facturas; }
        public async Task<PagoResponse> ObtenerPorGuidAsync(Guid guid, CurrentUserData user, CancellationToken cancellationToken = default) => Map.Pago(await _data.ObtenerPorGuidAsync(guid, cancellationToken) ?? throw new NotFoundException("Pago no encontrado."));
        public async Task<BusinessPagedResult<PagoResponse>> ListarAsync(PagoFiltroRequest filtro, CurrentUserData user, CancellationToken cancellationToken = default) { if (!user.EsAdmin) filtro.ClienteGuid = user.ClienteGuid ?? filtro.ClienteGuid; var result = await _data.ListarAsync(new PagoFiltroDataModel { ReservaGuid = filtro.ReservaGuid, ClienteGuid = filtro.ClienteGuid, Metodo = filtro.Metodo, Estado = filtro.Estado, FechaDesdeUtc = filtro.FechaDesdeUtc, FechaHastaUtc = filtro.FechaHastaUtc, Page = filtro.Page, Limit = filtro.Limit }, cancellationToken); return Support.Paging.Map(result, Map.Pago); }
        public async Task<Guid> ConfirmarPagoAsync(CrearPagoRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { request.Metodo = "TARJETA"; if (string.IsNullOrWhiteSpace(request.Referencia)) request.Referencia = $"PAY-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Guid.NewGuid():N}"[..32].ToUpperInvariant(); var errors = new List<string>(); Support.Guard.Required(request.Metodo, "Metodo", errors); Support.Guard.Required(request.Referencia, "Referencia", errors); Support.Guard.Positive(request.Monto, "Monto", errors); Support.Guard.MaxLength(request.Metodo, 50, "Metodo", errors); Support.Guard.MaxLength(request.Referencia, 100, "Referencia", errors); Support.Guard.MaxLength(request.OrigenCanal, 50, "OrigenCanal", errors); Support.Guard.ThrowIfAny(errors); return await _data.ConfirmarPagoAsync(new PagoCrearDataModel { ReservaGuid = request.ReservaGuid, Metodo = request.Metodo, Monto = request.Monto, Referencia = request.Referencia, Usuario = request.Usuario, Ip = request.Ip, OrigenCanal = request.OrigenCanal }, cancellationToken); }
        public async Task<PagoConfirmadoResponse> ConfirmarPagoYGenerarFacturaAsync(CrearPagoRequest request, Guid? datosFacturacionGuid, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            var pagoGuid = await ConfirmarPagoAsync(request, user, cancellationToken);
            var facturaGuid = await _facturas.GenerarFacturaAsync(request.ReservaGuid, datosFacturacionGuid, request.Usuario, request.Ip, "Factura generada automaticamente despues de pago aprobado.", request.OrigenCanal, cancellationToken);
            return new PagoConfirmadoResponse { PagoGuid = pagoGuid, FacturaGuid = facturaGuid };
        }
    }

