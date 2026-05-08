DROP TRIGGER IF EXISTS trg_clientes_row_version ON clientes;

CREATE TRIGGER trg_clientes_row_version
BEFORE UPDATE ON clientes
FOR EACH ROW
EXECUTE FUNCTION fn_increment_row_version_generic('cli_row_version');
