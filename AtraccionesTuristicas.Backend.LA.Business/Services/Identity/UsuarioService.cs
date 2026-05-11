namespace AtraccionesTuristicas.Backend.LA.Business.Services.Identity;

public sealed class UsuarioService : IUsuarioService
    {
        private const string SeedAdminLogin = "admin@travelofyourdreams.local";
        private readonly IUsuarioDataService _usuarios;
        private readonly IUsuarioRolDataService _usuarioRoles;
        private readonly IClienteDataService _clientes;
        private readonly IRolDataService _roles;
        public UsuarioService(IUsuarioDataService usuarios, IUsuarioRolDataService usuarioRoles, IClienteDataService clientes, IRolDataService roles) { _usuarios = usuarios; _usuarioRoles = usuarioRoles; _clientes = clientes; _roles = roles; }
        public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _usuarios.ListarAsync(cancellationToken)).Select(Map.Usuario).ToList();
        public async Task<UsuarioResponse?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _usuarios.ObtenerPorGuidAsync(guid, cancellationToken)) is { } u ? Map.Usuario(u) : null;
        public async Task<UsuarioResponse?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default) => (await _usuarios.ObtenerPorLoginAsync(login, cancellationToken)) is { } u ? Map.Usuario(u) : null;
        public async Task<UsuarioResponse> CrearAsync(CrearUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            Support.Guard.EnsureAdmin(user);
            request.Login = string.IsNullOrWhiteSpace(request.Login) ? request.Correo : request.Login;
            var errors = new List<string>();
            Support.Guard.Email(request.Login, "Login", errors);
            Support.Guard.Required(request.Password, "Password", errors);
            Support.Guard.IdentificationType(request.TipoIdentificacion, "TipoIdentificacion", errors);
            Support.Guard.Required(request.NumeroIdentificacion, "NumeroIdentificacion", errors);
            Support.Guard.Email(request.Correo, "Correo", errors);
            Support.Guard.Phone(request.Telefono, "Telefono", errors);
            Support.Guard.MaxLength(request.Login, 150, "Login", errors);
            Support.Guard.MaxLength(request.NumeroIdentificacion, 20, "NumeroIdentificacion", errors);
            Support.Guard.MaxLength(request.Nombres, 100, "Nombres", errors);
            Support.Guard.MaxLength(request.Apellidos, 100, "Apellidos", errors);
            Support.Guard.MaxLength(request.RazonSocial, 200, "RazonSocial", errors);
            Support.Guard.MaxLength(request.Correo, 150, "Correo", errors);
            Support.Guard.MaxLength(request.Telefono, 10, "Telefono", errors);
            Support.Guard.MaxLength(request.Direccion, 300, "Direccion", errors);
            if (request.Password.Length < 8) errors.Add("Password debe tener al menos 8 caracteres.");
            Support.Guard.ThrowIfAny(errors);
            request.TipoIdentificacion = request.TipoIdentificacion.Trim().ToUpperInvariant();
            if (await _usuarios.ObtenerPorLoginAsync(request.Login, cancellationToken) is not null) throw new ConflictBusinessException("El usuario ya existe.");
            if (await _clientes.ObtenerPorIdentificacionAsync(request.NumeroIdentificacion, cancellationToken) is not null) throw new ConflictBusinessException("La identificacion ya existe.");
            var salt = RandomNumberGenerator.GetBytes(16);
            var passwordHash = Rfc2898DeriveBytes.Pbkdf2(request.Password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            var hash = $"pbkdf2:100000:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(passwordHash)}";
            var usuario = await _usuarios.CrearAsync(new UsuarioDataModel { Login = request.Login, PasswordHash = hash, Estado = BusinessConstants.EstadoActivo }, request.UsuarioRegistro, request.IpRegistro, cancellationToken);
            var rolIds = request.RolIds.Where(x => x > 0).Distinct().ToList();
            if (rolIds.Count == 0 && await _roles.ObtenerPorDescripcionAsync(BusinessConstants.RolCliente, cancellationToken) is { } rolCliente)
            {
                rolIds.Add(rolCliente.Id);
            }
            foreach (var rolId in rolIds)
            {
                await _usuarioRoles.CrearAsync(new UsuarioRolDataModel { UsuarioId = usuario.Id, RolId = rolId, Estado = BusinessConstants.EstadoActivo }, cancellationToken);
            }

            await _clientes.CrearAsync(new ClienteDataModel
            {
                UsuarioId = usuario.Id,
                TipoIdentificacion = request.TipoIdentificacion,
                NumeroIdentificacion = request.NumeroIdentificacion,
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                RazonSocial = request.RazonSocial,
                Correo = request.Correo,
                Telefono = request.Telefono,
                Direccion = request.Direccion,
                UsuarioIngreso = request.UsuarioRegistro,
                IpIngreso = request.IpRegistro,
                Estado = BusinessConstants.EstadoActivo
            }, cancellationToken);

            return Map.Usuario(await _usuarios.ObtenerPorGuidAsync(usuario.Guid, cancellationToken) ?? usuario);
        }
        public async Task<UsuarioResponse> CambiarEstadoAsync(CambiarEstadoUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            Support.Guard.EnsureAdmin(user);
            var usuario = (await _usuarios.ListarAsync(cancellationToken)).FirstOrDefault(x => x.Guid == request.Guid) ?? throw new NotFoundException("Usuario no encontrado.");
            EnsureNotSeedAdmin(usuario);

            if (request.Estado == BusinessConstants.EstadoInactivo)
            {
                var usuarios = await _usuarios.ListarAsync(cancellationToken);
                var esAdmin = usuario.Roles.Contains(BusinessConstants.RolAdmin, StringComparer.OrdinalIgnoreCase);
                var adminsActivos = usuarios.Count(x => x.Estado == BusinessConstants.EstadoActivo && x.Roles.Contains(BusinessConstants.RolAdmin, StringComparer.OrdinalIgnoreCase));
                if (esAdmin && adminsActivos <= 1)
                {
                    throw new BusinessRuleException("Debe existir al menos un usuario administrador activo.");
                }
            }
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
            var usuario = (await _usuarios.ListarAsync(cancellationToken)).FirstOrDefault(x => x.Id == request.UsuarioId) ?? throw new NotFoundException("Usuario no encontrado.");
            EnsureNotSeedAdmin(usuario);

            await _usuarioRoles.ReemplazarRolesAsync(request.UsuarioId, rolIds, cancellationToken);
            return BusinessOperationResult.Ok("Roles actualizados.");
        }

        private static void EnsureNotSeedAdmin(UsuarioDataModel usuario)
        {
            if (string.Equals(usuario.Login, SeedAdminLogin, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessRuleException("El usuario administrador principal no puede desactivarse ni cambiar de rol.");
            }
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

