namespace AtraccionesTuristicas.Backend.LA.Business.Common;

public sealed class CurrentUserData
    {
        public Guid? UsuarioGuid { get; set; }
        public Guid? ClienteGuid { get; set; }
        public string Login { get; set; } = string.Empty;
        public IReadOnlyList<string> Roles { get; set; } = [];
        public string Ip { get; set; } = "127.0.0.1";
        public bool EsAdmin => Roles.Contains(BusinessConstants.RolAdmin, StringComparer.OrdinalIgnoreCase);
        public bool EsCliente => Roles.Contains(BusinessConstants.RolCliente, StringComparer.OrdinalIgnoreCase);
    }

