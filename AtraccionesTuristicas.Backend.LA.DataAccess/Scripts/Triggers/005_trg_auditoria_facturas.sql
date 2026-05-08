CREATE OR REPLACE FUNCTION trg_auditoria_facturas_fn()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        PERFORM fn_registrar_auditoria(
            'FACTURAS',
            'INSERT',
            NEW.fac_id,
            NEW.fac_guid,
            NULL,
            to_jsonb(NEW)::TEXT,
            NEW.fac_usuario_ingreso,
            NEW.fac_ip_ingreso,
            NEW.fac_origen_canal
        );
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        PERFORM fn_registrar_auditoria(
            'FACTURAS',
            'UPDATE',
            NEW.fac_id,
            NEW.fac_guid,
            to_jsonb(OLD)::TEXT,
            to_jsonb(NEW)::TEXT,
            COALESCE(NEW.fac_usuario_mod, NEW.fac_usuario_eliminacion, NEW.fac_usuario_ingreso),
            COALESCE(NEW.fac_ip_mod, NEW.fac_ip_eliminacion, NEW.fac_ip_ingreso),
            NEW.fac_origen_canal
        );
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        PERFORM fn_registrar_auditoria(
            'FACTURAS',
            'DELETE',
            OLD.fac_id,
            OLD.fac_guid,
            to_jsonb(OLD)::TEXT,
            NULL,
            COALESCE(OLD.fac_usuario_mod, OLD.fac_usuario_eliminacion, OLD.fac_usuario_ingreso),
            COALESCE(OLD.fac_ip_mod, OLD.fac_ip_eliminacion, OLD.fac_ip_ingreso),
            OLD.fac_origen_canal
        );
        RETURN OLD;
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_auditoria_facturas ON facturas;

CREATE TRIGGER trg_auditoria_facturas
AFTER INSERT OR UPDATE OR DELETE ON facturas
FOR EACH ROW
EXECUTE FUNCTION trg_auditoria_facturas_fn();
