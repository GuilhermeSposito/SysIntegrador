using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_COLUNAS_DE_INTEGRACAO_OTTO_E_DEMATCH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "integradelmatchentregas",
                table: "parametrosdosistema",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "integraottoentregas",
                table: "parametrosdosistema",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "integradelmatchentregas",
                table: "parametrosdosistema");

            migrationBuilder.DropColumn(
                name: "integraottoentregas",
                table: "parametrosdosistema");
        }
    }
}
