CREATE OR REPLACE FUNCTION fn_expirar_reservas_pendientes(
    p_usuario VARCHAR(100) DEFAULT 'system',
    p_ip VARCHAR(45) DEFAULT '127.0.0.1'
)
RETURNS INTEGER AS $$
DECLARE
    v_reserva RECORD;
    v_cantidad_total INTEGER;
    v_total_expiradas INTEGER := 0;
BEGIN
    FOR v_reserva IN
        SELECT rev_id, hor_id, rev_estado
        FROM reservas
        WHERE rev_estado = 'PENDIENTE'
          AND rev_fecha_expiracion_utc <= CURRENT_TIMESTAMP
        FOR UPDATE
    LOOP
        SELECT COALESCE(SUM(rdet_cantidad), 0)
        INTO v_cantidad_total
        FROM reserva_detalle
        WHERE rev_id = v_reserva.rev_id AND rdet_estado = 'A';

        UPDATE reservas
        SET rev_estado = 'EXPIRADA',
            rev_fecha_mod = CURRENT_TIMESTAMP,
            rev_usuario_mod = p_usuario,
            rev_ip_mod = p_ip
        WHERE rev_id = v_reserva.rev_id;

        UPDATE horario
        SET hor_cupos_disponibles = hor_cupos_disponibles + v_cantidad_total,
            hor_fecha_mod = CURRENT_TIMESTAMP,
            hor_usuario_mod = p_usuario,
            hor_ip_mod = p_ip
        WHERE hor_id = v_reserva.hor_id;

        INSERT INTO reserva_estado_historial (
            reh_guid,
            rev_id,
            reh_estado_anterior,
            reh_estado_nuevo,
            reh_fecha_utc,
            reh_usuario,
            reh_ip,
            reh_observacion
        )
        VALUES (
            gen_random_uuid(),
            v_reserva.rev_id,
            'PENDIENTE',
            'EXPIRADA',
            CURRENT_TIMESTAMP,
            p_usuario,
            p_ip,
            'Expiración automática'
        );

        v_total_expiradas := v_total_expiradas + 1;
    END LOOP;

    RETURN v_total_expiradas;
END;
$$ LANGUAGE plpgsql;
