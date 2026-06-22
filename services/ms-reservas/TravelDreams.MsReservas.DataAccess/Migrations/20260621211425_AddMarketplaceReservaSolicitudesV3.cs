using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelDreams.MsReservas.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceReservaSolicitudesV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservas_solicitudes_v3",
                columns: table => new
                {
                    rsol_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rsol_correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rsol_estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    cli_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    rev_guid = table.Column<Guid>(type: "uuid", nullable: true),
                    rev_codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    rsol_error = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    rsol_payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    rsol_created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rsol_updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservas_solicitudes_v3", x => x.rsol_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservas_solicitudes_v3_cli_guid",
                table: "reservas_solicitudes_v3",
                column: "cli_guid");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_solicitudes_v3_rsol_correlation_id",
                table: "reservas_solicitudes_v3",
                column: "rsol_correlation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservas_solicitudes_v3_rsol_estado",
                table: "reservas_solicitudes_v3",
                column: "rsol_estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservas_solicitudes_v3");
        }
    }
}
