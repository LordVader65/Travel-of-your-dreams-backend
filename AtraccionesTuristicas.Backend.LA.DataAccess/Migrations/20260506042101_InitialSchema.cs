using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");

            migrationBuilder.CreateTable(
                name: "auditoria_log",
                columns: table => new
                {
                    log_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    log_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    log_tabla = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    log_operacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    log_registro_id = table.Column<int>(type: "integer", nullable: true),
                    log_registro_guid = table.Column<Guid>(type: "uuid", nullable: true),
                    log_datos_anteriores = table.Column<string>(type: "text", nullable: true),
                    log_datos_nuevos = table.Column<string>(type: "text", nullable: true),
                    log_fecha_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    log_usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    log_ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    log_origen_canal = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_log", x => x.log_id);
                });

            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    cat_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cat_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cat_parent_id = table.Column<int>(type: "integer", nullable: true),
                    cat_nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cat_tagname = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    cat_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    cat_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cat_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    cat_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cat_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cat_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    cat_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cat_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cat_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    cat_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.cat_id);
                    table.CheckConstraint("ck_categoria_estado", "cat_estado IN ('A','I')");
                    table.CheckConstraint("ck_categoria_selfref", "cat_parent_id IS NULL OR cat_parent_id <> cat_id");
                    table.ForeignKey(
                        name: "FK_categoria_categoria_cat_parent_id",
                        column: x => x.cat_parent_id,
                        principalTable: "categoria",
                        principalColumn: "cat_id");
                });

            migrationBuilder.CreateTable(
                name: "destino",
                columns: table => new
                {
                    des_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    des_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    des_nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    des_pais = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    des_imagen_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    des_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    des_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    des_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    des_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    des_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    des_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    des_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    des_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    des_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    des_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_destino", x => x.des_id);
                    table.CheckConstraint("ck_destino_estado", "des_estado IN ('A','I')");
                });

            migrationBuilder.CreateTable(
                name: "idioma",
                columns: table => new
                {
                    id_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    id_descripcion = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    id_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    id_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    id_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    id_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    id_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    id_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    id_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idioma", x => x.id_id);
                    table.CheckConstraint("ck_idioma_estado", "id_estado IN ('A','I')");
                });

            migrationBuilder.CreateTable(
                name: "imagen",
                columns: table => new
                {
                    img_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    img_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    img_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    img_descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    img_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    img_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    img_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    img_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    img_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    img_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    img_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    img_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    img_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    img_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagen", x => x.img_id);
                    table.CheckConstraint("ck_imagen_estado", "img_estado IN ('A','I')");
                });

            migrationBuilder.CreateTable(
                name: "incluye",
                columns: table => new
                {
                    inc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    inc_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    inc_descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    inc_tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "INCLUYE"),
                    inc_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incluye", x => x.inc_id);
                    table.CheckConstraint("ck_incluye_estado", "inc_estado IN ('A','I')");
                    table.CheckConstraint("ck_incluye_tipo", "inc_tipo IN ('INCLUYE','NO_INCLUYE','ETIQUETA')");
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    rol_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rol_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rol_descripcion = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    rol_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rol_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rol_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    rol_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rol_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rol_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    rol_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.rol_id);
                    table.CheckConstraint("ck_roles_estado", "rol_estado IN ('A','I')");
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    usu_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usu_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    usu_login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usu_password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    usu_fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usu_usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usu_ip_registro = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    usu_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usu_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    usu_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    usu_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usu_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    usu_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    usu_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.usu_id);
                    table.CheckConstraint("ck_usuario_estado", "usu_estado IN ('A','I')");
                });

            migrationBuilder.CreateTable(
                name: "atraccion",
                columns: table => new
                {
                    at_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    at_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    des_id = table.Column<int>(type: "integer", nullable: false),
                    at_num_establecimiento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    at_nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    at_descripcion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    at_total_resenias = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    at_direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    at_duracion_minutos = table.Column<int>(type: "integer", nullable: true),
                    at_punto_encuentro = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    at_precio_referencia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    at_incluye_acompaniante = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    at_incluye_transporte = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    at_disponible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    at_free_cancellation = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    at_skip_the_line = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    at_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    at_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    at_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    at_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    at_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    at_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    at_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    at_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    at_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    at_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atraccion", x => x.at_id);
                    table.CheckConstraint("ck_atraccion_duracion", "at_duracion_minutos IS NULL OR at_duracion_minutos > 0");
                    table.CheckConstraint("ck_atraccion_estado", "at_estado IN ('A','I')");
                    table.CheckConstraint("ck_atraccion_precio", "at_precio_referencia IS NULL OR at_precio_referencia >= 0");
                    table.CheckConstraint("ck_atraccion_resenias", "at_total_resenias >= 0");
                    table.ForeignKey(
                        name: "FK_atraccion_destino_des_id",
                        column: x => x.des_id,
                        principalTable: "destino",
                        principalColumn: "des_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    cli_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cli_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    usu_id = table.Column<int>(type: "integer", nullable: false),
                    cli_tipo_identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cli_numero_identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cli_nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cli_apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cli_razon_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cli_correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cli_telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cli_direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    cli_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    cli_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cli_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    cli_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cli_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cli_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    cli_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A"),
                    cli_row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.cli_id);
                    table.CheckConstraint("ck_clientes_estado", "cli_estado IN ('A','I')");
                    table.CheckConstraint("ck_clientes_tipo_id", "cli_tipo_identificacion IN ('CC','RUC','PASAPORTE','CEDULA','OTRO')");
                    table.ForeignKey(
                        name: "FK_clientes_usuario_usu_id",
                        column: x => x.usu_id,
                        principalTable: "usuario",
                        principalColumn: "usu_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarioxroles",
                columns: table => new
                {
                    usu_rol_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usu_id = table.Column<int>(type: "integer", nullable: false),
                    rol_id = table.Column<int>(type: "integer", nullable: false),
                    usu_rol_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioxroles", x => x.usu_rol_id);
                    table.CheckConstraint("ck_usuarioxroles_estado", "usu_rol_estado IN ('A','I')");
                    table.ForeignKey(
                        name: "FK_usuarioxroles_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "rol_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarioxroles_usuario_usu_id",
                        column: x => x.usu_id,
                        principalTable: "usuario",
                        principalColumn: "usu_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "atraccion_incluye",
                columns: table => new
                {
                    inc_id = table.Column<int>(type: "integer", nullable: false),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    ai_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ai_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ai_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ai_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ai_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atraccion_incluye", x => new { x.inc_id, x.at_id });
                    table.ForeignKey(
                        name: "FK_atraccion_incluye_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_atraccion_incluye_incluye_inc_id",
                        column: x => x.inc_id,
                        principalTable: "incluye",
                        principalColumn: "inc_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "categoria_atraccion",
                columns: table => new
                {
                    cat_id = table.Column<int>(type: "integer", nullable: false),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    ca_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ca_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ca_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ca_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ca_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria_atraccion", x => new { x.cat_id, x.at_id });
                    table.ForeignKey(
                        name: "FK_categoria_atraccion_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_categoria_atraccion_categoria_cat_id",
                        column: x => x.cat_id,
                        principalTable: "categoria",
                        principalColumn: "cat_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "horario",
                columns: table => new
                {
                    hor_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hor_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    hor_fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hor_hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    hor_hora_fin = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    hor_cupos_disponibles = table.Column<int>(type: "integer", nullable: false),
                    hor_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    hor_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    hor_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    hor_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hor_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hor_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    hor_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hor_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hor_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    hor_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horario", x => x.hor_id);
                    table.CheckConstraint("ck_horario_cupos", "hor_cupos_disponibles >= 0");
                    table.CheckConstraint("ck_horario_estado", "hor_estado IN ('A','I')");
                    table.ForeignKey(
                        name: "FK_horario_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "idioma_atraccion",
                columns: table => new
                {
                    id_id = table.Column<int>(type: "integer", nullable: false),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    ia_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ia_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ia_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ia_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ia_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idioma_atraccion", x => new { x.id_id, x.at_id });
                    table.ForeignKey(
                        name: "FK_idioma_atraccion_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_idioma_atraccion_idioma_id_id",
                        column: x => x.id_id,
                        principalTable: "idioma",
                        principalColumn: "id_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "imagen_atraccion",
                columns: table => new
                {
                    img_id = table.Column<int>(type: "integer", nullable: false),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    ima_es_principal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ima_orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ima_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ima_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ima_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ima_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ima_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagen_atraccion", x => new { x.img_id, x.at_id });
                    table.ForeignKey(
                        name: "FK_imagen_atraccion_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_imagen_atraccion_imagen_img_id",
                        column: x => x.img_id,
                        principalTable: "imagen",
                        principalColumn: "img_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                columns: table => new
                {
                    tck_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tck_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    tck_titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    tck_precio = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    tck_moneda = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, defaultValue: "USD"),
                    tck_tipo_participante = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Adulto"),
                    tck_capacidad_maxima = table.Column<int>(type: "integer", nullable: false),
                    tck_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    tck_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tck_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    tck_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tck_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tck_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    tck_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tck_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tck_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    tck_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket", x => x.tck_id);
                    table.CheckConstraint("ck_ticket_capacidad", "tck_capacidad_maxima > 0");
                    table.CheckConstraint("ck_ticket_estado", "tck_estado IN ('A','I')");
                    table.CheckConstraint("ck_ticket_precio", "tck_precio >= 0");
                    table.ForeignKey(
                        name: "FK_ticket_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservas",
                columns: table => new
                {
                    rev_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rev_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rev_codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cli_id = table.Column<int>(type: "integer", nullable: false),
                    hor_id = table.Column<int>(type: "integer", nullable: false),
                    rev_fecha_reserva_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rev_fecha_expiracion_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    rev_subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    rev_valor_iva = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    rev_total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    rev_moneda = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, defaultValue: "USD"),
                    rev_origen_canal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    rev_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rev_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    rev_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rev_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rev_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    rev_fecha_cancelacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rev_usuario_cancelacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rev_ip_cancelacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    rev_motivo_cancelacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    rev_estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDIENTE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservas", x => x.rev_id);
                    table.CheckConstraint("ck_reservas_estado", "rev_estado IN ('PENDIENTE','PAGADA','CONFIRMADA','CANCELADA','EXPIRADA','USADA','NO_SHOW')");
                    table.CheckConstraint("ck_reservas_iva", "rev_valor_iva >= 0");
                    table.CheckConstraint("ck_reservas_subtotal", "rev_subtotal >= 0");
                    table.CheckConstraint("ck_reservas_total", "rev_total >= 0");
                    table.ForeignKey(
                        name: "FK_reservas_clientes_cli_id",
                        column: x => x.cli_id,
                        principalTable: "clientes",
                        principalColumn: "cli_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservas_horario_hor_id",
                        column: x => x.hor_id,
                        principalTable: "horario",
                        principalColumn: "hor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    pag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pag_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rev_id = table.Column<int>(type: "integer", nullable: false),
                    pag_referencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pag_metodo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    pag_monto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    pag_moneda = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, defaultValue: "USD"),
                    pag_fecha_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    pag_estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDIENTE"),
                    pag_origen_canal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    pag_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pag_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    pag_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pag_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pag_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.pag_id);
                    table.CheckConstraint("ck_pagos_estado", "pag_estado IN ('PENDIENTE','APROBADO','RECHAZADO')");
                    table.CheckConstraint("ck_pagos_monto", "pag_monto >= 0");
                    table.ForeignKey(
                        name: "FK_pagos_reservas_rev_id",
                        column: x => x.rev_id,
                        principalTable: "reservas",
                        principalColumn: "rev_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resenia",
                columns: table => new
                {
                    rsn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rsn_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    rev_id = table.Column<int>(type: "integer", nullable: false),
                    rsn_comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    rsn_rating = table.Column<short>(type: "smallint", nullable: false),
                    rsn_fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rsn_usuario_creacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rsn_ip_creacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    rsn_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rsn_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rsn_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    rsn_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rsn_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rsn_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    rsn_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resenia", x => x.rsn_id);
                    table.CheckConstraint("ck_resenia_rating", "rsn_rating BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_resenia_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resenia_reservas_rev_id",
                        column: x => x.rev_id,
                        principalTable: "reservas",
                        principalColumn: "rev_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reserva_detalle",
                columns: table => new
                {
                    rdet_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rdet_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rev_id = table.Column<int>(type: "integer", nullable: false),
                    tck_id = table.Column<int>(type: "integer", nullable: false),
                    rdet_cantidad = table.Column<int>(type: "integer", nullable: false),
                    rdet_precio_unit = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    rdet_subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    rdet_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rdet_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rdet_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    rdet_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rdet_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rdet_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    rdet_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reserva_detalle", x => x.rdet_id);
                    table.ForeignKey(
                        name: "FK_reserva_detalle_reservas_rev_id",
                        column: x => x.rev_id,
                        principalTable: "reservas",
                        principalColumn: "rev_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reserva_detalle_ticket_tck_id",
                        column: x => x.tck_id,
                        principalTable: "ticket",
                        principalColumn: "tck_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reserva_estado_historial",
                columns: table => new
                {
                    reh_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reh_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rev_id = table.Column<int>(type: "integer", nullable: false),
                    reh_estado_anterior = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    reh_estado_nuevo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reh_fecha_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    reh_usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    reh_ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    reh_observacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reserva_estado_historial", x => x.reh_id);
                    table.ForeignKey(
                        name: "FK_reserva_estado_historial_reservas_rev_id",
                        column: x => x.rev_id,
                        principalTable: "reservas",
                        principalColumn: "rev_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facturas",
                columns: table => new
                {
                    fac_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fac_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rev_id = table.Column<int>(type: "integer", nullable: false),
                    pag_id = table.Column<int>(type: "integer", nullable: true),
                    fac_numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fac_fecha_emision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fac_total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    fac_moneda = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, defaultValue: "USD"),
                    fac_observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fac_origen_canal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fac_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fac_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    fac_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fac_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fac_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    fac_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fac_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fac_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    fac_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A"),
                    fac_motivo_inhabilitacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    fac_row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facturas", x => x.fac_id);
                    table.CheckConstraint("ck_facturas_estado", "fac_estado IN ('A','I')");
                    table.CheckConstraint("ck_facturas_total", "fac_total >= 0");
                    table.ForeignKey(
                        name: "FK_facturas_pagos_pag_id",
                        column: x => x.pag_id,
                        principalTable: "pagos",
                        principalColumn: "pag_id");
                    table.ForeignKey(
                        name: "FK_facturas_reservas_rev_id",
                        column: x => x.rev_id,
                        principalTable: "reservas",
                        principalColumn: "rev_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "datos_facturacion",
                columns: table => new
                {
                    dfac_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dfac_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    fac_id = table.Column<int>(type: "integer", nullable: false),
                    dfac_nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dfac_apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dfac_correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    dfac_telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_datos_facturacion", x => x.dfac_id);
                    table.CheckConstraint("ck_dfac_correo", "dfac_correo LIKE '%@%.%'");
                    table.ForeignKey(
                        name: "FK_datos_facturacion_facturas_fac_id",
                        column: x => x.fac_id,
                        principalTable: "facturas",
                        principalColumn: "fac_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_atraccion_at_guid",
                table: "atraccion",
                column: "at_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_atraccion_at_nombre",
                table: "atraccion",
                column: "at_nombre");

            migrationBuilder.CreateIndex(
                name: "IX_atraccion_des_id",
                table: "atraccion",
                column: "des_id");

            migrationBuilder.CreateIndex(
                name: "IX_atraccion_incluye_at_id",
                table: "atraccion_incluye",
                column: "at_id");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_log_fecha_utc",
                table: "auditoria_log",
                column: "log_fecha_utc");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_log_guid",
                table: "auditoria_log",
                column: "log_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_log_tabla_log_registro_id",
                table: "auditoria_log",
                columns: new[] { "log_tabla", "log_registro_id" });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_log_usuario",
                table: "auditoria_log",
                column: "log_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_categoria_cat_guid",
                table: "categoria",
                column: "cat_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categoria_cat_parent_id",
                table: "categoria",
                column: "cat_parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_categoria_cat_tagname",
                table: "categoria",
                column: "cat_tagname");

            migrationBuilder.CreateIndex(
                name: "IX_categoria_atraccion_at_id",
                table: "categoria_atraccion",
                column: "at_id");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_cli_guid",
                table: "clientes",
                column: "cli_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_cli_numero_identificacion",
                table: "clientes",
                column: "cli_numero_identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_usu_id",
                table: "clientes",
                column: "usu_id");

            migrationBuilder.CreateIndex(
                name: "IX_datos_facturacion_dfac_guid",
                table: "datos_facturacion",
                column: "dfac_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datos_facturacion_fac_id",
                table: "datos_facturacion",
                column: "fac_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_destino_des_guid",
                table: "destino",
                column: "des_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_destino_des_pais",
                table: "destino",
                column: "des_pais");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_fac_guid",
                table: "facturas",
                column: "fac_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_fac_numero",
                table: "facturas",
                column: "fac_numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_pag_id",
                table: "facturas",
                column: "pag_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_rev_id",
                table: "facturas",
                column: "rev_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_horario_at_id_hor_fecha_hor_hora_inicio",
                table: "horario",
                columns: new[] { "at_id", "hor_fecha", "hor_hora_inicio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_horario_hor_guid",
                table: "horario",
                column: "hor_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_idioma_id_codigo",
                table: "idioma",
                column: "id_codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_idioma_id_descripcion",
                table: "idioma",
                column: "id_descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_idioma_id_guid",
                table: "idioma",
                column: "id_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_idioma_atraccion_at_id",
                table: "idioma_atraccion",
                column: "at_id");

            migrationBuilder.CreateIndex(
                name: "IX_imagen_img_guid",
                table: "imagen",
                column: "img_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_imagen_atraccion_at_id_ima_es_principal",
                table: "imagen_atraccion",
                columns: new[] { "at_id", "ima_es_principal" });

            migrationBuilder.CreateIndex(
                name: "IX_incluye_inc_guid",
                table: "incluye",
                column: "inc_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_pag_guid",
                table: "pagos",
                column: "pag_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_pag_referencia",
                table: "pagos",
                column: "pag_referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_rev_id",
                table: "pagos",
                column: "rev_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenia_at_id",
                table: "resenia",
                column: "at_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenia_rev_id",
                table: "resenia",
                column: "rev_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resenia_rsn_guid",
                table: "resenia",
                column: "rsn_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reserva_detalle_rdet_guid",
                table: "reserva_detalle",
                column: "rdet_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reserva_detalle_rev_id_tck_id",
                table: "reserva_detalle",
                columns: new[] { "rev_id", "tck_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reserva_detalle_tck_id",
                table: "reserva_detalle",
                column: "tck_id");

            migrationBuilder.CreateIndex(
                name: "IX_reserva_estado_historial_reh_guid",
                table: "reserva_estado_historial",
                column: "reh_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reserva_estado_historial_rev_id",
                table: "reserva_estado_historial",
                column: "rev_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_cli_id",
                table: "reservas",
                column: "cli_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_hor_id",
                table: "reservas",
                column: "hor_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_rev_codigo",
                table: "reservas",
                column: "rev_codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservas_rev_guid",
                table: "reservas",
                column: "rev_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_rol_guid",
                table: "roles",
                column: "rol_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_at_id",
                table: "ticket",
                column: "at_id");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_tck_guid",
                table: "ticket",
                column: "tck_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_usu_guid",
                table: "usuario",
                column: "usu_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_usu_login",
                table: "usuario",
                column: "usu_login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarioxroles_rol_id",
                table: "usuarioxroles",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarioxroles_usu_id_rol_id",
                table: "usuarioxroles",
                columns: new[] { "usu_id", "rol_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "atraccion_incluye");

            migrationBuilder.DropTable(
                name: "auditoria_log");

            migrationBuilder.DropTable(
                name: "categoria_atraccion");

            migrationBuilder.DropTable(
                name: "datos_facturacion");

            migrationBuilder.DropTable(
                name: "idioma_atraccion");

            migrationBuilder.DropTable(
                name: "imagen_atraccion");

            migrationBuilder.DropTable(
                name: "resenia");

            migrationBuilder.DropTable(
                name: "reserva_detalle");

            migrationBuilder.DropTable(
                name: "reserva_estado_historial");

            migrationBuilder.DropTable(
                name: "usuarioxroles");

            migrationBuilder.DropTable(
                name: "incluye");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "facturas");

            migrationBuilder.DropTable(
                name: "idioma");

            migrationBuilder.DropTable(
                name: "imagen");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "reservas");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "horario");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "atraccion");

            migrationBuilder.DropTable(
                name: "destino");
        }
    }
}
