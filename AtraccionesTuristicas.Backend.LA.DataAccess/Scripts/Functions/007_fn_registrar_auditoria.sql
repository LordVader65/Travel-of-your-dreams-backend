CREATE OR REPLACE FUNCTION fn_registrar_auditoria(
    p_tabla VARCHAR(100),
    p_operacion VARCHAR(20),
    p_registro_id INTEGER,
    p_registro_guid UUID,
    p_datos_anteriores TEXT,
    p_datos_nuevos TEXT,
    p_usuario VARCHAR(100),
    p_ip VARCHAR(45),
    p_origen_canal VARCHAR(200) DEFAULT NULL
)
RETURNS BIGINT AS $$
DECLARE
    v_log_id BIGINT;
BEGIN
    INSERT INTO auditoria_log (
        log_guid,
        log_tabla,
        log_operacion,
        log_registro_id,
        log_registro_guid,
        log_datos_anteriores,
        log_datos_nuevos,
        log_fecha_utc,
        log_usuario,
        log_ip,
        log_origen_canal
    )
    VALUES (
        gen_random_uuid(),
        p_tabla,
        p_operacion,
        p_registro_id,
        p_registro_guid,
        p_datos_anteriores,
        p_datos_nuevos,
        CURRENT_TIMESTAMP,
        p_usuario,
        p_ip,
        p_origen_canal
    )
    RETURNING log_id INTO v_log_id;

    RETURN v_log_id;
END;
$$ LANGUAGE plpgsql;
