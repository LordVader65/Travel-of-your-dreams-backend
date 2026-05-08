namespace AtraccionesTuristicas.Backend.LA.Business.Services.CatalogoRelaciones;

public sealed class AtraccionIncluyeService : IAtraccionIncluyeService
    {
        private readonly IAtraccionIncluyeDataService _data; public AtraccionIncluyeService(IAtraccionIncluyeDataService data) => _data = data;
        public async Task<IReadOnlyList<AtraccionIncluyeResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(x => new AtraccionIncluyeResponse { IncluyeId = x.IncluyeId, AtraccionId = x.AtraccionId, Estado = x.Estado }).ToList();
        public async Task<AtraccionIncluyeResponse> AsociarAsync(AsociarIncluyeAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var x = await _data.CrearAsync(new AtraccionIncluyeDataModel { IncluyeId = request.IncluyeId, AtraccionId = request.AtraccionId, UsuarioIngreso = request.UsuarioIngreso, Estado = BusinessConstants.EstadoActivo }, cancellationToken); return new AtraccionIncluyeResponse { IncluyeId = x.IncluyeId, AtraccionId = x.AtraccionId, Estado = x.Estado }; }
        public async Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
    }

