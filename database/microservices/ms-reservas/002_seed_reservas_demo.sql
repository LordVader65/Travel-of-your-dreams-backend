START TRANSACTION;

INSERT INTO clientes (
    cli_guid, usu_guid, cli_tipo_identificacion, cli_numero_identificacion,
    cli_nombres, cli_apellidos, cli_correo, cli_telefono, cli_direccion,
    cli_fecha_ingreso, cli_usuario_ingreso, cli_ip_ingreso, cli_estado
)
VALUES
    ('20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'CC', '0999999999', 'Admin', 'Principal', 'admin@travelofyourdreams.local', '0999999999', 'Quito', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A'),
    ('20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'CC', '0911111111', 'Camila', 'Torres', 'cliente1@travelofyourdreams.local', '0911111111', 'Guayaquil', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A'),
    ('20000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000003', 'CC', '0922222222', 'Mateo', 'Andrade', 'cliente2@travelofyourdreams.local', '0922222222', 'Cuenca', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (cli_numero_identificacion) DO NOTHING;

INSERT INTO reservas (
    rev_guid, rev_codigo, cli_id, at_guid, hor_guid,
    rev_fecha_reserva_utc, rev_fecha_expiracion_utc,
    rev_subtotal, rev_valor_iva, rev_total, rev_moneda, rev_origen_canal,
    rev_usuario_ingreso, rev_ip_ingreso, rev_estado
)
SELECT
    '30000000-0000-0000-0000-000000000001', 'RSV-SEED-001', c.cli_id,
    '40000000-0000-0000-0000-000000000001', '50000000-0000-0000-0000-000000000001',
    CURRENT_TIMESTAMP, TIMESTAMPTZ '2030-01-01T23:59:00Z',
    25.00, 3.00, 28.00, 'USD', 'WEB',
    'seed-demo', '127.0.0.1', 'PENDIENTE'
FROM clientes c
WHERE c.cli_guid = '20000000-0000-0000-0000-000000000002'
ON CONFLICT (rev_codigo) DO NOTHING;

INSERT INTO reservas (
    rev_guid, rev_codigo, cli_id, at_guid, hor_guid,
    rev_fecha_reserva_utc, rev_fecha_expiracion_utc,
    rev_subtotal, rev_valor_iva, rev_total, rev_moneda, rev_origen_canal,
    rev_usuario_ingreso, rev_ip_ingreso, rev_estado
)
SELECT
    '30000000-0000-0000-0000-000000000002', 'RSV-SEED-002', c.cli_id,
    '40000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000002',
    CURRENT_TIMESTAMP, TIMESTAMPTZ '2030-01-02T23:59:00Z',
    35.00, 4.20, 39.20, 'USD', 'BOOKING',
    'seed-demo', '127.0.0.1', 'PAGADA'
FROM clientes c
WHERE c.cli_guid = '20000000-0000-0000-0000-000000000003'
ON CONFLICT (rev_codigo) DO NOTHING;

INSERT INTO reserva_detalle (
    rdet_guid, rev_id, tck_guid, rdet_ticket_titulo, rdet_tipo_participante,
    rdet_cantidad, rdet_precio_unit, rdet_subtotal,
    rdet_usuario_ingreso, rdet_ip_ingreso, rdet_estado
)
SELECT
    '31000000-0000-0000-0000-000000000001', r.rev_id,
    '60000000-0000-0000-0000-000000000001', 'Entrada adulto - Centro historico', 'Adulto',
    1, 25.00, 25.00, 'seed-demo', '127.0.0.1', 'A'
FROM reservas r
WHERE r.rev_guid = '30000000-0000-0000-0000-000000000001'
ON CONFLICT (rev_id, tck_guid) DO NOTHING;

INSERT INTO reserva_detalle (
    rdet_guid, rev_id, tck_guid, rdet_ticket_titulo, rdet_tipo_participante,
    rdet_cantidad, rdet_precio_unit, rdet_subtotal,
    rdet_usuario_ingreso, rdet_ip_ingreso, rdet_estado
)
SELECT
    '31000000-0000-0000-0000-000000000002', r.rev_id,
    '60000000-0000-0000-0000-000000000002', 'Entrada adulto - Aventura natural', 'Adulto',
    1, 35.00, 35.00, 'seed-demo', '127.0.0.1', 'A'
FROM reservas r
WHERE r.rev_guid = '30000000-0000-0000-0000-000000000002'
ON CONFLICT (rev_id, tck_guid) DO NOTHING;

INSERT INTO reserva_estado_historial (reh_guid, rev_id, reh_estado_anterior, reh_estado_nuevo, reh_usuario, reh_ip, reh_observacion)
SELECT '32000000-0000-0000-0000-000000000001', r.rev_id, NULL, 'PENDIENTE', 'seed-demo', '127.0.0.1', 'Reserva demo creada'
FROM reservas r
WHERE r.rev_guid = '30000000-0000-0000-0000-000000000001'
ON CONFLICT (reh_guid) DO NOTHING;

INSERT INTO reserva_estado_historial (reh_guid, rev_id, reh_estado_anterior, reh_estado_nuevo, reh_usuario, reh_ip, reh_observacion)
SELECT '32000000-0000-0000-0000-000000000002', r.rev_id, 'PENDIENTE', 'PAGADA', 'seed-demo', '127.0.0.1', 'Pago demo aprobado'
FROM reservas r
WHERE r.rev_guid = '30000000-0000-0000-0000-000000000002'
ON CONFLICT (reh_guid) DO NOTHING;

COMMIT;
