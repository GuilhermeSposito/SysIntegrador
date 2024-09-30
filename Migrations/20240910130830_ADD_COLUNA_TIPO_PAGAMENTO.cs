using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADD_COLUNA_TIPO_PAGAMENTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tipodepagamentotaxymachine",
                table: "parametrosdosistema",
                type: "text",
                nullable: false,
                defaultValue: "R");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tipodepagamentotaxymachine",
                table: "parametrosdosistema");
        }
    }
}
