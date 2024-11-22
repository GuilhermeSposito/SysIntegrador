using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class OnPedido
{

    private readonly IMeuContexto _Context;
    private List<string> NumerosDosPedidosASerAceito = new List<string>();
    private int ReqDePollingCount { get; set; } = 0;

    public OnPedido(IMeuContexto context)
    {
        _Context = context;
    }

    public async Task Pooling2()
    {
        string url = @"https://merchant-api.onpedido.com.br/v1/events:polling";
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? Configs = db.parametrosdosistema.FirstOrDefault();

                // await PostgresConfigs.ConcluiPedidoOnPedido();
                await RefreshTokenOnPedidos();
                await ConcluirPedido(concluiuAut: true);
                await DespachaPedido2(DispachaAut: true);
                ConcluiPedidosAutomatico();

                HttpResponseMessage response = await EnviaReq(url, "GET");

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    PollingOnPedido? pooling = JsonConvert.DeserializeObject<PollingOnPedido>(responseString);

                    List<string> NumerosDosPedidosASerAdicionadoNoBancoDeDados = new List<string>();

                    foreach (var item in pooling!.Return)
                    {
                        switch (item.EventId)
                        {
                            case "0":
                                NumerosDosPedidosASerAdicionadoNoBancoDeDados.Add(item.orderId.ToString());
                                break;
                            case "1":
                                NumerosDosPedidosASerAdicionadoNoBancoDeDados.Add(item.orderId.ToString());
                                break;
                            case "2":
                                NumerosDosPedidosASerAdicionadoNoBancoDeDados.Add(item.orderId.ToString());
                                await MudaStatusPedido(item.orderId, "CONFIRMED");
                                break;
                            case "3":
                                await MudaStatusPedido(item.orderId, "DISPATCHED");
                                //muda status
                                ClsSons.StopSom();
                                break;
                            case "4":
                                await MudaStatusPedido(item.orderId, "CONCLUDED");
                                //muda status
                                ClsSons.StopSom();
                                break;
                            case "5":
                                await MudaStatusPedido(item.orderId, "CANCELLED");
                                ClsSons.StopSom();
                                break;
                        }
                    }

                    if (NumerosDosPedidosASerAdicionadoNoBancoDeDados.Count > 0)
                    {
                        await SetPedido2(NumerosDosPedidosASerAdicionadoNoBancoDeDados);

                        if (Configs!.AceitaPedidoAut)
                        {
                            if (NumerosDosPedidosASerAceito.Count > 0)
                                await AceitaPedidoVarios();
                        }
                    }
                }
                else
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    ClsErroApi clsErroApi = JsonConvert.DeserializeObject<ClsErroApi>(responseString)!;

                    if ((clsErroApi.Error.Code.Contains("OAuth39") || clsErroApi.Error.Code.Contains("OAuth40") || clsErroApi.Error.Code.Contains("OAuth41")) && ReqDePollingCount == 0)
                        await GetToken();
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public async Task SetPedido2(List<string> NumerosDosPedidos)
    {
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                string? NumerosDosPedidosString = string.Join("-", NumerosDosPedidos);
                string? Url = $"https://merchant-api.onpedido.com.br/v1/orders/{NumerosDosPedidosString}";

                HttpResponseMessage response = await EnviaReq(Url, "GET");

                if (response.IsSuccessStatusCode)
                {
                    PedidoOnPedido2? Pedidos;
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    if (NumerosDosPedidos.Count > 1)
                        Pedidos = JsonConvert.DeserializeObject<PedidoOnPedido2>(jsonContent);
                    else
                    {
                        PedidoOnPedido pedido = JsonConvert.DeserializeObject<PedidoOnPedido>(jsonContent)!;

                        Pedidos = new PedidoOnPedido2() { Status = pedido.Status, Return = new List<Return>() { pedido.Return } };
                    }

                    foreach (var pedido in Pedidos!.Return)
                    {
                        bool existePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == pedido.Id!.ToString());
                        if (!existePedido)
                        {
                            ClsSons.PlaySom();

                            ParametrosDoSistema? Configs = db.parametrosdosistema.ToList().FirstOrDefault();


                            int insertNoSysMenuConta = 0;
                            string? mesa = " ";
                            string? DataCertaEntregarEm = " ";
                            string? Complemento = " ";
                            string? EndEntrega = " ";
                            string? BairEntrega = " ";
                            string? Entregador = " ";
                            string? Status = " ";

                            if (Configs.IntegracaoSysMenu)
                            {
                                if (pedido.Type == "DELIVERY")
                                {
                                    mesa = "WEB";
                                    DataCertaEntregarEm = pedido.Delivery.DeliveryDateTime;
                                    Complemento = pedido.Delivery.DeliveryAddressON.Complement;
                                    EndEntrega = pedido.Delivery.DeliveryAddressON.FormattedAddress;
                                    BairEntrega = pedido.Delivery.DeliveryAddressON.District;
                                    Status = "P";
                                    Entregador = "00";
                                }

                                if (pedido.Type == "TAKEOUT")
                                {
                                    mesa = "WEBB";
                                    DataCertaEntregarEm = pedido.TakeOut.TakeoutDateTime;
                                    Entregador = "RETIRADA";
                                    Status = "P";
                                }

                                if (pedido.Type == "INDOOR")
                                {
                                    mesa = await RetiraNumeroDeMesa(pedido.Indoor.Place);
                                    DataCertaEntregarEm = pedido.Indoor.IndoorDateTime;
                                    Status = "A";
                                }

                                float ValorDescontosNum = 0.0f;

                                foreach (var item in pedido.Discounts)
                                {
                                    ValorDescontosNum += item.Amount.value;
                                }

                                float ValorEntrega = 0.0f;

                                var EntregaObj = pedido.OtherFees.Where(x => x.Type == "DELIVERY_FEE").FirstOrDefault();
                                ValorEntrega = EntregaObj.Price.Value;

                                if (pedido.Type != "INDOOR")
                                {
                                    insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                                            mesa: mesa,
                                            cortesia: ValorDescontosNum,
                                            taxaEntrega: ValorEntrega,
                                            taxaMotoboy: 0.00f,
                                            dtInicio: pedido.CreatedAt.ToString().Substring(0, 10),
                                             hrInicio: pedido.CreatedAt.ToString().Substring(11, 5),
                                             contatoNome: pedido.Customer.Name,
                                             usuario: "CAIXA",
                                             dataSaida: DataCertaEntregarEm.ToString().Substring(0, 10),
                                            hrSaida: DataCertaEntregarEm.ToString().Substring(11, 5),
                                             obsConta1: " ",
                                            iFoodPedidoID: pedido.Id,
                                            obsConta2: " ",
                                            referencia: Complemento,
                                             endEntrega: EndEntrega,
                                             bairEntrega: BairEntrega,
                                             entregador: Entregador,
                                             eOnpedido: true,
                                             telefone: pedido.Customer.PhoneOn?.Extension != null ? $"({pedido.Customer.PhoneOn.Extension}){pedido.Customer.PhoneOn.Number}" : " "
                                             ); //fim dos parâmetros do método de integração

                                    string type = pedido.Payments.Prepaid > 0 && pedido.Payments.Pending == 0 ? "ONLINE" : "OFFLINE";

                                    ClsDeIntegracaoSys.IntegracaoPagCartao(pedido.Payments.Methods[0].Method, insertNoSysMenuConta, pedido.Total.OrderAmount.value, type, "ONPEDIDO");

                                    SysIntegradorApp.ClassesAuxiliares.Payments payments = new();

                                    foreach (var item in pedido.Payments.Methods)
                                    {
                                        Cash SeForPagamentoEmDinherio = new Cash() { changeFor = item.ChangeFor };
                                        payments.methods.Add(new Methods() { method = item.Method, value = item.value, cash = SeForPagamentoEmDinherio });
                                    }

                                    ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(payments, insertNoSysMenuConta);
                                }
                            }

                            if (pedido.Type == "INDOOR")
                            {
                                insertNoSysMenuConta = 999;
                            }

                            db.parametrosdopedido.Add(new ParametrosDoPedido()
                            {
                                Id = pedido.Id,
                                Json = JsonConvert.SerializeObject(new PedidoOnPedido { Status = true, Return = pedido }),
                                Situacao = "Novo",
                                Conta = insertNoSysMenuConta,
                                CriadoEm = DateTime.Now.ToString(),
                                DisplayId = Convert.ToInt32(pedido.DisplayId),
                                JsonPolling = "Sem Polling ID",
                                CriadoPor = "ONPEDIDO",
                                PesquisaDisplayId = Convert.ToInt32(pedido.DisplayId),
                                PesquisaNome = pedido.Customer.Name
                            });

                            db.SaveChanges();


                            if (pedido.Type == "INDOOR")
                            {
                                insertNoSysMenuConta = 0;
                            }


                            if (Configs.IntegracaoSysMenu)
                            {
                                bool existeCliente = ClsDeIntegracaoSys.ProcuraCliente($"({pedido.Customer.PhoneOn.Extension}){pedido.Customer.PhoneOn.Number}");

                                if (!existeCliente && pedido.Type == "DELIVERY")
                                {
                                    ClsDeIntegracaoSys.CadastraClienteOnPedido(pedido.Customer, pedido.Delivery);
                                }

                                foreach (var item in pedido.ItemsOn)
                                {
                                    var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item, eIntegracao: true);

                                    ClsDeIntegracaoSys.IntegracaoContas(
                                              conta: insertNoSysMenuConta, //numero
                                              mesa: mesa, //texto curto 
                                              qtdade: item.quantity, //numero
                                              codCarda1: CaracteristicasPedido.ExternalCode1, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                              codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                              codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                              tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                              descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                              valorUnit: item.TotalPrice.Value / item.quantity, //moeda
                                              valorTotal: item.TotalPrice.Value, //moeda
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
                                              obs15: item.observations != null && item.observations.Length > 0 ? item.observations : " ",
                                              cliente: pedido.Customer.Name, // texto curto 80 letras
                                              telefone: pedido.Customer.PhoneOn.Extension != null ? $"({pedido.Customer.PhoneOn.Extension}){pedido.Customer.PhoneOn.Number}" : " ", // texto curto 14 letras
                                              impComanda: "Não",
                                              ImpComanda2: "Não",
                                              qtdComanda: 00f,//numero duplo 
                                              status: Status
                                         );//fim dos parâmetros

                                }

                            }

                            if (Configs.AceitaPedidoAut)
                            {
                                NumerosDosPedidosASerAceito.Add(pedido.Id!.ToString());
                            }

                            ClsDeSuporteAtualizarPanel.MudouDataBase = true;

                            if (opSistema.ImpressaoAut && opSistema.AceitaPedidoAut)
                            {
                                ImprimeAutomatico(new PedidoOnPedido { Status = true, Return = pedido });
                            }

                            if (pedido.Type == "INDOOR")
                            {
                                db.apoioonpedido.Add(new ApoioOnPedido { Id_Pedido = Convert.ToInt32(pedido.Id), Action = "CONCLUIR" });


                                //   await ConcluirPedido(pedido.Id!.ToString(), concluiuAut: true);

                            }
                        }
                    }

                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task AceitaPedidoVarios()
    {
        try
        {
            string? NumerosDosPedidosString = string.Join("-", NumerosDosPedidosASerAceito);
            string url = $"https://merchant-api.onpedido.com.br/v1/orders/{NumerosDosPedidosString}/confirm";

            string? BodyDeConfirmacao = " ";

            ClsParaConfirmarPedido ClsConfirma = new ClsParaConfirmarPedido() { Reason = "Confirmed", PreparationTime = 60 };
            BodyDeConfirmacao = JsonConvert.SerializeObject(ClsConfirma);

            HttpResponseMessage responseDeConfirmacao = await EnviaReq(url, "POST", BodyDeConfirmacao);

            if (!responseDeConfirmacao.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao Aceitar Pedidos");
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }


    public async void ConcluiPedidosAutomatico()
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                List<ParametrosDoSistema> ConfigsList = await db.parametrosdosistema.ToListAsync();
                ParametrosDoSistema? Configs = ConfigsList.FirstOrDefault();

                if (Configs.DtUltimaVerif != null)
                {
                    DateTime HoraAtual = DateTime.Now;
                    DateTime DtUltimaVerif = DateTime.Parse(Configs.DtUltimaVerif);

                    TimeSpan diferenca = HoraAtual - DtUltimaVerif;

                    if (diferenca.TotalMinutes > Configs.TempoConclonPedido)
                    {
                        Configs.DtUltimaVerif = HoraAtual.ToString();
                        db.SaveChanges();

                        int totalMinutos = db.parametrosdosistema.FirstOrDefault().TempoConclonPedido;

                        int horas = totalMinutos / 60;
                        int minutos = totalMinutos % 60;
                        int segundos = 0;

                        string tempoFormatado = $"{horas:D2}:{minutos:D2}:{segundos:D2}";

                        TimeSpan intervalo = TimeSpan.Parse(tempoFormatado);

                        var pedidosQuery = await db.parametrosdopedido.ToListAsync();

                        List<ParametrosDoPedido> pedidos = pedidosQuery
                            .AsEnumerable()
                            .Where(p => DateTime.Now - DateTime.Parse(p.CriadoEm) > intervalo && p.Situacao != "CANCELLED" && p.Situacao != "CONCLUDED" && p.CriadoPor == "ONPEDIDO")
                            .ToList();

                        if (pedidos.Count() > 0)
                        {
                            foreach (var pedido in pedidos)
                            {
                                bool ExistePedido = await db.apoioonpedido.AnyAsync(p => p.Id_Pedido == Convert.ToInt32(pedido.Id));

                                if (!ExistePedido)
                                {
                                    db.apoioonpedido.Add(new ApoioOnPedido { Id_Pedido = Convert.ToInt32(pedido.Id), Action = "CONCLUIR" });
                                    await db.SaveChangesAsync();
                                }
                            }
                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }


    public async Task MudaStatusPedido(int orderId, string? Status)
    {
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoPedido? Pedido = db.parametrosdopedido.Where(x => x.Id == orderId.ToString()).ToList().FirstOrDefault();

                if (Pedido != null)
                {
                    if (Pedido.Situacao != Status)
                    {
                        Pedido.Situacao = Status;
                        await db.SaveChangesAsync();
                        ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task DespachaPedido(string? orderId = "DEFAULT", bool DispachaAut = false)
    {
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ApoioOnPedido? PedidoApoio = db.apoioonpedido.FirstOrDefault(p => p.Action == "DESPACHAR");

                if (PedidoApoio is not null)
                {
                    ParametrosDoPedido? Pedido = await db.parametrosdopedido.FirstOrDefaultAsync(p => p.Id == PedidoApoio.Id_Pedido.ToString());

                    if (Pedido is not null && Pedido.Situacao != "DISPATCHED")
                    {
                        string url = $"https://merchant-api.onpedido.com.br/v1/orders/{PedidoApoio.Id_Pedido.ToString()}/dispatch";

                        var response = await EnviaReq(url, "POST");

                        if (response.IsSuccessStatusCode)
                        {
                            if (!DispachaAut)
                            {
                                MessageBox.Show($"Pedido de id: {orderId} Despachado com sucesso!");
                            }

                            db.apoioonpedido.Remove(PedidoApoio);
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            if (!DispachaAut)
                            {
                                MessageBox.Show("Não foi possivel despachar pedido", "Não foi possivel!");
                            }

                        }

                    }
                    else
                    {
                        if (!DispachaAut)
                        {
                            MessageBox.Show("Pedido já Despachado", "Não é possivel!");
                        }
                        db.apoioonpedido.Remove(PedidoApoio);
                        await db.SaveChangesAsync();
                    }
                }
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task DespachaPedido2(string? orderId = "DEFAULT", bool DispachaAut = false)
    {
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                List<ApoioOnPedido>? PedidosApoio = await db.apoioonpedido.Where(p => p.Action == "DESPACHAR").ToListAsync();

                if (PedidosApoio is not null)
                {
                    List<string> NumerosDoPedido = new List<string>();
                    foreach (var pedido in PedidosApoio)
                        NumerosDoPedido.Add(pedido.Id_Pedido.ToString());


                    string? NumerosDosPedidosString = string.Join("-", NumerosDoPedido);


                    string url = $"https://merchant-api.onpedido.com.br/v1/orders/{NumerosDosPedidosString}/dispatch";

                    var response = await EnviaReq(url, "POST");

                    if (response.IsSuccessStatusCode)
                    {
                        if (!DispachaAut)
                        {
                            MessageBox.Show($"Pedido de id: {orderId} Despachado com sucesso!");
                        }

                        db.apoioonpedido.RemoveRange(PedidosApoio);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        if (!DispachaAut)
                        {
                            MessageBox.Show("Não foi possivel despachar pedido", "Não foi possivel!");
                        }

                    }


                }
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task ConcluirPedido(string? orderId = "Default", bool concluiuAut = false)
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                List<ApoioOnPedido>? PedidoApoio = await db.apoioonpedido.Where(p => p.Action == "CONCLUIR").ToListAsync();

                if (PedidoApoio is not null)
                {
                    List<string> NumerosDosPedidos = new List<string>();
                    foreach (var pedido in PedidoApoio)
                        NumerosDosPedidos.Add(pedido.Id_Pedido.ToString());

                    string? NumerosDosPedidosString = string.Join("-", NumerosDosPedidos);

                    string url = $"https://merchant-api.onpedido.com.br/v1/orders/{NumerosDosPedidosString}/deliver";

                    var response = await EnviaReq(url, "POST");

                    if (response.IsSuccessStatusCode)
                    {
                        if (!concluiuAut)
                        {
                            MessageBox.Show($"Pedido de id {orderId} Concluido com sucesso!");
                        }

                        db.apoioonpedido.RemoveRange(PedidoApoio);
                        await db.SaveChangesAsync();

                    }
                    else
                    {
                        if (!concluiuAut)
                        {
                            MessageBox.Show(await response.Content.ReadAsStringAsync(), "Não foi possivel!");
                        }
                    }



                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task ConcluirPedido2(string? orderId = "Default", bool concluiuAut = false)
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ApoioOnPedido? PedidoApoio = await db.apoioonpedido.FirstOrDefaultAsync(p => p.Action == "CONCLUIR");

                if (PedidoApoio is not null)
                {
                    ParametrosDoPedido? Pedido = await db.parametrosdopedido.FirstOrDefaultAsync(p => p.Id == PedidoApoio.Id_Pedido.ToString());

                    if (Pedido is not null && Pedido.Situacao != "CONCLUDED")
                    {
                        string url = $"https://merchant-api.onpedido.com.br/v1/orders/{PedidoApoio.Id_Pedido}/deliver";

                        var response = await EnviaReq(url, "POST");

                        if (response.IsSuccessStatusCode)
                        {
                            if (!concluiuAut)
                            {
                                MessageBox.Show($"Pedido de id {orderId} Concluido com sucesso!");
                            }

                            db.apoioonpedido.Remove(PedidoApoio);
                            await db.SaveChangesAsync();

                        }
                        else
                        {
                            if (!concluiuAut)
                            {
                                MessageBox.Show(await response.Content.ReadAsStringAsync(), "Não foi possivel!");
                            }
                        }

                    }
                    else
                    {
                        if (!concluiuAut)
                        {
                            MessageBox.Show("Pedido já Confirmado", "Não é possivel!");
                        }
                        db.apoioonpedido.Remove(PedidoApoio);
                        await db.SaveChangesAsync();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task AceitaPedido(string? orderID, string? urlDoPedido = null)
    {
        string url = $"https://merchant-api.onpedido.com.br/v1/orders/{orderID}/confirm";
        try
        {
            HttpResponseMessage response;

            if (urlDoPedido == null)
                response = await EnviaReq(url, "GET");
            else
                response = await EnviaReq(urlDoPedido, "GET");

            if (response.IsSuccessStatusCode)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var Configs = db.parametrosdosistema.FirstOrDefault();

                string? jsonContent = await response.Content.ReadAsStringAsync();
                PedidoOnPedido? Pedido = JsonConvert.DeserializeObject<PedidoOnPedido>(jsonContent);

                string? BodyDeConfirmacao = " ";

                if (Pedido.Return.Type == "DELIVERY")
                {
                    ClsParaConfirmarPedido ClsConfirma = new ClsParaConfirmarPedido("Confirmed", Pedido.Return.CreatedAt, Pedido.Return.Id.ToString(), Configs.TempoEntrega);
                    BodyDeConfirmacao = JsonConvert.SerializeObject(ClsConfirma);
                }
                else
                {
                    ClsParaConfirmarPedido ClsConfirma = new ClsParaConfirmarPedido("Confirmed", Pedido.Return.CreatedAt, Pedido.Return.Id.ToString(), Configs.TempoRetirada);
                    BodyDeConfirmacao = JsonConvert.SerializeObject(ClsConfirma);
                }

                HttpResponseMessage responseDeConfirmacao = await EnviaReq(url, "POST", BodyDeConfirmacao);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Erro ao Aceitar Pedido");
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task<bool> CancelaPedido(string? orderID, string motivo)
    {
        bool pedidoCancelado = false;
        string url = $"https://merchant-api.onpedido.com.br/v1/orders/{orderID}/requestCancellation";
        try
        {
            ClsParaCancelarPedido motivos = new ClsParaCancelarPedido() { Reason = motivo, Code = motivo.ToUpper().Replace(" ", "_"), Mode = "MANUAL" };

            HttpResponseMessage response = await EnviaReq(url, "POST", motivo);

            if (response.IsSuccessStatusCode)
            {
                return pedidoCancelado = true;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
        return pedidoCancelado;
    }

    public async Task FechaMesa(string? orderID)
    {
        string url = $"https://delmatchcardapio.com/api/orders/{orderID}/statuses/finish.json";
        try
        {
            HttpResponseMessage response = await EnviaReq(url, "POST");
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task SetPedido(string? urlDOPedido, int OrderId)
    {
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                bool existePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == OrderId.ToString());

                if (!existePedido)
                {
                    HttpResponseMessage response = await EnviaReq(urlDOPedido, "GET");

                    if (response.IsSuccessStatusCode)
                    {
                        //await ClsSons.PlaySomAsync();

                        ParametrosDoSistema? Configs = db.parametrosdosistema.ToList().FirstOrDefault();
                        string? respondeJson = await response.Content.ReadAsStringAsync();
                        PedidoOnPedido? pedido = JsonConvert.DeserializeObject<PedidoOnPedido>(respondeJson);

                        int insertNoSysMenuConta = 0;
                        string? mesa = " ";
                        string? DataCertaEntregarEm = " ";
                        string? Complemento = " ";
                        string? EndEntrega = " ";
                        string? BairEntrega = " ";
                        string? Entregador = " ";
                        string? Status = " ";

                        if (Configs.IntegracaoSysMenu)
                        {
                            if (pedido.Return.Type == "DELIVERY")
                            {
                                mesa = "WEB";
                                DataCertaEntregarEm = pedido.Return.Delivery.DeliveryDateTime;
                                Complemento = pedido.Return.Delivery.DeliveryAddressON.Complement;
                                EndEntrega = pedido.Return.Delivery.DeliveryAddressON.FormattedAddress;
                                BairEntrega = pedido.Return.Delivery.DeliveryAddressON.District;
                                Status = "P";
                                Entregador = "00";
                            }

                            if (pedido.Return.Type == "TAKEOUT")
                            {
                                mesa = "WEBB";
                                DataCertaEntregarEm = pedido.Return.TakeOut.TakeoutDateTime;
                                Entregador = "RETIRADA";
                                Status = "P";
                            }

                            if (pedido.Return.Type == "INDOOR")
                            {
                                mesa = await RetiraNumeroDeMesa(pedido.Return.Indoor.Place);
                                DataCertaEntregarEm = pedido.Return.Indoor.IndoorDateTime;
                                Status = "A";
                            }

                            float ValorDescontosNum = 0.0f;

                            foreach (var item in pedido.Return.Discounts)
                            {
                                ValorDescontosNum += item.Amount.value;
                            }

                            float ValorEntrega = 0.0f;

                            var EntregaObj = pedido.Return.OtherFees.Where(x => x.Type == "DELIVERY_FEE").FirstOrDefault();
                            ValorEntrega = EntregaObj.Price.Value;

                            if (pedido.Return.Type != "INDOOR")
                            {
                                insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                                        mesa: mesa,
                                        cortesia: ValorDescontosNum,
                                        taxaEntrega: ValorEntrega,
                                        taxaMotoboy: 0.00f,
                                        dtInicio: pedido.Return.CreatedAt.ToString().Substring(0, 10),
                                         hrInicio: pedido.Return.CreatedAt.ToString().Substring(11, 5),
                                         contatoNome: pedido.Return.Customer.Name,
                                         usuario: "CAIXA",
                                         dataSaida: DataCertaEntregarEm.ToString().Substring(0, 10),
                                        hrSaida: DataCertaEntregarEm.ToString().Substring(11, 5),
                                         obsConta1: " ",
                                        iFoodPedidoID: pedido.Return.Id,
                                        obsConta2: " ",
                                        referencia: Complemento,
                                         endEntrega: EndEntrega,
                                         bairEntrega: BairEntrega,
                                         entregador: Entregador,
                                         eOnpedido: true,
                                         telefone: pedido.Return.Customer.PhoneOn?.Extension != null ? $"({pedido.Return.Customer.PhoneOn.Extension}){pedido.Return.Customer.PhoneOn.Number}" : " "
                                         ); //fim dos parâmetros do método de integração

                                string type = pedido.Return.Payments.Prepaid > 0 && pedido.Return.Payments.Pending == 0 ? "ONLINE" : "OFFLINE";

                                ClsDeIntegracaoSys.IntegracaoPagCartao(pedido.Return.Payments.Methods[0].Method, insertNoSysMenuConta, pedido.Return.Total.OrderAmount.value, type, "ONPEDIDO");

                                SysIntegradorApp.ClassesAuxiliares.Payments payments = new();

                                foreach (var item in pedido.Return.Payments.Methods)
                                {
                                    Cash SeForPagamentoEmDinherio = new Cash() { changeFor = item.ChangeFor };
                                    payments.methods.Add(new Methods() { method = item.Method, value = item.value, cash = SeForPagamentoEmDinherio });
                                }

                                ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(payments, insertNoSysMenuConta);
                            }
                        }

                        if (pedido.Return.Type == "INDOOR")
                        {
                            insertNoSysMenuConta = 999;
                        }

                        db.parametrosdopedido.Add(new ParametrosDoPedido()
                        {
                            Id = pedido.Return.Id,
                            Json = respondeJson,
                            Situacao = "Novo",
                            Conta = insertNoSysMenuConta,
                            CriadoEm = DateTime.Now.ToString(),
                            DisplayId = Convert.ToInt32(pedido.Return.DisplayId),
                            JsonPolling = "Sem Polling ID",
                            CriadoPor = "ONPEDIDO",
                            PesquisaDisplayId = Convert.ToInt32(pedido.Return.DisplayId),
                            PesquisaNome = pedido.Return.Customer.Name
                        });

                        db.SaveChanges();


                        if (pedido.Return.Type == "INDOOR")
                        {
                            insertNoSysMenuConta = 0;
                        }


                        if (Configs.IntegracaoSysMenu)
                        {
                            bool existeCliente = ClsDeIntegracaoSys.ProcuraCliente($"({pedido.Return.Customer.PhoneOn.Extension}){pedido.Return.Customer.PhoneOn.Number}");

                            if (!existeCliente && pedido.Return.Type == "DELIVERY")
                            {
                                ClsDeIntegracaoSys.CadastraClienteOnPedido(pedido.Return.Customer, pedido.Return.Delivery);
                            }

                            foreach (var item in pedido.Return.ItemsOn)
                            {
                                var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item, eIntegracao: true);

                                ClsDeIntegracaoSys.IntegracaoContas(
                                          conta: insertNoSysMenuConta, //numero
                                          mesa: mesa, //texto curto 
                                          qtdade: item.quantity, //numero
                                          codCarda1: CaracteristicasPedido.ExternalCode1, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                          codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                          codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                          tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                          descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                          valorUnit: item.TotalPrice.Value / item.quantity, //moeda
                                          valorTotal: item.TotalPrice.Value, //moeda
                                          dataInicio: pedido.Return.CreatedAt.Substring(0, 10).Replace("-", "/"), //data
                                          horaInicio: pedido.Return.CreatedAt.Substring(11, 5), //data
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
                                          obs15: item.observations != null && item.observations.Length > 0 ? item.observations : " ",
                                          cliente: pedido.Return.Customer.Name, // texto curto 80 letras
                                          telefone: pedido.Return.Customer.PhoneOn.Extension != null ? $"({pedido.Return.Customer.PhoneOn.Extension}){pedido.Return.Customer.PhoneOn.Number}" : " ", // texto curto 14 letras
                                          impComanda: "Não",
                                          ImpComanda2: "Não",
                                          qtdComanda: 00f,//numero duplo 
                                          status: Status
                                     );//fim dos parâmetros

                            }

                        }

                        ClsDeSuporteAtualizarPanel.MudouDataBase = true;

                        if (opSistema.ImpressaoAut && opSistema.AceitaPedidoAut)
                        {
                            ImprimeAutomatico(pedido);
                        }

                        if (pedido.Return.Type == "INDOOR")
                        {
                            await ConcluirPedido(pedido.Return.Id!.ToString(), concluiuAut: true);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task<string> RetiraNumeroDeMesa(string? place)
    {
        string? numeroMesa = " ";
        try
        {
            numeroMesa = place.Substring(5, 2).Trim();

            numeroMesa = numeroMesa.PadLeft(4, '0');

            return numeroMesa;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
        return numeroMesa;
    }

    public async void ImprimeAutomatico(PedidoOnPedido Pedido)
    {
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Pedido.Return.Id).FirstOrDefault();
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

                if (!opSistema.AgruparComandas)
                {
                    foreach (string imp in impressoras)
                    {
                        if (imp != "Sem Impressora" && imp != null)
                        {
                            ImpressaoONPedido.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp);
                        }
                    }
                }
                else
                {
                    ImpressaoONPedido.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
                }

                impressoras.Clear();
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public async Task<List<ParametrosDoPedido>> GetPedidoOnPedido(int? display_ID = null, string? pesquisaNome = null)
    {
        List<ParametrosDoPedido> pedidosFromDb = new List<ParametrosDoPedido>();

        List<PedidoCompleto> pedidos = new List<PedidoCompleto>();
        try
        {
            if (display_ID != null || pesquisaNome != null)
            {
                if (display_ID != null)
                {
                    using (ApplicationDbContext db = await _Context.GetContextoAsync())
                    {

                        pedidosFromDb = db.parametrosdopedido.Where(x => x.DisplayId == display_ID && x.CriadoPor == "ONPEDIDO" || x.Conta == display_ID && x.CriadoPor == "ONPEDIDO").AsNoTracking().ToList();


                        return pedidosFromDb;
                    }
                }

                if (pesquisaNome != null)
                {
                    using (ApplicationDbContext db = await _Context.GetContextoAsync())
                    {

                        pedidosFromDb = db.parametrosdopedido.Where(x => (x.PesquisaNome.ToLower().Contains(pesquisaNome) || x.PesquisaNome.Contains(pesquisaNome) || x.PesquisaNome.ToUpper().Contains(pesquisaNome)) && x.CriadoPor == "ONPEDIDO").AsNoTracking().ToList();


                        return pedidosFromDb;
                    }
                }
            }
            else
            {
                using (ApplicationDbContext db = await _Context.GetContextoAsync())
                {

                    pedidosFromDb = db.parametrosdopedido.Where(x => x.CriadoPor == "ONPEDIDO").AsNoTracking().ToList();

                    return pedidosFromDb;
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "ERRO AO GETPEDIDO");
        }

        return pedidosFromDb;
    }

    public async Task<PedidoCompleto> OnPedidoPedidoCompleto(PedidoOnPedido p)
    {
        PedidoCompleto PedidoCompletoConvertido = new PedidoCompleto();
        try
        {

            PedidoCompletoConvertido.CriadoPor = "ONPEDIDO"; // Valor fixo de exemplo
            PedidoCompletoConvertido.JsonPolling = "{}"; // Valor fixo de exemplo
            PedidoCompletoConvertido.id = p.Return.Id;
            PedidoCompletoConvertido.displayId = p.Return.DisplayId.ToString();
            PedidoCompletoConvertido.createdAt = p.Return.CreatedAt;
            PedidoCompletoConvertido.orderTiming = p.Return.OrderTiming; // Valor fixo de exemplo
            PedidoCompletoConvertido.orderType = p.Return.Type;

            string? dataLimite = "";
            string? DeliveryBy = "";

            if (p.Return.Type == "DELIVERY")
            {
                DeliveryBy = p.Return.Delivery.DeliveredBy;
                dataLimite = p.Return.Delivery.DeliveryDateTime;
            }

            if (p.Return.Type == "TAKEOUT")
            {
                DeliveryBy = "RETIRADA";
                dataLimite = p.Return.TakeOut.TakeoutDateTime;
            }

            if (p.Return.Type == "INDOOR")
            {
                DeliveryBy = "MESA";
                dataLimite = p.Return.Indoor.IndoorDateTime;
            }

            if (p.Return.OrderTiming == "SCHEDULED")
            {
                dataLimite = p.Return.Schedule.ScheduledDateTimeEnd;
            }

            PedidoCompletoConvertido.delivery.deliveredBy = DeliveryBy;
            PedidoCompletoConvertido.delivery.deliveryDateTime = dataLimite;
            PedidoCompletoConvertido.customer.id = p.Return.Customer.Id;
            PedidoCompletoConvertido.customer.name = p.Return.Customer.Name;
            PedidoCompletoConvertido.customer.documentNumber = p.Return.Customer.DocumentNumber;
            PedidoCompletoConvertido.salesChannel = "ONPEDIDO"; // Valor fixo de exemplo

            return PedidoCompletoConvertido;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
        return PedidoCompletoConvertido;
    }

    public async Task GetToken()
    {
        string url = @"https://merchant-api.onpedido.com.br/v1/oauth/";

        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                Token? AutBase = db.parametrosdeautenticacao.FirstOrDefault();
                ParametrosDoSistema? configs = db.parametrosdosistema.FirstOrDefault();

                ClsPedirToken InfosParaOToken = new ClsPedirToken();
                InfosParaOToken.MerchantOAuthToken = configs.TokenOnPedido;
                InfosParaOToken.SoftwareOAuthToken = "2361jmm-62a7m0p5o5r6m2j6q4n5j3q4k8a5k152";
                InfosParaOToken.MerchantUsername = configs.UserOnPedido;
                InfosParaOToken.MerchantPassword = configs.SenhaOnPedido;
                InfosParaOToken.ClearAnotherTokens = true;

                string? JsonContent = JsonConvert.SerializeObject(InfosParaOToken);

                HttpResponseMessage response = await EnviaReq(url, "GetToken", JsonContent);

                if (response.IsSuccessStatusCode)
                {
                    string? reposta = await response.Content.ReadAsStringAsync();

                    TokenOnPedido? TokenOnP = JsonConvert.DeserializeObject<TokenOnPedido>(reposta);

                    TokenOnPedido.TokenDaSessao = TokenOnP.AccessOAuthToken;

                    string? HorarioDeVencimento = DateTime.Now.AddHours(4).ToString();

                    AutBase.TokenOnPedido = TokenOnP.AccessOAuthToken;
                    AutBase.VenceEmOnPedido = HorarioDeVencimento;
                    await db.SaveChangesAsync();
                    ReqDePollingCount = 1;
                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao pegar token ONPedidos", "Ops");
        }
    }

    public async Task RefreshTokenOnPedidos()
    {
        string url = @"https://merchant-api.onpedido.com.br/v1/oauth/";
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                Token? AutBase = db.parametrosdeautenticacao.FirstOrDefault();
                ParametrosDoSistema? configs = db.parametrosdosistema.FirstOrDefault();

                DateTime HorarioAtualDoToken = DateTime.Parse(AutBase.VenceEmOnPedido);

                if (DateTime.Now > HorarioAtualDoToken)
                {
                    ClsPedirToken InfosParaOToken = new ClsPedirToken();
                    InfosParaOToken.MerchantOAuthToken = configs.TokenOnPedido;
                    InfosParaOToken.SoftwareOAuthToken = "2361jmm-62a7m0p5o5r6m2j6q4n5j3q4k8a5k152";
                    InfosParaOToken.MerchantUsername = configs.UserOnPedido;
                    InfosParaOToken.MerchantPassword = configs.SenhaOnPedido;
                    InfosParaOToken.ClearAnotherTokens = true;

                    string? JsonContent = JsonConvert.SerializeObject(InfosParaOToken);

                    HttpResponseMessage response = await EnviaReq(url, "GetToken", JsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string? reposta = await response.Content.ReadAsStringAsync();

                        TokenOnPedido? TokenOnP = JsonConvert.DeserializeObject<TokenOnPedido>(reposta);

                        TokenOnPedido.TokenDaSessao = TokenOnP.AccessOAuthToken;

                        string? HorarioDeVencimento = DateTime.Now.AddHours(4).ToString();

                        AutBase.TokenOnPedido = TokenOnP.AccessOAuthToken;
                        AutBase.VenceEmOnPedido = HorarioDeVencimento;
                        await db.SaveChangesAsync();

                        ReqDePollingCount = 1;
                    }
                    else
                    {
                        MessageBox.Show(await response.Content.ReadAsStringAsync());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao Dar o refresh token ONPedidos", "Ops");
        }
    }

    public async Task<HttpResponseMessage> EnviaReq(string? url, string? metodo, string? content = "")
    {
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                Token? AutBase = db.parametrosdeautenticacao.FirstOrDefault();

                if (metodo == "GET")
                {
                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AutBase.TokenOnPedido);

                    response = await client.GetAsync(url);

                    return response;
                }

                if (metodo == "POST")
                {
                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AutBase.TokenOnPedido);

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
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao enviar Requisição HTTPS OnPedido", "ERRO");
        }
        return response;
    }
}
