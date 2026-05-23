using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelDreams.MsAtracciones.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaAtraccionPrincipal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_categoria_atraccion_at_id",
                table: "categoria_atraccion");

            migrationBuilder.AddColumn<bool>(
                name: "ca_es_principal",
                table: "categoria_atraccion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_categoria_atraccion_at_id_ca_es_principal",
                table: "categoria_atraccion",
                columns: new[] { "at_id", "ca_es_principal" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_categoria_atraccion_at_id_ca_es_principal",
                table: "categoria_atraccion");

            migrationBuilder.DropColumn(
                name: "ca_es_principal",
                table: "categoria_atraccion");

            migrationBuilder.CreateIndex(
                name: "IX_categoria_atraccion_at_id",
                table: "categoria_atraccion",
                column: "at_id");
        }
    }
}
