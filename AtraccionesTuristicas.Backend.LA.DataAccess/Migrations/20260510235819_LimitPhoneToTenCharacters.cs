using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LimitPhoneToTenCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE datos_facturacion SET dfac_telefono = LEFT(dfac_telefono, 10) WHERE dfac_telefono IS NOT NULL AND LENGTH(dfac_telefono) > 10;");
            migrationBuilder.Sql("UPDATE clientes SET cli_telefono = LEFT(cli_telefono, 10) WHERE cli_telefono IS NOT NULL AND LENGTH(cli_telefono) > 10;");

            migrationBuilder.AlterColumn<string>(
                name: "dfac_telefono",
                table: "datos_facturacion",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cli_telefono",
                table: "clientes",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "dfac_telefono",
                table: "datos_facturacion",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cli_telefono",
                table: "clientes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
