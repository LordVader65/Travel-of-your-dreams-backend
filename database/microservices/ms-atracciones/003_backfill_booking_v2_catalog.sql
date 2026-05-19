START TRANSACTION;

UPDATE idioma
SET id_codigo = lower(id_codigo)
WHERE id_codigo IN ('ES', 'EN');

UPDATE categoria
SET cat_nombre = 'Tours',
    cat_tagname = 'tours'
WHERE cat_guid = '71000000-0000-0000-0000-000000000001';

INSERT INTO categoria (cat_guid, cat_nombre, cat_tagname, cat_parent_id, cat_usuario_ingreso, cat_ip_ingreso, cat_estado)
SELECT
    '71000000-0000-0000-0000-000000000003',
    'Tours de ciudad',
    'tours-ciudad',
    c.cat_id,
    'seed-demo',
    '127.0.0.1',
    'A'
FROM categoria c
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000001'
ON CONFLICT (cat_guid) DO NOTHING;

INSERT INTO categoria (cat_guid, cat_nombre, cat_tagname, cat_parent_id, cat_usuario_ingreso, cat_ip_ingreso, cat_estado)
SELECT
    '71000000-0000-0000-0000-000000000004',
    'Naturaleza',
    'naturaleza',
    c.cat_id,
    'seed-demo',
    '127.0.0.1',
    'A'
FROM categoria c
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000002'
ON CONFLICT (cat_guid) DO NOTHING;

INSERT INTO categoria_atraccion (cat_id, at_id, ca_usuario_ingreso, ca_estado)
SELECT c.cat_id, a.at_id, 'seed-demo', 'A'
FROM categoria c
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000001'
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000003'
ON CONFLICT (cat_id, at_id) DO NOTHING;

INSERT INTO categoria_atraccion (cat_id, at_id, ca_usuario_ingreso, ca_estado)
SELECT c.cat_id, a.at_id, 'seed-demo', 'A'
FROM categoria c
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000002'
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000004'
ON CONFLICT (cat_id, at_id) DO NOTHING;

INSERT INTO incluye (inc_guid, inc_descripcion, inc_tipo, inc_estado)
VALUES
    ('74000000-0000-0000-0000-000000000003', 'Propinas', 'NO_INCLUYE', 'A'),
    ('74000000-0000-0000-0000-000000000004', 'Alimentacion', 'NO_INCLUYE', 'A')
ON CONFLICT (inc_guid) DO NOTHING;

INSERT INTO atraccion_incluye (inc_id, at_id, ai_usuario_ingreso, ai_estado)
SELECT inc.inc_id, a.at_id, 'seed-demo', 'A'
FROM incluye inc
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000001'
WHERE inc.inc_guid = '74000000-0000-0000-0000-000000000003'
ON CONFLICT (inc_id, at_id) DO NOTHING;

INSERT INTO atraccion_incluye (inc_id, at_id, ai_usuario_ingreso, ai_estado)
SELECT inc.inc_id, a.at_id, 'seed-demo', 'A'
FROM incluye inc
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000002'
WHERE inc.inc_guid = '74000000-0000-0000-0000-000000000004'
ON CONFLICT (inc_id, at_id) DO NOTHING;

COMMIT;
