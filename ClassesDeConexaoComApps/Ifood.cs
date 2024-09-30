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
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using SysIntegradorApp.ClassesAuxiliares.Verificacoes;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;


namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class Ifood
{
    public readonly IMeuContexto _db;

    public Ifood(IMeuContexto DB)
    {
        _db = DB;
    }

    public string? CaminhoBaseSysMenu { get; set; } = ApplicationDbContext.RetornaCaminhoBaseSysMenu();

    public async Task Polling() //pulling feito de 30 em 30 Segundos, Caso seja encontrado algum novo pedido ele chama o GetPedidos
    {

        string url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
                var authBase = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                if (opcSistema.IntegracaoSysMenu)
                {
                    bool CaixaAberto = await ClsDeIntegracaoSys.VerificaCaixaAberto();

                    if (!CaixaAberto)
                    {
                        ClsSons.PlaySom2();
                        MessageBox.Show("Seu aplicativo está integrado com o SysMenu, abra o caixa para continuar", "Aplicativo Integrado");
                        ClsSons.StopSom();
                        Application.Exit();
                        return;
                    }
                }

                await RefreshTokenIfood();

                var reponse = await EnviaReqParaOIfood($"{url}:polling", "GET");

                int statusCode = (int)reponse.StatusCode;
                if (statusCode == 200)
                {
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
                                    await ConfirmarPedido(P);
                                    await AvisarAcknowledge(P);
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
                            case "CAR":
                                ClsSons.StopSom();
                                await AtualizarStatusPedido(P);
                                await AvisarAcknowledge(P);
                                ClsDeIntegracaoSys.ExcluiPedidoCasoCancelado(P.orderId);
                                break;
                            case "CON": //mudaria o status ou na tabela do sys menu
                                ClsSons.StopSom();
                                await AtualizarStatusPedido(P);
                                await AvisarAcknowledge(P);
                                break;
                            case "DDCR": //mudaria o status ou na tabela do sys menu
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "CAN": //mudaria o status ou na tabela do sys menu
                                ClsSons.StopSom();
                                await AtualizarStatusPedido(P);
                                await AvisarAcknowledge(P);
                                break;
                            case "DSP":
                                ClsSons.StopSom();
                                await AtualizarStatusPedido(P);
                                await AvisarAcknowledge(P);
                                break;
                            case "RDR":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "RDS":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "RTP":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "HSD":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "HSS":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "GTO":
                                ClsSons.StopSom();
                                await AtualizarStatusPedido(P);
                                await AvisarAcknowledge(P);
                                break;
                            case "AAD":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "DRGO":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                            case "DCR":
                                ClsSons.StopSom();
                                await AvisarAcknowledge(P);
                                break;
                        }
                    }

                }

                string statusMerchat = await GetStatusMerchant();

                if (statusMerchat == "OK")
                {
                    FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.MudaStatusMerchant()));
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            ClsSons.PlaySom2();
            MessageBox.Show("Por favor, verifique sua conexão com a internet. Ela pode estar oscilando ou desligada!", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ClsSons.StopSom();
        }

    }

    public async Task<bool> VerificaTokenValido()
    {
        bool TokenValido = false;
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                string url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
                var Token = db.parametrosdeautenticacao.FirstOrDefault();

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.accessToken);
                var reponse = await client.GetAsync($"{url}:polling");

                int statusCode = (int)reponse.StatusCode;

                if (statusCode == 401)
                {
                    TokenValido = false;
                }
                else
                {
                    TokenValido = true;
                }

                return TokenValido;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "OPS");
        }
        return TokenValido;
    }


    //função para avisar para o ifood o ACK
    public async Task AvisarAcknowledge(Polling polling)
    {
        string? url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
                var authBase = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authBase.accessToken);
                List<Polling> pollingList = new List<Polling>();
                pollingList.Add(polling);


                var polingToJson = JsonConvert.SerializeObject(pollingList);


                StringContent content = new StringContent(polingToJson, Encoding.UTF8, "application/json");

                await client.PostAsync($"{url}/acknowledgment", content);
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "OPS");
        }
    }


    public async Task AtualizarStatusPedido(Polling P)
    {
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                bool verificaSeExistePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == P.orderId);

                if (verificaSeExistePedido)
                {
                    await Console.Out.WriteLineAsync("\nStatus Do pedido atualizado com sucesso\n");
                    var pedido = db.parametrosdopedido.Where(x => x.Id == P.orderId).FirstOrDefault();

                    if (pedido.Situacao != P.fullCode)
                    {
                        pedido.Situacao = P.fullCode;
                        db.SaveChanges();
                        ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                        //FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
                    }

                }
            }
            //await db.DisposeAsync();
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao atualizar Status Pedido", "OPS");
        }
    }


    public async Task ChamaEntregador(string orderId)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{orderId}/requestDriver";
        try
        {
            HttpResponseMessage response = await EnviaReqParaOIfood(url, "POST");

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Erro ao solicitar entregador");
            }
            else
            {
                MessageBox.Show("Entregador chamado com sucesso!");
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "OPS");
        }
    }


    //Função que Insere o pediddo que vem no pulling no banco de dados

    public async Task SetPedido(string? orderId, Polling P)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{P.orderId}";
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.FirstOrDefault();
                var authBase = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                bool verificaSeExistePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == P.orderId);

                if (!verificaSeExistePedido)
                {
                    HttpResponseMessage response = await EnviaReqParaOIfood(url, "GET");

                    string? jsonContent = await response.Content.ReadAsStringAsync();
                    PedidoCompleto? pedidoCompletoDeserialiado = JsonConvert.DeserializeObject<PedidoCompleto>(jsonContent);


                    int insertNoSysMenuConta = 0;
                    string? mesa = pedidoCompletoDeserialiado.orderType == "DELIVERY" ? "WEB" : "WEBB";
                    ParametrosDoSistema? Configs = db.parametrosdosistema.ToList().FirstOrDefault();

                    if (Configs.IntegracaoSysMenu)
                    {
                        string? complementoDaEntrega = " ";

                        if (pedidoCompletoDeserialiado.delivery.deliveryAddress.complement != null && pedidoCompletoDeserialiado.delivery.deliveryAddress.complement.Length > 2)
                        {
                            complementoDaEntrega = pedidoCompletoDeserialiado.delivery.deliveryAddress.complement;
                        }

                        if(pedidoCompletoDeserialiado.customer.name.Length > 50)
                        {
                            pedidoCompletoDeserialiado.customer.name = pedidoCompletoDeserialiado.customer.name.Substring(0, 50);
                        }

                        DateTime DataCertaDaFeitoEmTimeStamp = DateTime.Parse(pedidoCompletoDeserialiado.createdAt);
                        DateTime DataCertaDaFeitoEm = DataCertaDaFeitoEmTimeStamp.ToLocalTime();

                        DateTime DataCertaEntregarEmTimeStamp = DateTime.Parse(pedidoCompletoDeserialiado.createdAt);
                        DateTime DataCertaEntregarEm = DataCertaEntregarEmTimeStamp.ToLocalTime();
                        DataCertaEntregarEm = DataCertaEntregarEm.AddMinutes(50);

                        insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                           mesa: mesa,
                           cortesia: pedidoCompletoDeserialiado.total.benefits,
                           taxaEntrega: pedidoCompletoDeserialiado.total.deliveryFee,
                           taxaMotoboy: pedidoCompletoDeserialiado.total.deliveryFee,
                           dtInicio: DataCertaDaFeitoEm.ToString().Substring(0, 10),
                           hrInicio: DataCertaDaFeitoEm.ToString().Substring(11, 5),
                           contatoNome: pedidoCompletoDeserialiado.customer.name,
                           usuario: "CAIXA",
                           dataSaida: DataCertaEntregarEm.ToString().Substring(0, 10),
                           hrSaida: DataCertaEntregarEm.ToString().Substring(11, 5),
                           obsConta1: " ",
                           iFoodPedidoID: pedidoCompletoDeserialiado.id,
                           obsConta2: " ",
                           referencia: complementoDaEntrega,
                           endEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress,
                           bairEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood,
                           entregador: pedidoCompletoDeserialiado.delivery.deliveredBy == null ? "RETIRADA" : "00",
                           eIfood: true); //fim dos parâmetros do método de integração

                        ClsDeIntegracaoSys.IntegracaoPagCartao(pedidoCompletoDeserialiado.payments.methods[0].method, insertNoSysMenuConta, pedidoCompletoDeserialiado.payments.methods[0].value, pedidoCompletoDeserialiado.payments.methods[0].type, "IFOOD");
                        ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(pedidoCompletoDeserialiado.payments, insertNoSysMenuConta, desconto: pedidoCompletoDeserialiado.total.benefits, acrecimo: pedidoCompletoDeserialiado.total.additionalFees, pedidoCompletoDeserialiado.benefits);
                    }

                    //serializar o polling para inserir no banco
                    string jsonDoPolling = JsonConvert.SerializeObject(P);

                    int DisplayId = Convert.ToInt32(pedidoCompletoDeserialiado.displayId) + Convert.ToInt32(pedidoCompletoDeserialiado.customer.phone.localizer);

                    var pedidoInserido = db.parametrosdopedido.Add(new ParametrosDoPedido()
                    {
                        Id = P.orderId,
                        Json = jsonContent,
                        Situacao = P.fullCode,
                        Conta = insertNoSysMenuConta,
                        CriadoEm = DateTimeOffset.Now.ToString(),
                        DisplayId = DisplayId,
                        JsonPolling = jsonDoPolling,
                        CriadoPor = "IFOOD",
                        PesquisaDisplayId = Convert.ToInt32(pedidoCompletoDeserialiado.displayId),
                        PesquisaNome = pedidoCompletoDeserialiado.customer.name
                    }
                    );

                    await db.SaveChangesAsync();
                    //await db.DisposeAsync();

                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                    ClsDeSuporteAtualizarPanel.MudouDataBasePedido = true;

                    // FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));

                    if (Configs.IntegracaoSysMenu)
                    {
                        bool existeCliente = ClsDeIntegracaoSys.ProcuraCliente(pedidoCompletoDeserialiado.customer.phone.localizer);

                        if (!existeCliente && pedidoCompletoDeserialiado.orderType == "DELIVERY")
                        {
                            ClsDeIntegracaoSys.CadastraCliente(pedidoCompletoDeserialiado.customer, pedidoCompletoDeserialiado.delivery);
                        }

                        foreach (Items item in pedidoCompletoDeserialiado.items)
                        {
                            bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B" ? true : false;

                            if (ePizza)
                            {
                                string obsDoItem = " ";
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
                                            NomeProduto += $"{item.quantity}X - {option.name}";
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
                                            obs1 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs1 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs2 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs2 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs2 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs3 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs3 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs3 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs4 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs4 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs4 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs5 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs5 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs5 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs6 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs6 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs6 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs7 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs7 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs7 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs8 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs8 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs8 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs9 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs9 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs9 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs10 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs10 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs10 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs11 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs11 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs11 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs12 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs12 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs12 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs13 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs13 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs13 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs14 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto && opcao.externalCode.Contains("m"))
                                        {
                                            obs14 = $"{item.quantity}X - {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else if (opcao.externalCode.Contains("m"))
                                        {
                                            obs14 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                }

                                //define as observações do item
                                if (item.observations != null && item.observations != "")
                                {
                                    if (item.observations.Length > 80)
                                    {
                                        obsDoItem = item.observations.Substring(0, 80);
                                    }
                                    else
                                    {
                                        obsDoItem = item.observations;

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
                                           descarda: NomeProduto == "" ? $"{item.quantity}X - {item.name}" : NomeProduto, // texto curto 31 letras
                                           valorUnit: item.totalPrice / item.quantity, //moeda
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
                                           obs15: obsDoItem,
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
                                string? obsDoItem = " ";
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
                                            obs1 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs1 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs2 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs2 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs2 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs3 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs3 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs3 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs4 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs4 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs4 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs5 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs5 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs5 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs6 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs6 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs6 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs7 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs7 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs7 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs8 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs8 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs8 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs9 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs9 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs9 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs10 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs10 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs10 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs11 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs11 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs11 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs12 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs12 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs12 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs13 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs13 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs13 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                    if (obs14 == " ")
                                    {
                                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                                        if (pesquisaProduto)
                                        {
                                            obs14 = $"{item.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                                        }
                                        else
                                        {
                                            obs14 = $"{opcao.quantity}X - {opcao.name} - {opcao.price.ToString("c")}";
                                        }

                                        continue;
                                    }

                                }

                                //define as observações do item
                                if (item.observations != null && item.observations != "")
                                {
                                    if (item.observations.Length > 80)
                                    {
                                        obsDoItem = item.observations.Substring(0, 80);
                                    }
                                    else
                                    {
                                        obsDoItem = item.observations;

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
                                           descarda: nomeProduto != null ? nomeProduto : $"{item.quantity}X - {item.name}", // texto curto 31 letras
                                           valorUnit: item.totalPrice / item.quantity, //moeda
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
                                           obs15: obsDoItem,
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
                                    Impressao.ChamaImpressoes(insertNoSysMenuConta, DisplayId, imp);
                                }
                            }
                        }
                        else
                        {
                            Impressao.ChamaImpressoesCasoSejaComandaSeparada(insertNoSysMenuConta, DisplayId, impressoras);
                        }

                        impressoras.Clear();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "ERRO AO INSERIR PEDIDO NO POSTGRES");
        }

    }

    //função que está retornando os pedidos para setar os pedidos no panel
    public async Task<List<ParametrosDoPedido>> GetPedido(int? display_ID = null, string? pesquisaNome = null)
    {
        List<ParametrosDoPedido> pedidosFromDb = new List<ParametrosDoPedido>();

        string path = CaminhoBaseSysMenu; //@"C:\Users\gui-c\OneDrive\Área de Trabalho\primeiro\testeSeriliazeJson.json";
        List<PedidoCompleto> pedidos = new List<PedidoCompleto>();
        try
        {


            if (display_ID != null || pesquisaNome != null)
            {
                if (display_ID != null)
                {
                    using (ApplicationDbContext db = await _db.GetContextoAsync())
                    {

                        pedidosFromDb = db.parametrosdopedido.Where(x => x.PesquisaDisplayId == display_ID && x.CriadoPor == "IFOOD" || x.Conta == display_ID && x.CriadoPor == "IFOOD").AsNoTracking().ToList();

                    }
                    return pedidosFromDb;
                }

                if (pesquisaNome != null)
                {

                    using (ApplicationDbContext db = await _db.GetContextoAsync())
                    {

                        pedidosFromDb = db.parametrosdopedido.Where(x => (x.PesquisaNome.ToLower().Contains(pesquisaNome) || x.PesquisaNome.Contains(pesquisaNome) || x.PesquisaNome.ToUpper().Contains(pesquisaNome)) && x.CriadoPor == "IFOOD").AsNoTracking().ToList();

                    }
                    return pedidosFromDb;
                }
            }
            else
            {
                using (ApplicationDbContext db = await _db.GetContextoAsync())
                {
                    pedidosFromDb = db.parametrosdopedido.Where(x => x.CriadoPor == "IFOOD").AsNoTracking().ToList();
                }

                return pedidosFromDb;
            }


        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "ERRO AO GETPEDIDO");
        }

        return pedidosFromDb;
    }


    public async Task ConfirmarPedido(Polling P)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{P.orderId}/confirm";
        try
        {
            HttpResponseMessage response = await EnviaReqParaOIfood(url, "POST", "");
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "ERRO AO CONFIRMAR PEDIDO");
        }
    }

    public async void DespacharPedido(string? orderId)
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
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public async void AvisoReadyToPickUp(string? orderId)
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
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public async Task<List<ClsMotivosDeCancelamento>> CancelaPedidoOpcoes(string orderId)
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
            await Logs.CriaLogDeErro(ex.ToString());
            Console.WriteLine(ex.Message);
        }
        return motivosDeCancelamento;
    }

    public async Task<int> CancelaPedido(string? orderId, string reason, string cancellationCode) //retorna o statuscode
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
            await Logs.CriaLogDeErro(ex.ToString());
            Console.WriteLine(ex.Message);
        }
        return statusCode;
    }

    public async Task<string> GetStatusMerchant()
    {
        string validationState = "ERROR";
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {

                Validation validations = new Validation();
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();
                string? merchantId = opSistema.MerchantId;

                if (merchantId != null)
                {
                    string url = $"https://merchant-api.ifood.com.br/merchant/v1.0/merchants/{merchantId}/status";

                    HttpResponseMessage response = await EnviaReqParaOIfood(url, "GET");
                    string? jsonContent = await response.Content.ReadAsStringAsync();

                    int statusCode = (int)response.StatusCode;

                    if (statusCode == 200)
                    {
                        var deliveryStatus = JsonConvert.DeserializeObject<List<DeliveryStatus>>(jsonContent).FirstOrDefault();
                        validations = deliveryStatus.Validations.FirstOrDefault();

                        return validations.State;
                    }

                    if (statusCode == 401)
                    {
                        ClsSons.PlaySom2();
                        MessageBox.Show("Login expirado, por favor refaça o login!", "Login Vencido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClsSons.StopSom();
                        Application.Exit();
                    }

                    if (statusCode != 200)
                    {
                        var message = JsonConvert.DeserializeObject<ClsDeserializacaoMessage>(jsonContent);
                        throw new Exception(message.message);
                    }

                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
        return validationState;
    }


    public async Task<HttpResponseMessage> EnviaReqParaOIfood(string? url, string? metodo, string? content = "")
    {
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {

                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();
                var Auth = db.parametrosdeautenticacao.FirstOrDefault();

                if (metodo == "POST")
                {
                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Auth.accessToken);
                    StringContent contentToReq = new StringContent(content, Encoding.UTF8, "application/json");

                    response = await client.PostAsync(url, contentToReq);

                    return response;
                }

                if (metodo == "GET")
                {
                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Auth.accessToken);


                    response = await client.GetAsync(url);

                    return response;
                }

                if (metodo == "REFRESHTOKEN")
                {
                    using HttpClient client = new HttpClient();
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

                //await db.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        return response;
    }

    public async Task RefreshTokenIfood()
    {
        try
        {
            using (ApplicationDbContext db = await _db.GetContextoAsync())
            {
                //using ApplicationDbContext db = new ApplicationDbContext();
                var AutenticacaoNaBase = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                if (AutenticacaoNaBase != null)
                {
                    DateTime dataHoraAtual = DateTime.Now;
                    string DataDeVencimentoString = AutenticacaoNaBase.VenceEm;
                    DateTime DataDeVencimento = DateTime.ParseExact(DataDeVencimentoString, "dd/MM/yyyy HH:mm:ss", null);

                    if (dataHoraAtual >= DataDeVencimento)
                    {
                        var RespostaDoRefreshTokenEndPoint = await EnviaReqParaOIfood(" ", "REFRESHTOKEN", " ");

                        if (RespostaDoRefreshTokenEndPoint.IsSuccessStatusCode)
                        {
                            var repsonseWithToken = await RespostaDoRefreshTokenEndPoint.Content.ReadAsStringAsync();
                            Token propriedadesAPIWithToken = JsonConvert.DeserializeObject<Token>(repsonseWithToken);

                            DateTime horaAtual = DateTime.Now;
                            DateTime horaFutura = horaAtual.AddSeconds(propriedadesAPIWithToken.expiresIn);
                            string HoraFormatada = horaFutura.ToString();

                            propriedadesAPIWithToken.VenceEm = HoraFormatada;
                            AutenticacaoNaBase.accessToken = propriedadesAPIWithToken.accessToken;
                            AutenticacaoNaBase.refreshToken = propriedadesAPIWithToken.refreshToken;
                            AutenticacaoNaBase.VenceEm = HoraFormatada;
                            await db.SaveChangesAsync();
                            Token.TokenDaSessao = AutenticacaoNaBase.accessToken;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public ApplicationDbContext GetContexto()
    {
        return new ApplicationDbContext();
    }
}
