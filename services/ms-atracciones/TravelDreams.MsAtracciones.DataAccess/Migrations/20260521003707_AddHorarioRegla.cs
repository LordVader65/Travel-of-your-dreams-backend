using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelDreams.MsAtracciones.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHorarioRegla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "horario_regla",
                columns: table => new
                {
                    hreg_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hreg_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    at_id = table.Column<int>(type: "integer", nullable: false),
                    hreg_hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    hreg_hora_fin = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    hreg_dias_semana = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "0,1,2,3,4,5,6"),
                    hreg_cupos = table.Column<int>(type: "integer", nullable: false),
                    hreg_fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    hreg_fecha_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    hreg_fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    hreg_usuario_ingreso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    hreg_ip_ingreso = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    hreg_fecha_mod = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hreg_usuario_mod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hreg_ip_mod = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    hreg_fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hreg_usuario_eliminacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hreg_ip_eliminacion = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    hreg_estado = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false, defaultValue: "A")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horario_regla", x => x.hreg_id);
                    table.CheckConstraint("ck_horario_regla_cupos", "hreg_cupos > 0");
                    table.CheckConstraint("ck_horario_regla_estado", "hreg_estado IN ('A','I')");
                    table.CheckConstraint("ck_horario_regla_rango", "hreg_fecha_fin >= hreg_fecha_inicio");
                    table.ForeignKey(
                        name: "FK_horario_regla_atraccion_at_id",
                        column: x => x.at_id,
                        principalTable: "atraccion",
                        principalColumn: "at_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_horario_regla_at_id_hreg_hora_inicio_hreg_fecha_inicio_hreg~",
                table: "horario_regla",
                columns: new[] { "at_id", "hreg_hora_inicio", "hreg_fecha_inicio", "hreg_fecha_fin" });

            migrationBuilder.CreateIndex(
                name: "IX_horario_regla_hreg_guid",
                table: "horario_regla",
                column: "hreg_guid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "horario_regla");
        }
    }
}
