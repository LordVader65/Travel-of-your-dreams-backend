using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelDreams.MsIdentidad.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentidadSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    rol_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rol_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    rol_descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rol_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rol_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rol_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    rol_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rol_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rol_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    rol_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.rol_id);
                    table.CheckConstraint("ck_roles_estado", "rol_estado IN ('A','I')");
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    usu_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usu_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    usu_login = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    usu_password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    usu_fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    usu_usuario_registro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usu_ip_registro = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    usu_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usu_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    usu_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    usu_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usu_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    usu_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    usu_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.usu_id);
                    table.CheckConstraint("ck_usuario_estado", "usu_estado IN ('A','I')");
                });

            migrationBuilder.CreateTable(
                name: "usuarioxroles",
                columns: table => new
                {
                    usu_rol_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usu_id = table.Column<int>(type: "integer", nullable: false),
                    rol_id = table.Column<int>(type: "integer", nullable: false),
                    usu_rol_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioxroles", x => x.usu_rol_id);
                    table.CheckConstraint("ck_usuarioxroles_estado", "usu_rol_estado IN ('A','I')");
                    table.ForeignKey(
                        name: "FK_usuarioxroles_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "rol_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuarioxroles_usuario_usu_id",
                        column: x => x.usu_id,
                        principalTable: "usuario",
                        principalColumn: "usu_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "rol_id", "rol_descripcion", "rol_estado", "rol_fecha_eliminacion", "rol_fecha_ingreso", "rol_guid", "rol_ip_eliminacion", "rol_ip_ingreso", "rol_usuario_eliminacion", "rol_usuario_ingreso" },
                values: new object[,]
                {
                    { 1, "ADMIN", "A", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), null, "migration", null, "seed" },
                    { 2, "CLIENTE", "A", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), null, "migration", null, "seed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_roles_rol_descripcion",
                table: "roles",
                column: "rol_descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_rol_guid",
                table: "roles",
                column: "rol_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_usu_guid",
                table: "usuario",
                column: "usu_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_usu_login",
                table: "usuario",
                column: "usu_login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarioxroles_rol_id",
                table: "usuarioxroles",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarioxroles_usu_id_rol_id",
                table: "usuarioxroles",
                columns: new[] { "usu_id", "rol_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarioxroles");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
