DROP TRIGGER IF EXISTS trg_facturas_row_version ON facturas;

CREATE TRIGGER trg_facturas_row_version
BEFORE UPDATE ON facturas
FOR EACH ROW
EXECUTE FUNCTION fn_increment_row_version_generic('fac_row_version');
