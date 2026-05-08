using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Cliente;

public sealed class DatosFacturacionDataService : IDatosFacturacionDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public DatosFacturacionDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<DatosFacturacionDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.DatosFacturacion.ListarAsync(cancellationToken)).Select(DatosFacturacionDataMapper.ToDataModel).ToList();
    public async Task<DatosFacturacionDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.DatosFacturacion.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? DatosFacturacionDataMapper.ToDataModel(e) : null;
    public async Task<DatosFacturacionDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _unitOfWork.DatosFacturacion.ObtenerPorGuidAsync(guid, cancellationToken)) is { } e ? DatosFacturacionDataMapper.ToDataModel(e) : null;
    public async Task<IReadOnlyList<DatosFacturacionDataModel>> ListarActivosPorClienteAsync(Guid clienteGuid, CancellationToken cancellationToken = default) => (await _unitOfWork.DatosFacturacion.ListarActivosPorClienteAsync(clienteGuid, cancellationToken)).Select(DatosFacturacionDataMapper.ToDataModel).ToList();
    public async Task<DatosFacturacionDataModel> CrearAsync(DatosFacturacionDataModel model, CancellationToken cancellationToken = default) { var e = DatosFacturacionDataMapper.ToEntity(model); await _unitOfWork.DatosFacturacion.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return DatosFacturacionDataMapper.ToDataModel(e); }
    public async Task<DatosFacturacionDataModel> ActualizarAsync(DatosFacturacionDataModel model, CancellationToken cancellationToken = default)
    {
        var e = await _unitOfWork.DatosFacturacion.ObtenerParaActualizarAsync(model.Guid, cancellationToken) ?? DatosFacturacionDataMapper.ToEntity(model);
        e.dfac_tipo_identificacion = model.TipoIdentificacion; e.dfac_numero_identificacion = model.NumeroIdentificacion;
        e.dfac_razon_social = model.RazonSocial; e.dfac_nombre = model.Nombre; e.dfac_apellido = model.Apellido;
        e.dfac_correo = model.Correo; e.dfac_telefono = model.Telefono; e.dfac_direccion = model.Direccion;
        e.dfac_estado = model.Estado;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return DatosFacturacionDataMapper.ToDataModel(e);
    }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.DatosFacturacion.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; e.dfac_estado = "I"; e.dfac_fecha_eliminacion = DateTime.UtcNow; await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
