using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysIntegradorApp.Migrations
{
    /// <inheritdoc />
    public partial class ALIMENTANDO_TABELA_PARAMETROS_DO_SISTEMA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO parametrosdosistema" +
               "(nomefantasia,endereco,impressaoaut,aceitapedidoaut,caminhodobanco,integracaosysmenu,impressora1," +
               "impressora2,impressora3,impressora4,impressora5,telefone,clientid,clientsecret,agruparcomandas,impressoraaux,merchantid," +
               "imprimircomandacaixa,tipocomanda,enviapedidoaut,dtultimaverif,integradelmatch,integraifood,impcompacta,removecomplmentos,integraonpedido," +
               "tempoentrega,tempoconclonpedido,temporetirada,integraccm,tokenccm,cardapiousando,empresadeentrega,cidade,comandareduzida,destacarobs) VALUES " +
               "('SysLogica - Testes','Rua Miguel Petroni 2338 - Sala 3',true,false," +
               "'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\SAAB\\BASE\\CONTAS.mdb',false,'Sem Impressora'," +
               "'Sem Impressora','Sem Impressora', 'Sem Impressora','Sem Impressora','(16) 3416-5248','7e476dce-79fa-4a7e-a605-aa2a1a40b803'," +
               " 'z5086yxoeeblv5go12ag9ynk2i8oan36l0gca8y9vs0h66yrorjh2nccdmxpbxk955lb0j6wc7vdpb2i3416aqs8ja4xjhbw3u0'," +
               "false, 'Sem Impressora','9362018a-6ae2-439c-968b-a40177a085ea',false,2, false,'31/05/2024 15:33:59',false,true,true,false,false,60," +
               "120,30,true,'nenhum','nenhum','nenhuma','São Carlos',true,true);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("delete from parametrosdosistema ");

        }
    }
}
