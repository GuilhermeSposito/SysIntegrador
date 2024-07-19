using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_COLUNA_PESQUISANOME : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pesquisanome",
                table: "parametrosdopedido",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("update parametrosdopedido set pesquisanome = 'padrao' where pesquisanome is null");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pesquisanome",
                table: "parametrosdopedido");
        }
    }
}
