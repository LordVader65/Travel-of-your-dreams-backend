using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelDreams.MsReservas.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddReservaBookingSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "rev_atraccion_nombre",
                table: "reservas",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "rev_hor_fecha",
                table: "reservas",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "rev_hor_hora_fin",
                table: "reservas",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "rev_hor_hora_inicio",
                table: "reservas",
                type: "time without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rev_atraccion_nombre",
                table: "reservas");

            migrationBuilder.DropColumn(
                name: "rev_hor_fecha",
                table: "reservas");

            migrationBuilder.DropColumn(
                name: "rev_hor_hora_fin",
                table: "reservas");

            migrationBuilder.DropColumn(
                name: "rev_hor_hora_inicio",
                table: "reservas");
        }
    }
}
