namespace AtraccionesTuristicas.Backend.LA.Business.Services.Catalogo;

public sealed class IdiomaService : IIdiomaService
    {
        private readonly IIdiomaDataService _data; public IdiomaService(IIdiomaDataService data) => _data = data;
        public async Task<IReadOnlyList<IdiomaResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(Map.Idioma).ToList();
        public async Task<IdiomaResponse> CrearAsync(CrearIdiomaRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request.Codigo, request.Descripcion); return Map.Idioma(await _data.CrearAsync(Map.Idioma(request), cancellationToken)); }
        public async Task<IdiomaResponse> ActualizarAsync(ActualizarIdiomaRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request.Codigo, request.Descripcion); var m = Map.Idioma((CrearIdiomaRequest)request); m.Id = request.Id; m.Estado = request.Estado; return Map.Idioma(await _data.ActualizarAsync(m, cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        private static void Validate(string codigo, string descripcion) { var errors = new List<string>(); Support.Guard.Required(codigo, "Codigo", errors); Support.Guard.Required(descripcion, "Descripcion", errors); Support.Guard.ThrowIfAny(errors); }
    }

