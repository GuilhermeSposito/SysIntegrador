using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class MODIFICANDO_PRECISAO_DA_COLUNA_PVENDA123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "pvenda3",
                table: "cardapio",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "pvenda2",
                table: "cardapio",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "pvenda1",
                table: "cardapio",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "pvenda3",
                table: "cardapio",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "numeric(10,2)");

            migrationBuilder.AlterColumn<double>(
                name: "pvenda2",
                table: "cardapio",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "numeric(10,2)");

            migrationBuilder.AlterColumn<double>(
                name: "pvenda1",
                table: "cardapio",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "numeric(10,2)");
        }
    }
}
