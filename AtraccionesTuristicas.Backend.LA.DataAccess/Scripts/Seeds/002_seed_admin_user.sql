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
