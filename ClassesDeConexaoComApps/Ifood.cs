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
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            var reponse = await client.GetAsync($"{url}:polling");

            int statusCode = (int)reponse.StatusCode;
            if (statusCode == 200)
            {
                using ApplicationDbContext db = new ApplicationDbContext();
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.Where(x => x.Id == 1).FirstOrDefault();

                string jsonContent = await reponse.Content.ReadAsStringAsync();
                List<Polling>? pollings = JsonConvert.DeserializeObject<List<Polling>>(jsonContent); //pedidos nesse caso é o pulling 



                foreach (var P in pollings)
                {
                    switch (P.code)
                    {
                        case "PLC": //caso entre aqui é porque é um novo pedido
                            if (opcSistema.AceitaPedidoAut)
                            {
                                ClsSons.PlaySom();
                                await SetPedido(P.orderId, P);
                                await AvisarAcknowledge(P);
                                ConfirmarPedido(P);
                            }
                            break;
                        case "CFM":
                            ClsSons.StopSom();
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                        case "CAN":
                            //mandaria um pedido pro ifood cancelando 
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

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "ERRO NO PULLING");
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

                string? mesa = pedidoCompletoDeserialiado.takeout.takeoutDateTime == null ? "WEB" : "WEBB";


                int insertNoSysMenuConta = await IntegracaoSequencia(
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
                    obsConta1: "teste1", obsConta2: "Teste2",
                    endEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress,
                    bairEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood,
                    entregador: pedidoCompletoDeserialiado.delivery.deliveredBy == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveredBy); //fim dos parâmetros do método de integração

                IntegracaoPagCartao(pedidoCompletoDeserialiado, insertNoSysMenuConta);

                UpdateMeiosDePagamentosSequencia(pedidoCompletoDeserialiado.payments, insertNoSysMenuConta);

                db.parametrosdopedido.Add(new ParametrosDoPedido() { Id = P.orderId, Json = jsonContent, Situacao = P.fullCode, Conta = insertNoSysMenuConta });
                db.SaveChanges();

                bool existeCliente = ProcuraCliente(pedidoCompletoDeserialiado.customer.phone.localizer);

                if (!existeCliente && pedidoCompletoDeserialiado.orderType == "DELIVERY")
                {
                    CadastraCliente(pedidoCompletoDeserialiado.customer, pedidoCompletoDeserialiado.delivery);
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
                                ePizza1 = option.externalCode;
                                bool pesquisaProduto = PesquisaCodCardapio(option.externalCode);

                                if (pesquisaProduto)
                                {
                                    NomeProduto += NomeProdutoCardapio(option.externalCode);
                                }
                                continue;
                            }

                            if (!option.externalCode.Contains("m") && ePizza2 == null)
                            {
                                ePizza2 = option.externalCode;
                                bool pesquisaProduto = PesquisaCodCardapio(option.externalCode);

                                if (pesquisaProduto)
                                {
                                    NomeProduto += " / " + NomeProdutoCardapio(option.externalCode);
                                }
                                continue;
                            }

                            if (!option.externalCode.Contains("m") && ePizza3 == null)
                            {
                                ePizza3 = option.externalCode;
                                bool pesquisaProduto = PesquisaCodCardapio(option.externalCode);

                                if (pesquisaProduto)
                                {
                                    NomeProduto += " / " + NomeProdutoCardapio(option.externalCode);
                                }
                                continue;
                            }

                        }

                        foreach (var opcao in item.options)
                        {
                            if (obs1 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs1 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs1 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs2 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs2 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs2 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs3 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs3 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs3 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs4 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs4 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs4 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs5 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs5 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs5 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs6 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs6 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs6 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs7 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs7 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs7 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs8 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs8 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs8 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs9 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs9 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs9 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs10 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs10 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs10 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs11 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs11 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs11 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs12 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs12 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs12 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs13 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs13 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs13 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs14 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                {
                                    obs14 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs14 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                        }

                        IntegracaoContas(
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

                        bool existeProduto = PesquisaCodCardapio(externalCode);

                        if (existeProduto)
                        {
                            nomeProduto = NomeProdutoCardapio(externalCode);
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
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs1 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs1 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs2 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs2 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs2 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs3 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs3 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs3 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs4 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs4 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs4 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs5 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs5 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs5 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs6 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs6 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs6 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs7 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs7 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs7 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs8 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs8 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs8 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs9 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs9 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs9 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs10 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs10 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs10 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs11 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs11 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs11 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs12 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs12 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs12 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs13 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs13 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs13 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs14 == " ")
                            {
                                bool pesquisaProduto = PesquisaCodCardapio(opcao.externalCode);

                                if (pesquisaProduto)
                                {
                                    obs14 = NomeProdutoCardapio(opcao.externalCode);
                                }
                                else
                                {
                                    obs14 = $"{opcao.index} - {opcao.name} - {opcao.price.ToString("c")}";
                                }

                                continue;
                            }

                        }


                        IntegracaoContas(
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
                                   telefone: mesa == "WEB" ? pedidoCompletoDeserialiado.customer.phone.localizer : pedidoCompletoDeserialiado.customer.name, // texto curto 14 letras
                                   impComanda: "Não",
                                   ImpComanda2: "Não",
                                   qtdComanda: 00f  //numero duplo 
                              );//fim dos parâmetros


                    }


                    ParametrosDoSistema? opSistema = db.parametrosdosistema.Where(x => x.Id == 1).FirstOrDefault();
                    if (opSistema.ImpressaoAut)
                    {
                        Impressao.ChamaImpressoes(insertNoSysMenuConta, impressora1: opSistema.Impressora1);
                    }
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

    public static async Task<int> IntegracaoSequencia(string? mesa, //COMEÇO DOS PARÂMETROS DO MÉTODO
      float cortesia,
      float taxaEntrega,
      float taxaMotoboy,
      string? dtInicio,
      string? hrInicio,
      string? contatoNome,
      string? usuario,
      string? dataSaida,
      string? hrSaida,
      string? obsConta1,
      string? obsConta2 = null,
      string? endEntrega = "RETIRADA",
      string? bairEntrega = "RETIRADA",
      string? entregador = "RETIRADA"
      ) //método que está sendo usado para integrar a tabela contas do banco de dados com a tabela de pedido do SysIntegrador
    {

        int ultimoNumeroConta = 0;
        try
        {
            string banco = CaminhoBaseSysMenu; //@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            List<int> numerosSequencia = new List<int>();
            numerosSequencia.Clear();

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                string sqlInsert = $"INSERT INTO Sequencia (MESA, STATUS,CORTESIA ,TAXAENTREGA,TAXAMOTOBOY, DTINICIO, HRINICIO, ENDENTREGA, BAIENTREGA, CONTATO, ENTREGADOR, USUARIO, DTSAIDA, HRSAIDA, OBSCONTA1, OBSCONTA2) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    await Console.Out.WriteLineAsync("\nPedido Inserido no banco de dados do access Também\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@MESA", mesa);
                    command.Parameters.AddWithValue("@STATUS", "P");
                    command.Parameters.AddWithValue("@CORTESIA", cortesia);
                    command.Parameters.AddWithValue("@TAXAENTREGA", taxaEntrega);
                    command.Parameters.AddWithValue("@TAXAMOTOBOY", taxaMotoboy);
                    command.Parameters.AddWithValue("@DTINICIO", dtInicio.Replace("-", "/"));
                    command.Parameters.AddWithValue("@HRINICIO", hrInicio);
                    command.Parameters.AddWithValue("@ENDENTREGA", endEntrega); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@BAIENTREGA", bairEntrega);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@CONTATO", contatoNome);
                    command.Parameters.AddWithValue("@ENTREGADOR", entregador); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@USUARIO", usuario);
                    command.Parameters.AddWithValue("@DTSAIDA", dataSaida.Replace("-", "/"));
                    command.Parameters.AddWithValue("@HRSAIDA", hrSaida);
                    command.Parameters.AddWithValue("@OBSCONTA1", obsConta1);
                    command.Parameters.AddWithValue("@OBSCONTA2", obsConta2);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();


                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Inserção realizada com sucesso!");

                        //Se a inserção foi feita com sucesso, nós vamos pegar o número da sequencia
                        string sqlQuery = "SELECT * FROM Sequencia";

                        using (OleDbCommand comando = new OleDbCommand(sqlQuery, connection))
                        using (OleDbDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Pedidos:\n");
                                while (reader.Read())
                                {
                                    // Exibe o conteúdo de cada coluna na linha
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (reader.GetName(i) == "CONTA")
                                        {
                                            //se entrar aqui vai adicionar nos número da sequencia em um array 
                                            int valorConvertido = Convert.ToInt32(reader.GetValue(i));
                                            numerosSequencia.Add(valorConvertido);
                                        }
                                    }
                                }

                                //aqui retorna o ultimo número inserido
                                ultimoNumeroConta = numerosSequencia[numerosSequencia.Count() - 1];

                                return ultimoNumeroConta;
                            } // fechamento if hasRows
                        }   //fechamento terceiro using

                    } //fechamento de chave do if RowsAfected
                }//fechamento chave segundo using 
            }//fechamento chave primeiro using
        }// fechamento chave if
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "ERRO AO INSERIR O PEDIDO NO BANCO DE DADOS DO ACCESS");
        }

        return ultimoNumeroConta;
    }

    public static void UpdateMeiosDePagamentosSequencia(Payments pagamento, int numConta)
    {
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;


            foreach (Methods pagamentoAtual in pagamento.methods)
            {
                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    // Abrindo a conexão
                    connection.Open();
                    string? tipoPagamento = "PAGCRT";


                    switch (pagamentoAtual.method)
                    {
                        case "CREDIT":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "MEAL_VOUCHER":
                            tipoPagamento = "VOUCHER";
                            break;
                        case "DEBIT":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "PIX":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "CASH":
                            tipoPagamento = "PAGDNH";
                            break;
                        case "BANK_PAY ":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "FOOD_VOUCHER ":
                            tipoPagamento = "PAGCRT";
                            break;
                        default:
                            tipoPagamento = "PAGCRT";
                            break;
                    }


                    if (tipoPagamento == "PAGCRT")
                    {
                        string updateQuery = $"UPDATE Sequencia SET PAGCRT = @NovoValor WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;

                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@NovoValor", valor);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }

                    if (tipoPagamento == "PAGDNH")
                    {
                        string updateQuery = $"UPDATE Sequencia SET PAGDNH = @Valor, TROCO = @ValorTroco, TROCOENTREGA = @ValorTrocoEntrega WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;
                        double troco = 0.0;

                        if (pagamentoAtual.cash.changeFor > 0)
                        {
                            valor = pagamentoAtual.cash.changeFor;
                            troco = pagamentoAtual.cash.changeFor - pagamentoAtual.value;
                        }


                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@Valor", valor);
                            command.Parameters.AddWithValue("@ValorTroco", troco);
                            command.Parameters.AddWithValue("@ValorTrocoEntrega", troco);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }

                    if (tipoPagamento == "VOUCHER")
                    {
                        string updateQuery = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;

                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@NovoValor", valor);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }


                }
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro dentro do insere valores");
        }
    }

    public static void IntegracaoContas(int conta, //numero
        string? mesa, //texto curto 
        float qtdade, //numero
        string? codCarda1, //texto curto 4 letras
        string? codCarda2, // texto curto 4 letras
        string? codCarda3, //texto curto 4 letras
        string? tamanho,  //texto curto 1 letra
        string? descarda, // texto curto 31 letras
        float valorUnit, //moeda
        float valorTotal, // moeda
        string dataInicio, //data/hora
        string horaInicio,  //data/hora
        string? obs1,  //texto curto 80 letras
        string? obs2,  //texto curto 80 letras
        string? obs3,  //texto curto 80 letras
        string? obs4,    //  ||
        string? obs5,    //  ||
        string? obs6,    //  ||
        string? obs7,    //  ||
        string? obs8,    //  ||
        string? obs9,    //  ||
        string? obs10,    //  ||
        string? obs11,    //  ||
        string? obs12,    //  ||
        string? obs13,    //  ||
        string? obs14,    //  ||
        string? obs15,    //  ||
        string? cliente, // texto curto 40 letras
        string? telefone, // texto curto 14 letras
        string? impComanda, // texto curto 3 letras
        string? ImpComanda2, // texto curto 3 letras
        float qtdComanda, //numero duplo 
        string? usuario = "CAIXA"
        )
    { //aqui começa o código para inserção na tabela CONTAS
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                string sqlInsert = $"INSERT INTO Contas (CONTA,MESA,QTDADE,CODCARDA1,CODCARDA2,CODCARDA3,TAMANHO,DESCARDA,VALORUNIT,VALORTOTAL,DATAINICIO,HORAINICIO,OBS1,OBS2,OBS3,OBS4,OBS5,OBS6,OBS7,OBS8,OBS9,OBS10,OBS11,OBS12,OBS13,OBS14,OBS15,CLIENTE,STATUS,TELEFONE,IMPCOMANDA,IMPCOMANDA2,QTDCOMANDA,USUARIO ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@CONTA", conta);
                    command.Parameters.AddWithValue("@MESA", mesa);
                    command.Parameters.AddWithValue("@QTDADE", qtdade);
                    command.Parameters.AddWithValue("@CODCARDA1", codCarda1);
                    command.Parameters.AddWithValue("@CODCARDA2", codCarda2);
                    command.Parameters.AddWithValue("@CODCARDA3", codCarda3);
                    command.Parameters.AddWithValue("@TAMANHO", tamanho);
                    command.Parameters.AddWithValue("@DESCARDA", descarda);
                    command.Parameters.AddWithValue("@VALORUNIT", valorUnit); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@VALORTOTAL", valorTotal);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@DATAINICIO", dataInicio);
                    command.Parameters.AddWithValue("@HORAINICIO", horaInicio); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@OBS1", obs1);
                    command.Parameters.AddWithValue("@OBS2", obs2);
                    command.Parameters.AddWithValue("@OBS3", obs3);
                    command.Parameters.AddWithValue("@OBS4", obs4);
                    command.Parameters.AddWithValue("@OBS5", obs5);
                    command.Parameters.AddWithValue("@OBS6", obs6);
                    command.Parameters.AddWithValue("@OBS7", obs7);
                    command.Parameters.AddWithValue("@OBS8", obs8);
                    command.Parameters.AddWithValue("@OBS9", obs9);
                    command.Parameters.AddWithValue("@OBS10", obs10);
                    command.Parameters.AddWithValue("@OBS11", obs11);
                    command.Parameters.AddWithValue("@OBS12", obs12);
                    command.Parameters.AddWithValue("@OBS13", obs13);
                    command.Parameters.AddWithValue("@OBS14", obs14);
                    command.Parameters.AddWithValue("@OBS15", obs15);
                    command.Parameters.AddWithValue("@CLIENTE", cliente);
                    command.Parameters.AddWithValue("@STATUS", "P");
                    command.Parameters.AddWithValue("@TELEFONE", telefone);
                    command.Parameters.AddWithValue("@IMPCOMANDA", impComanda);
                    command.Parameters.AddWithValue("@IMPCOMANDA2", ImpComanda2);
                    command.Parameters.AddWithValue("@QTDCOMANDA", qtdComanda);
                    command.Parameters.AddWithValue("@USUARIO", usuario);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();

                }//fechamento chave segundo using 
            }//fechamento chave primeiro using
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public static void IntegracaoPagCartao(PedidoCompleto pedidoCompleto, int NumContas)
    {
        string? cartao = "";
        string? tipo = "";
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string? tipoPagamento = DefinePagamento(pedidoCompleto.payments.methods[0].method);


                string SqlSelectIntoCadastros = $"SELECT * FROM CARTAO WHERE NOME = {tipoPagamento}";


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@NOME", tipoPagamento);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cartao = reader["NOME"].ToString();
                            tipo = reader["TIPO"].ToString();
                        }
                    }
                }
            }

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                float valorPagamento = pedidoCompleto.payments.methods[0].value;

                string sqlInsert = $"INSERT INTO PagCartao (CONTA, CARTAO, TIPO, VALOR) VALUES (?,?,?,?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@CONTA", NumContas);
                    command.Parameters.AddWithValue("@CARTAO", cartao);
                    command.Parameters.AddWithValue("@TIPO", tipo);
                    command.Parameters.AddWithValue("@VALOr", valorPagamento);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();

                }//fechamento chave segundo using 
            }//fechamento chave primeiro using
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static bool ProcuraCliente(string? telefone)
    {
        bool existeCliente = false;
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string sqlSelect = "SELECT COUNT(*) FROM Clientes WHERE TELEFONE = @TELEFONE";


                using (OleDbCommand selectCommand = new OleDbCommand(sqlSelect, connection))
                {
                    selectCommand.Parameters.AddWithValue("@TELEFONE", telefone);

                    // Executar a consulta SELECT
                    int count = (int)selectCommand.ExecuteScalar();
                    existeCliente = count > 0;
                }
                return existeCliente;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Problema em procurar cliente");
        }
        return existeCliente;
    }

    public static void CadastraCliente(Customer cliente, Delivery entrega)
    {
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string SqlSelectIntoCadastros = "INSERT INTO Clientes (TELEFONE, NOME, ENDERECO, BAIRRO, CIDADE, ESTADO, CEP, REFERE) VALUES (?,?,?,?,?,?,?,?) ";


                using (OleDbCommand command = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    command.Parameters.AddWithValue("@TELEFONE", cliente.phone.localizer);
                    command.Parameters.AddWithValue("@NOME", cliente.name);
                    command.Parameters.AddWithValue("@ENDERECO", entrega.deliveryAddress.formattedAddress);
                    command.Parameters.AddWithValue("@BAIRRO", entrega.deliveryAddress.neighborhood);
                    command.Parameters.AddWithValue("@CIDADE", entrega.deliveryAddress.city);
                    command.Parameters.AddWithValue("@ESTADO", "SP");
                    command.Parameters.AddWithValue("@CEP", entrega.deliveryAddress.postalCode != null ? entrega.deliveryAddress.postalCode : " ");
                    command.Parameters.AddWithValue("@REFERE", entrega.deliveryAddress.reference != null ? entrega.deliveryAddress.reference : " ");


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Problema em procurar cliente");
        }
    }

    public static bool PesquisaCodCardapio(string? codPdv)
    {
        bool existeProduto = false;
        try
        {

            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string sqlSelect = "SELECT COUNT(*) FROM Cardapio WHERE CODIGO = @CODIGO";


                using (OleDbCommand selectCommand = new OleDbCommand(sqlSelect, connection))
                {
                    selectCommand.Parameters.AddWithValue("@CODIGO", codPdv);

                    // Executar a consulta SELECT
                    int count = (int)selectCommand.ExecuteScalar();
                    existeProduto = count > 0;
                }
                return existeProduto;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao pesquisar pelo codigo do cardapio");
        }
        return existeProduto;
    }

    public static string NomeProdutoCardapio(string codCardapio)
    {
        string? NomeProduto = null;
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string SqlSelectIntoCadastros = "SELECT * FROM Cardapio WHERE CODIGO = @CODIGO";


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@CODIGO", codCardapio);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NomeProduto = reader["DESCRICAO"].ToString();
                        }
                    }
                    return NomeProduto;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao definir nome do produto");
        }
        return NomeProduto;
    }

    public static string DefinePagamento(string? tipoPagamento) //define o nome do pagamento para inserir no pagCartão
    {
        string pagamento = "";

        switch (tipoPagamento)
        {
            case "CREDIT":
                pagamento = "Crédito";
                break;
            case "MEAL_VOUCHER":
                pagamento = "Débito";
                break;
            case "DEBIT":
                pagamento = "Débito";
                break;
            case "PIX":
                pagamento = "PIX";
                break;
            case "BANK_PAY ":
                pagamento = "Débito";
                break;
            case "FOOD_VOUCHER ":
                pagamento = "Débito";
                break;
            default:
                pagamento = "Débito";
                break;
        }

        return pagamento;
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
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();
            string? merchantId = opSistema.MerchantId;

            string url = $"https://merchant-api.ifood.com.br/merchant/v1.0/merchants/{merchantId}/status";

            HttpResponseMessage response = await EnviaReqParaOIfood(url, "GET");
            string? jsonContent = await response.Content.ReadAsStringAsync();

            DeliveryStatus deliveryStatus = JsonConvert.DeserializeObject<List<DeliveryStatus>>(jsonContent).FirstOrDefault();
            Validation validations = deliveryStatus.Validations.FirstOrDefault();

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
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
        return response;
    }
}
