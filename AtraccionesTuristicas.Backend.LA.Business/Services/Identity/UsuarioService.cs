namespace AtraccionesTuristicas.Backend.LA.Business.Services.Identity;

public sealed class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioDataService _usuarios;
        private readonly IUsuarioRolDataService _usuarioRoles;
        public UsuarioService(IUsuarioDataService usuarios, IUsuarioRolDataService usuarioRoles) { _usuarios = usuarios; _usuarioRoles = usuarioRoles; }
        public async Task<UsuarioResponse?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _usuarios.ObtenerPorGuidAsync(guid, cancellationToken)) is { } u ? Map.Usuario(u) : null;
        public async Task<UsuarioResponse?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default) => (await _usuarios.ObtenerPorLoginAsync(login, cancellationToken)) is { } u ? Map.Usuario(u) : null;
        public async Task<UsuarioResponse> CrearAsync(CrearUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            Support.Guard.EnsureAdmin(user);
            var errors = new List<string>(); Support.Guard.Required(request.Login, "Login", errors); Support.Guard.Required(request.Password, "Password", errors); Support.Guard.ThrowIfAny(errors);
            if (await _usuarios.ObtenerPorLoginAsync(request.Login, cancellationToken) is not null) throw new ConflictBusinessException("El usuario ya existe.");
            var salt = RandomNumberGenerator.GetBytes(16);
            var passwordHash = Rfc2898DeriveBytes.Pbkdf2(request.Password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            var hash = $"pbkdf2:100000:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(passwordHash)}";
            return Map.Usuario(await _usuarios.CrearAsync(new UsuarioDataModel { Login = request.Login, PasswordHash = hash, Estado = BusinessConstants.EstadoActivo }, request.UsuarioRegistro, request.IpRegistro, cancellationToken));
        }
        public async Task<UsuarioResponse> CambiarEstadoAsync(CambiarEstadoUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            Support.Guard.EnsureAdmin(user);
            return Map.Usuario(await _usuarios.CambiarEstadoAsync(request.Guid, request.Estado, request.Usuario, request.Ip, cancellationToken) ?? throw new NotFoundException("Usuario no encontrado."));
        }

        public async Task<UsuarioResponse> CambiarPasswordAsync(CambiarPasswordRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            if (!user.EsAdmin && user.UsuarioGuid != request.UsuarioGuid) throw new ForbiddenBusinessException("No puede cambiar la contrasena de otro usuario.");
            var errors = new List<string>(); Support.Guard.Required(request.PasswordNueva, "PasswordNueva", errors); if (request.PasswordNueva.Length < 8) errors.Add("PasswordNueva debe tener al menos 8 caracteres."); Support.Guard.ThrowIfAny(errors);
            var usuario = await _usuarios.ObtenerPorGuidAsync(request.UsuarioGuid, cancellationToken) ?? throw new NotFoundException("Usuario no encontrado.");
            if (!user.EsAdmin && !PasswordMatches(request.PasswordActual, usuario.PasswordHash)) throw new UnauthorizedBusinessException("La contrasena actual no es valida.");
            return Map.Usuario(await _usuarios.CambiarPasswordAsync(request.UsuarioGuid, HashPassword(request.PasswordNueva), request.Usuario, request.Ip, cancellationToken) ?? throw new NotFoundException("Usuario no encontrado."));
        }

        public async Task<BusinessOperationResult> CambiarRolesAsync(CambiarRolUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            Support.Guard.EnsureAdmin(user);
            var rolIds = request.RolIds.Count > 0 ? request.RolIds : [request.RolId];
            if (request.UsuarioId <= 0 || rolIds.Count == 0 || rolIds.Any(x => x <= 0)) throw new ValidationException(["UsuarioId y RolId son obligatorios."]);
            await _usuarioRoles.ReemplazarRolesAsync(request.UsuarioId, rolIds, cancellationToken);
            return BusinessOperationResult.Ok("Roles actualizados.");
        }

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var passwordHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            return $"pbkdf2:100000:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(passwordHash)}";
        }

        private static bool PasswordMatches(string password, string storedHash)
        {
            if (!storedHash.StartsWith("pbkdf2:", StringComparison.OrdinalIgnoreCase)) return false;
            var parts = storedHash.Split(':');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
    }

