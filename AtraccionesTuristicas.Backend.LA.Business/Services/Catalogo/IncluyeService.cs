namespace AtraccionesTuristicas.Backend.LA.Business.Services.Catalogo;

public sealed class IncluyeService : IIncluyeService
    {
        private readonly IIncluyeDataService _data; public IncluyeService(IIncluyeDataService data) => _data = data;
        public async Task<IReadOnlyList<IncluyeResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(Map.Incluye).ToList();
        public async Task<IncluyeResponse> CrearAsync(CrearIncluyeRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var errors = new List<string>(); Support.Guard.Required(request.Descripcion, "Descripcion", errors); Support.Guard.ThrowIfAny(errors); return Map.Incluye(await _data.CrearAsync(Map.Incluye(request), cancellationToken)); }
        public async Task<IncluyeResponse> ActualizarAsync(ActualizarIncluyeRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var m = Map.Incluye((CrearIncluyeRequest)request); m.Id = request.Id; m.Estado = request.Estado; return Map.Incluye(await _data.ActualizarAsync(m, cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
    }

