START TRANSACTION;

INSERT INTO destino (des_guid, des_nombre, des_pais, des_imagen_url, des_usuario_ingreso, des_ip_ingreso, des_estado)
VALUES
    ('70000000-0000-0000-0000-000000000001', 'Quito', 'Ecuador', 'https://example.com/quito.jpg', 'seed-demo', '127.0.0.1', 'A'),
    ('70000000-0000-0000-0000-000000000002', 'Banos', 'Ecuador', 'https://example.com/banos.jpg', 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (des_guid) DO NOTHING;

INSERT INTO categoria (cat_guid, cat_nombre, cat_tagname, cat_usuario_ingreso, cat_ip_ingreso, cat_estado)
VALUES
    ('71000000-0000-0000-0000-000000000001', 'Tours', 'tours', 'seed-demo', '127.0.0.1', 'A'),
    ('71000000-0000-0000-0000-000000000002', 'Aventura', 'aventura', 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (cat_guid) DO UPDATE
SET cat_nombre = EXCLUDED.cat_nombre,
    cat_tagname = EXCLUDED.cat_tagname,
    cat_estado = EXCLUDED.cat_estado;

INSERT INTO categoria (cat_guid, cat_nombre, cat_tagname, cat_parent_id, cat_usuario_ingreso, cat_ip_ingreso, cat_estado)
SELECT '71000000-0000-0000-0000-000000000003', 'Tours de ciudad', 'tours-ciudad', c.cat_id, 'seed-demo', '127.0.0.1', 'A'
FROM categoria c
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000001'
ON CONFLICT (cat_guid) DO NOTHING;

INSERT INTO categoria (cat_guid, cat_nombre, cat_tagname, cat_parent_id, cat_usuario_ingreso, cat_ip_ingreso, cat_estado)
SELECT '71000000-0000-0000-0000-000000000004', 'Naturaleza', 'naturaleza', c.cat_id, 'seed-demo', '127.0.0.1', 'A'
FROM categoria c
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000002'
ON CONFLICT (cat_guid) DO NOTHING;

INSERT INTO idioma (id_guid, id_codigo, id_descripcion, id_usuario_ingreso, id_ip_ingreso, id_estado)
VALUES
    ('72000000-0000-0000-0000-000000000001', 'es', 'Espanol', 'seed-demo', '127.0.0.1', 'A'),
    ('72000000-0000-0000-0000-000000000002', 'en', 'Ingles', 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (id_guid) DO UPDATE
SET id_codigo = EXCLUDED.id_codigo,
    id_descripcion = EXCLUDED.id_descripcion,
    id_estado = EXCLUDED.id_estado;

INSERT INTO imagen (img_guid, img_url, img_descripcion, img_usuario_ingreso, img_ip_ingreso, img_estado)
VALUES
    ('73000000-0000-0000-0000-000000000001', 'https://example.com/atracciones/centro-historico.jpg', 'Centro historico de Quito', 'seed-demo', '127.0.0.1', 'A'),
    ('73000000-0000-0000-0000-000000000002', 'https://example.com/atracciones/ruta-cascadas.jpg', 'Ruta de cascadas en Banos', 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (img_guid) DO NOTHING;

INSERT INTO incluye (inc_guid, inc_descripcion, inc_tipo, inc_estado)
VALUES
    ('74000000-0000-0000-0000-000000000001', 'Guia local certificado', 'INCLUYE', 'A'),
    ('74000000-0000-0000-0000-000000000002', 'Transporte compartido', 'INCLUYE', 'A'),
    ('74000000-0000-0000-0000-000000000003', 'Propinas', 'NO_INCLUYE', 'A'),
    ('74000000-0000-0000-0000-000000000004', 'Alimentacion', 'NO_INCLUYE', 'A')
ON CONFLICT (inc_guid) DO NOTHING;

INSERT INTO atraccion (
    at_guid, des_id, at_num_establecimiento, at_nombre, at_descripcion,
    at_duracion_minutos, at_punto_encuentro, at_precio_referencia,
    at_incluye_acompaniante, at_incluye_transporte, at_disponible,
    at_free_cancellation, at_skip_the_line,
    at_usuario_ingreso, at_ip_ingreso, at_estado
)
SELECT
    '40000000-0000-0000-0000-000000000001', d.des_id, 'QUI-001',
    'Tour Centro Historico de Quito',
    'Recorrido guiado por plazas, iglesias y miradores del centro historico.',
    180, 'Plaza Grande', 25.00,
    TRUE, FALSE, TRUE, TRUE, FALSE,
    'seed-demo', '127.0.0.1', 'A'
FROM destino d
WHERE d.des_guid = '70000000-0000-0000-0000-000000000001'
ON CONFLICT (at_guid) DO NOTHING;

INSERT INTO atraccion (
    at_guid, des_id, at_num_establecimiento, at_nombre, at_descripcion,
    at_duracion_minutos, at_punto_encuentro, at_precio_referencia,
    at_incluye_acompaniante, at_incluye_transporte, at_disponible,
    at_free_cancellation, at_skip_the_line,
    at_usuario_ingreso, at_ip_ingreso, at_estado
)
SELECT
    '40000000-0000-0000-0000-000000000002', d.des_id, 'BAN-001',
    'Ruta de Cascadas en Banos',
    'Experiencia de aventura por miradores y cascadas principales.',
    240, 'Terminal turistico de Banos', 35.00,
    TRUE, TRUE, TRUE, TRUE, TRUE,
    'seed-demo', '127.0.0.1', 'A'
FROM destino d
WHERE d.des_guid = '70000000-0000-0000-0000-000000000002'
ON CONFLICT (at_guid) DO NOTHING;

INSERT INTO categoria_atraccion (cat_id, at_id, ca_usuario_ingreso, ca_estado)
SELECT c.cat_id, a.at_id, 'seed-demo', 'A'
FROM categoria c
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000001'
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000001'
ON CONFLICT (cat_id, at_id) DO NOTHING;

INSERT INTO categoria_atraccion (cat_id, at_id, ca_usuario_ingreso, ca_estado)
SELECT c.cat_id, a.at_id, 'seed-demo', 'A'
FROM categoria c
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000002'
WHERE c.cat_guid = '71000000-0000-0000-0000-000000000002'
ON CONFLICT (cat_id, at_id) DO NOTHING;

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

INSERT INTO idioma_atraccion (id_id, at_id, ia_usuario_ingreso, ia_estado)
SELECT i.id_id, a.at_id, 'seed-demo', 'A'
FROM idioma i
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000001'
WHERE i.id_codigo = 'es'
ON CONFLICT (id_id, at_id) DO NOTHING;

INSERT INTO idioma_atraccion (id_id, at_id, ia_usuario_ingreso, ia_estado)
SELECT i.id_id, a.at_id, 'seed-demo', 'A'
FROM idioma i
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000002'
WHERE i.id_codigo = 'en'
ON CONFLICT (id_id, at_id) DO NOTHING;

INSERT INTO imagen_atraccion (img_id, at_id, ima_es_principal, ima_orden, ima_usuario_ingreso, ima_estado)
SELECT img.img_id, a.at_id, TRUE, 1, 'seed-demo', 'A'
FROM imagen img
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000001'
WHERE img.img_guid = '73000000-0000-0000-0000-000000000001'
ON CONFLICT (img_id, at_id) DO NOTHING;

INSERT INTO imagen_atraccion (img_id, at_id, ima_es_principal, ima_orden, ima_usuario_ingreso, ima_estado)
SELECT img.img_id, a.at_id, TRUE, 1, 'seed-demo', 'A'
FROM imagen img
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000002'
WHERE img.img_guid = '73000000-0000-0000-0000-000000000002'
ON CONFLICT (img_id, at_id) DO NOTHING;

INSERT INTO atraccion_incluye (inc_id, at_id, ai_usuario_ingreso, ai_estado)
SELECT inc.inc_id, a.at_id, 'seed-demo', 'A'
FROM incluye inc
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000001'
WHERE inc.inc_guid = '74000000-0000-0000-0000-000000000001'
ON CONFLICT (inc_id, at_id) DO NOTHING;

INSERT INTO atraccion_incluye (inc_id, at_id, ai_usuario_ingreso, ai_estado)
SELECT inc.inc_id, a.at_id, 'seed-demo', 'A'
FROM incluye inc
JOIN atraccion a ON a.at_guid = '40000000-0000-0000-0000-000000000002'
WHERE inc.inc_guid = '74000000-0000-0000-0000-000000000002'
ON CONFLICT (inc_id, at_id) DO NOTHING;

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

INSERT INTO horario (hor_guid, at_id, hor_fecha, hor_hora_inicio, hor_hora_fin, hor_cupos_disponibles, hor_usuario_ingreso, hor_ip_ingreso, hor_estado)
SELECT '50000000-0000-0000-0000-000000000001', a.at_id, DATE '2030-01-01', TIME '09:00', TIME '12:00', 20, 'seed-demo', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '40000000-0000-0000-0000-000000000001'
ON CONFLICT (hor_guid) DO NOTHING;

INSERT INTO horario (hor_guid, at_id, hor_fecha, hor_hora_inicio, hor_hora_fin, hor_cupos_disponibles, hor_usuario_ingreso, hor_ip_ingreso, hor_estado)
SELECT '50000000-0000-0000-0000-000000000002', a.at_id, DATE '2030-01-02', TIME '10:00', TIME '14:00', 15, 'seed-demo', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '40000000-0000-0000-0000-000000000002'
ON CONFLICT (hor_guid) DO NOTHING;

INSERT INTO ticket (tck_guid, at_id, tck_titulo, tck_precio, tck_moneda, tck_tipo_participante, tck_capacidad_maxima, tck_usuario_ingreso, tck_ip_ingreso, tck_estado)
SELECT '60000000-0000-0000-0000-000000000001', a.at_id, 'Entrada adulto - Centro historico', 25.00, 'USD', 'Adulto', 20, 'seed-demo', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '40000000-0000-0000-0000-000000000001'
ON CONFLICT (tck_guid) DO NOTHING;

INSERT INTO ticket (tck_guid, at_id, tck_titulo, tck_precio, tck_moneda, tck_tipo_participante, tck_capacidad_maxima, tck_usuario_ingreso, tck_ip_ingreso, tck_estado)
SELECT '60000000-0000-0000-0000-000000000002', a.at_id, 'Entrada adulto - Aventura natural', 35.00, 'USD', 'Adulto', 15, 'seed-demo', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '40000000-0000-0000-0000-000000000002'
ON CONFLICT (tck_guid) DO NOTHING;

INSERT INTO resenia (rsn_guid, at_id, rev_guid, rsn_comentario, rsn_rating, rsn_usuario_creacion, rsn_ip_creacion, rsn_estado)
SELECT '75000000-0000-0000-0000-000000000001', a.at_id, '30000000-0000-0000-0000-000000000001', 'Excelente recorrido y guia muy claro.', 5, 'cliente1@travelofyourdreams.local', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '40000000-0000-0000-0000-000000000001'
ON CONFLICT (rsn_guid) DO NOTHING;

INSERT INTO resenia (rsn_guid, at_id, rev_guid, rsn_comentario, rsn_rating, rsn_usuario_creacion, rsn_ip_creacion, rsn_estado)
SELECT '75000000-0000-0000-0000-000000000002', a.at_id, '30000000-0000-0000-0000-000000000002', 'Buena experiencia para probar el microservicio.', 4, 'cliente2@travelofyourdreams.local', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '40000000-0000-0000-0000-000000000002'
ON CONFLICT (rsn_guid) DO NOTHING;

COMMIT;
