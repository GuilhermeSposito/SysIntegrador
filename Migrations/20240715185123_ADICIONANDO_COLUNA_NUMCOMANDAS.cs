using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_COLUNA_NUMCOMANDAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "numviascomanda",
                table: "parametrosdosistema",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("update parametrosdosistema set numviascomanda = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numviascomanda",
                table: "parametrosdosistema");
        }
    }
}
