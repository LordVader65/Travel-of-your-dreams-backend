using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.DataManagement.Models;

namespace TravelDreams.MsIdentidad.Business.Services;

internal static class IdentidadMappers
{
    public static UsuarioResponse Usuario(UsuarioDataModel model) => new()
    {
        Guid = model.Guid,
        Login = model.Login,
        Estado = model.Estado,
        Roles = model.Roles.Select(Rol).ToList()
    };

    public static RolResponse Rol(RolDataModel model) => new()
    {
        Id = model.Id,
        Guid = model.Guid,
        Descripcion = model.Descripcion,
        Estado = model.Estado
    };
}
