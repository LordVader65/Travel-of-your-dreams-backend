namespace AtraccionesTuristicas.Backend.LA.Business.Services.Auth
{
    using AtraccionesTuristicas.Backend.LA.Business.Common;
    using AtraccionesTuristicas.Backend.LA.Business.DTOs.Auth;
    using AtraccionesTuristicas.Backend.LA.Business.Exceptions;
    using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auth;

    public sealed class AuthService : IAuthService
    {
        private readonly IUsuarioDataService _usuarios;
        private readonly IClienteDataService _clientes;
        private readonly IRolDataService _roles;
        private readonly IUsuarioRolDataService _usuarioRoles;
        private readonly JwtOptions _jwt;

        public AuthService(IUsuarioDataService usuarios, IClienteDataService clientes, IRolDataService roles, IUsuarioRolDataService usuarioRoles, JwtOptions jwt)
        {
            _usuarios = usuarios; _clientes = clientes; _roles = roles; _usuarioRoles = usuarioRoles; _jwt = jwt;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var errors = new List<string>(); Support.Guard.Required(request.Login, "Login", errors); Support.Guard.Required(request.Password, "Password", errors); Support.Guard.ThrowIfAny(errors);
            var usuario = await _usuarios.ObtenerConRolesAsync(request.Login, cancellationToken) ?? throw new UnauthorizedBusinessException("Credenciales invalidas.");
            if (usuario.Estado != BusinessConstants.EstadoActivo) throw new UnauthorizedBusinessException("Usuario inactivo.");
            if (!PasswordMatches(request.Password, usuario.PasswordHash)) throw new UnauthorizedBusinessException("Credenciales invalidas.");

            var cliente = await _clientes.ObtenerPorUsuarioIdAsync(usuario.Id, cancellationToken);
            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);
            return new LoginResponse { UsuarioGuid = usuario.Guid, ClienteGuid = cliente?.Guid, Login = usuario.Login, Roles = usuario.Roles, Token = BuildToken(usuario, expires, cliente?.Guid), ExpiraEnUtc = expires };
        }

        public async Task<LoginResponse> LoginAdminAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var response = await LoginAsync(request, cancellationToken);
            if (!response.Roles.Contains(BusinessConstants.RolAdmin, StringComparer.OrdinalIgnoreCase))
            {
                throw new ForbiddenBusinessException("El usuario no tiene rol administrador activo.");
            }

            return response;
        }

        public async Task<RegisterClienteResponse> RegistrarClienteAsync(RegisterClienteRequest request, CancellationToken cancellationToken = default)
        {
            var errors = new List<string>();
            Support.Guard.IdentificationType(request.TipoIdentificacion, "TipoIdentificacion", errors);
            Support.Guard.Required(request.NumeroIdentificacion, "NumeroIdentificacion", errors);
            Support.Guard.Phone(request.Telefono, "Telefono", errors);
            Support.Guard.Email(request.Correo, "Correo", errors);
            Support.Guard.Required(request.Password, "Password", errors);
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

            if (await _usuarios.ObtenerPorLoginAsync(request.Correo, cancellationToken) is not null) throw new ConflictBusinessException("El correo ya esta registrado como usuario.");
            if (await _clientes.ObtenerPorIdentificacionAsync(request.NumeroIdentificacion, cancellationToken) is not null) throw new ConflictBusinessException("La identificacion ya esta registrada.");

            var usuario = await _usuarios.CrearAsync(new UsuarioDataModel { Login = request.Correo, PasswordHash = HashPassword(request.Password), Estado = BusinessConstants.EstadoActivo }, request.Usuario, request.Ip, cancellationToken);
            var rolCliente = await _roles.ObtenerPorDescripcionAsync(BusinessConstants.RolCliente, cancellationToken);
            if (rolCliente is not null) await _usuarioRoles.CrearAsync(new UsuarioRolDataModel { UsuarioId = usuario.Id, RolId = rolCliente.Id, Estado = BusinessConstants.EstadoActivo }, cancellationToken);

            var cliente = await _clientes.CrearAsync(new ClienteDataModel { UsuarioId = usuario.Id, TipoIdentificacion = request.TipoIdentificacion, NumeroIdentificacion = request.NumeroIdentificacion, Nombres = request.Nombres, Apellidos = request.Apellidos, RazonSocial = request.RazonSocial, Correo = request.Correo, Telefono = request.Telefono, Direccion = request.Direccion, UsuarioIngreso = request.Usuario, IpIngreso = request.Ip, Estado = BusinessConstants.EstadoActivo }, cancellationToken);
            return new RegisterClienteResponse { ClienteGuid = cliente.Guid, UsuarioGuid = usuario.Guid, Correo = cliente.Correo };
        }

        public Task<BusinessOperationResult> CerrarSesionAsync(LogoutRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(BusinessOperationResult.Ok("Sesion cerrada. El token JWT expira segun su vigencia configurada."));
        }

        private string BuildToken(UsuarioDataModel usuario, DateTime expires, Guid? clienteGuid)
        {
            var secret = string.IsNullOrWhiteSpace(_jwt.SecretKey) ? "development-secret" : _jwt.SecretKey;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = new DateTimeOffset(expires).ToUnixTimeSeconds();
            var header = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object> { ["alg"] = "HS256", ["typ"] = "JWT" }));
            var payload = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object?>
            {
                ["sub"] = usuario.Guid.ToString(),
                ["cliente_guid"] = clienteGuid?.ToString(),
                ["name"] = usuario.Login,
                ["roles"] = usuario.Roles,
                ["iss"] = _jwt.Issuer,
                ["aud"] = _jwt.Audience,
                ["iat"] = now,
                ["exp"] = exp
            }));
            var signingInput = $"{header}.{payload}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var signature = Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
            return $"{signingInput}.{signature}";
        }

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            return $"pbkdf2:100000:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private static bool PasswordMatches(string password, string storedHash)
        {
            if (storedHash.StartsWith("pbkdf2:", StringComparison.OrdinalIgnoreCase))
            {
                var parts = storedHash.Split(':');
                if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations)) return false;
                var salt = Convert.FromBase64String(parts[2]);
                var expected = Convert.FromBase64String(parts[3]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }

            if (storedHash.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
            {
                using var sha = SHA256.Create();
                var legacy = "sha256:" + Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
                return string.Equals(legacy, storedHash, StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(password, storedHash, StringComparison.Ordinal);
        }

        private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}

