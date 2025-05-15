using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDOTABELADEROTEAMENTODEIMPRESSAO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roteamentodeimpressoras",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome_rota = table.Column<string>(type: "text", nullable: true),
                    impressora_caixa = table.Column<string>(type: "text", nullable: true, defaultValue: "Sem Impressora"),
                    impressora_auxiliar = table.Column<string>(type: "text", nullable: true, defaultValue: "Sem Impressora"),
                    impressora_cozinha1 = table.Column<string>(type: "text", nullable: true, defaultValue: "Sem Impressora"),
                    impressora_cozinha2 = table.Column<string>(type: "text", nullable: true, defaultValue: "Sem Impressora"),
                    impressora_cozinha3 = table.Column<string>(type: "text", nullable: true, defaultValue: "Sem Impressora"),
                    impressora_bar = table.Column<string>(type: "text", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roteamentodeimpressoras", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roteamentodeimpressoras");
        }
    }
}
