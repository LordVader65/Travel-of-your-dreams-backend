CREATE OR REPLACE FUNCTION fn_generar_factura(
    p_rev_guid UUID,
    p_dfac_guid UUID,
    p_usuario VARCHAR(100),
    p_ip VARCHAR(45),
    p_observacion VARCHAR(500) DEFAULT NULL,
    p_origen_canal VARCHAR(50) DEFAULT NULL
)
RETURNS UUID AS $$
DECLARE
    v_rev_id INTEGER;
    v_cli_id INTEGER;
    v_pag_id INTEGER;
    v_dfac_id INTEGER;
    v_subtotal NUMERIC(10,2);
    v_iva NUMERIC(10,2);
    v_total NUMERIC(10,2);
    v_estado VARCHAR(20);
    v_fac_guid UUID;
    v_fac_dfac_id INTEGER;
    v_numero VARCHAR(20);
BEGIN
    SELECT rev_id, cli_id, rev_subtotal, rev_valor_iva, rev_total, rev_estado
    INTO v_rev_id, v_cli_id, v_subtotal, v_iva, v_total, v_estado
    FROM reservas
    WHERE rev_guid = p_rev_guid;

    IF v_rev_id IS NULL THEN
        RAISE EXCEPTION 'Reserva no encontrada: %', p_rev_guid;
    END IF;

    IF v_estado NOT IN ('PAGADA','CONFIRMADA') THEN
        RAISE EXCEPTION 'La reserva debe estar PAGADA o CONFIRMADA para facturar. Estado: %', v_estado;
    END IF;

    SELECT fac_guid, dfac_id
    INTO v_fac_guid, v_fac_dfac_id
    FROM facturas
    WHERE rev_id = v_rev_id;

    SELECT pag_id
    INTO v_pag_id
    FROM pagos
    WHERE rev_id = v_rev_id AND pag_estado = 'APROBADO'
    ORDER BY pag_fecha_utc DESC
    LIMIT 1;

    IF p_dfac_guid IS NOT NULL THEN
        SELECT dfac_id
        INTO v_dfac_id
        FROM datos_facturacion
        WHERE dfac_guid = p_dfac_guid
          AND cli_id = v_cli_id
          AND dfac_estado = 'A';

        IF v_dfac_id IS NULL THEN
            RAISE EXCEPTION 'Datos de facturación activos no encontrados para el cliente';
        END IF;
    END IF;

    IF v_fac_guid IS NOT NULL THEN
        IF p_dfac_guid IS NOT NULL AND v_fac_dfac_id IS NULL THEN
            UPDATE facturas
            SET dfac_id = v_dfac_id,
                fac_fecha_mod = CURRENT_TIMESTAMP,
                fac_usuario_mod = p_usuario,
                fac_ip_mod = p_ip
            WHERE fac_guid = v_fac_guid;
        END IF;

        RETURN v_fac_guid;
    END IF;

    v_numero := CONCAT('FAC-', UPPER(SUBSTRING(REPLACE(gen_random_uuid()::TEXT, '-', ''), 1, 12)));

    INSERT INTO facturas (
        fac_guid,
        rev_id,
        pag_id,
        dfac_id,
        fac_numero,
        fac_fecha_emision,
        fac_subtotal,
        fac_valor_iva,
        fac_total,
        fac_moneda,
        fac_observacion,
        fac_origen_canal,
        fac_usuario_ingreso,
        fac_ip_ingreso,
        fac_estado
    )
    VALUES (
        gen_random_uuid(),
        v_rev_id,
        v_pag_id,
        v_dfac_id,
        v_numero,
        CURRENT_TIMESTAMP,
        v_subtotal,
        v_iva,
        v_total,
        'USD',
        p_observacion,
        p_origen_canal,
        p_usuario,
        p_ip,
        'A'
    )
    RETURNING fac_guid INTO v_fac_guid;

    RETURN v_fac_guid;
END;
$$ LANGUAGE plpgsql;
