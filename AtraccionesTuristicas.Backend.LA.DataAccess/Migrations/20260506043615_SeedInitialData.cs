using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadScript("Seeds", "001_seed_roles.sql"));
            migrationBuilder.Sql(ReadScript("Seeds", "002_seed_admin_user.sql"));
            migrationBuilder.Sql(ReadScript("Seeds", "003_seed_catalogos_base.sql"));
            migrationBuilder.Sql(ReadScript("Seeds", "004_seed_atracciones_demo.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM horario WHERE hor_guid IN ('37000000-0000-0000-0000-000000000001','37000000-0000-0000-0000-000000000002');");
            migrationBuilder.Sql("DELETE FROM ticket WHERE tck_guid IN ('36000000-0000-0000-0000-000000000001','36000000-0000-0000-0000-000000000002');");
            migrationBuilder.Sql("DELETE FROM categoria_atraccion WHERE at_id IN (SELECT at_id FROM atraccion WHERE at_guid IN ('35000000-0000-0000-0000-000000000001','35000000-0000-0000-0000-000000000002'));");
            migrationBuilder.Sql("DELETE FROM imagen_atraccion WHERE at_id IN (SELECT at_id FROM atraccion WHERE at_guid IN ('35000000-0000-0000-0000-000000000001','35000000-0000-0000-0000-000000000002'));");
            migrationBuilder.Sql("DELETE FROM atraccion WHERE at_guid IN ('35000000-0000-0000-0000-000000000001','35000000-0000-0000-0000-000000000002');");
            migrationBuilder.Sql("DELETE FROM imagen WHERE img_guid IN ('34000000-0000-0000-0000-000000000001','34000000-0000-0000-0000-000000000002');");
            migrationBuilder.Sql("DELETE FROM incluye WHERE inc_guid IN ('33000000-0000-0000-0000-000000000001','33000000-0000-0000-0000-000000000002','33000000-0000-0000-0000-000000000003','33000000-0000-0000-0000-000000000004','33000000-0000-0000-0000-000000000005');");
            migrationBuilder.Sql("DELETE FROM idioma WHERE id_guid IN ('32000000-0000-0000-0000-000000000001','32000000-0000-0000-0000-000000000002','32000000-0000-0000-0000-000000000003','32000000-0000-0000-0000-000000000004','32000000-0000-0000-0000-000000000005','32000000-0000-0000-0000-000000000006');");
            migrationBuilder.Sql("DELETE FROM categoria WHERE cat_guid IN ('31000000-0000-0000-0000-000000000001','31000000-0000-0000-0000-000000000002','31000000-0000-0000-0000-000000000003','31000000-0000-0000-0000-000000000004');");
            migrationBuilder.Sql("DELETE FROM destino WHERE des_guid IN ('30000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000002','30000000-0000-0000-0000-000000000003');");
            migrationBuilder.Sql("DELETE FROM usuarioxroles WHERE usu_id IN (SELECT usu_id FROM usuario WHERE usu_guid = '20000000-0000-0000-0000-000000000001');");
            migrationBuilder.Sql("DELETE FROM usuario WHERE usu_guid = '20000000-0000-0000-0000-000000000001';");
            migrationBuilder.Sql("DELETE FROM roles WHERE rol_guid IN ('10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002');");
        }

        private static string ReadScript(string folder, string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", folder, fileName));
        }
    }
}
