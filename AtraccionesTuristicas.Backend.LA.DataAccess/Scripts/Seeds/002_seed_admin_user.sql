-- TODO: reemplazar este hash por uno generado desde el flujo formal de usuarios antes de producción.
-- Usuario administrador inicial de desarrollo.
INSERT INTO usuario (
    usu_guid,
    usu_login,
    usu_password_hash,
    usu_fecha_registro,
    usu_usuario_registro,
    usu_ip_registro,
    usu_estado
)
VALUES (
    '20000000-0000-0000-0000-000000000001',
    'admin@travelofyourdreams.local',
    'pbkdf2:100000:Lb+6DgW9jRVuNwjnR+yZ/g==:medD9r9GCBDiqtk+aN1r+RL2KFSzkqXFNp87c3SVzKc=',
    CURRENT_TIMESTAMP,
    'seed',
    '127.0.0.1',
    'A'
)
ON CONFLICT (usu_guid) DO UPDATE
SET usu_login = EXCLUDED.usu_login,
    usu_password_hash = EXCLUDED.usu_password_hash,
    usu_estado = 'A';

INSERT INTO usuarioxroles (
    usu_id,
    rol_id,
    usu_rol_estado
)
SELECT u.usu_id, r.rol_id, 'A'
FROM usuario u
JOIN roles r ON r.rol_descripcion = 'ADMIN'
WHERE u.usu_guid = '20000000-0000-0000-0000-000000000001'
ON CONFLICT (usu_id, rol_id) DO UPDATE
SET usu_rol_estado = 'A';

INSERT INTO clientes (
    cli_guid,
    usu_id,
    cli_tipo_identificacion,
    cli_numero_identificacion,
    cli_nombres,
    cli_apellidos,
    cli_razon_social,
    cli_correo,
    cli_telefono,
    cli_direccion,
    cli_usuario_ingreso,
    cli_ip_ingreso,
    cli_estado
)
SELECT
    '20000000-0000-0000-0000-000000000101',
    u.usu_id,
    'CEDULA',
    '9999999999',
    'Administrador',
    'General',
    'Travel of Your Dreams',
    u.usu_login,
    '0999999999',
    'Quito, Ecuador',
    'seed',
    '127.0.0.1',
    'A'
FROM usuario u
WHERE u.usu_guid = '20000000-0000-0000-0000-000000000001'
ON CONFLICT (cli_numero_identificacion) DO UPDATE
SET usu_id = EXCLUDED.usu_id,
    cli_nombres = EXCLUDED.cli_nombres,
    cli_apellidos = EXCLUDED.cli_apellidos,
    cli_razon_social = EXCLUDED.cli_razon_social,
    cli_correo = EXCLUDED.cli_correo,
    cli_telefono = EXCLUDED.cli_telefono,
    cli_direccion = EXCLUDED.cli_direccion,
    cli_estado = 'A';
