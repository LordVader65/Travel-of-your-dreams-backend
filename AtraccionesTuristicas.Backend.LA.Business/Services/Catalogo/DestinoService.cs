namespace AtraccionesTuristicas.Backend.LA.Business.Services.Catalogo;

public sealed class DestinoService : IDestinoService
    {
        private readonly IDestinoDataService _data; public DestinoService(IDestinoDataService data) => _data = data;
        public async Task<IReadOnlyList<DestinoResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(Map.Destino).ToList();
        public async Task<DestinoResponse> CrearAsync(CrearDestinoRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request.Nombre, request.Pais); return Map.Destino(await _data.CrearAsync(Map.Destino(request), cancellationToken)); }
        public async Task<DestinoResponse> ActualizarAsync(ActualizarDestinoRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request.Nombre, request.Pais); var m = Map.Destino((CrearDestinoRequest)request); m.Id = request.Id; m.Guid = request.Guid; m.Estado = request.Estado; return Map.Destino(await _data.ActualizarAsync(m, cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        private static void Validate(string nombre, string pais) { var errors = new List<string>(); Support.Guard.Required(nombre, "Nombre", errors); Support.Guard.Required(pais, "Pais", errors); Support.Guard.ThrowIfAny(errors); }
    }

