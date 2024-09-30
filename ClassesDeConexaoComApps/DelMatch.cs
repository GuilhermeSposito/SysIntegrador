using Entidades;
using ExCSS;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.ClassesAuxiliares.Verificacoes;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
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
    public readonly IMeuContexto _Contxt;

    public DelMatch(IMeuContexto contxt)
    {
        _Contxt = contxt;
    }

    public async Task PoolingDelMatch()
    {
        string url = "https://delmatchcardapio.com/api/orders.json";
        try
        {
            bool verificaInternet = true; //await VerificaInternet.InternetAtiva();

            if (!verificaInternet)
            {
                throw new Exception("Por favor verifique sua conexão com a internet");
            }

            await RefreshTokenDelMatch();

            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
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
                            bool ExistePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == idPedido);

                            if (!ExistePedido)
                            {
                                ClsSons.PlaySom();
                                await SetPedidoDelMatch(pedido);

                                if (Configs.AceitaPedidoAut)
                                {
                                    await ConfirmaPedidoDelMatch(pedido);
                                    ClsSons.StopSom();
                                }
                            }

                            if (ExistePedido && pedido.Type == "INDOOR")
                            {
                                ClsSons.PlaySom();
                                await SetPedidoDelMatch(pedido, true);
                            }
                        }

                    }

                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            ClsSons.PlaySom2();
            MessageBox.Show(ex.ToString(), "Ops", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ClsSons.StopSom();
        }
    }

    public async Task ConfirmaPedidoDelMatch(PedidoDelMatch pedido)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{pedido.Reference.ToString()}/statuses/confirmation.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("CONFIRMED", pedido.Reference.ToString());

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
    }

    public async Task LimparPedidosDelMatch()
    {
        string url = "https://delmatchcardapio.com/api/orders.json";
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
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
                            var dataPedido = DateTime.Parse(pedido.CreatedAt);

                            if (dataPedido != DateTime.Now)
                            {
                                await ConfirmaPedidoDelMatch(pedido);
                            }

                        }

                    }

                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao limpar pedidos", "Ops");
        }
    }

    public async Task AtualizarStatusPedido(string? Status, string? orderId)
    {
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
                bool verificaSeExistePedido = db.parametrosdopedido.Any(x => x.Id == orderId);

                if (verificaSeExistePedido)
                {
                    var pedido = db.parametrosdopedido.Where(x => x.Id == orderId).FirstOrDefault();
                    pedido.Situacao = Status;
                    db.SaveChanges();

                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                    //FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao atualizar Status Pedido", "OPS");
        }
    }

    public async Task DispachaPedidoDelMatch(int reference)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{reference.ToString()}/statuses/dispatch.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("DISPATCHED", reference.ToString());
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
    }

    public async Task MarcaEntregePedidoDelMatch(int reference)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{reference.ToString()}/statuses/delivered.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("CONCLUDED", reference.ToString());
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
    }

    public async Task CancelaPedidoDelMatch(int reference)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{reference.ToString()}/statuses/cancellation.json";
        try
        {
            await EnviaReqParaDelMatch(url, "POST");
            await AtualizarStatusPedido("CANCELLED", reference.ToString());
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
    }

    public async Task FechaMesa()
    {
        try
        {
            bool ProcuraMesaFechada = ClsDeIntegracaoSys.ProcuraMesaFechada();

            if (ProcuraMesaFechada)
            {
                ClsApoioFechamanetoDeMesa MesasFechadas = ClsDeIntegracaoSys.MesasFechadas();

                foreach (var item in MesasFechadas.Mesas)
                {
                    string url = $"https://delmatchcardapio.com/api/orders/{item.PedidoID}/statuses/finish.json";

                    await EnviaReqParaDelMatch(url, "POST");

                    await AtualizarStatusPedido("CONCLUDED", item.PedidoID);
                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
    }

    public async Task SetPedidoDelMatch(PedidoDelMatch pedido, bool pedidoMesa = false)
    {
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                string IdPedido = pedido.Id.ToString();
                string jsonContent = JsonConvert.SerializeObject(pedido);
                string statusPedido = "Pendente";
                int insertNoSysMenuConta = 0;
                int displayId = pedido.Reference;
                string? mesa = " ";
                string Status = " ";
                string telefone = " ";

                if (pedido.Customer.Phone != null && pedido.Customer.Phone.Length > 0)
                {
                    telefone = pedido.Customer.Phone;
                }


                string? ComplementoDaEntrega = pedido.deliveryAddress.Complement;

                if (String.IsNullOrEmpty(ComplementoDaEntrega))
                {
                    ComplementoDaEntrega = " ";
                }

                if (opSistema.IntegracaoSysMenu)
                {
                    if (pedido.Type == "DELIVERY")
                    {
                        mesa = "WEB";
                        Status = "P";
                    }

                    if (pedido.Type == "TOGO")
                    {
                        mesa = "WEBB";
                        Status = "P";
                    }

                    if (pedido.Type == "INDOOR")
                    {
                        mesa = pedido.Indoor.table.PadLeft(4, '0');
                        Status = "A";
                    }

                    if (pedido.Type != "INDOOR")
                    {
                        if (pedido.Customer.Name.Length > 50)
                        {
                            pedido.Customer.Name = pedido.Customer.Name.Substring(0, 50);
                        }

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
                           referencia: ComplementoDaEntrega,
                           endEntrega: mesa == "WEBB" ? "RETIRADA" : pedido.deliveryAddress.StreetName + ", " + pedido.deliveryAddress.StreetNumber,
                           bairEntrega: mesa == "WEBB" ? "RETIRADA" : pedido.deliveryAddress.Neighboardhood,
                           entregador: mesa == "WEBB" ? "RETIRADA" : "99",
                           eDelMatch: true
                           );//mesa == "WEBB" ? "RETIRADA" : " ") ; //fim dos parâmetros do método de integração

                        ClsDeIntegracaoSys.IntegracaoPagCartao(pedido.Payments[0].Code, insertNoSysMenuConta, pedido.Payments[0].Value, pedido.Payments[0].Code, "DELMATCH");//por enquanto tudo esta caindo debito


                        SysIntegradorApp.ClassesAuxiliares.Payments payments = new();

                        foreach (var item in pedido.Payments)
                        {
                            Cash SeForPagamentoEmDinherio = new Cash() { changeFor = item.CashChange };
                            payments.methods.Add(new Methods() { method = item.Code, value = item.Value, cash = SeForPagamentoEmDinherio });
                        }


                        ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(payments, insertNoSysMenuConta);
                    }
                }

                if (pedido.Type == "INDOOR")
                {
                    insertNoSysMenuConta = 999;
                }

                if (!pedidoMesa)
                {
                    if (pedido.Type != "INDOOR") //entra aqui caso não seja mesa
                    {
                        var pedidoInserido = db.parametrosdopedido.Add(new ParametrosDoPedido()
                        {
                            Id = IdPedido,
                            Json = jsonContent,
                            Situacao = statusPedido,
                            Conta = insertNoSysMenuConta,
                            CriadoEm = DateTimeOffset.Now.ToString(),
                            DisplayId = displayId,
                            JsonPolling = "Sem Polling ID",
                            CriadoPor = "DELMATCH",
                            PesquisaDisplayId = displayId,
                            PesquisaNome = pedido.Customer.Name
                        });
                        await db.SaveChangesAsync();
                    }

                    if (pedido.Type == "INDOOR")
                    {
                        string? urlLimpaPedido = $"https://delmatchcardapio.com/api/orders/{pedido.Reference.ToString()}/statuses/confirmation.json";
                        ClsParaConfirmarItem confirmarItens = new ClsParaConfirmarItem();

                        var pedidoInserido = db.parametrosdopedido.Add(new ParametrosDoPedido()
                        {
                            Id = IdPedido,
                            Json = jsonContent,
                            Situacao = statusPedido,
                            Conta = insertNoSysMenuConta,
                            CriadoEm = DateTimeOffset.Now.ToString(),
                            DisplayId = displayId,
                            JsonPolling = "Sem Polling ID",
                            CriadoPor = "DELMATCH",
                            PesquisaDisplayId = displayId,
                            PesquisaNome = pedido.Customer.Name
                        });
                        await db.SaveChangesAsync();

                        foreach (var item in pedido.Items)
                        {
                            if (!item.Is_Read)
                            {
                                confirmarItens.Itens.Add(item.Item_Id);

                            }
                        }

                        if (confirmarItens.Itens.Count() > 0)
                        {
                            await EnviaReqParaDelMatch(urlLimpaPedido, "POST", JsonConvert.SerializeObject(confirmarItens));
                        }
                    }
                }

                ClsDeSuporteAtualizarPanel.MudouDataBasePedido = true;
                ClsDeSuporteAtualizarPanel.MudouDataBase = true;


                if (opSistema.IntegracaoSysMenu)
                {

                    bool existeCliente = pedido.Customer.Phone != null ? ClsDeIntegracaoSys.ProcuraCliente(pedido.Customer.Phone) : false;
                    bool pedidoOnLineMesa = false;
                    string idPedido = " ";


                    if (!existeCliente && pedido.Type == "DELIVERY")
                    {
                        //  ClsDeIntegracaoSys.CadastraCliente(pedidoCompletoDeserialiado.customer, pedidoCompletoDeserialiado.delivery);
                    }

                    if (pedido.Type == "INDOOR")
                    {
                        insertNoSysMenuConta = 0;
                        pedidoOnLineMesa = true;
                        idPedido = pedido.Reference.ToString();
                    }


                    if (!pedidoMesa)
                    {
                        foreach (items item in pedido.Items)
                        {
                            var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemDelMatch(item);

                            ClsDeIntegracaoSys.IntegracaoContas(
                                             conta: insertNoSysMenuConta, //numero
                                             mesa: mesa, //texto curto 
                                             qtdade: 1, //numero
                                             codCarda1: CaracteristicasPedido.ExternalCode1, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                             codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                             codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                             tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                             descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                             valorUnit: item.TotalPrice, //moeda
                                             valorTotal: item.TotalPrice, //moeda
                                             dataInicio: pedido.CreatedAt.Substring(0, 10).Replace("-", "/"), //data
                                             horaInicio: pedido.CreatedAt.Substring(11, 5), //data
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
                                             cliente: pedido.Customer.Name, // texto curto 80 letras
                                             telefone: pedido.Customer.Phone != null && pedido.Customer.Phone != "" ? pedido.Customer.Phone : " ", // texto curto 14 letras
                                             impComanda: "Não",
                                             ImpComanda2: "Não",
                                             qtdComanda: 00f,//numero duplo 
                                             status: Status,
                                             pedidoOnLineMesa: pedidoOnLineMesa,
                                             idPedido: idPedido
                                        );//fim dos parâmetros
                        }
                    }

                    if (pedidoMesa)
                    {
                        string? urlLimpaPedido = $"https://delmatchcardapio.com/api/orders/{pedido.Reference.ToString()}/statuses/confirmation.json";
                        ParametrosDoPedido? pedidoJaExistenteDB = db.parametrosdopedido.Where(x => x.Id == pedido.Id.ToString()).FirstOrDefault();
                        PedidoDelMatch? pedidoJaExistente = JsonConvert.DeserializeObject<PedidoDelMatch>(pedidoJaExistenteDB.Json);
                        ClsParaConfirmarItem confirmarItens = new ClsParaConfirmarItem();

                        foreach (var item in pedido.Items)
                        {
                            if (!item.Is_Read)
                            {
                                var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemDelMatch(item);

                                ClsDeIntegracaoSys.IntegracaoContas(
                                                 conta: insertNoSysMenuConta, //numero
                                                 mesa: mesa, //texto curto 
                                                 qtdade: 1, //numero
                                                 codCarda1: CaracteristicasPedido.ExternalCode1, //texto curto 4 letras
                                                 codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                                 codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                                 tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                                 descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                                 valorUnit: item.TotalPrice, //moeda
                                                 valorTotal: item.TotalPrice, //moeda
                                                 dataInicio: pedido.CreatedAt.Substring(0, 10).Replace("-", "/"), //data
                                                 horaInicio: pedido.CreatedAt.Substring(11, 5), //data
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
                                                 obs15: item.Observations != null && item.Observations.Length > 0 ? item.Observations : " ",
                                                 cliente: pedido.Customer.Name, // texto curto 80 letras
                                                 telefone: telefone,
                                                 impComanda: "Não",
                                                 ImpComanda2: "Não",
                                                 qtdComanda: 00f,//numero duplo 
                                                 status: Status,
                                                 pedidoOnLineMesa: true,
                                                 idPedido: pedido.Reference.ToString()
                                            );//fim dos parâmetros

                                confirmarItens.Itens.Add(item.Item_Id);

                            }
                        }

                        if (confirmarItens.Itens.Count() > 0)
                        {
                            await EnviaReqParaDelMatch(urlLimpaPedido, "POST", JsonConvert.SerializeObject(confirmarItens));
                        }


                        pedidoJaExistenteDB.Json = JsonConvert.SerializeObject(pedido);
                        await db.SaveChangesAsync();
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
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
    }

    public async Task<List<ParametrosDoPedido>> GetPedidoDelMatch(int? display_ID = null, string? pesquisaNome = null)
    {
        List<ParametrosDoPedido> pedidosFromDb = new List<ParametrosDoPedido>();

        List<PedidoCompleto> pedidos = new List<PedidoCompleto>();
        try
        {
            if (display_ID != null || pesquisaNome != null)
            {
                if (display_ID != null)
                {
                    using (ApplicationDbContext dataBase = await _Contxt.GetContextoAsync())
                    {

                        pedidosFromDb = dataBase.parametrosdopedido.Where(x => x.DisplayId == display_ID && x.CriadoPor == "DELMATCH" || x.Conta == display_ID && x.CriadoPor == "DELMATCH").AsNoTracking().ToList();


                    }
                    return pedidosFromDb;
                }

                if(pesquisaNome != null)
                {
                    using (ApplicationDbContext dataBase = await _Contxt.GetContextoAsync())
                    {

                        pedidosFromDb = dataBase.parametrosdopedido.Where(x => (x.PesquisaNome.ToLower().Contains(pesquisaNome) || x.PesquisaNome.Contains(pesquisaNome) || x.PesquisaNome.ToUpper().Contains(pesquisaNome)) && x.CriadoPor == "DELMATCH" ).AsNoTracking().ToList();


                    }
                    return pedidosFromDb;
                }

            }
            else
            {
                using (ApplicationDbContext dataBase = await _Contxt.GetContextoAsync())
                {

                    pedidosFromDb = dataBase.parametrosdopedido.Where(x => x.CriadoPor == "DELMATCH").AsNoTracking().ToList();

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

    public async Task<PedidoCompleto> DelMatchPedidoCompleto(PedidoDelMatch p)
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
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
        return PedidoCompletoConvertido;
    }

    public async Task<List<Sequencia>> ListarPedidosAbertos() //método que serve para enviarmos um pedido
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Contxt.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

                string entregador = "99";
                string SqlSelectIntoCadastros = "SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND DelMatchId IS NULL AND (MESA = 'WEB' OR MESA LIKE '%E%');";  //$"SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND DelMatchId IS NULL AND MESA = WEB";

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
                                Sequencia sequencia = await PesquisaEnderecoDeEntrega(reader["CONTA"].ToString(), "ENVIARPEDIDO"); //PesquisaClientesNoCadastro(telefone.Trim());

                                if (sequencia.DeliveryAddress.FormattedAddress == null || sequencia.DeliveryAddress.FormattedAddress.Length <= 3)
                                {
                                    sequencia = await PesquisaClientesNoCadastro(telefone.Trim());
                                }

                                sequencia.numConta = Convert.ToInt32(reader["CONTA"].ToString());

                                if (sequencia.DeliveryAddress.FormattedAddress != null)
                                {
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
                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }

    public async Task<List<Sequencia>> ListarPedidosJaEnviados()
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Contxt.GetContextoAsync())
            {
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
                                string telefone = reader["TELEFONE"].ToString();
                                Sequencia sequencia = await PesquisaEnderecoDeEntrega(reader["CONTA"].ToString(), "GETPEDIDO"); //PesquisaClientesNoCadastro(telefone.Trim());

                                if (sequencia.DeliveryAddress.FormattedAddress == null || sequencia.DeliveryAddress.FormattedAddress.Length <= 3)
                                {
                                    sequencia = await PesquisaClientesNoCadastro(telefone.Trim());
                                }

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
                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }

    public async Task GerarPedido(string? jsonContent)
    {
        string? url = "https://delmatchapp.com/api/deliveries/default/";
        try
        {
            using HttpClient client = new HttpClient();
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string resposta = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode != 200)
            {
                string? jsonContentFromApi = await response.Content.ReadAsStringAsync();

                ClsDeserializacaoPedidoFalho? reposta = JsonConvert.DeserializeObject<ClsDeserializacaoPedidoFalho>(jsonContentFromApi);

                string? Titulo = reposta.Success ? "Sucesso Ao enviar pedido" : "Erro ao enviar pedido";
                string Erro = "";


                Erro += reposta.Response;


                Sequencia? Pedido = JsonConvert.DeserializeObject<Sequencia>(jsonContent);

                ClsSuporteDePedidoNaoEnviadoDelmatch.ErroDeEnvioDePedido = true;
                ClsSuporteDePedidoNaoEnviadoDelmatch.PedidosQueNaoForamEnviados.Add(Pedido.ShortReference);
                ClsSuporteDePedidoNaoEnviadoDelmatch.MotivosDeNaoTerEnviado.Add($"\n{Erro}\n");

                ClsSons.PlaySom2();
                await Logs.CriaLogDeErro(await response.Content.ReadAsStringAsync());
                ClsSons.StopSom();
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            Console.WriteLine(ex.Message, "Ops");
        }
    }

    public async Task<HttpResponseMessage> GerarPedidoManual(string? jsonContent)
    {
        string? url = "https://delmatchapp.com/api/deliveries/default/";
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            using HttpClient client = new HttpClient();
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            response = await client.PostAsync(url, content);
            string resposta = await response.Content.ReadAsStringAsync();

            return response;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            Console.WriteLine(ex.Message, "Ops");
        }
        return response;
    }


    public async Task<ClsDeserializacaoDelMatchEntrega> GetPedido(string? delMatchId)
    {
        ClsDeserializacaoDelMatchEntrega pedido = new ClsDeserializacaoDelMatchEntrega();

        string apiUrl = "https://delmatchapp.com/api/deliveries-list/";
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
                var ConfigsSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                string token = ConfigsSistema.DelMatchId;

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

                var response = await client.GetAsync(apiUrl);

                string responseString = await response.Content.ReadAsStringAsync();

                string responseJson = await response.Content.ReadAsStringAsync();
                var pedidos = JsonConvert.DeserializeObject<List<ClsDeserializacaoDelMatchEntrega>>(responseJson);

                pedido = pedidos.Where(x => x.IdOrder == delMatchId).FirstOrDefault();

            }

            if (pedido == null)
            {
                return new ClsDeserializacaoDelMatchEntrega() { Status = "Não Enviado" };
            }

            return pedido;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            Console.WriteLine(ex.Message);
        }
        return pedido;
    }

    public async Task<bool> VerificaSePedidoFoiEnviado(List<string> pedidosId)
    {
        string apiUrl = "https://delmatchapp.com/api/deliveries-list/";
        bool ExistePedido = false;
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
                var ConfigsSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                string token = ConfigsSistema.DelMatchId;

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    string responseJson = await response.Content.ReadAsStringAsync();
                    var pedidos = JsonConvert.DeserializeObject<List<ClsDeserializacaoDelMatchEntrega>>(responseJson);

                    foreach (string PedidoRef in pedidosId)
                    {
                        var pedidosValidos = pedidos.Where(x => x.IdOrder == PedidoRef).ToList();

                        if (pedidosValidos.Count > 0)
                        {
                            bool ExistePedidoENviado = pedidosValidos.Any(x => x.Status != "Created");

                            if (ExistePedidoENviado)
                            {
                                ExistePedido = true;
                            }
                        }

                    }
                }
            }

            return ExistePedido;

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro ao enviar Req DelMatch");
        }
        return ExistePedido;
    }

    public async void UpdateDelMatchId(int numConta, string delmatchId)
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Contxt.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

                string updateQuery = "UPDATE Sequencia SET DelMatchId = @NovoValor WHERE CONTA = @CONDICAO AND (MESA = 'WEB' OR MESA LIKE '%E%');";

                string DelMatchId = delmatchId;

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    // Abrindo a conexão
                    connection.Open();

                    using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                    {
                        // Definindo os parâmetros para a instrução SQL
                        command.Parameters.AddWithValue("@NovoValor", DelMatchId);
                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                        // Executando o comando UPDATE
                        command.ExecuteNonQuery();
                    }
                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao encontra Id. Por favor comunique o suporte da syslogica", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public async Task<Sequencia> PesquisaClientesNoCadastro(string? telefone)
    {
        Sequencia sequencia = new Sequencia();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Contxt.GetContextoAsync())
            {
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
                        }

                    }
                }
            }

            return sequencia;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Cliente Cadastrado");
        }
        return sequencia;
    }

    public async Task<Sequencia> PesquisaEnderecoDeEntrega(string? numConta, string? metodo)
    {
        Sequencia sequencia = new Sequencia();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Contxt.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco;
                string? SqlSelectIntoCadastros = "";

                if (metodo == "ENVIARPEDIDO")
                {
                    SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE CONTA = @NUMCONTA AND DelMatchId IS NULL";
                }

                if (metodo == "GETPEDIDO")
                {
                    SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE CONTA = @NUMCONTA AND DelMatchId IS NOT NULL";
                }

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@NUMCONTA", numConta);

                        // Executar a consulta SELECT
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sequencia.Customer.Name = reader["CONTATO"].ToString();
                                sequencia.Customer.Phone = reader["TELEFONE"].ToString();
                                sequencia.Customer.TaxPayerIdentificationNumber = "CPF"; //falta terminar

                                sequencia.DeliveryAddress.FormattedAddress = reader["ENDENTREGA"].ToString();
                                sequencia.DeliveryAddress.Country = "BR";
                                sequencia.DeliveryAddress.State = "SP";
                                sequencia.DeliveryAddress.City = opcSistema.Cidade;
                                sequencia.DeliveryAddress.Neighborhood = reader["BAIENTREGA"].ToString();
                                sequencia.DeliveryAddress.StreetName = reader["ENDENTREGA"].ToString();
                                sequencia.DeliveryAddress.StreetNumber = "";
                                sequencia.DeliveryAddress.PostalCode = "";
                                sequencia.DeliveryAddress.Complement = reader["REFENTREGA"].ToString();

                                sequencia.DeliveryAddress.Coordinates.Latitude = 0;
                                sequencia.DeliveryAddress.Coordinates.Longitude = 0;
                            }
                        }

                    }
                }
            }
            return sequencia;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Cliente Cadastrado");
        }
        return sequencia;
    }


    public async Task EnviaPedidosAut()
    {
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
                ParametrosDoSistema? Configuracoes = db.parametrosdosistema.ToList().FirstOrDefault();

                if (Configuracoes.EnviaPedidoAut)
                {
                    List<Sequencia> pedidosAbertos = await ListarPedidosAbertos();
                    int contagemdepedidos = pedidosAbertos.Count;
                    List<Sequencia> ItensAEnviarDelMach = FormDePedidosAbertos.ItensAEnviarDelMach;

                    if (contagemdepedidos > 0)
                    {
                        ItensAEnviarDelMach.AddRange(pedidosAbertos);
                    }

                    if (ItensAEnviarDelMach.Count() > 0)
                    {
                        foreach (var item in ItensAEnviarDelMach.ToList())
                        {
                            string jsonContent = JsonConvert.SerializeObject(item);
                            await GerarPedido(jsonContent);
                            UpdateDelMatchId(item.numConta, item.ShortReference);
                        }

                        ItensAEnviarDelMach.Clear();

                        if (ClsSuporteDePedidoNaoEnviadoDelmatch.ErroDeEnvioDePedido)
                        {
                            string PedidosASerINformados = "Erro a enviar pedidos. Pedidos que não foram Enviados: ";

                            foreach (string item in ClsSuporteDePedidoNaoEnviadoDelmatch.PedidosQueNaoForamEnviados)
                            {
                                string Motivos = "";

                                foreach (var motivo in ClsSuporteDePedidoNaoEnviadoDelmatch.MotivosDeNaoTerEnviado)
                                {
                                    Motivos += motivo;
                                }

                                PedidosASerINformados += $"\n{item}\n {Motivos}";
                            }

                            ClsSons.PlaySom2();
                            MessageBox.Show(PedidosASerINformados, "Erro ao enviar pedido automatico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClsSons.StopSom();

                            ClsSuporteDePedidoNaoEnviadoDelmatch.ErroDeEnvioDePedido = false;
                            ClsSuporteDePedidoNaoEnviadoDelmatch.PedidosQueNaoForamEnviados.Clear();
                            ClsSuporteDePedidoNaoEnviadoDelmatch.MotivosDeNaoTerEnviado.Clear();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao enviar pedidos automatico para delmatch", "Ops");
        }
    }

    public async Task GetToken()
    {
        string url = @"https://delmatchcardapio.com/api/oauth/token.json";
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
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
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro Ao pegar o token Del Match", "Ops");
        }
    }

    public async Task RefreshTokenDelMatch()
    {
        string url = @"https://delmatchcardapio.com/api/oauth/token.json";
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
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

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro Ao pegar o token Del Match", "Ops");
        }
    }


    public async Task<Sequencia> CriarPedidoParaEnviar(PedidoDelMatch Pedido)
    {
        Sequencia ClsParaEnviarPedido = new Sequencia();
        try
        {
            using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
            {
                ParametrosDoSistema Configs = db.parametrosdosistema.FirstOrDefault();

                ClsParaEnviarPedido.Id = Pedido.Id.ToString();
                ClsParaEnviarPedido.ShortReference = Pedido.Reference.ToString();
                ClsParaEnviarPedido.CreatedAt = Pedido.CreatedAt;
                ClsParaEnviarPedido.Type = "DELIVERY";
                ClsParaEnviarPedido.TimeMax = "";
                ClsParaEnviarPedido.ValorConta = Convert.ToDecimal(Pedido.TotalPrice);

                ClsParaEnviarPedido.Merchant.RestaurantId = Configs.DelMatchId;
                ClsParaEnviarPedido.Merchant.Id = Configs.DelMatchId;
                ClsParaEnviarPedido.Merchant.Name = Configs.NomeFantasia;
                ClsParaEnviarPedido.Merchant.RestaurantId = Configs.DelMatchId;
                ClsParaEnviarPedido.Merchant.Unit = Configs.DelMatchId;

                ClsParaEnviarPedido.Customer.Name = Pedido.Customer.Name;
                ClsParaEnviarPedido.Customer.Phone = Pedido.Customer.Phone;
                ClsParaEnviarPedido.Customer.TaxPayerIdentificationNumber = Pedido.Customer.CPF;

                ClsParaEnviarPedido.DeliveryAddress.FormattedAddress = $"{Pedido.deliveryAddress.StreetName}, {Pedido.deliveryAddress.StreetNumber} - {Pedido.deliveryAddress.Neighborhood}";
                ClsParaEnviarPedido.DeliveryAddress.Country = Pedido.deliveryAddress.Country;
                ClsParaEnviarPedido.DeliveryAddress.State = Pedido.deliveryAddress.State;
                ClsParaEnviarPedido.DeliveryAddress.City = Pedido.deliveryAddress.City;
                ClsParaEnviarPedido.DeliveryAddress.Neighborhood = Pedido.deliveryAddress.Neighborhood;
                ClsParaEnviarPedido.DeliveryAddress.StreetName = Pedido.deliveryAddress.StreetName;
                ClsParaEnviarPedido.DeliveryAddress.StreetNumber = Pedido.deliveryAddress.StreetNumber;
                ClsParaEnviarPedido.DeliveryAddress.PostalCode = Pedido.deliveryAddress.PostalCode;
                ClsParaEnviarPedido.DeliveryAddress.Complement = Pedido.deliveryAddress.Complement;

                ClsParaEnviarPedido.DeliveryAddress.Coordinates.Latitude = 0;
                ClsParaEnviarPedido.DeliveryAddress.Coordinates.Longitude = 0;

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
        return ClsParaEnviarPedido;
    }

    public async Task<HttpResponseMessage> EnviaReqParaDelMatch(string? url, string? metodo, string? content = "")
    {
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            if (metodo == "GET")
            {
                using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
                {
                    var Configs = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configs.TokenDelMatch);

                    response = await client.GetAsync(url);
                }
                return response;
            }

            if (metodo == "POST")
            {
                using (ApplicationDbContext db = await _Contxt.GetContextoAsync())
                {
                    var Configs = await db.parametrosdeautenticacao.FirstOrDefaultAsync();
                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configs.TokenDelMatch);

                    StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                    response = await client.PostAsync(url, contentToPost);
                }
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
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Por favor, verifique sua conexão com a internet. Ela pode estar oscilando ou desligada!", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
        return response;
    }


}

