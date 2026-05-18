using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelDreams.MsReservas.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialReservasSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    cli_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cli_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    usu_guid = table.Column<Guid>(type: "uuid", nullable: true),
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
                    at_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    hor_guid = table.Column<Guid>(type: "uuid", nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "reserva_detalle",
                columns: table => new
                {
                    rdet_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rdet_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rev_id = table.Column<int>(type: "integer", nullable: false),
                    tck_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    rdet_ticket_titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    rdet_tipo_participante = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    rdet_cantidad = table.Column<int>(type: "integer", nullable: false),
                    rdet_precio_unit = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    rdet_subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    rdet_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rdet_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rdet_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
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
                name: "IX_reserva_detalle_rdet_guid",
                table: "reserva_detalle",
                column: "rdet_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reserva_detalle_rev_id_tck_guid",
                table: "reserva_detalle",
                columns: new[] { "rev_id", "tck_guid" },
                unique: true);

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
                name: "IX_reservas_at_guid",
                table: "reservas",
                column: "at_guid");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_cli_id",
                table: "reservas",
                column: "cli_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_hor_guid",
                table: "reservas",
                column: "hor_guid");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reserva_detalle");

            migrationBuilder.DropTable(
                name: "reserva_estado_historial");

            migrationBuilder.DropTable(
                name: "reservas");

            migrationBuilder.DropTable(
                name: "clientes");
        }
    }
}
