INSERT INTO destino (
    des_guid,
    des_nombre,
    des_pais,
    des_imagen_url,
    des_fecha_ingreso,
    des_usuario_ingreso,
    des_ip_ingreso,
    des_estado
)
VALUES
    ('30000000-0000-0000-0000-000000000001', 'Quito', 'Ecuador', NULL, CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('30000000-0000-0000-0000-000000000002', 'Guayaquil', 'Ecuador', NULL, CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('30000000-0000-0000-0000-000000000003', 'Cuenca', 'Ecuador', NULL, CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A')
ON CONFLICT (des_guid) DO UPDATE
SET des_nombre = EXCLUDED.des_nombre,
    des_pais = EXCLUDED.des_pais,
    des_estado = 'A';

INSERT INTO categoria (
    cat_guid,
    cat_parent_id,
    cat_nombre,
    cat_tagname,
    cat_fecha_ingreso,
    cat_usuario_ingreso,
    cat_ip_ingreso,
    cat_estado
)
VALUES
    ('31000000-0000-0000-0000-000000000001', NULL, 'Cultura', 'culture', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('31000000-0000-0000-0000-000000000002', NULL, 'Aventura', 'adventure', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('31000000-0000-0000-0000-000000000003', NULL, 'Naturaleza', 'nature', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('31000000-0000-0000-0000-000000000004', NULL, 'Gastronomía', 'food', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A')
ON CONFLICT (cat_guid) DO UPDATE
SET cat_nombre = EXCLUDED.cat_nombre,
    cat_tagname = EXCLUDED.cat_tagname,
    cat_estado = 'A';

INSERT INTO idioma (
    id_guid,
    id_codigo,
    id_descripcion,
    id_fecha_ingreso,
    id_usuario_ingreso,
    id_ip_ingreso,
    id_estado
)
VALUES
    ('32000000-0000-0000-0000-000000000001', 'es', 'Español', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('32000000-0000-0000-0000-000000000002', 'en', 'Inglés', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('32000000-0000-0000-0000-000000000003', 'fr', 'Francés', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('32000000-0000-0000-0000-000000000004', 'it', 'Italiano', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('32000000-0000-0000-0000-000000000005', 'de', 'Alemán', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A'),
    ('32000000-0000-0000-0000-000000000006', 'pt', 'Portugués', CURRENT_TIMESTAMP, 'seed', '127.0.0.1', 'A')
ON CONFLICT (id_guid) DO UPDATE
SET id_codigo = EXCLUDED.id_codigo,
    id_descripcion = EXCLUDED.id_descripcion,
    id_estado = 'A';

INSERT INTO incluye (
    inc_guid,
    inc_descripcion,
    inc_tipo,
    inc_estado
)
VALUES
    ('33000000-0000-0000-0000-000000000001', 'Guía local', 'INCLUYE', 'A'),
    ('33000000-0000-0000-0000-000000000002', 'Transporte', 'INCLUYE', 'A'),
    ('33000000-0000-0000-0000-000000000003', 'Comidas y bebidas', 'NO_INCLUYE', 'A'),
    ('33000000-0000-0000-0000-000000000004', 'Cancelación gratis', 'ETIQUETA', 'A'),
    ('33000000-0000-0000-0000-000000000005', 'Sin filas', 'ETIQUETA', 'A')
ON CONFLICT (inc_guid) DO UPDATE
SET inc_descripcion = EXCLUDED.inc_descripcion,
    inc_tipo = EXCLUDED.inc_tipo,
    inc_estado = 'A';
