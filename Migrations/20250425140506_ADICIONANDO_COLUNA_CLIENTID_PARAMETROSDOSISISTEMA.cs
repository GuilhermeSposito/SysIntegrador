using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_COLUNA_CLIENTID_PARAMETROSDOSISISTEMA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "client_secret_aiqfome",
                table: "parametrosdosistema",
                type: "text",
                nullable: false,
                defaultValue: "2Z0wKTlp6Uu3z0__NAfZlCAYVA9wwWOTlgjTIKzfEtg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "client_secret_aiqfome",
                table: "parametrosdosistema");
        }
    }
}
