using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelDreams.MsFacturacion.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplacePagoSolicitudesV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "facturacion_solicitudes_v3",
                columns: table => new
                {
                    fsol_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fsol_correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fsol_estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    cli_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    rev_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    fac_guid = table.Column<Guid>(type: "uuid", nullable: true),
                    fac_numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fsol_error = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fsol_payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    fsol_created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fsol_updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facturacion_solicitudes_v3", x => x.fsol_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_facturacion_solicitudes_v3_cli_guid",
                table: "facturacion_solicitudes_v3",
                column: "cli_guid");

            migrationBuilder.CreateIndex(
                name: "IX_facturacion_solicitudes_v3_fsol_correlation_id",
                table: "facturacion_solicitudes_v3",
                column: "fsol_correlation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturacion_solicitudes_v3_fsol_estado",
                table: "facturacion_solicitudes_v3",
                column: "fsol_estado");

            migrationBuilder.CreateIndex(
                name: "IX_facturacion_solicitudes_v3_rev_guid",
                table: "facturacion_solicitudes_v3",
                column: "rev_guid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "facturacion_solicitudes_v3");
        }
    }
}
