using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class FacturaDataMapper
{
    public static FacturaDataModel ToDataModel(FacturaEntity entity) => new()
    {
        Id = entity.fac_id,
        Guid = entity.fac_guid,
        ReservaId = entity.rev_id,
        PagoId = entity.pag_id,
        DatosFacturacionId = entity.dfac_id,
        Numero = entity.fac_numero,
        FechaEmision = entity.fac_fecha_emision,
        Subtotal = entity.fac_subtotal,
        ValorIva = entity.fac_valor_iva,
        Total = entity.fac_total,
        Moneda = entity.fac_moneda,
        Observacion = entity.fac_observacion,
        Estado = entity.fac_estado,
        Reserva = entity.Reserva is null ? null : new FacturaReservaDataModel
        {
            Id = entity.Reserva.rev_id,
            Guid = entity.Reserva.rev_guid,
            Codigo = entity.Reserva.rev_codigo,
            FechaReservaUtc = entity.Reserva.rev_fecha_reserva_utc,
            FechaExpiracionUtc = entity.Reserva.rev_fecha_expiracion_utc,
            Estado = entity.Reserva.rev_estado
        },
        Cliente = entity.Reserva?.Cliente is null ? null : new FacturaClienteDataModel
        {
            Id = entity.Reserva.Cliente.cli_id,
            Guid = entity.Reserva.Cliente.cli_guid,
            TipoIdentificacion = entity.Reserva.Cliente.cli_tipo_identificacion,
            NumeroIdentificacion = entity.Reserva.Cliente.cli_numero_identificacion,
            Nombres = entity.Reserva.Cliente.cli_nombres,
            Apellidos = entity.Reserva.Cliente.cli_apellidos,
            RazonSocial = entity.Reserva.Cliente.cli_razon_social,
            Correo = entity.Reserva.Cliente.cli_correo,
            Telefono = entity.Reserva.Cliente.cli_telefono,
            Direccion = entity.Reserva.Cliente.cli_direccion
        },
        DatosFacturacion = entity.DatosFacturacion is null ? null : new FacturaDatosFacturacionDataModel
        {
            Id = entity.DatosFacturacion.dfac_id,
            Guid = entity.DatosFacturacion.dfac_guid,
            TipoIdentificacion = entity.DatosFacturacion.dfac_tipo_identificacion,
            NumeroIdentificacion = entity.DatosFacturacion.dfac_numero_identificacion,
            RazonSocial = entity.DatosFacturacion.dfac_razon_social,
            Nombre = entity.DatosFacturacion.dfac_nombre,
            Apellido = entity.DatosFacturacion.dfac_apellido,
            Correo = entity.DatosFacturacion.dfac_correo,
            Telefono = entity.DatosFacturacion.dfac_telefono,
            Direccion = entity.DatosFacturacion.dfac_direccion
        },
        Atraccion = entity.Reserva?.Horario?.Atraccion is null ? null : new FacturaAtraccionDataModel
        {
            Id = entity.Reserva.Horario.Atraccion.at_id,
            Guid = entity.Reserva.Horario.Atraccion.at_guid,
            Nombre = entity.Reserva.Horario.Atraccion.at_nombre,
            Direccion = entity.Reserva.Horario.Atraccion.at_direccion,
            Fecha = entity.Reserva.Horario.hor_fecha,
            HoraInicio = entity.Reserva.Horario.hor_hora_inicio,
            HoraFin = entity.Reserva.Horario.hor_hora_fin,
            Destino = entity.Reserva.Horario.Atraccion.Destino?.des_nombre,
            Pais = entity.Reserva.Horario.Atraccion.Destino?.des_pais
        },
        Pago = entity.Pago is null ? null : new FacturaPagoDataModel
        {
            Id = entity.Pago.pag_id,
            Guid = entity.Pago.pag_guid,
            Referencia = entity.Pago.pag_referencia,
            Metodo = entity.Pago.pag_metodo,
            FechaUtc = entity.Pago.pag_fecha_utc,
            Estado = entity.Pago.pag_estado
        },
        Detalles = entity.Reserva?.Detalles.Select(x => new FacturaDetalleDataModel
        {
            Id = x.rdet_id,
            TicketTitulo = x.Ticket?.tck_titulo ?? $"Ticket #{x.tck_id}",
            TipoParticipante = x.Ticket?.tck_tipo_participante ?? string.Empty,
            Cantidad = x.rdet_cantidad,
            PrecioUnitario = x.rdet_precio_unit,
            Subtotal = x.rdet_subtotal
        }).ToList() ?? []
    };
}
