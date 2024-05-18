using Entidades;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.data;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class DelMatch
{

    public async static Task PoolingDelMatch()
    {
        string url = "https://delmatchcardapio.com/api/orders.json";
        try
        {
            await RefreshTokenDelMatch();

            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? Configs = db.parametrosdosistema.ToList().FirstOrDefault();

            HttpResponseMessage response = await EnviaReqParaDelMatch(url, "GET");
            string reponseJson = await response.Content.ReadAsStringAsync();

            if (reponseJson != null || reponseJson != "")
            {
                if ((int)response.StatusCode == 200)
                {
                    List<PedidoDelMatch> pedidosRecebido = JsonConvert.DeserializeObject<List<PedidoDelMatch>>(reponseJson);

                    foreach (var pedido in pedidosRecebido)
                    {
                        string idPedido = pedido.Id.ToString();
                        bool ExistePedido = db.parametrosdopedido.Any(x => x.Id == idPedido);

                        if (!ExistePedido)
                        {
                            ClsSons.PlaySom();
                            await SetPedidoDelMatch(pedido);

                            if (Configs.AceitaPedidoAut)
                            {
                                await ConfirmaPedidoDelMatch(pedido);
                                ClsSons.StopSom();
                            }

                            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
                        }
                    }

                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static async Task ConfirmaPedidoDelMatch(PedidoDelMatch pedido)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{pedido.Reference.ToString()}/statuses/confirmation.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("Confirmado", pedido.Reference.ToString());

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static async Task AtualizarStatusPedido(string? Status, string? orderId)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            bool verificaSeExistePedido = db.parametrosdopedido.Any(x => x.Id == orderId);

            if (verificaSeExistePedido)
            {
                var pedido = db.parametrosdopedido.Where(x => x.Id == orderId).FirstOrDefault();
                pedido.Situacao = Status;
                db.SaveChanges();

                FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao atualizar Status Pedido", "OPS");
        }
    }

    public static async Task DispachaPedidoDelMatch(int reference)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{reference.ToString()}/statuses/dispatch.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("Despachado", reference.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static async Task MarcaEntregePedidoDelMatch(int reference)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{reference.ToString()}/statuses/delivered.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("Concluido", reference.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static async Task CancelaPedidoDelMatch(int reference)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{reference.ToString()}/statuses/cancellation.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("Cancelado", reference.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static async Task SetPedidoDelMatch(PedidoDelMatch pedido)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            string IdPedido = pedido.Id.ToString();
            string jsonContent = JsonConvert.SerializeObject(pedido);
            string statusPedido = "Pendente";
            int insertNoSysMenuConta = 0;
            int displayId = pedido.Reference;

            if (opSistema.IntegracaoSysMenu)
            {
                string mesa = pedido.Type == "TOGO" ? "WEBB" : "WEB";

                insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                   mesa: mesa,
                   cortesia: pedido.Discount,
                   taxaEntrega: pedido.deliveryFee,
                   taxaMotoboy: 0.00f,
                   dtInicio: pedido.CreatedAt.Substring(0, 10),
                   hrInicio: pedido.CreatedAt.ToString().Substring(11, 5),
                   contatoNome: pedido.Customer.Name,
                   usuario: "CAIXA",
                   dataSaida: pedido.CreatedAt.Substring(0, 10),
                   hrSaida: pedido.CreatedAt.Substring(11, 5),
                   obsConta1: " ",
                   iFoodPedidoID: pedido.Id.ToString(),
                   obsConta2: " ",
                   endEntrega: mesa == "WEBB" ? "RETIRADA" : pedido.deliveryAddress.StreetName + ", " + pedido.deliveryAddress.StreetNumber,
                   bairEntrega: mesa == "WEBB" ? "RETIRADA" : pedido.deliveryAddress.Neighboardhood,
                   entregador: mesa == "WEBB" ? "RETIRADA" : " "); //fim dos parâmetros do método de integração

                ClsDeIntegracaoSys.IntegracaoPagCartao(pedido.Payments[0].Code, insertNoSysMenuConta, pedido.Payments[0].Value);//por enquanto tudo esta caindo debito


                SysIntegradorApp.ClassesAuxiliares.Payments payments = new();

                foreach (var item in pedido.Payments)
                {
                    Cash SeForPagamentoEmDinherio = new Cash() { changeFor = item.CashChange };
                    payments.methods.Add(new Methods() { method = item.Code, value = item.Value, cash = SeForPagamentoEmDinherio });
                }


                ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(payments, insertNoSysMenuConta);
            }


            var pedidoInserido = db.parametrosdopedido.Add(new ParametrosDoPedido()
            {
                Id = IdPedido,
                Json = jsonContent,
                Situacao = statusPedido,
                Conta = insertNoSysMenuConta,
                CriadoEm = DateTimeOffset.Now.ToString(),
                DisplayId = displayId,
                JsonPolling = "Sem Polling ID",
                CriadoPor = "DELMATCH"
            });
            await db.SaveChangesAsync();



            if (opSistema.IntegracaoSysMenu)
            {

                bool existeCliente = ClsDeIntegracaoSys.ProcuraCliente(pedido.Customer.Phone);

                if (!existeCliente && pedido.Type == "DELIVERY")
                {
                    //  ClsDeIntegracaoSys.CadastraCliente(pedidoCompletoDeserialiado.customer, pedidoCompletoDeserialiado.delivery);
                }


                foreach (items item in pedido.Items)
                {
                    bool ePizza = item.ExternalCode == "G" || item.ExternalCode == "M" || item.ExternalCode == "P" || item.ExternalCode == "B" ? true : false;
                    string mesa = pedido.Type == "TOGO" ? "WEBB" : "WEB";

                    if (ePizza)
                    {
                        string obs = item.Observations == null || item.Observations == "" ? " " : item.Observations.ToString();
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

                        foreach (var option in item.SubItems)
                        {
                            if (!option.ExternalCode.Contains("m") && ePizza1 == null)
                            {
                                ePizza1 = option.ExternalCode == "" ? " " : option.ExternalCode;
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    NomeProduto += $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(option.ExternalCode)}";
                                }
                                else
                                {
                                    NomeProduto += $"{item.Quantity}X - {option.Name}";
                                }
                                continue;
                            }

                            if (!option.ExternalCode.Contains("m") && ePizza2 == null)
                            {
                                ePizza2 = option.ExternalCode == "" ? " " : option.ExternalCode;
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.ExternalCode);
                                }
                                else
                                {
                                    NomeProduto += " / " + option.Name;
                                }
                                continue;
                            }

                            if (!option.ExternalCode.Contains("m") && ePizza3 == null)
                            {
                                ePizza3 = option.ExternalCode == "" ? " " : option.ExternalCode;
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.ExternalCode);
                                }
                                else
                                {
                                    NomeProduto += " / " + option.Name;
                                }
                                continue;
                            }

                        }

                        foreach (var opcao in item.SubItems)
                        {
                            if (obs1 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs1 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs1 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs2 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs2 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs2 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs3 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs3 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs3 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs4 == " ")
                            {

                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs4 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs4 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs5 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs5 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs5 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs6 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs6 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs6 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs7 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs7 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs7 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs8 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs8 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs8 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs9 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs9 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs9 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs10 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs10 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs10 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs11 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs11 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs11 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs12 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs12 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs12 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs13 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs13 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs13 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs14 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                                {
                                    obs14 = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else if (opcao.ExternalCode.Contains("m"))
                                {
                                    obs14 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                        }

                        ClsDeIntegracaoSys.IntegracaoContas(
                                   conta: insertNoSysMenuConta, //numero
                                   mesa: mesa, //texto curto 
                                   qtdade: 1, //numero
                                   codCarda1: ePizza1 != null ? ePizza1 : externalCode, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                   codCarda2: ePizza2 != null ? ePizza2 : externalCode, //texto curto 4 letras
                                   codCarda3: ePizza3 != null ? ePizza3 : externalCode, //texto curto 4 letras
                                   tamanho: item.ExternalCode == "G" || item.ExternalCode == "M" || item.ExternalCode == "P" ? item.ExternalCode : "U", ////texto curto 1 letra
                                   descarda: NomeProduto == "" ? $"{item.Quantity}X - {item.Name}" : NomeProduto, // texto curto 31 letras
                                   valorUnit: (item.Price + item.SubItemsPrice) * item.Quantity, //moeda
                                   valorTotal: item.TotalPrice, //moeda
                                   dataInicio: pedido.CreatedAt.Substring(0, 10).Replace("-", "/"), //data
                                   horaInicio: pedido.CreatedAt.Substring(11, 5), //data
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
                                   cliente: pedido.Customer.Name, // texto curto 80 letras
                                   telefone: pedido.Customer.Phone == "" || pedido.Customer.Phone == null ? " " : pedido.Customer.Phone.Replace(" ", ""), // texto curto 14 letras
                                   impComanda: "Não",
                                   ImpComanda2: "Não",
                                   qtdComanda: 00f  //numero duplo 
                              );//fim dos parâmetros
                    }
                    else
                    {
                        string? externalCode = item.ExternalCode == null || item.ExternalCode == "" ? " " : item.ExternalCode;
                        string? obs = item.Observations == null || item.Observations == "" ? " " : item.Observations.ToString();
                        string? nomeProduto = null;

                        bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

                        if (existeProduto)
                        {
                            nomeProduto = $"{item.Quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode)}";
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


                        foreach (var opcao in item.SubItems)
                        {
                            if (obs1 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs1 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs1 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs2 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs2 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs2 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs3 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs3 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs3 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs4 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs4 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs4 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs5 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs5 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs5 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs6 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs6 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs6 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs7 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs7 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs7 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs8 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs8 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs8 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs9 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs9 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs9 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs10 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs10 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs10 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs11 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs11 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs11 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs12 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs12 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs12 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs13 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs13 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs13 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                            if (obs14 == " ")
                            {
                                bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                                if (pesquisaProduto)
                                {
                                    obs14 = $"{item.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                                }
                                else
                                {
                                    obs14 = $"{opcao.Quantity}X - {opcao.Name} - {opcao.Price.ToString("c")}";
                                }

                                continue;
                            }

                        }

                        ClsDeIntegracaoSys.IntegracaoContas(
                                   conta: insertNoSysMenuConta, //numero
                                   mesa: mesa, //texto curto 
                                   qtdade: 1, //numero
                                   codCarda1: externalCode, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                   codCarda2: " ", //texto curto 4 letras
                                   codCarda3: " ",//texto curto 4 letras
                                   tamanho: item.ExternalCode == "G" || item.ExternalCode == "M" || item.ExternalCode == "P" ? item.ExternalCode : "U", ////texto curto 1 letra
                                   descarda: nomeProduto != null ? nomeProduto : $"{item.Quantity}X - {item.Name}", // texto curto 31 letras
                                   valorUnit: (item.Price + item.SubItemsPrice) * item.Quantity, //moeda
                                   valorTotal: item.TotalPrice, //moeda
                                   dataInicio: pedido.CreatedAt.Substring(0, 10).Replace("-", "/"), //data
                                   horaInicio: pedido.CreatedAt.Substring(11, 5), //data
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
                                   cliente: pedido.Customer.Name, // texto curto 80 letras
                                   telefone: pedido.Customer.Phone == "" || pedido.Customer.Phone == null ? " " : pedido.Customer.Phone.Replace(" ", ""), // texto curto 14 letras
                                   impComanda: "Não",
                                   ImpComanda2: "Não",
                                   qtdComanda: 00f  //numero duplo 
                              );//fim dos parâmetros

                    }
                }
            }


            if (opSistema.ImpressaoAut)
            {
                List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

                if (!opSistema.AgruparComandas)
                {
                    foreach (string imp in impressoras)
                    {
                        if (imp != "Sem Impressora" && imp != null)
                        {
                            ImpressaoDelMatch.ChamaImpressoes(insertNoSysMenuConta, displayId, imp);
                        }
                    }
                }
                else
                {
                    ImpressaoDelMatch.ChamaImpressoesCasoSejaComandaSeparada(insertNoSysMenuConta, displayId, impressoras);
                }



                impressoras.Clear();
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static async Task<List<ParametrosDoPedido>> GetPedidoDelMatch(int? display_ID = null)
    {
        List<ParametrosDoPedido> pedidosFromDb = new List<ParametrosDoPedido>();

        List<PedidoCompleto> pedidos = new List<PedidoCompleto>();
        try
        {
            if (display_ID != null)
            {
                using ApplicationDbContext dataBase = new ApplicationDbContext();

                pedidosFromDb = dataBase.parametrosdopedido.Where(x => x.DisplayId == display_ID && x.CriadoPor == "DELMATCH" || x.Conta == display_ID && x.CriadoPor == "DELMATCH").ToList();


                return pedidosFromDb;
            }
            else
            {
                using ApplicationDbContext db = new ApplicationDbContext();

                pedidosFromDb = db.parametrosdopedido.Where(x => x.CriadoPor == "DELMATCH").ToList();

                return pedidosFromDb;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO AO GETPEDIDO");
        }

        return pedidosFromDb;
    }

    public static PedidoCompleto DelMatchPedidoCompleto(PedidoDelMatch p)
    {
        PedidoCompleto PedidoCompletoConvertido = new PedidoCompleto();
        try
        {

            PedidoCompletoConvertido.CriadoPor = "DELMATCH"; // Valor fixo de exemplo
            PedidoCompletoConvertido.JsonPolling = "{}"; // Valor fixo de exemplo
            PedidoCompletoConvertido.id = p.Id.ToString();
            PedidoCompletoConvertido.displayId = p.Reference.ToString();
            PedidoCompletoConvertido.createdAt = p.CreatedAt;
            PedidoCompletoConvertido.orderTiming = "IMEDIATE"; // Valor fixo de exemplo
            PedidoCompletoConvertido.orderType = p.Type;

            PedidoCompletoConvertido.takeout.mode = p.Takeout.Mode;
            PedidoCompletoConvertido.delivery.deliveredBy = "MERCHANT";
            PedidoCompletoConvertido.delivery.deliveryDateTime = p.CreatedAt;
            PedidoCompletoConvertido.delivery.observations = p.deliveryAddress.Reference;
            PedidoCompletoConvertido.delivery.pickupCode = "1254"; // Valor fixo de exemplo
            PedidoCompletoConvertido.delivery.deliveryAddress.streetName = p.deliveryAddress.StreetName;
            PedidoCompletoConvertido.delivery.deliveryAddress.streetNumber = p.deliveryAddress.StreetNumber;
            PedidoCompletoConvertido.delivery.deliveryAddress.formattedAddress = p.deliveryAddress.FormattedAddress;
            PedidoCompletoConvertido.delivery.deliveryAddress.neighborhood = p.deliveryAddress.Neighborhood;
            PedidoCompletoConvertido.delivery.deliveryAddress.complement = p.deliveryAddress.Complement;
            PedidoCompletoConvertido.delivery.deliveryAddress.postalCode = p.deliveryAddress.PostalCode;
            PedidoCompletoConvertido.delivery.deliveryAddress.city = p.deliveryAddress.City;
            PedidoCompletoConvertido.delivery.deliveryAddress.reference = p.deliveryAddress.Reference;
            PedidoCompletoConvertido.customer.id = p.Customer.Id;
            PedidoCompletoConvertido.customer.name = p.Customer.Name;
            PedidoCompletoConvertido.customer.documentNumber = p.Customer.CPF;
            PedidoCompletoConvertido.salesChannel = "DELMATCH"; // Valor fixo de exemplo
            PedidoCompletoConvertido.total.additionalFees = p.AdditionalFee;
            PedidoCompletoConvertido.total.subTotal = p.SubTotal;
            PedidoCompletoConvertido.total.deliveryFee = p.deliveryFee;
            PedidoCompletoConvertido.total.benefits = p.Discount;
            PedidoCompletoConvertido.total.orderAmount = p.TotalPrice;


            return PedidoCompletoConvertido;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
        return PedidoCompletoConvertido;
    }

    public static List<Sequencia> ListarPedidosAbertos() //método que serve para enviarmos um pedido
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            string entregador = "99";
            string SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND DelMatchId IS NULL";

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@ENTREGADOR", entregador);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string telefone = reader["TELEFONE"].ToString();
                            Sequencia sequencia = PesquisaClientesNoCadastro(telefone.Trim());

                            if (sequencia.DeliveryAddress.FormattedAddress != null)
                            {
                                sequencia.numConta = Convert.ToInt32(reader["CONTA"].ToString());
                                sequencia.ValorConta = Convert.ToDecimal(reader["ACRESCIMO"]) +
                                                       Convert.ToDecimal(reader["SERVICO"]) +
                                                       Convert.ToDecimal(reader["COUVERT"]) +
                                                       Convert.ToDecimal(reader["TAXAENTREGA"]) +
                                                       Convert.ToDecimal(reader["TAXAMOTOBOY"]) +
                                                       Convert.ToDecimal(reader["PAGDNH"]) +
                                                       Convert.ToDecimal(reader["PAGCHQ"]) +
                                                       Convert.ToDecimal(reader["PAGCRT"]) +
                                                       Convert.ToDecimal(reader["PAGTKT"]) +
                                                       Convert.ToDecimal(reader["PAGCVALE"]) +
                                                       Convert.ToDecimal(reader["PAGCC"]) +
                                                       Convert.ToDecimal(reader["PAGONLINE"]) +
                                                       Convert.ToDecimal(reader["VOUCHER"]) -
                                                       Convert.ToDecimal(reader["TROCO"]) -
                                                       Convert.ToDecimal(reader["CORTESIA"]);
                                string NumConta = sequencia.numConta.ToString();

                                var idPedido = NumConta.PadLeft(4, '0') + "-" + DateTime.Now.ToString().Substring(0, 10).Replace("-", "/");

                                sequencia.DelMatchId = idPedido.ToString();
                                sequencia.Id = "dd56e3a213da0d221091d3bc6a0e621071550b80";
                                sequencia.ShortReference = idPedido.ToString();
                                sequencia.CreatedAt = "";
                                sequencia.Type = "DELIVERY";
                                sequencia.TimeMax = "";

                                sequencia.Merchant.RestaurantId = opcSistema.DelMatchId;
                                sequencia.Merchant.Name = opcSistema.NomeFantasia;
                                sequencia.Merchant.Id = opcSistema.DelMatchId;
                                sequencia.Merchant.Unit = "62e91b20e390370012f9802e";

                                sequencias.Add(sequencia);
                            }

                        }


                    }

                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }

    public static List<Sequencia> ListarPedidosJaEnviados()
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            string entregador = "99";
            string SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND DelMatchId IS NOT NULL";

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@ENTREGADOR", entregador);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Sequencia sequencia = PesquisaClientesNoCadastro(reader["TELEFONE"].ToString());

                            if (sequencia.DeliveryAddress.FormattedAddress != null)
                            {

                                sequencia.numConta = Convert.ToInt32(reader["CONTA"].ToString());
                                sequencia.ValorConta = Convert.ToDecimal(reader["ACRESCIMO"]) +
                                                       Convert.ToDecimal(reader["SERVICO"]) +
                                                       Convert.ToDecimal(reader["COUVERT"]) +
                                                       Convert.ToDecimal(reader["TAXAENTREGA"]) +
                                                       Convert.ToDecimal(reader["TAXAMOTOBOY"]) +
                                                       Convert.ToDecimal(reader["PAGDNH"]) +
                                                       Convert.ToDecimal(reader["PAGCHQ"]) +
                                                       Convert.ToDecimal(reader["PAGCRT"]) +
                                                       Convert.ToDecimal(reader["PAGTKT"]) +
                                                       Convert.ToDecimal(reader["PAGCVALE"]) +
                                                       Convert.ToDecimal(reader["PAGCC"]) +
                                                       Convert.ToDecimal(reader["PAGONLINE"]) +
                                                       Convert.ToDecimal(reader["VOUCHER"]) -
                                                       Convert.ToDecimal(reader["TROCO"]) -
                                                       Convert.ToDecimal(reader["CORTESIA"]);
                                string NumConta = sequencia.numConta.ToString();

                                sequencia.ShortReference = NumConta.PadLeft(4, '0');
                                sequencia.DelMatchId = reader["DelMatchId"].ToString();

                                sequencias.Add(sequencia);
                            }
                        }

                    }

                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }

    public static async Task GerarPedido(string? jsonContent)
    {
        string? url = "https://delmatchapp.com/api/deliveries/default/";
        try
        {
            using HttpClient client = new HttpClient();
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string resposta = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static async Task<ClsDeserializacaoDelMatchEntrega> GetPedido(string? delMatchId)
    {
        ClsDeserializacaoDelMatchEntrega pedido = new ClsDeserializacaoDelMatchEntrega();

        string apiUrl = "https://delmatchapp.com/api/deliveries-list/";
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            var ConfigsSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            string token = ConfigsSistema.DelMatchId;

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

            var response = await client.GetAsync(apiUrl);

            string responseString = await response.Content.ReadAsStringAsync();

            string responseJson = await response.Content.ReadAsStringAsync();
            var pedidos = JsonConvert.DeserializeObject<List<ClsDeserializacaoDelMatchEntrega>>(responseJson);

            pedido = pedidos.Where(x => x.IdOrder == delMatchId).FirstOrDefault();

            if (pedido == null)
            {
                return new ClsDeserializacaoDelMatchEntrega() { Status = "Não Enviado" };
            }

            return pedido;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return pedido;
    }


    public static void UpdateDelMatchId(int numConta, string delmatchId)
    {
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            string updateQuery = "UPDATE Sequencia SET DelMatchId = @NovoValor WHERE CONTA = @CONDICAO;";


            string DelMatchId = delmatchId;

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                // Abrindo a conexão
                connection.Open();

                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    // Definindo os parâmetros para a instrução SQL
                    command.Parameters.AddWithValue("@NovoValor1", DelMatchId);
                    command.Parameters.AddWithValue("@CONDICAO", numConta);

                    // Executando o comando UPDATE
                    command.ExecuteNonQuery();
                }
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO NO UPDATE DELMATCHID");
        }
    }

    public static Sequencia PesquisaClientesNoCadastro(string? telefone)
    {
        Sequencia sequencia = new Sequencia();
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            string SqlSelectIntoCadastros = $"SELECT * FROM Clientes WHERE TRIM(TELEFONE) = '{telefone}'";

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@TELEFONE", telefone);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sequencia.Customer.Name = reader["NOME"].ToString();
                            sequencia.Customer.Phone = reader["TELEFONE"].ToString();
                            sequencia.Customer.TaxPayerIdentificationNumber = "CPF"; //falta terminar

                            sequencia.DeliveryAddress.FormattedAddress = reader["ENDERECO"].ToString();
                            sequencia.DeliveryAddress.Country = "BR";
                            sequencia.DeliveryAddress.State = reader["ESTADO"].ToString();
                            sequencia.DeliveryAddress.City = reader["CIDADE"].ToString();
                            sequencia.DeliveryAddress.Neighborhood = reader["BAIRRO"].ToString();
                            sequencia.DeliveryAddress.StreetName = reader["ENDERECO"].ToString();
                            sequencia.DeliveryAddress.StreetNumber = "";                        //falta terminar
                            sequencia.DeliveryAddress.PostalCode = reader["CEP"].ToString() == null ? " " : reader["CEP"].ToString();
                            sequencia.DeliveryAddress.Complement = reader["REFERE"].ToString();

                            sequencia.DeliveryAddress.Coordinates.Latitude = 0;
                            sequencia.DeliveryAddress.Coordinates.Longitude = 0;
                        }
                        return sequencia;
                    }

                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro Ao listar Cliente Cadastrado");
        }
        return sequencia;
    }


    public async static void EnviaPedidosAut()
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? Configuracoes = db.parametrosdosistema.ToList().FirstOrDefault();

            if (Configuracoes.EnviaPedidoAut)
            {
                List<Sequencia> pedidosAbertos = DelMatch.ListarPedidosAbertos();
                int contagemdepedidos = pedidosAbertos.Count;
                List<Sequencia> ItensAEnviarDelMach = FormDePedidosAbertos.ItensAEnviarDelMach;

                if (contagemdepedidos > 0)
                {
                    foreach (var item in pedidosAbertos)
                    {
                        ItensAEnviarDelMach.Add(item);
                    }

                }

                if (ItensAEnviarDelMach.Count() > 0)
                {

                    foreach (var item in ItensAEnviarDelMach)
                    {
                        string jsonContent = JsonConvert.SerializeObject(item);
                        await DelMatch.GerarPedido(jsonContent);
                        DelMatch.UpdateDelMatchId(item.numConta, item.ShortReference);
                    }

                    ItensAEnviarDelMach.Clear();
                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao enviar pedidos automatico para delmatch", "Ops");
        }
    }

    public static async Task GetToken()
    {
        string url = @"https://delmatchcardapio.com/api/oauth/token.json";
        try
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? Config = db.parametrosdosistema.ToList().FirstOrDefault();
            Token? Autentic = db.parametrosdeautenticacao.ToList().FirstOrDefault();

            string user = $"{Config.UserDelMatch}@delmatchcardapio.com";
            string pass = $"{Config.SenhaDelMatch}";

            var ValidacaoParaEnvio = new ClsParaPedirToken() { GrantType = "password", Username = user, Password = pass };
            string JsonBody = JsonConvert.SerializeObject(ValidacaoParaEnvio);

            HttpResponseMessage? responseDoEndPoint = await EnviaReqParaDelMatch(url, "GetToken", JsonBody);

            if (responseDoEndPoint.IsSuccessStatusCode)
            {
                string TokenObjectJson = await responseDoEndPoint.Content.ReadAsStringAsync();
                DateTime horario = DateTime.Now;
                string HorarioDeVencimento = horario.AddMinutes(50).ToString();

                TokenDelMatch? TokenObject = JsonConvert.DeserializeObject<TokenDelMatch>(TokenObjectJson);

                Autentic.TokenDelMatch = TokenObject.Token;
                Autentic.VenceEmDelMatch = HorarioDeVencimento;
                db.SaveChanges();

                TokenDelMatch.TokenDaSessao = TokenObject.Token;
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro Ao pegar o token Del Match", "Ops");
        }
    }

    public static async Task RefreshTokenDelMatch()
    {
        string url = @"https://delmatchcardapio.com/api/oauth/token.json";
        try
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? Config = db.parametrosdosistema.ToList().FirstOrDefault();
            Token? Autentic = db.parametrosdeautenticacao.ToList().FirstOrDefault();
            DateTime horario = DateTime.Now;

            DateTime HorarioDataBaseConvetido = DateTime.Parse(Autentic.VenceEmDelMatch);

            if (horario > HorarioDataBaseConvetido)
            {
                string user = $"{Config.UserDelMatch}@delmatchcardapio.com";
                string pass = $"{Config.SenhaDelMatch}";

                var ValidacaoParaEnvio = new ClsParaPedirToken() { GrantType = "password", Username = user, Password = pass };
                string JsonBody = JsonConvert.SerializeObject(ValidacaoParaEnvio);

                HttpResponseMessage? responseDoEndPoint = await EnviaReqParaDelMatch(url, "GetToken", JsonBody);

                if (responseDoEndPoint.IsSuccessStatusCode)
                {
                    string TokenObjectJson = await responseDoEndPoint.Content.ReadAsStringAsync();

                    string HorarioDeVencimento = horario.AddMinutes(50).ToString();

                    TokenDelMatch? TokenObject = JsonConvert.DeserializeObject<TokenDelMatch>(TokenObjectJson);

                    Autentic.TokenDelMatch = TokenObject.Token;
                    Autentic.VenceEmDelMatch = HorarioDeVencimento;
                    db.SaveChanges();

                    TokenDelMatch.TokenDaSessao = TokenObject.Token;
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro Ao pegar o token Del Match", "Ops");
        }
    }


    public static async Task<HttpResponseMessage> EnviaReqParaDelMatch(string? url, string? metodo, string? content = "")
    {
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            if (metodo == "GET")
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenDelMatch.TokenDaSessao);

                response = await client.GetAsync(url);

                return response;
            }

            if (metodo == "POST")
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenDelMatch.TokenDaSessao);

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, contentToPost);

                return response;

            }

            if (metodo == "GetToken")
            {
                using HttpClient client = new HttpClient();

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, contentToPost);

                return response;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao enviar Req DelMatch");
        }
        return response;
    }


}

