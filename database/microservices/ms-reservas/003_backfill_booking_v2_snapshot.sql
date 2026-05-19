START TRANSACTION;

UPDATE reservas
SET
    rev_atraccion_nombre = 'Tour Centro Historico de Quito',
    rev_hor_fecha = DATE '2030-01-01',
    rev_hor_hora_inicio = TIME '09:00',
    rev_hor_hora_fin = TIME '12:00'
WHERE rev_guid = '30000000-0000-0000-0000-000000000001';

UPDATE reservas
SET
    rev_atraccion_nombre = 'Ruta de Cascadas en Banos',
    rev_hor_fecha = DATE '2030-01-02',
    rev_hor_hora_inicio = TIME '10:00',
    rev_hor_hora_fin = TIME '14:00'
WHERE rev_guid = '30000000-0000-0000-0000-000000000002';

COMMIT;
