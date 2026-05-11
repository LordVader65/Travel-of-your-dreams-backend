using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class HorarioDataService : IHorarioDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public HorarioDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<HorarioDataModel>> ListarAsync(CancellationToken cancellationToken = default) =>
        (await _unitOfWork.Horarios.ListarAsync(cancellationToken)).Select(HorarioDataMapper.ToDataModel).ToList();

    public async Task<HorarioDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Horarios.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null ? null : HorarioDataMapper.ToDataModel(entity);
    }

    public async Task<IReadOnlyList<HorarioDataModel>> ListarDisponiblesPorAtraccionAsync(Guid atraccionGuid, DateOnly? fecha = null, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.HorarioQueries.ListarDisponiblesPorAtraccionAsync(atraccionGuid, fecha, cancellationToken);
        return entities.Select(HorarioDataMapper.ToDataModel).ToList();
    }

    public async Task<HorarioDataModel?> MaterializarParaFechaAsync(Guid horarioBaseGuid, DateOnly fecha, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Horarios.MaterializarParaFechaAsync(horarioBaseGuid, fecha, usuario, ip, cancellationToken);
        if (entity is null) return null;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return HorarioDataMapper.ToDataModel(entity);
    }

    public async Task<HorarioDataModel> CrearAsync(HorarioDataModel model, CancellationToken cancellationToken = default)
    {
        var entity = new HorarioEntity
        {
            hor_guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid,
            at_id = model.AtraccionId,
            hor_fecha = model.Fecha,
            hor_hora_inicio = model.HoraInicio,
            hor_hora_fin = model.HoraFin,
            hor_cupos_disponibles = model.CuposDisponibles,
            hor_dias_semana = model.DiasSemana,
            hor_fecha_ingreso = DateTime.UtcNow,
            hor_usuario_ingreso = model.UsuarioIngreso,
            hor_ip_ingreso = model.IpIngreso,
            hor_estado = string.IsNullOrWhiteSpace(model.Estado) ? DatabaseConstants.EstadoActivo : model.Estado
        };

        await _unitOfWork.Horarios.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return HorarioDataMapper.ToDataModel(entity);
    }

    public async Task<HorarioDataModel?> ActualizarAsync(HorarioDataModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Horarios.ObtenerParaActualizarAsync(model.Guid, cancellationToken);
        if (entity is null) return null;

        entity.at_id = model.AtraccionId;
        entity.hor_fecha = model.Fecha;
        entity.hor_hora_inicio = model.HoraInicio;
        entity.hor_hora_fin = model.HoraFin;
        entity.hor_cupos_disponibles = model.CuposDisponibles;
        entity.hor_dias_semana = model.DiasSemana;
        entity.hor_estado = model.Estado;
        entity.hor_fecha_mod = DateTime.UtcNow;
        entity.hor_usuario_mod = model.UsuarioModificacion;
        entity.hor_ip_mod = model.IpModificacion;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return HorarioDataMapper.ToDataModel(entity);
    }

    public async Task<HorarioDataModel?> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Horarios.ObtenerParaActualizarAsync(guid, cancellationToken);
        if (entity is null) return null;

        entity.hor_estado = estado;
        entity.hor_fecha_mod = DateTime.UtcNow;
        entity.hor_usuario_mod = usuario;
        entity.hor_ip_mod = ip;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return HorarioDataMapper.ToDataModel(entity);
    }

    public async Task<int> DesactivarPasadosOSinCuposAsync(string usuario, string ip, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Horarios.DesactivarHorariosPasadosOSinCuposAsync(usuario, ip, cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
