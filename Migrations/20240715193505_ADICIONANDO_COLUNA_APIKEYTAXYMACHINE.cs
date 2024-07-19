using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_COLUNA_APIKEYTAXYMACHINE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "apikeytaxymachine",
                table: "parametrosdosistema",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("update parametrosdosistema set apikeytaxymachine = 'Valor Padrão' ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "apikeytaxymachine",
                table: "parametrosdosistema");
        }
    }
}
