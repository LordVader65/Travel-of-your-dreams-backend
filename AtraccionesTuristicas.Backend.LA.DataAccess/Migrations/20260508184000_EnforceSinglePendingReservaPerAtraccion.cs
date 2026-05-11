using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EnforceSinglePendingReservaPerAtraccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Functions", "002_fn_crear_reserva.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Functions", "002_fn_crear_reserva.sql"));
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
