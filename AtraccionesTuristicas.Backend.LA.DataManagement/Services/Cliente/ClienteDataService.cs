using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Cliente;

public sealed class ClienteDataService : IClienteDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<DataPagedResult<ClienteDataModel>> ListarAsync(ClienteFiltroDataModel filtro, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ClienteQueries.ListarAsync(filtro.NumeroIdentificacion, filtro.Correo, filtro.Estado, filtro.Page, filtro.Limit, cancellationToken);
        return new DataPagedResult<ClienteDataModel>
        {
            Items = result.Items.Select(ClienteDataMapper.ToDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public async Task<ClienteDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Clientes.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Clientes.ObtenerPorIdentificacionAsync(numeroIdentificacion, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Clientes.ObtenerPorUsuarioIdAsync(usuarioId, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel> CrearAsync(ClienteDataModel model, CancellationToken cancellationToken = default)
    {
        var entity = ClienteDataMapper.ToEntity(model);
        await _unitOfWork.Clientes.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel?> ActualizarAsync(ClienteDataModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Clientes.ObtenerParaActualizarAsync(model.Guid, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.cli_tipo_identificacion = model.TipoIdentificacion;
        entity.cli_numero_identificacion = model.NumeroIdentificacion;
        entity.cli_nombres = model.Nombres;
        entity.cli_apellidos = model.Apellidos;
        entity.cli_razon_social = model.RazonSocial;
        entity.cli_correo = model.Correo;
        entity.cli_telefono = model.Telefono;
        entity.cli_direccion = model.Direccion;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<bool> EliminarLogicamenteAsync(Guid guid, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Clientes.ObtenerParaActualizarAsync(guid, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.cli_estado = "I";
        entity.cli_fecha_eliminacion = DateTime.UtcNow;
        entity.cli_usuario_eliminacion = usuario;
        entity.cli_ip_eliminacion = ip;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Clientes.ObtenerParaActualizarAsync(guid, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.cli_estado = estado;
        if (estado == "I")
        {
            entity.cli_fecha_eliminacion = DateTime.UtcNow;
            entity.cli_usuario_eliminacion = usuario;
            entity.cli_ip_eliminacion = ip;
        }
        else
        {
            entity.cli_fecha_eliminacion = null;
            entity.cli_usuario_eliminacion = null;
            entity.cli_ip_eliminacion = null;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
