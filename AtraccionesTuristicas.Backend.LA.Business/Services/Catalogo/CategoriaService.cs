namespace AtraccionesTuristicas.Backend.LA.Business.Services.Catalogo;

public sealed class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaDataService _data; public CategoriaService(ICategoriaDataService data) => _data = data;
        public async Task<IReadOnlyList<CategoriaResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(Map.Categoria).ToList();
        public async Task<CategoriaResponse> CrearAsync(CrearCategoriaRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request.Nombre); return Map.Categoria(await _data.CrearAsync(Map.Categoria(request), cancellationToken)); }
        public async Task<CategoriaResponse> ActualizarAsync(ActualizarCategoriaRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request.Nombre); var m = Map.Categoria((CrearCategoriaRequest)request); m.Id = request.Id; m.Guid = request.Guid; m.Estado = request.Estado; return Map.Categoria(await _data.ActualizarAsync(m, cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        private static void Validate(string nombre) { var errors = new List<string>(); Support.Guard.Required(nombre, "Nombre", errors); Support.Guard.MaxLength(nombre, 100, "Nombre", errors); Support.Guard.ThrowIfAny(errors); }
    }
