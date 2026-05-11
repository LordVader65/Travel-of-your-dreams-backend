using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ExternalClientsAndHorarioWeekdays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clientes_usuario_usu_id",
                table: "clientes");

            migrationBuilder.AddColumn<string>(
                name: "hor_dias_semana",
                table: "horario",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "0,1,2,3,4,5,6");

            migrationBuilder.AlterColumn<int>(
                name: "usu_id",
                table: "clientes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_clientes_usuario_usu_id",
                table: "clientes",
                column: "usu_id",
                principalTable: "usuario",
                principalColumn: "usu_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clientes_usuario_usu_id",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "hor_dias_semana",
                table: "horario");

            migrationBuilder.AlterColumn<int>(
                name: "usu_id",
                table: "clientes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_clientes_usuario_usu_id",
                table: "clientes",
                column: "usu_id",
                principalTable: "usuario",
                principalColumn: "usu_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
