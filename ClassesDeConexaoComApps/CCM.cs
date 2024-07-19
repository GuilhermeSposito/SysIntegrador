using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class CCM
{
    private readonly IMeuContexto _Db;

    public CCM(IMeuContexto context)
    {
        _Db = context;
    }

    public async Task Pooling()
    {
        string? url = "https://api.ccmpedidoonline.com.br/wsccm_v2.php";
        try
        {
            // await FechaMesas();

            bool AceitaPedidoAut = false;

            var resposta = await RequisicaoHttp(metodo: "PING");

            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                AceitaPedidoAut = db.parametrosdosistema.FirstOrDefault().AceitaPedidoAut;
            }

            HttpResponseMessage response = await RequisicaoHttp(url, "GET");

            if (response.IsSuccessStatusCode)
            {
                string? reponseXml = await response.Content.ReadAsStringAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(Pedidos));

                using var streamReaderXML = new StringReader(reponseXml);

                Pedidos Pedidos = (Pedidos)serializer.Deserialize(streamReaderXML);

                if (Pedidos != null)
                {
                    if (Pedidos.PedidoList.Count() > 0)
                    {
                        foreach (var pedido in Pedidos.PedidoList)
                        {
                            ClsSons.PlaySom();
                            await SetPedido(pedido);
                            if (AceitaPedidoAut)
                            {
                                await AceitaPedido(pedido.NroPedido);
                            }
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro pooling CCM");
        }
    }

    public async Task AtualizaStatus(int nmroPedido, string status = "CONFIRMED", bool manualmente = false)
    {
        try
        {
            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                var Pedido = await db.parametrosdopedido.FirstOrDefaultAsync(x => x.PesquisaDisplayId == nmroPedido);

                if (!manualmente)
                {
                    Pedido.Situacao = status;

                    await db.SaveChangesAsync();

                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                }
                else
                {
                    var config = await db.parametrosdosistema.FirstOrDefaultAsync();

                    string newStatus = "";

                    switch (status)
                    {
                        case "5":
                            newStatus = "DISPATCHED";
                            break;
                        case "6":
                            newStatus = "CONCLUDED";
                            break;
                        default:
                            newStatus = status;
                            break;
                    }

                    using HttpClient client = new HttpClient();
                    string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token={config.TokenCCM}&funcao=updateStatus&pedido={nmroPedido}&valor={status}";

                    StringContent contentToPost = new StringContent("", Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Newurl, contentToPost);

                    if (response.IsSuccessStatusCode)
                    {
                        Pedido.Situacao = newStatus;

                        await db.SaveChangesAsync();

                        switch (status)
                        {
                            case "5":
                                MessageBox.Show("Pedido Despachado", "Pedido Despachado com sucesso!");
                                break;
                            case "6":
                                MessageBox.Show("Pedido concluido", "Pedido concluido com sucesso!");
                                break;
                        }

                        ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                    }

                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao Atualizar pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }


    public async Task AceitaPedido(int nmroPedido, string msg = "Seu pedido foi aceito e está em preparo, para mais informações contate-nos")
    {
        try
        {
            await RequisicaoHttp(metodo: "ACEITAPEDIDO", content: msg, numPedido: nmroPedido);

            await RequisicaoHttp(metodo: "LIMPAPEDIDO", numPedido: nmroPedido);

            await using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                var PedidoDB = await db.parametrosdopedido.FirstOrDefaultAsync(x => x.Id == nmroPedido.ToString());

                if (PedidoDB is not null)
                {
                    Pedido? Pedido = JsonConvert.DeserializeObject<Pedido>(PedidoDB.Json);

                    var resposta = await RequisicaoHttp(url: Pedido.Cliente.Codigo.ToString(), metodo: "MSGCLIENTEACEITE", content: msg);

                    // MessageBox.Show(resposta.ToString());
                }

            }

            ClsSons.StopSom();

            await this.AtualizaStatus(nmroPedido);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao aceitar pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task RecusaPedido(int nmroPedido, string msg = "Infelizmente não poderemos aceitar seu pedido, para mais informações ligue-nos!")
    {
        try
        {
            await RequisicaoHttp(metodo: "RECUSAPEDIDO", content: msg, numPedido: nmroPedido);

            await this.AtualizaStatus(nmroPedido, status: "CANCELLED");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao negar pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task SetPedido(Pedido pedido)
    {
        try
        {
            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                bool existePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == pedido.NroPedido.ToString());

                if (!existePedido)
                {
                    var ConfigSistema = await db.parametrosdosistema.FirstOrDefaultAsync();

                    int insertNoSysMenuConta = 0;
                    string? mesa = " ";
                    string? HorarioDoEntregarAte = " ";
                    string? Complemento = " ";
                    string? EndEntrega = " ";
                    string? BairEntrega = " ";
                    string? Entregador = " ";
                    string? Status = " ";
                    float ValorDescontosNum = 0.0f;
                    float ValorEntrega = 0.0f;
                    float ValorDeTroco = 0.0f;
                    bool pedidoOnLineMesa = false;

                    string? TipoPedido = pedido.Retira == 1 ? "TAKEOUT" : "DELIVERY";
                    bool agendado = pedido.Agendamento == 1 ? true : false;
                    bool PedidoMesa = pedido.NumeroMesa > 0 ? true : false;

                    if (ConfigSistema.IntegracaoSysMenu)
                    {
                        ValorEntrega = pedido.ValorTaxa;
                        ValorDescontosNum = pedido.ValorCupom + pedido.CreditoUtilizado;

                        if (!agendado)
                        {
                            if (TipoPedido == "DELIVERY")
                            {
                                HorarioDoEntregarAte = DateTime.Parse(pedido.DataHoraPedido).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoEntrega).ToString();
                                mesa = "WEB";
                                Complemento = pedido.Endereco.Complemento;
                                EndEntrega = $"{pedido.Endereco.Rua}, {pedido.Endereco.Numero} - {pedido.Endereco.Bairro}";
                                BairEntrega = pedido.Endereco.Bairro;
                                Status = "P";
                            }

                            if (TipoPedido == "TAKEOUT")
                            {
                                HorarioDoEntregarAte = DateTime.Parse(pedido.DataHoraPedido).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoRetirada).ToString();
                                mesa = "WEBB";
                                Status = "P";
                            }
                        }
                        else
                        {
                            if (TipoPedido == "DELIVERY")
                            {
                                HorarioDoEntregarAte = DateTime.Parse(pedido.DataHoraAgendamento).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoEntrega).ToString();
                                mesa = "WEB";
                                Complemento = pedido.Endereco.Complemento;
                                EndEntrega = $"{pedido.Endereco.Rua}, {pedido.Endereco.Numero} - {pedido.Endereco.Bairro}";
                                BairEntrega = pedido.Endereco.Bairro;
                                Status = "P";
                            }

                            if (TipoPedido == "TAKEOUT")
                            {
                                HorarioDoEntregarAte = DateTime.Parse(pedido.DataHoraAgendamento).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoRetirada).ToString();
                                mesa = "WEBB";
                                Status = "P";
                            }
                        }

                        if (PedidoMesa)
                        {
                            mesa = pedido.NumeroMesa.ToString().ToString().PadLeft(4, '0');
                            Status = "A";
                        }

                        if (!PedidoMesa)
                        {
                            insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                                           mesa: mesa,
                                           cortesia: ValorDescontosNum,
                                           taxaEntrega: ValorEntrega,
                                           taxaMotoboy: 0.00f,
                                           dtInicio: pedido.DataHoraPedido.ToString().Substring(0, 10),
                                            hrInicio: pedido.DataHoraPedido.ToString().Substring(11, 5),
                                            contatoNome: pedido.Cliente.Nome,
                                            usuario: "CAIXA",
                                            dataSaida: HorarioDoEntregarAte.ToString().Substring(0, 10),
                                           hrSaida: HorarioDoEntregarAte.ToString().Substring(11, 5),
                                            obsConta1: " ",
                                           iFoodPedidoID: pedido.NroPedido.ToString(),
                                           obsConta2: " ",
                                           referencia: Complemento,
                                            endEntrega: EndEntrega,
                                            bairEntrega: BairEntrega,
                                            entregador: Entregador,
                                            eCCM: true
                                            ); //fim dos parâmetros do método de integração

                            string type = pedido.PagamentoOnline == 1 ? "ONLINE" : "OFFLINE";

                            ClsDeIntegracaoSys.IntegracaoPagCartao(pedido.DescricaoPagamento, insertNoSysMenuConta, pedido.ValorTotal, type, "CCM");

                            SysIntegradorApp.ClassesAuxiliares.Payments payments = new();

                            if (pedido.DescricaoPagamento == "Dinheiro")
                            {
                                if (!String.IsNullOrEmpty(pedido.TrocoPara))
                                {
                                    var TrocoPara = float.Parse(pedido.TrocoPara.Replace(".", ","));
                                    ValorDeTroco = TrocoPara;
                                }
                            }

                            SysIntegradorApp.ClassesAuxiliares.Cash SeForPagamentoEmDinherio = new Cash() { changeFor = ValorDeTroco };
                            payments.methods.Add(new Methods() { method = pedido.DescricaoPagamento, value = pedido.ValorTotal, cash = SeForPagamentoEmDinherio });


                            ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(payments, insertNoSysMenuConta);
                        }
                    }

                    pedido.EntregarAte = HorarioDoEntregarAte;

                    string jsonContent = JsonConvert.SerializeObject(pedido);

                    if (PedidoMesa)
                        insertNoSysMenuConta = 999;

                    await db.parametrosdopedido.AddAsync(new ParametrosDoPedido()
                    {
                        Id = pedido.NroPedido.ToString(),
                        Json = jsonContent,//serializedXml,
                        Situacao = pedido.StatusAcompanhamento,
                        Conta = insertNoSysMenuConta,
                        CriadoEm = DateTimeOffset.Now.ToString(),
                        DisplayId = Convert.ToInt32(pedido.NroPedido),
                        JsonPolling = "Sem Polling ID",
                        CriadoPor = "CCM",
                        PesquisaDisplayId = Convert.ToInt32(pedido.NroPedido),
                        PesquisaNome = pedido.Cliente.Nome
                    });

                    await db.SaveChangesAsync();

                    if (PedidoMesa)
                    {
                        insertNoSysMenuConta = 0;
                        pedidoOnLineMesa = true;
                    }

                    if (ConfigSistema.IntegracaoSysMenu)
                    {

                        if (TipoPedido == "DELIVERY")
                        {
                            bool existeCliente = ClsDeIntegracaoSys.ProcuraCliente($"{pedido.Cliente.Telefone}");
                            if (!existeCliente)
                            {
                                ClsDeIntegracaoSys.CadastraClienteCCM(pedido.Cliente, pedido.Endereco);
                            }

                        }

                        foreach (var item in pedido.Itens)
                        {
                            var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemCCM(item);

                            ClsDeIntegracaoSys.IntegracaoContas(
                                      conta: insertNoSysMenuConta, //numero
                                      mesa: mesa, //texto curto 
                                      qtdade: 1, //numero
                                      codCarda1: CaracteristicasPedido.ExternalCode1, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                      codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                      codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                      tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                      descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                      valorUnit: item.ValorUnit, //moeda
                                      valorTotal: item.ValorUnit, //moeda
                                      dataInicio: pedido.DataHoraPedido.Substring(0, 10).Replace("-", "/"), //data
                                      horaInicio: pedido.DataHoraPedido.Substring(11, 5), //data
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
                                      cliente: pedido.Cliente.Nome, // texto curto 80 letras
                                      telefone: pedido.Cliente.Telefone, // texto curto 14 letras
                                      impComanda: "Não",
                                      ImpComanda2: "Não",
                                      qtdComanda: 00f,//numero duplo 
                                      status: Status,
                                      pedidoOnLineMesa: pedidoOnLineMesa,
                                      idPedido: pedido.NroPedido.ToString()
                                 );//fim dos parâmetros

                        }

                    }

                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;

                    if (db.parametrosdosistema.FirstOrDefault().ImpressaoAut && db.parametrosdosistema.FirstOrDefault().AceitaPedidoAut)
                    {
                        ChamaImpressaoAutomatica(pedido);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao inserir pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task FechaMesas()
    {
        try
        {
            bool ProcuraMesaFechada = ClsDeIntegracaoSys.ProcuraMesaFechada();

            if (ProcuraMesaFechada)
            {
                ClsApoioFechamanetoDeMesa MesasFechadas = ClsDeIntegracaoSys.MesasFechadas();

                foreach (var item in MesasFechadas.Mesas)
                {
                    await AtualizaStatus(Convert.ToInt32(item.PedidoID), status: "6", true);
                }

            }
        }
        catch (Exception ex)
        {

            MessageBox.Show("Erro ao inserir pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
    }

    public async void ChamaImpressaoAutomatica(Pedido PedidoCCM)
    {
        try
        {
            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == PedidoCCM.NroPedido.ToString()).FirstOrDefault();
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

                if (!opSistema.AgruparComandas)
                {
                    foreach (string imp in impressoras)
                    {
                        if (imp != "Sem Impressora" && imp != null)
                        {
                            ImpressaoCCM.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp);
                        }
                    }
                }
                else
                {
                    ImpressaoCCM.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
                }

                impressoras.Clear();
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task<IEnumerable<ParametrosDoPedido>> GetPedidos(int? pesquisaID = null, string? pesquisaNome = null)
    {
        IEnumerable<ParametrosDoPedido> pedidos = new List<ParametrosDoPedido>();
        try
        {
            if (pesquisaID != null || pesquisaNome != null)
            {
                if (pesquisaID != null)
                {
                    using (ApplicationDbContext db = await _Db.GetContextoAsync())
                    {
                        pedidos = await db.parametrosdopedido.Where(x => x.CriadoPor == "CCM" && x.PesquisaDisplayId == pesquisaID).ToListAsync();
                    }
                }

                if(pesquisaNome != null)
                {

                    using (ApplicationDbContext db = await _Db.GetContextoAsync())
                    {
                        pedidos = await db.parametrosdopedido.Where(x => (x.CriadoPor == "CCM" && (x.PesquisaNome.ToLower().Contains(pesquisaNome) || x.PesquisaNome.Contains(pesquisaNome) || x.PesquisaNome.ToUpper().Contains(pesquisaNome)))).ToListAsync();
                    }
                }
            }
            else
            {
                using (ApplicationDbContext db = await _Db.GetContextoAsync())
                {
                    pedidos = await db.parametrosdopedido.Where(x => x.CriadoPor == "CCM").ToListAsync();
                }

            }


            return pedidos;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao buscar pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return pedidos;
    }

    public async Task<Pedido> RetornaPedido(ParametrosDoPedido pedidoParaDeserializar)
    {
        Pedido pedido = new Pedido();
        try
        {
            Pedido? PedidoDeserializado = JsonConvert.DeserializeObject<Pedido>(pedidoParaDeserializar.Json);

            return PedidoDeserializado;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao inserir pedido na base de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return pedido;
    }

    public async Task<PedidoCompleto> CCMPedidoCompleto(Pedido p)
    {
        PedidoCompleto pedidoCompleto = new PedidoCompleto();
        try
        {
            pedidoCompleto.CriadoPor = "CCM"; // Valor fixo de exemplo
            pedidoCompleto.JsonPolling = "{}"; // Valor fixo de exemplo
            pedidoCompleto.id = p.NroPedido.ToString();
            pedidoCompleto.displayId = p.NroPedido.ToString();
            pedidoCompleto.createdAt = p.DataHoraPedido;
            pedidoCompleto.orderTiming = p.Agendamento == 1 ? "SCHEDULED" : "IMMEDIATE"; // Valor fixo de exemplo
            pedidoCompleto.orderType = p.Retira == 1 ? "TAKEOUT" : "DELIVERY";

            string? dataLimite = "";
            string? DeliveryBy = "";

            if (p.Retira != 1)
            {
                using (ApplicationDbContext db = await _Db.GetContextoAsync())
                {

                    DeliveryBy = "MERCHANT";
                    dataLimite = DateTime.Parse(p.DataHoraPedido).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoEntrega).ToString();

                }
            }

            if (p.Retira == 1)
            {
                using (ApplicationDbContext db = await _Db.GetContextoAsync())
                {
                    DeliveryBy = "RETIRADA";
                    dataLimite = DateTime.Parse(p.DataHoraPedido).AddMinutes(db.parametrosdosistema.FirstOrDefault().TempoRetirada).ToString();
                }
            }

            //if (p.Return.Type == "INDOOR")
            // {
            //    DeliveryBy = "MESA";
            //  dataLimite = p.Return.Indoor.IndoorDateTime;
            // }

            pedidoCompleto.delivery.deliveredBy = DeliveryBy;
            pedidoCompleto.delivery.deliveryDateTime = dataLimite;
            pedidoCompleto.customer.id = p.Cliente.Codigo.ToString();
            pedidoCompleto.customer.name = p.Cliente.Nome;
            pedidoCompleto.customer.documentNumber = p.Cliente.Telefone;
            pedidoCompleto.salesChannel = "CCM"; // Valor fixo de exemplo

            return pedidoCompleto;

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao Converter Pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return pedidoCompleto;
    }



    public async Task<HttpResponseMessage> RequisicaoHttp(string? url = null, string? metodo = null, string content = "", int numPedido = 0)
    {
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
            string TokenCCM = "";

            using (ApplicationDbContext db = await _Db.GetContextoAsync())
            {
                var Config = db.parametrosdosistema.FirstOrDefault();
                TokenCCM = Config.TokenCCM;
            }

            if (metodo == "GET")
            {
                using HttpClient client = new HttpClient();
                string Newurl = url += $"?token={TokenCCM}";

                response = await client.GetAsync(Newurl);

                return response;
            }

            if (metodo == "POST")
            {
                using HttpClient client = new HttpClient();
                string Newurl = url += $"?token={TokenCCM}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "LIMPAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm_v2.php?token={TokenCCM}&import={numPedido}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "ACEITAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token={TokenCCM}&funcao=aceitarPedido&pedido={numPedido}&msg={content}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "RECUSAPEDIDO")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token={TokenCCM}&funcao=recusarPedido&pedido={numPedido}&msg={content}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;
            }

            if (metodo == "PING")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm.php?token={TokenCCM}&funcao=activePing&codFilial=1&primeiraVerificacao=1";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.GetAsync(Newurl);

                return response;

            }

            if (metodo == "MSGCLIENTEACEITE")
            {
                using HttpClient client = new HttpClient();
                string Newurl = $"http://api.ccmpedidoonline.com.br/wsccm_v2.php?token={TokenCCM}&funcao=pushCliente&msgPush={content}&codCliente={url}";

                StringContent contentToPost = new StringContent(content, Encoding.UTF8, "application/json");

                response = await client.PostAsync(Newurl, contentToPost);

                return response;

            }

        }//HAZZRWXX5GWYBNQ1BZXBQNK8WP5P6CQT  //uhux4nqnp-89p3p4o4n0a3n2j7a2q8r5n0q1o881 /on
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro ao enviar Req CCM");
        }
        return response;
    }

}
