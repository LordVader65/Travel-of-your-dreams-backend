namespace AtraccionesTuristicas.Backend.LA.Business.Services.Identity;

public sealed class RolService : IRolService
    {
        private readonly IRolDataService _roles;
        public RolService(IRolDataService roles) => _roles = roles;
        public async Task<IReadOnlyList<RolResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _roles.ListarAsync(cancellationToken)).Select(Map.Rol).ToList();
    }

