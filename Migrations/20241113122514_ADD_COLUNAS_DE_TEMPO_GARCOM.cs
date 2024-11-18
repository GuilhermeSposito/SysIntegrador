using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADD_COLUNAS_DE_TEMPO_GARCOM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "tempopollinggarcom",
                table: "parametrosdosistema",
                type: "integer",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "tempoenviopedido",
                table: "configappgarcom",
                type: "integer",
                nullable: false,
                defaultValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tempopollinggarcom",
                table: "parametrosdosistema");

            migrationBuilder.DropColumn(
                name: "tempoenviopedido",
                table: "configappgarcom");
        }
    }
}
