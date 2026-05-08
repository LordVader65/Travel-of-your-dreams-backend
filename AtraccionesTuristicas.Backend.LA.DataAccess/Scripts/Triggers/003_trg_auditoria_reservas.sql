CREATE OR REPLACE FUNCTION trg_auditoria_reservas_fn()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        PERFORM fn_registrar_auditoria(
            'RESERVAS',
            'INSERT',
            NEW.rev_id,
            NEW.rev_guid,
            NULL,
            to_jsonb(NEW)::TEXT,
            NEW.rev_usuario_ingreso,
            NEW.rev_ip_ingreso,
            NEW.rev_origen_canal
        );
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
        PERFORM fn_registrar_auditoria(
            'RESERVAS',
            'UPDATE',
            NEW.rev_id,
            NEW.rev_guid,
            to_jsonb(OLD)::TEXT,
            to_jsonb(NEW)::TEXT,
            COALESCE(NEW.rev_usuario_mod, NEW.rev_usuario_cancelacion, NEW.rev_usuario_ingreso),
            COALESCE(NEW.rev_ip_mod, NEW.rev_ip_cancelacion, NEW.rev_ip_ingreso),
            NEW.rev_origen_canal
        );
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        PERFORM fn_registrar_auditoria(
            'RESERVAS',
            'DELETE',
            OLD.rev_id,
            OLD.rev_guid,
            to_jsonb(OLD)::TEXT,
            NULL,
            COALESCE(OLD.rev_usuario_mod, OLD.rev_usuario_cancelacion, OLD.rev_usuario_ingreso),
            COALESCE(OLD.rev_ip_mod, OLD.rev_ip_cancelacion, OLD.rev_ip_ingreso),
            OLD.rev_origen_canal
        );
        RETURN OLD;
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_auditoria_reservas ON reservas;

CREATE TRIGGER trg_auditoria_reservas
AFTER INSERT OR UPDATE OR DELETE ON reservas
FOR EACH ROW
EXECUTE FUNCTION trg_auditoria_reservas_fn();
