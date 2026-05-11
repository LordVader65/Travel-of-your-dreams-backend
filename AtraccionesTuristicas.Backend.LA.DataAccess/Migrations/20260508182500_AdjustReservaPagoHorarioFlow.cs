using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AdjustReservaPagoHorarioFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Functions", "002_fn_crear_reserva.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "003_fn_cancelar_reserva.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "005_fn_confirmar_pago.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "006_fn_generar_factura.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Functions", "002_fn_crear_reserva.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "003_fn_cancelar_reserva.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "005_fn_confirmar_pago.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "006_fn_generar_factura.sql"));
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
