CREATE OR REPLACE FUNCTION trg_auditoria_usuarios_fn()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        PERFORM fn_registrar_auditoria(
            'usuario',
            TG_OP,
            NEW.usu_id,
            NEW.usu_guid,
            NULL,
            row_to_json(NEW)::TEXT,
            NEW.usu_usuario_registro,
            NEW.usu_ip_registro,
            'API'
        );
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        PERFORM fn_registrar_auditoria(
            'usuario',
            TG_OP,
            NEW.usu_id,
            NEW.usu_guid,
            row_to_json(OLD)::TEXT,
            row_to_json(NEW)::TEXT,
            COALESCE(NEW.usu_usuario_mod, NEW.usu_usuario_eliminacion, NEW.usu_usuario_registro),
            COALESCE(NEW.usu_ip_mod, NEW.usu_ip_eliminacion, NEW.usu_ip_registro),
            'API'
        );
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        PERFORM fn_registrar_auditoria(
            'usuario',
            TG_OP,
            OLD.usu_id,
            OLD.usu_guid,
            row_to_json(OLD)::TEXT,
            NULL,
            COALESCE(OLD.usu_usuario_mod, OLD.usu_usuario_eliminacion, OLD.usu_usuario_registro),
            COALESCE(OLD.usu_ip_mod, OLD.usu_ip_eliminacion, OLD.usu_ip_registro),
            'API'
        );
        RETURN OLD;
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_auditoria_usuarios ON usuario;

CREATE TRIGGER trg_auditoria_usuarios
AFTER INSERT OR UPDATE OR DELETE ON usuario
FOR EACH ROW
EXECUTE FUNCTION trg_auditoria_usuarios_fn();
