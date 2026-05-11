CREATE OR REPLACE FUNCTION fn_crear_reserva(
    p_cli_guid UUID,
    p_hor_guid UUID,
    p_tickets JSONB,
    p_usuario VARCHAR(100),
    p_ip VARCHAR(45),
    p_origen_canal VARCHAR(50) DEFAULT NULL,
    p_expiracion_minutos INTEGER DEFAULT 15,
    p_porcentaje_iva NUMERIC DEFAULT 0
)
RETURNS UUID AS $$
DECLARE
    v_cli_id INTEGER;
    v_hor_id INTEGER;
    v_at_id INTEGER;
    v_cupos_disponibles INTEGER;
    v_cantidad_total INTEGER;
    v_tickets_validos INTEGER;
    v_subtotal NUMERIC(10,2);
    v_iva NUMERIC(10,2);
    v_total NUMERIC(10,2);
    v_rev_id INTEGER;
    v_rev_guid UUID;
    v_codigo VARCHAR(20);
    v_reserva_pendiente INTEGER;
BEGIN
    SELECT cli_id
    INTO v_cli_id
    FROM clientes
    WHERE cli_guid = p_cli_guid AND cli_estado = 'A';

    IF v_cli_id IS NULL THEN
        RAISE EXCEPTION 'Cliente activo no encontrado: %', p_cli_guid;
    END IF;

    SELECT h.hor_id, h.at_id, h.hor_cupos_disponibles
    INTO v_hor_id, v_at_id, v_cupos_disponibles
    FROM horario h
    JOIN atraccion a ON a.at_id = h.at_id
    WHERE h.hor_guid = p_hor_guid
      AND h.hor_estado = 'A'
      AND h.hor_fecha >= CURRENT_DATE
      AND a.at_estado = 'A'
      AND a.at_disponible = TRUE
    FOR UPDATE;

    IF v_hor_id IS NULL THEN
        RAISE EXCEPTION 'Horario activo no encontrado: %', p_hor_guid;
    END IF;

    SELECT r.rev_id
    INTO v_reserva_pendiente
    FROM reservas r
    JOIN horario hr ON hr.hor_id = r.hor_id
    WHERE r.cli_id = v_cli_id
      AND hr.at_id = v_at_id
      AND r.rev_estado = 'PENDIENTE'
    LIMIT 1;

    IF v_reserva_pendiente IS NOT NULL THEN
        RAISE EXCEPTION 'Ya tienes una reserva pendiente para esta atraccion. Debes pagarla, cancelarla o esperar su expiracion antes de crear otra reserva para la misma atraccion.';
    END IF;

    SELECT COALESCE(SUM((item->>'cantidad')::INTEGER), 0)
    INTO v_cantidad_total
    FROM jsonb_array_elements(p_tickets) AS item;

    IF v_cantidad_total <= 0 THEN
        RAISE EXCEPTION 'La reserva debe incluir al menos un ticket';
    END IF;

    IF v_cupos_disponibles < v_cantidad_total THEN
        RAISE EXCEPTION 'Cupos insuficientes. Disponibles: %, solicitados: %', v_cupos_disponibles, v_cantidad_total;
    END IF;

    SELECT COUNT(*)
    INTO v_tickets_validos
    FROM jsonb_array_elements(p_tickets) AS item
    JOIN ticket t ON t.tck_guid = (item->>'tck_guid')::UUID
    WHERE t.tck_estado = 'A'
      AND t.at_id = v_at_id
      AND (item->>'cantidad')::INTEGER > 0
      AND (item->>'cantidad')::INTEGER <= t.tck_capacidad_maxima;

    IF v_tickets_validos <> jsonb_array_length(p_tickets) THEN
        RAISE EXCEPTION 'Todos los tickets deben estar activos, pertenecer a la atraccion del horario y respetar su capacidad maxima';
    END IF;

    SELECT COALESCE(SUM(t.tck_precio * (item->>'cantidad')::INTEGER), 0)
    INTO v_subtotal
    FROM jsonb_array_elements(p_tickets) AS item
    JOIN ticket t ON t.tck_guid = (item->>'tck_guid')::UUID
    WHERE t.tck_estado = 'A'
      AND t.at_id = v_at_id;

    IF v_subtotal < 0 THEN
        RAISE EXCEPTION 'Subtotal de reserva invalido';
    END IF;

    v_iva := ROUND(v_subtotal * COALESCE(p_porcentaje_iva, 0) / 100, 2);
    v_total := v_subtotal + v_iva;
    v_codigo := CONCAT('REV-', UPPER(SUBSTRING(REPLACE(gen_random_uuid()::TEXT, '-', ''), 1, 12)));

    INSERT INTO reservas (
        rev_guid,
        rev_codigo,
        cli_id,
        hor_id,
        rev_fecha_reserva_utc,
        rev_fecha_expiracion_utc,
        rev_subtotal,
        rev_valor_iva,
        rev_total,
        rev_moneda,
        rev_origen_canal,
        rev_usuario_ingreso,
        rev_ip_ingreso,
        rev_estado
    )
    VALUES (
        gen_random_uuid(),
        v_codigo,
        v_cli_id,
        v_hor_id,
        CURRENT_TIMESTAMP,
        CURRENT_TIMESTAMP + MAKE_INTERVAL(mins => p_expiracion_minutos),
        v_subtotal,
        v_iva,
        v_total,
        'USD',
        p_origen_canal,
        p_usuario,
        p_ip,
        'PENDIENTE'
    )
    RETURNING rev_id, rev_guid INTO v_rev_id, v_rev_guid;

    INSERT INTO reserva_detalle (
        rdet_guid,
        rev_id,
        tck_id,
        rdet_cantidad,
        rdet_precio_unit,
        rdet_subtotal,
        rdet_fecha_ingreso,
        rdet_usuario_ingreso,
        rdet_ip_ingreso,
        rdet_estado
    )
    SELECT
        gen_random_uuid(),
        v_rev_id,
        t.tck_id,
        (item->>'cantidad')::INTEGER,
        t.tck_precio,
        t.tck_precio * (item->>'cantidad')::INTEGER,
        CURRENT_TIMESTAMP,
        p_usuario,
        p_ip,
        'A'
    FROM jsonb_array_elements(p_tickets) AS item
    JOIN ticket t ON t.tck_guid = (item->>'tck_guid')::UUID
    WHERE t.tck_estado = 'A';

    UPDATE horario
    SET hor_cupos_disponibles = hor_cupos_disponibles - v_cantidad_total,
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
        NULL,
        'PENDIENTE',
        CURRENT_TIMESTAMP,
        p_usuario,
        p_ip,
        'Reserva creada'
    );

    RETURN v_rev_guid;
END;
$$ LANGUAGE plpgsql;
