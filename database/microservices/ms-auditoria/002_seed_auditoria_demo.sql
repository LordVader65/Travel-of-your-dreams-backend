START TRANSACTION;

INSERT INTO eventos_procesados (
    ep_evento_id, ep_tipo, ep_origen_servicio, ep_fecha_procesado_utc, ep_correlation_id
)
VALUES
    ('90000000-0000-0000-0000-000000000001', 'UsuarioCreado', 'ms-identidad', CURRENT_TIMESTAMP, 'seed-correlation-001'),
    ('90000000-0000-0000-0000-000000000002', 'ReservaPagada', 'ms-reservas', CURRENT_TIMESTAMP, 'seed-correlation-002')
ON CONFLICT (ep_evento_id) DO NOTHING;

INSERT INTO auditoria_log (
    log_guid, log_servicio, log_tabla, log_operacion, log_registro_guid,
    log_datos_anteriores, log_datos_nuevos, log_fecha_utc,
    log_usuario, log_ip, log_origen_canal, log_correlation_id, evento_id
)
VALUES
    (
        '91000000-0000-0000-0000-000000000001',
        'ms-identidad', 'usuario', 'INSERT', '10000000-0000-0000-0000-000000000002',
        NULL, '{"usu_login":"cliente1@travelofyourdreams.local","rol":"CLIENTE"}',
        CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'seed', 'seed-correlation-001',
        '90000000-0000-0000-0000-000000000001'
    ),
    (
        '91000000-0000-0000-0000-000000000002',
        'ms-reservas', 'reservas', 'UPDATE', '30000000-0000-0000-0000-000000000002',
        '{"rev_estado":"PENDIENTE"}', '{"rev_estado":"PAGADA"}',
        CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'seed', 'seed-correlation-002',
        '90000000-0000-0000-0000-000000000002'
    )
ON CONFLICT (log_guid) DO NOTHING;

COMMIT;
