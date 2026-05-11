using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityAuditTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Triggers", "006_trg_auditoria_usuarios.sql"));
            migrationBuilder.Sql(ReadScript("Triggers", "007_trg_auditoria_clientes.sql"));
            migrationBuilder.Sql(ReadScript("Triggers", "008_trg_auditoria_usuarioxroles.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_auditoria_usuarioxroles ON usuarioxroles;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS trg_auditoria_usuarioxroles_fn();");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_auditoria_clientes ON clientes;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS trg_auditoria_clientes_fn();");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_auditoria_usuarios ON usuario;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS trg_auditoria_usuarios_fn();");
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
