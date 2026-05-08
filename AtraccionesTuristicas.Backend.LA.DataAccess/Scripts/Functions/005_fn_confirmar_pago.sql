CREATE OR REPLACE FUNCTION fn_confirmar_pago(
    p_rev_guid UUID,
    p_metodo VARCHAR(50),
    p_monto NUMERIC(10,2),
    p_referencia VARCHAR(100),
    p_usuario VARCHAR(100),
    p_ip VARCHAR(45),
    p_origen_canal VARCHAR(50) DEFAULT NULL
)
RETURNS UUID AS $$
DECLARE
    v_rev_id INTEGER;
    v_estado_actual VARCHAR(20);
    v_total NUMERIC(10,2);
    v_expiracion TIMESTAMPTZ;
    v_pag_guid UUID;
BEGIN
    SELECT rev_id, rev_estado, rev_total, rev_fecha_expiracion_utc
    INTO v_rev_id, v_estado_actual, v_total, v_expiracion
    FROM reservas
    WHERE rev_guid = p_rev_guid
    FOR UPDATE;

    IF v_rev_id IS NULL THEN
        RAISE EXCEPTION 'Reserva no encontrada: %', p_rev_guid;
    END IF;

    IF v_estado_actual <> 'PENDIENTE' THEN
        RAISE EXCEPTION 'Solo se puede pagar una reserva PENDIENTE. Estado actual: %', v_estado_actual;
    END IF;

    IF v_expiracion <= CURRENT_TIMESTAMP THEN
        RAISE EXCEPTION 'La reserva se encuentra expirada';
    END IF;

    IF p_monto <> v_total THEN
        RAISE EXCEPTION 'Monto de pago inválido. Esperado: %, recibido: %', v_total, p_monto;
    END IF;

    INSERT INTO pagos (
        pag_guid,
        rev_id,
        pag_referencia,
        pag_metodo,
        pag_monto,
        pag_moneda,
        pag_fecha_utc,
        pag_estado,
        pag_origen_canal,
        pag_usuario_ingreso,
        pag_ip_ingreso
    )
    VALUES (
        gen_random_uuid(),
        v_rev_id,
        p_referencia,
        p_metodo,
        p_monto,
        'USD',
        CURRENT_TIMESTAMP,
        'APROBADO',
        p_origen_canal,
        p_usuario,
        p_ip
    )
    RETURNING pag_guid INTO v_pag_guid;

    UPDATE reservas
    SET rev_estado = 'PAGADA',
        rev_fecha_mod = CURRENT_TIMESTAMP,
        rev_usuario_mod = p_usuario,
        rev_ip_mod = p_ip
    WHERE rev_id = v_rev_id;

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
        'PAGADA',
        CURRENT_TIMESTAMP,
        p_usuario,
        p_ip,
        'Pago aprobado'
    );

    RETURN v_pag_guid;
END;
$$ LANGUAGE plpgsql;
