CREATE OR REPLACE VIEW vw_reservas_detalle AS
SELECT
    r.rev_id,
    r.rev_guid,
    r.rev_codigo,
    r.rev_estado,
    r.rev_fecha_reserva_utc,
    r.rev_fecha_expiracion_utc,
    r.rev_total,
    r.rev_moneda,
    c.cli_id,
    c.cli_guid,
    c.cli_correo,
    h.hor_id,
    h.hor_guid,
    h.hor_fecha,
    h.hor_hora_inicio,
    a.at_id,
    a.at_guid,
    a.at_nombre,
    rd.rdet_id,
    rd.rdet_guid,
    rd.rdet_cantidad,
    rd.rdet_precio_unit,
    rd.rdet_subtotal,
    t.tck_id,
    t.tck_guid,
    t.tck_titulo,
    t.tck_tipo_participante
FROM reservas r
JOIN clientes c ON c.cli_id = r.cli_id
JOIN horario h ON h.hor_id = r.hor_id
JOIN atraccion a ON a.at_id = h.at_id
JOIN reserva_detalle rd ON rd.rev_id = r.rev_id
JOIN ticket t ON t.tck_id = rd.tck_id;
