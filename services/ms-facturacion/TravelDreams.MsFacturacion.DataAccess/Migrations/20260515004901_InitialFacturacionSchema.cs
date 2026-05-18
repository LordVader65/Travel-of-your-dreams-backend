using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelDreams.MsFacturacion.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialFacturacionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "datos_facturacion",
                columns: table => new
                {
                    dfac_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dfac_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cli_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    dfac_tipo_identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dfac_numero_identificacion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    dfac_razon_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    dfac_nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dfac_apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dfac_correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    dfac_telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    dfac_direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    dfac_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    dfac_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dfac_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    dfac_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dfac_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dfac_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    dfac_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dfac_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dfac_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    dfac_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A"),
                    dfac_row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_datos_facturacion", x => x.dfac_id);
                    table.CheckConstraint("ck_datos_facturacion_estado", "dfac_estado IN ('A','I')");
                    table.CheckConstraint("ck_datos_facturacion_tipo_id", "dfac_tipo_identificacion IN ('CC','RUC','PASAPORTE','CEDULA','OTRO')");
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    pag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pag_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rev_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    cli_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    dfac_id = table.Column<int>(type: "integer", nullable: true),
                    pag_monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    pag_moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    pag_metodo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    pag_referencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pag_fecha_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    pag_origen_canal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    pag_estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "APROBADO"),
                    pag_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pag_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    pag_observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    pag_row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.pag_id);
                    table.CheckConstraint("ck_pagos_estado", "pag_estado IN ('PENDIENTE','APROBADO','RECHAZADO')");
                    table.CheckConstraint("ck_pagos_monto", "pag_monto >= 0");
                    table.ForeignKey(
                        name: "FK_pagos_datos_facturacion_dfac_id",
                        column: x => x.dfac_id,
                        principalTable: "datos_facturacion",
                        principalColumn: "dfac_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "facturas",
                columns: table => new
                {
                    fac_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fac_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    fac_numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    rev_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    cli_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    pag_id = table.Column<int>(type: "integer", nullable: false),
                    dfac_id = table.Column<int>(type: "integer", nullable: true),
                    fac_fecha_emision_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fac_subtotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    fac_valor_iva = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    fac_total = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    fac_moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    fac_observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fac_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fac_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    fac_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fac_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fac_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    fac_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A"),
                    fac_row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facturas", x => x.fac_id);
                    table.CheckConstraint("ck_facturas_estado", "fac_estado IN ('A','I')");
                    table.CheckConstraint("ck_facturas_iva", "fac_valor_iva >= 0");
                    table.CheckConstraint("ck_facturas_subtotal", "fac_subtotal >= 0");
                    table.CheckConstraint("ck_facturas_total", "fac_total >= 0");
                    table.ForeignKey(
                        name: "FK_facturas_datos_facturacion_dfac_id",
                        column: x => x.dfac_id,
                        principalTable: "datos_facturacion",
                        principalColumn: "dfac_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_facturas_pagos_pag_id",
                        column: x => x.pag_id,
                        principalTable: "pagos",
                        principalColumn: "pag_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_datos_facturacion_cli_guid",
                table: "datos_facturacion",
                column: "cli_guid");

            migrationBuilder.CreateIndex(
                name: "IX_datos_facturacion_dfac_guid",
                table: "datos_facturacion",
                column: "dfac_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_cli_guid",
                table: "facturas",
                column: "cli_guid");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_dfac_id",
                table: "facturas",
                column: "dfac_id");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_fac_guid",
                table: "facturas",
                column: "fac_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_fac_numero",
                table: "facturas",
                column: "fac_numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_pag_id",
                table: "facturas",
                column: "pag_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facturas_rev_guid",
                table: "facturas",
                column: "rev_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_cli_guid",
                table: "pagos",
                column: "cli_guid");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_dfac_id",
                table: "pagos",
                column: "dfac_id");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_pag_guid",
                table: "pagos",
                column: "pag_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_pag_referencia",
                table: "pagos",
                column: "pag_referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_rev_guid",
                table: "pagos",
                column: "rev_guid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "facturas");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "datos_facturacion");
        }
    }
}
