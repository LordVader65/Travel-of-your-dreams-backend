namespace AtraccionesTuristicas.Backend.LA.Business.Services.CatalogoRelaciones;

public sealed class IdiomaAtraccionService : IIdiomaAtraccionService
    {
        private readonly IIdiomaAtraccionDataService _data; public IdiomaAtraccionService(IIdiomaAtraccionDataService data) => _data = data;
        public async Task<IReadOnlyList<IdiomaAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(x => new IdiomaAtraccionResponse { IdiomaId = x.IdiomaId, AtraccionId = x.AtraccionId, Estado = x.Estado }).ToList();
        public async Task<IdiomaAtraccionResponse> AsociarAsync(AsociarIdiomaAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request.AtraccionId, request.IdiomaId, "IdiomaId"); var x = await _data.CrearAsync(new IdiomaAtraccionDataModel { IdiomaId = request.IdiomaId, AtraccionId = request.AtraccionId, UsuarioIngreso = request.UsuarioIngreso, Estado = BusinessConstants.EstadoActivo }, cancellationToken); return new IdiomaAtraccionResponse { IdiomaId = x.IdiomaId, AtraccionId = x.AtraccionId, Estado = x.Estado }; }
        public async Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        public async Task<BusinessOperationResult> RemoverAsync(int atraccionId, int idiomaId, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(atraccionId, idiomaId, "IdiomaId"); await _data.RemoverAsync(atraccionId, idiomaId, cancellationToken); return BusinessOperationResult.Ok("Idioma desasociado."); }
        private static void Validate(int atraccionId, int relacionadoId, string relacionadoNombre) { var errors = new List<string>(); Support.Guard.Positive(atraccionId, "AtraccionId", errors); Support.Guard.Positive(relacionadoId, relacionadoNombre, errors); Support.Guard.ThrowIfAny(errors); }
    }

