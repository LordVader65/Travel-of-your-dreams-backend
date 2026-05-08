CREATE OR REPLACE FUNCTION fn_increment_row_version_generic()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_ARGV[0] = 'cli_row_version' THEN
        NEW.cli_row_version := COALESCE(OLD.cli_row_version, 0) + 1;
    ELSIF TG_ARGV[0] = 'fac_row_version' THEN
        NEW.fac_row_version := COALESCE(OLD.fac_row_version, 0) + 1;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
