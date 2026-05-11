using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ImprovePagoFacturaDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Functions", "005_fn_confirmar_pago.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Functions", "005_fn_confirmar_pago.sql"));
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
