CREATE OR REPLACE FUNCTION trg_auditoria_pagos_fn()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        PERFORM fn_registrar_auditoria(
            'PAGOS',
            'INSERT',
            NEW.pag_id,
            NEW.pag_guid,
            NULL,
            to_jsonb(NEW)::TEXT,
            NEW.pag_usuario_ingreso,
            NEW.pag_ip_ingreso,
            NEW.pag_origen_canal
        );
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        PERFORM fn_registrar_auditoria(
            'PAGOS',
            'UPDATE',
            NEW.pag_id,
            NEW.pag_guid,
            to_jsonb(OLD)::TEXT,
            to_jsonb(NEW)::TEXT,
            COALESCE(NEW.pag_usuario_mod, NEW.pag_usuario_ingreso),
            COALESCE(NEW.pag_ip_mod, NEW.pag_ip_ingreso),
            NEW.pag_origen_canal
        );
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        PERFORM fn_registrar_auditoria(
            'PAGOS',
            'DELETE',
            OLD.pag_id,
            OLD.pag_guid,
            to_jsonb(OLD)::TEXT,
            NULL,
            COALESCE(OLD.pag_usuario_mod, OLD.pag_usuario_ingreso),
            COALESCE(OLD.pag_ip_mod, OLD.pag_ip_ingreso),
            OLD.pag_origen_canal
        );
        RETURN OLD;
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_auditoria_pagos ON pagos;

CREATE TRIGGER trg_auditoria_pagos
AFTER INSERT OR UPDATE OR DELETE ON pagos
FOR EACH ROW
EXECUTE FUNCTION trg_auditoria_pagos_fn();
