using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Identity;

public sealed class UsuarioDataService : IUsuarioDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<UsuarioDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Usuarios.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null ? null : UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Usuarios.ObtenerPorLoginAsync(login, cancellationToken);
        return entity is null ? null : UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> ObtenerConRolesAsync(string login, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Usuarios.ObtenerConRolesAsync(login, cancellationToken);
        return entity is null ? null : UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel> CrearAsync(UsuarioDataModel model, string usuarioRegistro, string ipRegistro, CancellationToken cancellationToken = default)
    {
        var entity = new UsuarioEntity
        {
            usu_guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid,
            usu_login = model.Login,
            usu_password_hash = model.PasswordHash,
            usu_estado = model.Estado,
            usu_fecha_registro = DateTime.UtcNow,
            usu_usuario_registro = usuarioRegistro,
            usu_ip_registro = ipRegistro
        };

        await _unitOfWork.Usuarios.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Usuarios.ObtenerParaActualizarAsync(guid, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.usu_estado = estado;
        entity.usu_fecha_mod = DateTime.UtcNow;
        entity.usu_usuario_mod = usuario;
        entity.usu_ip_mod = ip;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UsuarioDataMapper.ToDataModel(entity);
    }

    public async Task<UsuarioDataModel?> CambiarPasswordAsync(Guid guid, string passwordHash, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Usuarios.ObtenerParaActualizarAsync(guid, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.usu_password_hash = passwordHash;
        entity.usu_fecha_mod = DateTime.UtcNow;
        entity.usu_usuario_mod = usuario;
        entity.usu_ip_mod = ip;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UsuarioDataMapper.ToDataModel(entity);
    }
}
