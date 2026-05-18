using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelDreams.MsAuditoria.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuditoriaSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auditoria_log",
                columns: table => new
                {
                    log_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    log_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    log_servicio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    log_tabla = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    log_operacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    log_registro_id = table.Column<int>(type: "integer", nullable: true),
                    log_registro_guid = table.Column<Guid>(type: "uuid", nullable: true),
                    log_datos_anteriores = table.Column<string>(type: "text", nullable: true),
                    log_datos_nuevos = table.Column<string>(type: "text", nullable: true),
                    log_fecha_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    log_usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    log_ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    log_origen_canal = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    log_correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    evento_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_log", x => x.log_id);
                    table.CheckConstraint("ck_auditoria_log_delete", "log_operacion <> 'DELETE' OR log_datos_nuevos IS NULL");
                    table.CheckConstraint("ck_auditoria_log_insert", "log_operacion <> 'INSERT' OR log_datos_anteriores IS NULL");
                    table.CheckConstraint("ck_auditoria_log_operacion", "log_operacion IN ('INSERT','UPDATE','DELETE','LOGIN','LOGOUT','BUSINESS_EVENT')");
                });

            migrationBuilder.CreateTable(
                name: "eventos_procesados",
                columns: table => new
                {
                    ep_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ep_evento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ep_tipo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ep_origen_servicio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ep_fecha_procesado_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ep_correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos_procesados", x => x.ep_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_evento_id",
                table: "auditoria_log",
                column: "evento_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_log_correlation_id",
                table: "auditoria_log",
                column: "log_correlation_id");

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
                name: "IX_auditoria_log_log_servicio_log_tabla_log_registro_id",
                table: "auditoria_log",
                columns: new[] { "log_servicio", "log_tabla", "log_registro_id" });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_log_usuario",
                table: "auditoria_log",
                column: "log_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_procesados_ep_evento_id",
                table: "eventos_procesados",
                column: "ep_evento_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_eventos_procesados_ep_origen_servicio",
                table: "eventos_procesados",
                column: "ep_origen_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_procesados_ep_tipo",
                table: "eventos_procesados",
                column: "ep_tipo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria_log");

            migrationBuilder.DropTable(
                name: "eventos_procesados");
        }
    }
}
