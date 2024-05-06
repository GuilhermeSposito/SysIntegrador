using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Data.OleDb;
using System.Net.Http.Json;
using SysIntegradorApp.data;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class Ifood
{
    public static string? CaminhoBaseSysMenu { get; set; } = ApplicationDbContext.RetornaCaminhoBaseSysMenu();
    public static async Task Polling() //pulling feito de 30 em 30 Segundos, Caso seja encontrado algum novo pedido ele chama o GetPedidos
    {
        string url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        try
        {
            RefreshTokenIfood();

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            var reponse = await client.GetAsync($"{url}:polling");

            int statusCode = (int)reponse.StatusCode;
            if (statusCode == 200)
            {
                using ApplicationDbContext db = new ApplicationDbContext();
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                string jsonContent = await reponse.Content.ReadAsStringAsync();
                List<Polling>? pollings = JsonConvert.DeserializeObject<List<Polling>>(jsonContent); //pedidos nesse caso é o pulling 



                foreach (var P in pollings)
                {
                    switch (P.code)
                    {
                        case "PLC": //caso entre aqui é porque é um novo pedido
                            ClsSons.PlaySom();
                            if (opcSistema.AceitaPedidoAut)
                            {
                                await SetPedido(P.orderId, P);
                                await AvisarAcknowledge(P);
                                ConfirmarPedido(P);
                            }
                            else
                            {
                                await SetPedido(P.orderId, P);
                            }
                            break;
                        case "CFM":
                            ClsSons.StopSom();
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                        case "CAN":
                            ClsDeIntegracaoSys.ExcluiPedidoCasoCancelado(P.orderId);
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                        case "CON": //mudaria o status ou na tabela do sys menu
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                        case "DSP":
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                    }
                }

                FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));

            }

            string statusMerchat = await GetStatusMerchant();


            if (statusMerchat == "OK")
            {
                FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.MudaStatusMerchant()));
            }

            PostgresConfigs.LimpaPedidosACada8horas();

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }

    }


    //função para avisar para o ifood o ACK
    public static async Task AvisarAcknowledge(Polling polling)
    {
        string? url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            List<Polling> pollingList = new List<Polling>();
            pollingList.Add(polling);


            var polingToJson = JsonConvert.SerializeObject(pollingList);


            StringContent content = new StringContent(polingToJson, Encoding.UTF8, "application/json");

            await client.PostAsync($"{url}/acknowledgment", content);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "OPS");
        }
    }


    public static async Task AtualizarStatusPedido(Polling P)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            bool verificaSeExistePedido = db.parametrosdopedido.Any(x => x.Id == P.orderId);

            if (verificaSeExistePedido)
            {
                await Console.Out.WriteLineAsync("\nStatus Do pedido atualizado com sucesso\n");
                var pedido = db.parametrosdopedido.Where(x => x.Id == P.orderId).FirstOrDefault();
                pedido.Situacao = P.fullCode;
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao atualizar Status Pedido", "OPS");
        }
    }


    //Função que Insere o pediddo que vem no pulling no banco de dados

    public static async Task SetPedido(string? orderId, Polling P)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{P.orderId}";
        try
        {
            using var db = new ApplicationDbContext();
            bool verificaSeExistePedido = db.parametrosdopedido.Any(x => x.Id == P.orderId);

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            HttpResponseMessage response = await client.GetAsync(url);

            string? jsonContent = await response.Content.ReadAsStringAsync();
            PedidoCompleto? pedidoCompletoDeserialiado = JsonConvert.DeserializeObject<PedidoCompleto>(jsonContent);


            if (verificaSeExistePedido == false)
            {
                int insertNoSysMenuConta = 0;
                string? mesa = pedidoCompletoDeserialiado.takeout.takeoutDateTime == null ? "WEB" : "WEBB";
                ParametrosDoSistema? Configs = db.parametrosdosistema.ToList().FirstOrDefault();

                if (Configs.IntegracaoSysMenu)
                {

                    insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                       mesa: mesa,
                       cortesia: pedidoCompletoDeserialiado.total.benefits,
                       taxaEntrega: pedidoCompletoDeserialiado.total.deliveryFee,
                       taxaMotoboy: 0.00f,
                       dtInicio: pedidoCompletoDeserialiado.createdAt.Substring(0, 10),
                       hrInicio: pedidoCompletoDeserialiado.createdAt.Substring(11, 5),
                       contatoNome: pedidoCompletoDeserialiado.customer.name,
                       usuario: "CAIXA",
                       dataSaida: pedidoCompletoDeserialiado.createdAt.Substring(0, 10),
                       hrSaida: pedidoCompletoDeserialiado.createdAt.Substring(11, 5),
                       obsConta1: " ", obsConta2: " ",
                       endEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress,
                       bairEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood,
                       entregador: pedidoCompletoDeserialiado.delivery.deliveredBy == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveredBy); //fim dos parâmetros do método de integração

                    ClsDeIntegracaoSys.IntegracaoPagCartao(pedidoCompletoDeserialiado, insertNoSysMenuConta);

                    ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(pedidoCompletoDeserialiado.payments, insertNoSysMenuConta);
                }

                //serializar o polling para inserir no banco
                string jsonDoPolling = JsonConvert.SerializeObject(P);

                var pedidoInserido = db.parametrosdopedido.Add(new ParametrosDoPedido() { Id = P.orderId, Json = jsonContent, Situacao = P.fullCode, Conta = insertNoSysMenuConta, CriadoEm = DateTimeOffset.Now.ToString(), DisplayId = Convert.ToInt32(pedidoCompletoDeserialiado.displayId), JsonPolling = jsonDoPolling });
                db.SaveChanges();


                if (Configs.IntegracaoSysMenu)
                {
                    bool existeCliente = ClsDeIntegracaoSys.ProcuraCliente(pedidoCompletoDeserialiado.customer.phone.localizer);

                    if (!existeCliente && pedidoCompletoDeserialiado.orderType == "DELIVERY")
                    {
                        ClsDeIntegracaoSys.CadastraCliente(pedidoCompletoDeserialiado.customer, pedidoCompletoDeserialiado.delivery);
                    }

                    foreach (Items item in pedidoCompletoDeserialiado.items)
                    {
                        bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" ? true : false;

                        if (ePizza)
                        {
                            string obs = item.observations == null || item.observations == "" ? " " : item.observations.ToString();
                            string externalCode = " ";
                            string? NomeProduto = "";

                            string? ePizza1 = null;
                            string? ePizza2 = null;
                            string? ePizza3 = null;

                            string? obs1 = " ";
                            string? obs2 = " ";
                            string? obs3 = " ";
                            string? obs4 = " ";
                            string? obs5 = " ";
                            string? obs6 = " ";
                            string? obs7 = " ";
                            string? obs8 = " ";
                            string? obs9 = " ";
                            string? obs10 = " ";
                            string? obs11 = " ";
                            string? obs12 = " ";
                            string? obs13 = " ";
                            string? obs14 = " ";

                            foreach (var option in item.options)
                            {
                                if (!option.externalCode.Contains("m") && ePizza1 == null)
                                {
                                    ePizza1 = option.externalCode == "" ? " " : option.externalCode;
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        NomeProduto += ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                                    }
                                    else
                                    {
                                        NomeProduto += option.name;
                                    }
                                    continue;
                                }

                                if (!option.externalCode.Contains("m") && ePizza2 == null)
                                {
                                    ePizza2 = option.externalCode == "" ? " " : option.externalCode;
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                                    }
                                    else
                                    {
                                        NomeProduto += " / " + option.name;
                                    }
                                    continue;
                                }

                                if (!option.externalCode.Contains("m") && ePizza3 == null)
                                {
                                    ePizza3 = option.externalCode == "" ? " " : option.externalCode;
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                                    }
                                    else
                                    {
                                        NomeProduto += " / " + option.name;
                                    }
                                    continue;
                                }

                            }

                            foreach (var opcao in item.options)
                            {
                                if (obs1 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs1 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs1 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs2 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs2 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs2 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs3 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs3 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs3 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs4 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs4 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs4 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs5 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs5 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs5 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs6 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs6 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs6 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs7 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs7 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs7 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs8 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs8 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs8 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs9 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs9 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs9 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs10 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs10 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs10 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs11 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs11 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs11 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs12 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs12 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs12 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs13 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs13 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs13 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs14 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                    {
                                        obs14 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else if (opcao.externalCode.Contains("m"))
                                    {
                                        obs14 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                            }

                            ClsDeIntegracaoSys.IntegracaoContas(
                                       conta: insertNoSysMenuConta, //numero
                                       mesa: mesa, //texto curto 
                                       qtdade: item.quantity, //numero
                                       codCarda1: ePizza1 != null ? ePizza1 : externalCode, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                       codCarda2: ePizza2 != null ? ePizza2 : externalCode, //texto curto 4 letras
                                       codCarda3: ePizza3 != null ? ePizza3 : externalCode, //texto curto 4 letras
                                       tamanho: item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" ? item.externalCode : "U", ////texto curto 1 letra
                                       descarda: NomeProduto == "" ? item.name : NomeProduto, // texto curto 31 letras
                                       valorUnit: item.price, //moeda
                                       valorTotal: item.totalPrice, //moeda
                                       dataInicio: pedidoCompletoDeserialiado.createdAt.Substring(0, 10).Replace("-", "/"), //data
                                       horaInicio: pedidoCompletoDeserialiado.createdAt.Substring(11, 5), //data
                                       obs1: obs1,
                                       obs2: obs2,
                                       obs3: obs3,
                                       obs4: obs4,
                                       obs5: obs5,
                                       obs6: obs6,
                                       obs7: obs7,
                                       obs8: obs8,
                                       obs9: obs9,
                                       obs10: obs10,
                                       obs11: obs11,
                                       obs12: obs12,
                                       obs13: obs13,
                                       obs14: obs14,
                                       obs15: obs,
                                       cliente: pedidoCompletoDeserialiado.customer.name, // texto curto 80 letras
                                       telefone: mesa == "WEB" ? pedidoCompletoDeserialiado.customer.phone.localizer : pedidoCompletoDeserialiado.customer.name, // texto curto 14 letras
                                       impComanda: "Não",
                                       ImpComanda2: "Não",
                                       qtdComanda: 00f  //numero duplo 
                                  );//fim dos parâmetros
                        }
                        else
                        {
                            string? externalCode = item.externalCode == null || item.externalCode == "" ? " " : item.externalCode;
                            string? obs = item.observations == null || item.observations == "" ? " " : item.observations.ToString();
                            string? nomeProduto = null;

                            bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

                            if (existeProduto)
                            {
                                nomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                            }

                            string? obs1 = " ";
                            string? obs2 = " ";
                            string? obs3 = " ";
                            string? obs4 = " ";
                            string? obs5 = " ";
                            string? obs6 = " ";
                            string? obs7 = " ";
                            string? obs8 = " ";
                            string? obs9 = " ";
                            string? obs10 = " ";
                            string? obs11 = " ";
                            string? obs12 = " ";
                            string? obs13 = " ";
                            string? obs14 = " ";


                            foreach (var opcao in item.options)
                            {
                                if (obs1 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs1 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs1 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs2 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs2 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs2 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs3 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs3 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs3 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs4 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs4 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs4 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs5 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs5 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs5 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs6 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs6 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs6 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs7 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs7 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs7 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs8 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs8 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs8 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs9 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs9 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs9 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs10 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs10 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs10 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs11 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs11 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs11 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs12 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs12 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs12 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs13 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs13 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs13 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                                if (obs14 == " ")
                                {
                                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                    if (pesquisaProduto)
                                    {
                                        obs14 = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode);
                                    }
                                    else
                                    {
                                        obs14 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                    }

                                    continue;
                                }

                            }


                            ClsDeIntegracaoSys.IntegracaoContas(
                                       conta: insertNoSysMenuConta, //numero
                                       mesa: mesa, //texto curto 
                                       qtdade: item.quantity, //numero
                                       codCarda1: externalCode, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                       codCarda2: " ", //texto curto 4 letras
                                       codCarda3: " ",//texto curto 4 letras
                                       tamanho: item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" ? item.externalCode : "U", ////texto curto 1 letra
                                       descarda: nomeProduto != null ? nomeProduto : item.name, // texto curto 31 letras
                                       valorUnit: item.price, //moeda
                                       valorTotal: item.totalPrice, //moeda
                                       dataInicio: pedidoCompletoDeserialiado.createdAt.Substring(0, 10).Replace("-", "/"), //data
                                       horaInicio: pedidoCompletoDeserialiado.createdAt.Substring(11, 5), //data
                                       obs1: obs1,
                                       obs2: obs2,
                                       obs3: obs3,
                                       obs4: obs4,
                                       obs5: obs5,
                                       obs6: obs6,
                                       obs7: obs7,
                                       obs8: obs8,
                                       obs9: obs9,
                                       obs10: obs10,
                                       obs11: obs11,
                                       obs12: obs12,
                                       obs13: obs13,
                                       obs14: obs14,
                                       obs15: obs,
                                       cliente: pedidoCompletoDeserialiado.customer.name, // texto curto 80 letras
                                       telefone: mesa == "WEB" ? pedidoCompletoDeserialiado.customer.phone.localizer : " ", // texto curto 14 letras
                                       impComanda: "Não",
                                       ImpComanda2: "Não",
                                       qtdComanda: 00f  //numero duplo 
                                  );//fim dos parâmetros

                        }
                    }

                }

                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

                if (opSistema.ImpressaoAut && opSistema.AceitaPedidoAut)
                {
                    if (!opSistema.AgruparComandas)
                    {
                        foreach (string imp in impressoras)
                        {
                            if (imp != "Sem Impressora" && imp != null)
                            {
                                Impressao.ChamaImpressoes(insertNoSysMenuConta, Convert.ToInt32(pedidoCompletoDeserialiado.displayId), imp);
                            }
                        }
                    }
                    else
                    {
                        Impressao.ChamaImpressoesCasoSejaComandaSeparada(insertNoSysMenuConta, Convert.ToInt32(pedidoCompletoDeserialiado.displayId), impressoras);
                    }

                    impressoras.Clear();
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "ERRO AO INSERIR PEDIDO NO POSTGRES");
        }

    }

    //função que está retornando os pedidos para setar os pedidos no panel
    public static async Task<List<ParametrosDoPedido>> GetPedido(string? pedido_id = null)
    {
        List<ParametrosDoPedido> pedidosFromDb = new List<ParametrosDoPedido>();

        string path = CaminhoBaseSysMenu; //@"C:\Users\gui-c\OneDrive\Área de Trabalho\primeiro\testeSeriliazeJson.json";
        List<PedidoCompleto> pedidos = new List<PedidoCompleto>();
        try
        {
            if (pedido_id != null)
            {
                using ApplicationDbContext dataBase = new ApplicationDbContext();

                pedidosFromDb = dataBase.parametrosdopedido.Where(p => p.Id == pedido_id).ToList();
                //adicionar cada json em uma lista para poder deserializar nas funções

                return pedidosFromDb;
            }

            using ApplicationDbContext db = new ApplicationDbContext();

            pedidosFromDb = db.parametrosdopedido.ToList();
            //adicionar cada json em uma lista para poder deserializar nas funções

            return pedidosFromDb;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO AO GETPEDIDO");
        }

        return pedidosFromDb;
    }




    public static async void ConfirmarPedido(Polling P)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{P.orderId}/confirm";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            StringContent content = new StringContent("", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO AO CONFIRMAR PEDIDO");
        }
    }

    public static async void DespacharPedido(string? orderId)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{orderId}/dispatch"; ///orders/{id}/dispatch
        try
        {
            HttpResponseMessage resp = await EnviaReqParaOIfood(url, "POST", "");

            int statusCode = (int)resp.StatusCode;

            if (statusCode == 202)
            {
                MessageBox.Show("Pedido Despachado com sucesso!", "Despachado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                string? message = await resp.Content.ReadAsStringAsync();

                MessageBox.Show(message, "Ops");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static async void AvisoReadyToPickUp(string? orderId)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{orderId}/readyToPickup"; ///orders/{id}/dispatch
        try
        {
            HttpResponseMessage resp = await EnviaReqParaOIfood(url, "POST", "");

            int statusCode = (int)resp.StatusCode;

            if (statusCode == 202)
            {
                MessageBox.Show("Pedido Pronto Para Retirada avisado com sucesso!", "Pedido Pronto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                string? message = await resp.Content.ReadAsStringAsync();

                MessageBox.Show(message, "Ops");
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static async Task<List<ClsMotivosDeCancelamento>> CancelaPedidoOpcoes(string orderId)
    {
        List<ClsMotivosDeCancelamento>? motivosDeCancelamento = new();
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{orderId}/cancellationReasons";
        try
        {
            HttpResponseMessage response = await EnviaReqParaOIfood(url, "GET");
            int statusCode = (int)response.StatusCode;

            if (statusCode == 200)
            {
                string? jsonResponse = await response.Content.ReadAsStringAsync();
                motivosDeCancelamento = JsonConvert.DeserializeObject<List<ClsMotivosDeCancelamento>>(jsonResponse);


                return motivosDeCancelamento;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return motivosDeCancelamento;
    }

    public static async Task<int> CancelaPedido(string? orderId, string reason, string cancellationCode) //retorna o statuscode
    {
        int statusCode = 500;
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{orderId}/requestCancellation";
        try
        {
            ClsParaEnvioDeCancelamento codesParaCancelar = new ClsParaEnvioDeCancelamento() { reason = reason, cancellationCode = cancellationCode };
            string? content = JsonConvert.SerializeObject(codesParaCancelar);



            HttpResponseMessage response = await EnviaReqParaOIfood(url, "POST", content);
            statusCode = (int)response.StatusCode;

            if (statusCode == 202)
            {
                MessageBox.Show("Cancelamento Enviado com sucesso", "Tudo Certo!");
            }

            return statusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return statusCode;
    }

    public static async Task<string> GetStatusMerchant()
    {
        string validationState = "ERROR";
        try
        {
            Validation validations = new Validation();
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();
            string? merchantId = opSistema.MerchantId;

            if (merchantId != null)
            {
                string url = $"https://merchant-api.ifood.com.br/merchant/v1.0/merchants/{merchantId}/status";

                HttpResponseMessage response = await EnviaReqParaOIfood(url, "GET");
                string? jsonContent = await response.Content.ReadAsStringAsync();

                var deliveryStatus = JsonConvert.DeserializeObject<List<DeliveryStatus>>(jsonContent).FirstOrDefault();
                validations = deliveryStatus.Validations.FirstOrDefault();

            }
            return validations.State;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
        return validationState;
    }


    public static async Task<HttpResponseMessage> EnviaReqParaOIfood(string? url, string? metodo, string? content = "")
    {
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            if (metodo == "POST")
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
                StringContent contentToReq = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, contentToReq);

                return response;
            }

            if (metodo == "GET")
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);


                response = await client.GetAsync(url);

                return response;
            }

            if (metodo == "REFRESHTOKEN")
            {
                using HttpClient client = new HttpClient();
                using ApplicationDbContext db = new ApplicationDbContext();
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
                Token AutSist = db.parametrosdeautenticacao.ToList().FirstOrDefault();

                FormUrlEncodedContent formDataToGetTheToken = new FormUrlEncodedContent(new[]
                     {
                        new KeyValuePair<string, string>("grantType", "refresh_token"),
                        new KeyValuePair<string, string>("clientId", opcSistema.ClientId),
                        new KeyValuePair<string, string>("clientSecret", opcSistema.ClientSecret),
                        new KeyValuePair<string, string>("refreshToken", AutSist.refreshToken),

                });

                url = "https://merchant-api.ifood.com.br/authentication/v1.0/oauth/";

                response = await client.PostAsync($"{url}/token", formDataToGetTheToken);

                return response;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
        return response;
    }

    public static async void RefreshTokenIfood()
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            var AutenticacaoNaBase = db.parametrosdeautenticacao.ToList().FirstOrDefault();

            if (AutenticacaoNaBase != null)
            {
                DateTime dataHoraAtual = DateTime.Now;
                string DataDeVencimentoString = AutenticacaoNaBase.VenceEm;
                DateTime DataDeVencimento = DateTime.ParseExact(DataDeVencimentoString, "dd/MM/yyyy HH:mm:ss", null);

                if (dataHoraAtual >= DataDeVencimento)
                {
                    var RespostaDoRefreshTokenEndPoint = await Ifood.EnviaReqParaOIfood(" ", "REFRESHTOKEN", " ");

                    if (RespostaDoRefreshTokenEndPoint.IsSuccessStatusCode)
                    {
                        var repsonseWithToken = await RespostaDoRefreshTokenEndPoint.Content.ReadAsStringAsync();
                        Token propriedadesAPIWithToken = JsonConvert.DeserializeObject<Token>(repsonseWithToken);//JsonSerializer.Deserialize<Token>(repsonseWithToken);

                        DateTime horaAtual = DateTime.Now;
                        double milissegundosAdicionais = 21600;
                        DateTime horaFutura = horaAtual.AddSeconds(propriedadesAPIWithToken.expiresIn);
                        string HoraFormatada = horaFutura.ToString();

                        propriedadesAPIWithToken.VenceEm = HoraFormatada;
                        AutenticacaoNaBase.accessToken = propriedadesAPIWithToken.accessToken;
                        AutenticacaoNaBase.VenceEm = HoraFormatada;
                        db.SaveChanges();
                        Token.TokenDaSessao = AutenticacaoNaBase.accessToken;
                    }


                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

}
