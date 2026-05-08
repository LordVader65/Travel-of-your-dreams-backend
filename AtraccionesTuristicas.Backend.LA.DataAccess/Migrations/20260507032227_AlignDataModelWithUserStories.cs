using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlignDataModelWithUserStories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_datos_facturacion_facturas_fac_id",
                table: "datos_facturacion");

            migrationBuilder.DropIndex(
                name: "IX_datos_facturacion_fac_id",
                table: "datos_facturacion");

            migrationBuilder.RenameColumn(
                name: "fac_id",
                table: "datos_facturacion",
                newName: "cli_id");

            migrationBuilder.AddColumn<int>(
                name: "dfac_id",
                table: "facturas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "fac_subtotal",
                table: "facturas",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "fac_valor_iva",
                table: "facturas",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "dfac_direccion",
                table: "datos_facturacion",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dfac_estado",
                table: "datos_facturacion",
                type: "character(1)",
                fixedLength: true,
                maxLength: 1,
                nullable: false,
                defaultValue: "A");

            migrationBuilder.AddColumn<DateTime>(
                name: "dfac_fecha_eliminacion",
                table: "datos_facturacion",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dfac_fecha_ingreso",
                table: "datos_facturacion",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "dfac_fecha_mod",
                table: "datos_facturacion",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dfac_ip_eliminacion",
                table: "datos_facturacion",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dfac_ip_ingreso",
                table: "datos_facturacion",
                type: "character varying(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dfac_ip_mod",
                table: "datos_facturacion",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dfac_numero_identificacion",
                table: "datos_facturacion",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dfac_razon_social",
                table: "datos_facturacion",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dfac_tipo_identificacion",
                table: "datos_facturacion",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dfac_usuario_eliminacion",
                table: "datos_facturacion",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dfac_usuario_ingreso",
                table: "datos_facturacion",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "dfac_usuario_mod",
                table: "datos_facturacion",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_dfac_id",
                table: "facturas",
                column: "dfac_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_facturas_iva",
                table: "facturas",
                sql: "fac_valor_iva >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_facturas_subtotal",
                table: "facturas",
                sql: "fac_subtotal >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_datos_facturacion_cli_id",
                table: "datos_facturacion",
                column: "cli_id");

            migrationBuilder.CreateIndex(
                name: "IX_datos_facturacion_cli_id_dfac_numero_identificacion",
                table: "datos_facturacion",
                columns: new[] { "cli_id", "dfac_numero_identificacion" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_dfac_estado",
                table: "datos_facturacion",
                sql: "dfac_estado IN ('A','I')");

            migrationBuilder.AddForeignKey(
                name: "FK_datos_facturacion_clientes_cli_id",
                table: "datos_facturacion",
                column: "cli_id",
                principalTable: "clientes",
                principalColumn: "cli_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_facturas_datos_facturacion_dfac_id",
                table: "facturas",
                column: "dfac_id",
                principalTable: "datos_facturacion",
                principalColumn: "dfac_id");

            migrationBuilder.Sql(ReadScript("Functions", "002_fn_crear_reserva.sql"));
            migrationBuilder.Sql(ReadScript("Functions", "006_fn_generar_factura.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_datos_facturacion_clientes_cli_id",
                table: "datos_facturacion");

            migrationBuilder.DropForeignKey(
                name: "FK_facturas_datos_facturacion_dfac_id",
                table: "facturas");

            migrationBuilder.DropIndex(
                name: "IX_facturas_dfac_id",
                table: "facturas");

            migrationBuilder.DropCheckConstraint(
                name: "ck_facturas_iva",
                table: "facturas");

            migrationBuilder.DropCheckConstraint(
                name: "ck_facturas_subtotal",
                table: "facturas");

            migrationBuilder.DropIndex(
                name: "IX_datos_facturacion_cli_id",
                table: "datos_facturacion");

            migrationBuilder.DropIndex(
                name: "IX_datos_facturacion_cli_id_dfac_numero_identificacion",
                table: "datos_facturacion");

            migrationBuilder.DropCheckConstraint(
                name: "ck_dfac_estado",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_id",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "fac_subtotal",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "fac_valor_iva",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "dfac_direccion",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_estado",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_fecha_eliminacion",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_fecha_ingreso",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_fecha_mod",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_ip_eliminacion",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_ip_ingreso",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_ip_mod",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_numero_identificacion",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_razon_social",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_tipo_identificacion",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_usuario_eliminacion",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_usuario_ingreso",
                table: "datos_facturacion");

            migrationBuilder.DropColumn(
                name: "dfac_usuario_mod",
                table: "datos_facturacion");

            migrationBuilder.RenameColumn(
                name: "cli_id",
                table: "datos_facturacion",
                newName: "fac_id");

            migrationBuilder.CreateIndex(
                name: "IX_datos_facturacion_fac_id",
                table: "datos_facturacion",
                column: "fac_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_datos_facturacion_facturas_fac_id",
                table: "datos_facturacion",
                column: "fac_id",
                principalTable: "facturas",
                principalColumn: "fac_id",
                onDelete: ReferentialAction.Cascade);
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
