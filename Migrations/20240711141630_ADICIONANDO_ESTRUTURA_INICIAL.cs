using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ADICIONANDO_ESTRUTURA_INICIAL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apoioonpedido",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pedido = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apoioonpedido", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parametrosdeautenticacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accesstoken = table.Column<string>(type: "text", nullable: true),
                    refreshtoken = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    expiresin = table.Column<int>(type: "integer", nullable: false),
                    venceem = table.Column<string>(type: "text", nullable: true),
                    tokendelmatch = table.Column<string>(type: "text", nullable: true),
                    venceemdelmatch = table.Column<string>(type: "text", nullable: true),
                    tokenonpedido = table.Column<string>(type: "text", nullable: true),
                    venceemonpedido = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametrosdeautenticacao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parametrosdopedido",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    json = table.Column<string>(type: "text", nullable: true),
                    situacao = table.Column<string>(type: "text", nullable: true),
                    conta = table.Column<int>(type: "integer", nullable: false),
                    criadoem = table.Column<string>(type: "text", nullable: false),
                    displayid = table.Column<int>(type: "integer", nullable: false),
                    jsonpolling = table.Column<string>(type: "text", nullable: false),
                    criadopor = table.Column<string>(type: "text", nullable: false),
                    pesquisadisplayid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametrosdopedido", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parametrosdosistema",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nomefantasia = table.Column<string>(type: "text", nullable: true),
                    endereco = table.Column<string>(type: "text", nullable: true),
                    impressaoaut = table.Column<bool>(type: "boolean", nullable: false),
                    aceitapedidoaut = table.Column<bool>(type: "boolean", nullable: false),
                    caminhodobanco = table.Column<string>(type: "text", nullable: true),
                    caminhoservidor = table.Column<string>(type: "text", nullable: true),
                    integracaosysmenu = table.Column<bool>(type: "boolean", nullable: false),
                    impressora1 = table.Column<string>(type: "text", nullable: true),
                    impressora2 = table.Column<string>(type: "text", nullable: true),
                    impressora3 = table.Column<string>(type: "text", nullable: true),
                    impressora4 = table.Column<string>(type: "text", nullable: true),
                    impressora5 = table.Column<string>(type: "text", nullable: true),
                    impressoraaux = table.Column<string>(type: "text", nullable: true),
                    telefone = table.Column<string>(type: "text", nullable: true),
                    clientid = table.Column<string>(type: "text", nullable: true),
                    clientsecret = table.Column<string>(type: "text", nullable: true),
                    merchantid = table.Column<string>(type: "text", nullable: true),
                    delmatchid = table.Column<string>(type: "text", nullable: true),
                    agruparcomandas = table.Column<bool>(type: "boolean", nullable: false),
                    imprimircomandacaixa = table.Column<bool>(type: "boolean", nullable: false),
                    tipocomanda = table.Column<int>(type: "integer", nullable: false),
                    enviapedidoaut = table.Column<bool>(type: "boolean", nullable: false),
                    integradelmatch = table.Column<bool>(type: "boolean", nullable: false),
                    integraifood = table.Column<bool>(type: "boolean", nullable: false),
                    userdelmatch = table.Column<string>(type: "text", nullable: true),
                    senhadelmatch = table.Column<string>(type: "text", nullable: true),
                    impcompacta = table.Column<bool>(type: "boolean", nullable: false),
                    removecomplmentos = table.Column<bool>(type: "boolean", nullable: false),
                    integraonpedido = table.Column<bool>(type: "boolean", nullable: false),
                    tokenonpedido = table.Column<string>(type: "text", nullable: true),
                    useronpedido = table.Column<string>(type: "text", nullable: true),
                    senhaonpedido = table.Column<string>(type: "text", nullable: true),
                    tempoentrega = table.Column<int>(type: "integer", nullable: false),
                    tempoconclonpedido = table.Column<int>(type: "integer", nullable: false),
                    temporetirada = table.Column<int>(type: "integer", nullable: false),
                    dtultimaverif = table.Column<string>(type: "text", nullable: false),
                    integraccm = table.Column<bool>(type: "boolean", nullable: false),
                    tokenccm = table.Column<string>(type: "text", nullable: false),
                    cardapiousando = table.Column<string>(type: "text", nullable: false),
                    empresadeentrega = table.Column<string>(type: "text", nullable: false),
                    cidade = table.Column<string>(type: "text", nullable: false),
                    comandareduzida = table.Column<bool>(type: "boolean", nullable: false),
                    destacarobs = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametrosdosistema", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apoioonpedido");

            migrationBuilder.DropTable(
                name: "parametrosdeautenticacao");

            migrationBuilder.DropTable(
                name: "parametrosdopedido");

            migrationBuilder.DropTable(
                name: "parametrosdosistema");
        }
    }
}
