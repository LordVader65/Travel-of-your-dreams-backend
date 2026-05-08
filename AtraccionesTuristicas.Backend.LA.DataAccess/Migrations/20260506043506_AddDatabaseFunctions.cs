using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Functions", "001_fn_increment_row_version_generic.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "002_fn_crear_reserva.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "003_fn_cancelar_reserva.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "004_fn_expirar_reservas_pendientes.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "005_fn_confirmar_pago.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "006_fn_generar_factura.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "007_fn_registrar_auditoria.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_registrar_auditoria(VARCHAR, VARCHAR, INTEGER, UUID, TEXT, TEXT, VARCHAR, VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_generar_factura(UUID, UUID, VARCHAR, VARCHAR, VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_confirmar_pago(UUID, VARCHAR, NUMERIC, VARCHAR, VARCHAR, VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_expirar_reservas_pendientes(VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_cancelar_reserva(UUID, VARCHAR, VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_crear_reserva(UUID, UUID, JSONB, VARCHAR, VARCHAR, VARCHAR, INTEGER, NUMERIC);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_increment_row_version_generic();");
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
