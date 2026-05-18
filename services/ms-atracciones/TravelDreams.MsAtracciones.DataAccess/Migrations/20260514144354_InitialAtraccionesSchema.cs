using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelDreams.MsAtracciones.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialAtraccionesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");

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
                    hor_dias_semana = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "0,1,2,3,4,5,6"),
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
                name: "resenia",
                columns: table => new
                {
                    rsn_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rsn_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    rev_guid = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.CheckConstraint("ck_resenia_estado", "rsn_estado IN ('A','I')");
                    table.CheckConstraint("ck_resenia_rating", "rsn_rating BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_resenia_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
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
                name: "IX_destino_des_guid",
                table: "destino",
                column: "des_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_destino_des_pais",
                table: "destino",
                column: "des_pais");

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
                name: "IX_resenia_at_id",
                table: "resenia",
                column: "at_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenia_rev_guid",
                table: "resenia",
                column: "rev_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resenia_rsn_guid",
                table: "resenia",
                column: "rsn_guid",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "atraccion_incluye");

            migrationBuilder.DropTable(
                name: "categoria_atraccion");

            migrationBuilder.DropTable(
                name: "horario");

            migrationBuilder.DropTable(
                name: "idioma_atraccion");

            migrationBuilder.DropTable(
                name: "imagen_atraccion");

            migrationBuilder.DropTable(
                name: "resenia");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "incluye");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "idioma");

            migrationBuilder.DropTable(
                name: "imagen");

            migrationBuilder.DropTable(
                name: "atraccion");

            migrationBuilder.DropTable(
                name: "destino");
        }
    }
}
