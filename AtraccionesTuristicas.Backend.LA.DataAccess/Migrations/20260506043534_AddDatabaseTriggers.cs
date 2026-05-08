using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Triggers", "001_trg_clientes_row_version.sql"));
            migrationBuilder.Sql(ReadScript("Triggers", "002_trg_facturas_row_version.sql"));
            migrationBuilder.Sql(ReadScript("Triggers", "003_trg_auditoria_reservas.sql"));
            migrationBuilder.Sql(ReadScript("Triggers", "004_trg_auditoria_pagos.sql"));
            migrationBuilder.Sql(ReadScript("Triggers", "005_trg_auditoria_facturas.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_auditoria_facturas ON facturas;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS trg_auditoria_facturas_fn();");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_auditoria_pagos ON pagos;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS trg_auditoria_pagos_fn();");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_auditoria_reservas ON reservas;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS trg_auditoria_reservas_fn();");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_facturas_row_version ON facturas;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_clientes_row_version ON clientes;");
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
