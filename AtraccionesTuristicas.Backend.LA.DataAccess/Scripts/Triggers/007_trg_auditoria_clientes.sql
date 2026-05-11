CREATE OR REPLACE FUNCTION trg_auditoria_clientes_fn()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        PERFORM fn_registrar_auditoria(
            'clientes',
            TG_OP,
            NEW.cli_id,
            NEW.cli_guid,
            NULL,
            row_to_json(NEW)::TEXT,
            NEW.cli_usuario_ingreso,
            NEW.cli_ip_ingreso,
            'API'
        );
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        PERFORM fn_registrar_auditoria(
            'clientes',
            TG_OP,
            NEW.cli_id,
            NEW.cli_guid,
            row_to_json(OLD)::TEXT,
            row_to_json(NEW)::TEXT,
            COALESCE(NEW.cli_usuario_eliminacion, NEW.cli_usuario_ingreso),
            COALESCE(NEW.cli_ip_eliminacion, NEW.cli_ip_ingreso),
            'API'
        );
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        PERFORM fn_registrar_auditoria(
            'clientes',
            TG_OP,
            OLD.cli_id,
            OLD.cli_guid,
            row_to_json(OLD)::TEXT,
            NULL,
            COALESCE(OLD.cli_usuario_eliminacion, OLD.cli_usuario_ingreso),
            COALESCE(OLD.cli_ip_eliminacion, OLD.cli_ip_ingreso),
            'API'
        );
        RETURN OLD;
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_auditoria_clientes ON clientes;

CREATE TRIGGER trg_auditoria_clientes
AFTER INSERT OR UPDATE OR DELETE ON clientes
FOR EACH ROW
EXECUTE FUNCTION trg_auditoria_clientes_fn();
