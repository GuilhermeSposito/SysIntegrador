using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADD_TABELA_PRODUTOSVERSAO1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cardapio",
                columns: table => new
                {
                    codigo = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    grupo = table.Column<string>(type: "text", nullable: true),
                    pvenda1 = table.Column<double>(type: "double precision", nullable: false),
                    pvenda2 = table.Column<double>(type: "double precision", nullable: false),
                    pvenda3 = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardapio", x => x.codigo);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cardapio");
        }
    }
}
