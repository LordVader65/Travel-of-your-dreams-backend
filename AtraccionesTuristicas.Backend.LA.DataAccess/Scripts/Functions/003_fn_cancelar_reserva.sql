CREATE OR REPLACE FUNCTION fn_cancelar_reserva(
    p_rev_guid UUID,
    p_usuario VARCHAR(100),
    p_ip VARCHAR(45),
    p_motivo VARCHAR(300)
)
RETURNS VOID AS $$
DECLARE
    v_rev_id INTEGER;
    v_hor_id INTEGER;
    v_estado_actual VARCHAR(20);
    v_cantidad_total INTEGER;
BEGIN
    SELECT rev_id, hor_id, rev_estado
    INTO v_rev_id, v_hor_id, v_estado_actual
    FROM reservas
    WHERE rev_guid = p_rev_guid
    FOR UPDATE;

    IF v_rev_id IS NULL THEN
        RAISE EXCEPTION 'Reserva no encontrada: %', p_rev_guid;
    END IF;

    IF v_estado_actual IN ('CANCELADA','EXPIRADA','USADA','NO_SHOW') THEN
        RAISE EXCEPTION 'La reserva no es cancelable. Estado actual: %', v_estado_actual;
    END IF;

    SELECT COALESCE(SUM(rdet_cantidad), 0)
    INTO v_cantidad_total
    FROM reserva_detalle
    WHERE rev_id = v_rev_id AND rdet_estado = 'A';

    UPDATE reservas
    SET rev_estado = 'CANCELADA',
        rev_fecha_cancelacion = CURRENT_TIMESTAMP,
        rev_usuario_cancelacion = p_usuario,
        rev_ip_cancelacion = p_ip,
        rev_motivo_cancelacion = p_motivo,
        rev_fecha_mod = CURRENT_TIMESTAMP,
        rev_usuario_mod = p_usuario,
        rev_ip_mod = p_ip
    WHERE rev_id = v_rev_id;

    UPDATE horario
    SET hor_cupos_disponibles = hor_cupos_disponibles + v_cantidad_total,
        hor_fecha_mod = CURRENT_TIMESTAMP,
        hor_usuario_mod = p_usuario,
        hor_ip_mod = p_ip
    WHERE hor_id = v_hor_id;

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
        v_rev_id,
        v_estado_actual,
        'CANCELADA',
        CURRENT_TIMESTAMP,
        p_usuario,
        p_ip,
        p_motivo
    );
END;
$$ LANGUAGE plpgsql;
