namespace AtraccionesTuristicas.Backend.LA.Business.Services.Cliente;

public sealed class ClienteService : IClienteService
    {
        private readonly IClienteDataService _clientes;
        public ClienteService(IClienteDataService clientes) => _clientes = clientes;
        public async Task<BusinessPagedResult<ClienteResponse>> ListarAsync(ClienteFiltroRequest filtro, CancellationToken cancellationToken = default)
        {
            var result = await _clientes.ListarAsync(new ClienteFiltroDataModel { NumeroIdentificacion = filtro.NumeroIdentificacion, Correo = filtro.Correo, Estado = filtro.Estado, Page = filtro.Page, Limit = filtro.Limit }, cancellationToken);
            return Support.Paging.Map(result, Map.Cliente);
        }
        public async Task<ClienteResponse> CrearAsync(CrearClienteRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCliente(request.TipoIdentificacion, request.NumeroIdentificacion, request.Correo);
            if (await _clientes.ObtenerPorIdentificacionAsync(request.NumeroIdentificacion, cancellationToken) is not null) throw new ConflictBusinessException("La identificacion ya existe.");
            return Map.Cliente(await _clientes.CrearAsync(Map.Cliente(request), cancellationToken));
        }
        public async Task<ClienteResponse?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _clientes.ObtenerPorGuidAsync(guid, cancellationToken)) is { } c ? Map.Cliente(c) : null;
        public async Task<ClienteResponse?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken cancellationToken = default) => (await _clientes.ObtenerPorIdentificacionAsync(numeroIdentificacion, cancellationToken)) is { } c ? Map.Cliente(c) : null;
        public async Task<ClienteResponse> ActualizarAsync(ActualizarClienteRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCliente(request.TipoIdentificacion, request.NumeroIdentificacion, request.Correo);
            return Map.Cliente(await _clientes.ActualizarAsync(Map.Cliente(request), cancellationToken) ?? throw new NotFoundException("Cliente no encontrado."));
        }
        public async Task<BusinessOperationResult> CambiarEstadoAsync(CambiarEstadoClienteRequest request, CancellationToken cancellationToken = default)
        {
            var estado = string.IsNullOrWhiteSpace(request.Estado) ? BusinessConstants.EstadoActivo : request.Estado;
            if (estado is not (BusinessConstants.EstadoActivo or BusinessConstants.EstadoInactivo)) throw new ValidationException(["Estado de cliente no permitido."]);
            var ok = await _clientes.CambiarEstadoAsync(request.ClienteGuid, estado, request.Usuario, request.Ip, cancellationToken);
            if (!ok) throw new NotFoundException("Cliente no encontrado.");
            return BusinessOperationResult.Ok("Estado de cliente actualizado.");
        }
        private static void ValidateCliente(string tipo, string identificacion, string correo) { var errors = new List<string>(); Support.Guard.Required(tipo, "TipoIdentificacion", errors); Support.Guard.Required(identificacion, "NumeroIdentificacion", errors); Support.Guard.Email(correo, "Correo", errors); Support.Guard.ThrowIfAny(errors); }
    }

