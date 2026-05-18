START TRANSACTION;

INSERT INTO roles (rol_guid, rol_descripcion, rol_fecha_ingreso, rol_usuario_ingreso, rol_ip_ingreso, rol_estado)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'ADMIN', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A'),
    ('22222222-2222-2222-2222-222222222222', 'CLIENTE', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (rol_descripcion) DO NOTHING;

INSERT INTO usuario (usu_guid, usu_login, usu_password_hash, usu_fecha_registro, usu_usuario_registro, usu_ip_registro, usu_estado)
VALUES
    ('10000000-0000-0000-0000-000000000001', 'admin@travelofyourdreams.local', 'pbkdf2:100000:6yZBXMZiFG9oRWz02gtD0g==:yCyDxLjcCyMd4oFmb+oRjqUvssBrfvPBROCLDe1OyOM=', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A'),
    ('10000000-0000-0000-0000-000000000002', 'cliente1@travelofyourdreams.local', 'pbkdf2:100000:PFuHy84bMnKp8Sj2U6deMg==:YGMu/I/h1PsBk8bdpI/DP035aFt4IMYhMPOeAvFjL4M=', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A'),
    ('10000000-0000-0000-0000-000000000003', 'cliente2@travelofyourdreams.local', 'pbkdf2:100000:Yd9FwpDr0E3eVuon7zyNHA==:pN3xtb6WtmDbOSJn1vtHL9kvlOAXetf64UNZQ93K9rw=', CURRENT_TIMESTAMP, 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (usu_login) DO NOTHING;

INSERT INTO usuarioxroles (usu_id, rol_id, usu_rol_estado)
SELECT u.usu_id, r.rol_id, 'A'
FROM usuario u
JOIN roles r ON r.rol_descripcion = 'ADMIN'
WHERE u.usu_login = 'admin@travelofyourdreams.local'
ON CONFLICT (usu_id, rol_id) DO NOTHING;

INSERT INTO usuarioxroles (usu_id, rol_id, usu_rol_estado)
SELECT u.usu_id, r.rol_id, 'A'
FROM usuario u
JOIN roles r ON r.rol_descripcion = 'CLIENTE'
WHERE u.usu_login IN ('cliente1@travelofyourdreams.local', 'cliente2@travelofyourdreams.local')
ON CONFLICT (usu_id, rol_id) DO NOTHING;

COMMIT;
