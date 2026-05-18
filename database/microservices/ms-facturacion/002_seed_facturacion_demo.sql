START TRANSACTION;

INSERT INTO datos_facturacion (
    dfac_guid, cli_guid, dfac_tipo_identificacion, dfac_numero_identificacion,
    dfac_nombre, dfac_apellido, dfac_correo, dfac_telefono, dfac_direccion,
    dfac_usuario_ingreso, dfac_ip_ingreso, dfac_estado
)
VALUES
    ('82000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000002', 'CC', '0911111111', 'Camila', 'Torres', 'cliente1@travelofyourdreams.local', '0911111111', 'Guayaquil', 'seed-demo', '127.0.0.1', 'A'),
    ('82000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000003', 'CC', '0922222222', 'Mateo', 'Andrade', 'cliente2@travelofyourdreams.local', '0922222222', 'Cuenca', 'seed-demo', '127.0.0.1', 'A')
ON CONFLICT (dfac_guid) DO NOTHING;

INSERT INTO pagos (
    pag_guid, rev_guid, cli_guid, dfac_id, pag_monto, pag_moneda,
    pag_metodo, pag_referencia, pag_origen_canal, pag_estado,
    pag_usuario_ingreso, pag_ip_ingreso, pag_observacion
)
SELECT
    '80000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001',
    '20000000-0000-0000-0000-000000000002', d.dfac_id, 28.00, 'USD',
    'TARJETA', 'PAY-SEED-001', 'WEB', 'APROBADO',
    'seed-demo', '127.0.0.1', 'Pago demo cliente 1'
FROM datos_facturacion d
WHERE d.dfac_guid = '82000000-0000-0000-0000-000000000001'
ON CONFLICT (pag_guid) DO NOTHING;

INSERT INTO pagos (
    pag_guid, rev_guid, cli_guid, dfac_id, pag_monto, pag_moneda,
    pag_metodo, pag_referencia, pag_origen_canal, pag_estado,
    pag_usuario_ingreso, pag_ip_ingreso, pag_observacion
)
SELECT
    '80000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000002',
    '20000000-0000-0000-0000-000000000003', d.dfac_id, 39.20, 'USD',
    'TRANSFERENCIA', 'PAY-SEED-002', 'BOOKING', 'APROBADO',
    'seed-demo', '127.0.0.1', 'Pago demo cliente 2'
FROM datos_facturacion d
WHERE d.dfac_guid = '82000000-0000-0000-0000-000000000002'
ON CONFLICT (pag_guid) DO NOTHING;

INSERT INTO facturas (
    fac_guid, fac_numero, rev_guid, cli_guid, pag_id, dfac_id,
    fac_subtotal, fac_valor_iva, fac_total, fac_moneda,
    fac_observacion, fac_usuario_ingreso, fac_ip_ingreso, fac_estado
)
SELECT
    '81000000-0000-0000-0000-000000000001', 'FAC-SEED-001',
    '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000002',
    p.pag_id, d.dfac_id, 25.00, 3.00, 28.00, 'USD',
    'Factura demo cliente 1', 'seed-demo', '127.0.0.1', 'A'
FROM pagos p
JOIN datos_facturacion d ON d.dfac_id = p.dfac_id
WHERE p.pag_guid = '80000000-0000-0000-0000-000000000001'
ON CONFLICT (fac_numero) DO NOTHING;

INSERT INTO facturas (
    fac_guid, fac_numero, rev_guid, cli_guid, pag_id, dfac_id,
    fac_subtotal, fac_valor_iva, fac_total, fac_moneda,
    fac_observacion, fac_usuario_ingreso, fac_ip_ingreso, fac_estado
)
SELECT
    '81000000-0000-0000-0000-000000000002', 'FAC-SEED-002',
    '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000003',
    p.pag_id, d.dfac_id, 35.00, 4.20, 39.20, 'USD',
    'Factura demo cliente 2', 'seed-demo', '127.0.0.1', 'A'
FROM pagos p
JOIN datos_facturacion d ON d.dfac_id = p.dfac_id
WHERE p.pag_guid = '80000000-0000-0000-0000-000000000002'
ON CONFLICT (fac_numero) DO NOTHING;

COMMIT;
