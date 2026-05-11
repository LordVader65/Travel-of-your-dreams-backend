CREATE OR REPLACE FUNCTION trg_auditoria_usuarioxroles_fn()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        PERFORM fn_registrar_auditoria(
            'usuarioxroles',
            TG_OP,
            NEW.usu_rol_id,
            NULL,
            NULL,
            row_to_json(NEW)::TEXT,
            'system',
            '127.0.0.1',
            'API'
        );
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        PERFORM fn_registrar_auditoria(
            'usuarioxroles',
            TG_OP,
            NEW.usu_rol_id,
            NULL,
            row_to_json(OLD)::TEXT,
            row_to_json(NEW)::TEXT,
            'system',
            '127.0.0.1',
            'API'
        );
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        PERFORM fn_registrar_auditoria(
            'usuarioxroles',
            TG_OP,
            OLD.usu_rol_id,
            NULL,
            row_to_json(OLD)::TEXT,
            NULL,
            'system',
            '127.0.0.1',
            'API'
        );
        RETURN OLD;
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_auditoria_usuarioxroles ON usuarioxroles;

CREATE TRIGGER trg_auditoria_usuarioxroles
AFTER INSERT OR UPDATE OR DELETE ON usuarioxroles
FOR EACH ROW
EXECUTE FUNCTION trg_auditoria_usuarioxroles_fn();
