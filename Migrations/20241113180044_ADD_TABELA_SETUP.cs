using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADD_TABELA_SETUP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "setup",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    couvertvalor = table.Column<float>(type: "real", nullable: false),
                    couvertdom = table.Column<bool>(type: "boolean", nullable: false),
                    couvertseg = table.Column<bool>(type: "boolean", nullable: false),
                    couvertter = table.Column<bool>(type: "boolean", nullable: false),
                    couvertquar = table.Column<bool>(type: "boolean", nullable: false),
                    couvertquin = table.Column<bool>(type: "boolean", nullable: false),
                    couvertsex = table.Column<bool>(type: "boolean", nullable: false),
                    couvertsab = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setup", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "setup");
        }
    }
}
