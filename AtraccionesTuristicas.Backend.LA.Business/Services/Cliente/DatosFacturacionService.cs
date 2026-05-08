namespace AtraccionesTuristicas.Backend.LA.Business.Services.Cliente;

public sealed class DatosFacturacionService : IDatosFacturacionService
    {
        private readonly IDatosFacturacionDataService _datos;
        public DatosFacturacionService(IDatosFacturacionDataService datos) => _datos = datos;
        public async Task<IReadOnlyList<DatosFacturacionResponse>> ListarActivosPorClienteAsync(Guid clienteGuid, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureOwnerOrAdmin(user, clienteGuid); return (await _datos.ListarActivosPorClienteAsync(clienteGuid, cancellationToken)).Select(Map.Datos).ToList(); }
        public async Task<DatosFacturacionResponse> CrearAsync(CrearDatosFacturacionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Validate(request); return Map.Datos(await _datos.CrearAsync(Map.Datos(request), cancellationToken)); }
        public async Task<DatosFacturacionResponse> ActualizarAsync(ActualizarDatosFacturacionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Validate(request); return Map.Datos(await _datos.ActualizarAsync(Map.Datos(request), cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { await _datos.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok("Datos de facturacion inactivados."); }
        private static void Validate(CrearDatosFacturacionRequest request) { var errors = new List<string>(); Support.Guard.Positive(request.ClienteId, "ClienteId", errors); Support.Guard.Required(request.TipoIdentificacion, "TipoIdentificacion", errors); Support.Guard.Required(request.NumeroIdentificacion, "NumeroIdentificacion", errors); Support.Guard.Required(request.Nombre, "Nombre", errors); Support.Guard.Email(request.Correo, "Correo", errors); Support.Guard.ThrowIfAny(errors); }
    }

