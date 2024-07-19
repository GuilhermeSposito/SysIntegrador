using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_COLUNAS_AUTENTICACAO_TAXYMACHINE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "passwordtaxymachine",
                table: "parametrosdosistema",
                type: "text",
                nullable: false,
                defaultValue: "vazio");

            migrationBuilder.AddColumn<string>(
                name: "usernametaxymachine",
                table: "parametrosdosistema",
                type: "text",
                nullable: false,
                defaultValue: "vazio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passwordtaxymachine",
                table: "parametrosdosistema");

            migrationBuilder.DropColumn(
                name: "usernametaxymachine",
                table: "parametrosdosistema");
        }
    }
}
