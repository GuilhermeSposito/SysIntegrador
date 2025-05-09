using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class AiQFome
{
    public readonly IMeuContexto _db;

    public AiQFome(IMeuContexto DB)
    {
        _db = DB;
    }

    public async Task Polling()
    {
        // Implementar a lógica de polling para o AiQFome
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                string Url = "https://plataforma.aiqfome.io/orders/v1";
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
                bool integraSysMenu = opcSistema!.IntegracaoSysMenu;
                if (integraSysMenu)
                {
                    bool CaixaAberto = await ClsDeIntegracaoSys.VerificaCaixaAberto();
                    if (!CaixaAberto)
                        await EnviaAvisoDeCaixaAberto();
                }

                List<ClsEmpresasAiqFome> EmpresasAiQueFome = await db.empresasaiqfome.ToListAsync();

                foreach (ClsEmpresasAiqFome empresaAtual in EmpresasAiQueFome)
                {
                    HttpResponseMessage? response = await EnviaRequisicao(Url, "GET", empresaAtual.TokenReq!)!;

                    if (response is null)
                        throw new Exception("Erro ao enviar requisição para AiQFome");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        ClsPedidos pedidos = JsonConvert.DeserializeObject<ClsPedidos>(responseBody)!;


                        foreach (Data pedido in pedidos.Data)
                        {
                            await SetPedidoAiQFome(pedido, empresaAtual, integraSysMenu);
                        }

                    }

                }
            }

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro ao listar pedidos", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    public async Task SetPedidoAiQFome(Data pedido, ClsEmpresasAiqFome empresaAtual, bool integraSysMenu)
    {
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                string Url = $"https://plataforma.aiqfome.io/orders/v1/{pedido.Id}";
                List<ParametrosDoPedido>? pedidosnodb = await db.parametrosdopedido.ToListAsync();

                if (pedidosnodb.Any(x => x.Id == pedido.Id.ToString() && x.CriadoPor == "AIQFOME"))
                    throw new Exception("Pedido já existe no banco de dados");

                HttpResponseMessage? response = await EnviaRequisicao(Url, "GET", empresaAtual.TokenReq!)!;
                if (response is null)
                    throw new Exception("Erro ao enviar requisição para AiQFome");

                if (response.IsSuccessStatusCode)
                {
                    ParametrosDoSistema? opcSystem = await db.parametrosdosistema.FirstOrDefaultAsync();

                    string ResponseBody = await response.Content.ReadAsStringAsync();
                    PedidoAiQFome pedidoAiQFome = JsonConvert.DeserializeObject<PedidoAiQFome>(ResponseBody)!;

                    int insertNoSysMenuConta = 0;
                    string? mesa = pedido.DeliveryType is not null && pedido.DeliveryType == "normal" ? "WEB" : "WEBB";
                    int DisplayId = 0;
                    //caso for integrado com o sysmenu vai inserir no access
                    if (integraSysMenu)
                    {
                        string? Complemento = " ";
                        string? EndEntrega = " ";
                        string? BairEntrega = " ";
                        string? Entregador = "00";
                        string? Telefone = " ";

                        if (pedidoAiQFome.Data.DeliveryType is not null)
                        {
                             Complemento = pedidoAiQFome.Data.Endereco.Complement;
                            EndEntrega = pedidoAiQFome.Data.Endereco.StreetName;
                            BairEntrega = pedidoAiQFome.Data.Endereco.NeighborhoodName == "" || pedidoAiQFome.Data.Endereco.NeighborhoodName is null ? " " : pedidoAiQFome.Data.Endereco.NeighborhoodName;
                          // Telefone = pedidoAiQFome
                        }

                        insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                                           mesa: mesa,
                                           cortesia: pedidoAiQFome.Data.PaymentMethod.CouponValue,
                                           taxaEntrega: pedidoAiQFome.Data.PaymentMethod.DeliveryTax,
                                           taxaMotoboy: pedidoAiQFome.Data.PaymentMethod.DeliveryTax,
                                           dtInicio: pedidoAiQFome.Data.Timeline.CreatedAt.Substring(0, 10).Replace("-", "/"),
                                            hrInicio: pedidoAiQFome.Data.Timeline.CreatedAt.Substring(11, 5),
                                            contatoNome: pedidoAiQFome.Data.User.Name,
                                            usuario: "CAIXA",
                                            dataSaida: pedidoAiQFome.Data.Timeline.CreatedAt.Substring(0, 10).Replace("-", "/"), //verificar depois
                                           hrSaida: pedidoAiQFome.Data.Timeline.CreatedAt.Substring(11, 5), //verificar depois 
                                            obsConta1: " ",
                                           iFoodPedidoID: pedido.Id.ToString(),
                                           obsConta2: " ",
                                           referencia: Complemento,
                                            endEntrega: EndEntrega,
                                            bairEntrega: BairEntrega,
                                            entregador: Entregador,
                                            telefone: Telefone,
                                            eAiQFome: true
                                            ); 

                        string type = pedidoAiQFome.Data.PaymentMethod.Name == "online" ? "ONLINE" : "OFFLINE";

                        ClsDeIntegracaoSys.IntegracaoPagCartao(pedidoAiQFome.Data.PaymentMethod.Name, insertNoSysMenuConta, pedidoAiQFome.Data.PaymentMethod.Total, pedidoAiQFome.Data.PaymentMethod.Name, "AIQFOME");

                        SysIntegradorApp.ClassesAuxiliares.Payments payments = new();

                        Cash SeForPagamentoEmDinherio = new Cash() { changeFor = pedidoAiQFome.Data.PaymentMethod.Change };
                        payments.methods.Add(new Methods() { method = type, value = pedidoAiQFome.Data.PaymentMethod.Total, cash = SeForPagamentoEmDinherio });


                        ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(payments, insertNoSysMenuConta);
                    }


                    //colocar o pedido no banco de dados do postgres
                    var pedidoInserido = db.parametrosdopedido.Add(new ParametrosDoPedido()
                    {
                        Id = pedidoAiQFome.Data.Id.ToString(),
                        Json = JsonConvert.SerializeObject(pedidoAiQFome),
                        Situacao = pedidoAiQFome.Data.Status,
                        Conta = insertNoSysMenuConta,
                        CriadoEm = DateTimeOffset.Now.ToString(),
                        DisplayId = DisplayId,
                        JsonPolling = "",
                        CriadoPor = "AIQFOME",
                        PesquisaDisplayId = pedido.Id,
                        PesquisaNome = pedidoAiQFome.Data.User.Name
                    }
                   );

                    await db.SaveChangesAsync();
                    //await db.DisposeAsync();

                    if (integraSysMenu)
                    {
                        //procurar se já tem o usuário que pediu o pedido cadastrado no sysmenu 
                        //se não tiver vai cadastrar o usuário

                        //definir as caracteristicas de cada item para inserir no sysmenu

                        foreach (ItemAiQFome item in pedidoAiQFome.Data.Items)
                        {
                            var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAiQFome(item);

                            ClsDeIntegracaoSys.IntegracaoContas(
                                               conta: insertNoSysMenuConta, //numero
                                               mesa: mesa, //texto curto 
                                               qtdade: item.Quantity, //numero
                                               codCarda1: CaracteristicasPedido.ExternalCode1, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                               codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                               codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                               tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                               descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                               valorUnit: item.Value / item.Quantity, //moeda
                                               valorTotal: item.Value, //moeda
                                               dataInicio: pedidoAiQFome.Data.Timeline.CreatedAt.Substring(0, 10).Replace("-", "/"), //data
                                               horaInicio: pedidoAiQFome.Data.Timeline.CreatedAt.Substring(11, 5), //data
                                               obs1: CaracteristicasPedido.Obs1,
                                               obs2: CaracteristicasPedido.Obs2,
                                               obs3: CaracteristicasPedido.Obs3,
                                               obs4: CaracteristicasPedido.Obs4,
                                               obs5: CaracteristicasPedido.Obs5,
                                               obs6: CaracteristicasPedido.Obs6,
                                               obs7: CaracteristicasPedido.Obs7,
                                               obs8: CaracteristicasPedido.Obs8,
                                               obs9: CaracteristicasPedido.Obs9,
                                               obs10: CaracteristicasPedido.Obs10,
                                               obs11: CaracteristicasPedido.Obs11,
                                               obs12: CaracteristicasPedido.Obs12,
                                               obs13: CaracteristicasPedido.Obs13,
                                               obs14: CaracteristicasPedido.Obs14,
                                               obs15: CaracteristicasPedido.ObsDoItem,
                                               cliente: pedidoAiQFome.Data.User.Name, // texto curto 80 letras
                                               telefone: " ", // texto curto 14 letras
                                               impComanda: "Não",
                                               ImpComanda2: "Não",
                                               qtdComanda: 00f//numero duplo 
                                          );//fim dos parâmetros

                        }

                    }


                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                    ClsDeSuporteAtualizarPanel.MudouDataBasePedido = true;



                    if (opcSystem!.AceitaPedidoAut)
                    {
                        await AceitarPedido(pedidoAiQFome, empresaAtual);
                    }

                }

            }
        }
        catch (Exception ex) when (ex.Message.Contains("Pedido já existe"))
        {
            //se chegar aqui vai mudar o status do pedido 
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro ao processar pedido", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    public async Task AceitarPedido(PedidoAiQFome pedido, ClsEmpresasAiqFome empresaAtual)
    {
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                string Url = $"https://plataforma.aiqfome.io/orders/v1/{pedido.Data.Id}/read";
                HttpResponseMessage? response = await EnviaRequisicao(Url, "PUT", empresaAtual.TokenReq!)!;
                if (response is null)
                    throw new Exception("Erro ao enviar requisição para AiQFome");

                if (response.IsSuccessStatusCode)
                {
                    await SysAlerta.Alerta("pedido aceito", "aceito", SysAlertaTipo.Sucesso, SysAlertaButtons.Ok);

                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro ao aceitar pedido", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    public async Task EnviaAvisoDeCaixaAberto()
    {
        ClsSons.PlaySom2();
        await SysAlerta.Alerta("Aplicativo Integrado", "Seu aplicativo está integrado com o SysMenu, abra o caixa para continuar", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        ClsSons.StopSom();
        Application.Exit();
        return;
    }

    public async Task<HttpResponseMessage>? EnviaRequisicao(string? url, string? metodo, string AcessToken, string? content = "", string? refreshToken = null)
    {
        // Implementar a lógica de envio de requisição para o AiQFome
        try
        {
            if (metodo == "GET")
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AcessToken);


                HttpResponseMessage response = await client.GetAsync(url);

                return response;
            }

            if (metodo == "PUT")
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AcessToken);

                var HttpContent = new StringContent(content!, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(url, HttpContent);

                return response;
            }

            return null;
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro", "Erro ao enviar requisição para AiQFome: " + ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
            return null;
        }
    }
}
