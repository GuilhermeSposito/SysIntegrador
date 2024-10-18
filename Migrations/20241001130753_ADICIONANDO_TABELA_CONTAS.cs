using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_TABELA_CONTAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conta = table.Column<string>(type: "text", nullable: true),
                    mesa = table.Column<string>(type: "text", nullable: true),
                    qtdade = table.Column<int>(type: "integer", nullable: true),
                    codcarda1 = table.Column<string>(type: "text", nullable: true),
                    codcarda2 = table.Column<string>(type: "text", nullable: true),
                    codcarda3 = table.Column<string>(type: "text", nullable: true),
                    tamanho = table.Column<string>(type: "text", nullable: true),
                    descarda = table.Column<string>(type: "text", nullable: true),
                    valorunit = table.Column<string>(type: "text", nullable: true),
                    valortotal = table.Column<string>(type: "text", nullable: true),
                    datainicio = table.Column<string>(type: "text", nullable: true),
                    horainicio = table.Column<string>(type: "text", nullable: true),
                    obs1 = table.Column<string>(type: "text", nullable: true),
                    obs2 = table.Column<string>(type: "text", nullable: true),
                    obs3 = table.Column<string>(type: "text", nullable: true),
                    obs4 = table.Column<string>(type: "text", nullable: true),
                    obs5 = table.Column<string>(type: "text", nullable: true),
                    obs6 = table.Column<string>(type: "text", nullable: true),
                    obs7 = table.Column<string>(type: "text", nullable: true),
                    obs8 = table.Column<string>(type: "text", nullable: true),
                    obs9 = table.Column<string>(type: "text", nullable: true),
                    obs10 = table.Column<string>(type: "text", nullable: true),
                    obs11 = table.Column<string>(type: "text", nullable: true),
                    obs12 = table.Column<string>(type: "text", nullable: true),
                    obs13 = table.Column<string>(type: "text", nullable: true),
                    obs14 = table.Column<string>(type: "text", nullable: true),
                    obs15 = table.Column<string>(type: "text", nullable: true),
                    cliente = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    telefone = table.Column<string>(type: "text", nullable: true),
                    impcomanda = table.Column<string>(type: "text", nullable: true),
                    impcomanda2 = table.Column<string>(type: "text", nullable: true),
                    qtdcomanda = table.Column<float>(type: "real", nullable: true),
                    usuario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contas", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contas");
        }
    }
}
