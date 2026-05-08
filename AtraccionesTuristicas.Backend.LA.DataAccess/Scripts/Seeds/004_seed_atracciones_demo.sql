INSERT INTO imagen (
    img_guid,
    img_url,
    img_descripcion,
    img_fecha_ingreso,
    img_usuario_ingreso,
    img_ip_ingreso,
    img_estado
)
VALUES
    ('34000000-0000-0000-0000-000000000001', 'https://images.unsplash.com/photo-1588421357574-87938a86fa28', 'Centro histórico de Quito', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('34000000-0000-0000-0000-000000000002', 'https://images.unsplash.com/photo-1578894381163-e72c17f2d45f', 'Malecón y ciudad', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A')
ON CONFLICT (img_guid) DO UPDATE
SET img_url = EXCLUDED.img_url,
    img_descripcion = EXCLUDED.img_descripcion,
    img_estado = 'A';

INSERT INTO atraccion (
    at_guid,
    des_id,
    at_nombre,
    at_descripcion,
    at_total_resenias,
    at_direccion,
    at_duracion_minutos,
    at_punto_encuentro,
    at_precio_referencia,
    at_incluye_acompaniante,
    at_incluye_transporte,
    at_disponible,
    at_free_cancellation,
    at_skip_the_line,
    at_fecha_ingreso,
    at_usuario_ingreso,
    at_ip_ingreso,
    at_estado
)
SELECT
    '35000000-0000-0000-0000-000000000001',
    d.des_id,
    'Tour Centro Histórico de Quito',
    'Recorrido guiado por plazas, iglesias y miradores del centro histórico.',
    0,
    'Centro Histórico',
    180,
    'Plaza Grande',
    35.00,
    TRUE,
    FALSE,
    TRUE,
    TRUE,
    FALSE,
    CURRENT_TIMESTAMP,
    'seed',
    '127.0.0.1',
    'A'
FROM destino d
WHERE d.des_guid = '30000000-0000-0000-0000-000000000001'
ON CONFLICT (at_guid) DO UPDATE
SET at_nombre = EXCLUDED.at_nombre,
    at_descripcion = EXCLUDED.at_descripcion,
    at_precio_referencia = EXCLUDED.at_precio_referencia,
    at_estado = 'A';

INSERT INTO atraccion (
    at_guid,
    des_id,
    at_nombre,
    at_descripcion,
    at_total_resenias,
    at_direccion,
    at_duracion_minutos,
    at_punto_encuentro,
    at_precio_referencia,
    at_incluye_acompaniante,
    at_incluye_transporte,
    at_disponible,
    at_free_cancellation,
    at_skip_the_line,
    at_fecha_ingreso,
    at_usuario_ingreso,
    at_ip_ingreso,
    at_estado
)
SELECT
    '35000000-0000-0000-0000-000000000002',
    d.des_id,
    'Paseo panorámico por Guayaquil',
    'Experiencia urbana por el malecón, Las Peñas y puntos icónicos.',
    0,
    'Malecón 2000',
    120,
    'Hemiciclo de la Rotonda',
    28.00,
    TRUE,
    FALSE,
    TRUE,
    TRUE,
    TRUE,
    CURRENT_TIMESTAMP,
    'seed',
    '127.0.0.1',
    'A'
FROM destino d
WHERE d.des_guid = '30000000-0000-0000-0000-000000000002'
ON CONFLICT (at_guid) DO UPDATE
SET at_nombre = EXCLUDED.at_nombre,
    at_descripcion = EXCLUDED.at_descripcion,
    at_precio_referencia = EXCLUDED.at_precio_referencia,
    at_estado = 'A';

INSERT INTO imagen_atraccion (
    img_id,
    at_id,
    ima_es_principal,
    ima_orden,
    ima_fecha_ingreso,
    ima_usuario_ingreso,
    ima_estado
)
SELECT i.img_id, a.at_id, TRUE, 1, CURRENT_TIMESTAMP, 'seed', 'A'
FROM imagen i
JOIN atraccion a ON a.at_guid = '35000000-0000-0000-0000-000000000001'
WHERE i.img_guid = '34000000-0000-0000-0000-000000000001'
ON CONFLICT (img_id, at_id) DO UPDATE
SET ima_es_principal = TRUE,
    ima_orden = 1,
    ima_estado = 'A';

INSERT INTO imagen_atraccion (
    img_id,
    at_id,
    ima_es_principal,
    ima_orden,
    ima_fecha_ingreso,
    ima_usuario_ingreso,
    ima_estado
)
SELECT i.img_id, a.at_id, TRUE, 1, CURRENT_TIMESTAMP, 'seed', 'A'
FROM imagen i
JOIN atraccion a ON a.at_guid = '35000000-0000-0000-0000-000000000002'
WHERE i.img_guid = '34000000-0000-0000-0000-000000000002'
ON CONFLICT (img_id, at_id) DO UPDATE
SET ima_es_principal = TRUE,
    ima_orden = 1,
    ima_estado = 'A';

INSERT INTO categoria_atraccion (cat_id, at_id, ca_fecha_ingreso, ca_usuario_ingreso, ca_estado)
SELECT c.cat_id, a.at_id, CURRENT_TIMESTAMP, 'seed', 'A'
FROM categoria c
JOIN atraccion a ON a.at_guid = '35000000-0000-0000-0000-000000000001'
WHERE c.cat_tagname = 'culture'
ON CONFLICT (cat_id, at_id) DO UPDATE SET ca_estado = 'A';

INSERT INTO categoria_atraccion (cat_id, at_id, ca_fecha_ingreso, ca_usuario_ingreso, ca_estado)
SELECT c.cat_id, a.at_id, CURRENT_TIMESTAMP, 'seed', 'A'
FROM categoria c
JOIN atraccion a ON a.at_guid = '35000000-0000-0000-0000-000000000002'
WHERE c.cat_tagname = 'culture'
ON CONFLICT (cat_id, at_id) DO UPDATE SET ca_estado = 'A';

INSERT INTO ticket (
    tck_guid,
    at_id,
    tck_titulo,
    tck_precio,
    tck_moneda,
    tck_tipo_participante,
    tck_capacidad_maxima,
    tck_fecha_ingreso,
    tck_usuario_ingreso,
    tck_ip_ingreso,
    tck_estado
)
SELECT '36000000-0000-0000-0000-000000000001', a.at_id, 'Adulto', 35.00, 'USD', 'Adulto', 20, CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '35000000-0000-0000-0000-000000000001'
ON CONFLICT (tck_guid) DO UPDATE
SET tck_precio = EXCLUDED.tck_precio,
    tck_estado = 'A';

INSERT INTO ticket (
    tck_guid,
    at_id,
    tck_titulo,
    tck_precio,
    tck_moneda,
    tck_tipo_participante,
    tck_capacidad_maxima,
    tck_fecha_ingreso,
    tck_usuario_ingreso,
    tck_ip_ingreso,
    tck_estado
)
SELECT '36000000-0000-0000-0000-000000000002', a.at_id, 'Adulto', 28.00, 'USD', 'Adulto', 25, CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '35000000-0000-0000-0000-000000000002'
ON CONFLICT (tck_guid) DO UPDATE
SET tck_precio = EXCLUDED.tck_precio,
    tck_estado = 'A';

INSERT INTO horario (
    hor_guid,
    at_id,
    hor_fecha,
    hor_hora_inicio,
    hor_hora_fin,
    hor_cupos_disponibles,
    hor_fecha_ingreso,
    hor_usuario_ingreso,
    hor_ip_ingreso,
    hor_estado
)
SELECT '37000000-0000-0000-0000-000000000001', a.at_id, CURRENT_DATE + 7, TIME '09:00', TIME '12:00', 20, CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '35000000-0000-0000-0000-000000000001'
ON CONFLICT (hor_guid) DO UPDATE
SET hor_fecha = EXCLUDED.hor_fecha,
    hor_cupos_disponibles = EXCLUDED.hor_cupos_disponibles,
    hor_estado = 'A';

INSERT INTO horario (
    hor_guid,
    at_id,
    hor_fecha,
    hor_hora_inicio,
    hor_hora_fin,
    hor_cupos_disponibles,
    hor_fecha_ingreso,
    hor_usuario_ingreso,
    hor_ip_ingreso,
    hor_estado
)
SELECT '37000000-0000-0000-0000-000000000002', a.at_id, CURRENT_DATE + 8, TIME '10:00', TIME '12:00', 25, CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'
FROM atraccion a
WHERE a.at_guid = '35000000-0000-0000-0000-000000000002'
ON CONFLICT (hor_guid) DO UPDATE
SET hor_fecha = EXCLUDED.hor_fecha,
    hor_cupos_disponibles = EXCLUDED.hor_cupos_disponibles,
    hor_estado = 'A';
