INSERT INTO roles (
    rol_guid,
    rol_descripcion,
    rol_fecha_ingreso,
    rol_usuario_ingreso,
    rol_ip_ingreso,
    rol_estado
)
VALUES
    ('10000000-0000-0000-0000-000000000001', 'ADMIN', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('10000000-0000-0000-0000-000000000002', 'CLIENTE', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A')
ON CONFLICT (rol_guid) DO UPDATE
SET rol_descripcion = EXCLUDED.rol_descripcion,
    rol_estado = 'A';
