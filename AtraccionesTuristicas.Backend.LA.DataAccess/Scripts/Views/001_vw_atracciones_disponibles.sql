CREATE OR REPLACE VIEW vw_atracciones_disponibles AS
SELECT
    a.at_id,
    a.at_guid,
    a.at_nombre,
    a.at_descripcion,
    a.at_precio_referencia,
    a.at_total_resenias,
    a.at_free_cancellation,
    a.at_skip_the_line,
    d.des_id,
    d.des_guid,
    d.des_nombre,
    d.des_pais,
    MIN(t.tck_precio) AS precio_minimo,
    COUNT(DISTINCT h.hor_id) AS horarios_disponibles,
    COALESCE(SUM(h.hor_cupos_disponibles), 0) AS cupos_disponibles,
    img.img_url AS imagen_principal_url
FROM atraccion a
JOIN destino d ON d.des_id = a.des_id
LEFT JOIN ticket t ON t.at_id = a.at_id AND t.tck_estado = 'A'
LEFT JOIN horario h ON h.at_id = a.at_id AND h.hor_estado = 'A' AND h.hor_cupos_disponibles > 0
LEFT JOIN imagen_atraccion ia ON ia.at_id = a.at_id AND ia.ima_es_principal = TRUE AND ia.ima_estado = 'A'
LEFT JOIN imagen img ON img.img_id = ia.img_id AND img.img_estado = 'A'
WHERE a.at_estado = 'A'
  AND a.at_disponible = TRUE
GROUP BY
    a.at_id,
    a.at_guid,
    a.at_nombre,
    a.at_descripcion,
    a.at_precio_referencia,
    a.at_total_resenias,
    a.at_free_cancellation,
    a.at_skip_the_line,
    d.des_id,
    d.des_guid,
    d.des_nombre,
    d.des_pais,
    img.img_url;
