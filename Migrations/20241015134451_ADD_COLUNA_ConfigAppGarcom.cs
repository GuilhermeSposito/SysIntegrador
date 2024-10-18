using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADD_COLUNA_ConfigAppGarcom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configappgarcom",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    listadeitens = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    buscadeitens = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    listaporgrupo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    requisicaoalfanumerica = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    requisicaonumerica = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    comanda = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    mesa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    semrequisicao = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configappgarcom", x => x.id);
                });

                 migrationBuilder.Sql(@"
                        INSERT INTO configappgarcom (listadeitens, buscadeitens, listaporgrupo, requisicaoalfanumerica, 
                                                     requisicaonumerica, comanda, mesa, semrequisicao) 
                        VALUES 
                        (false, false, true, false, false, false, true, false);
                  ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configappgarcom");
        }
    }
}
