using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class AnotaAi
{
    private readonly IMeuContexto _Context;
    private string? UrlApi { get { return "https://api-parceiros.anota.ai/partnerauth"; } }
    private string? ApiToken { get; set; } = string.Empty;

    public AnotaAi(MeuContexto context)
    {
        _Context = context;
    }

    public async Task Pooling()
    {
        try
        {
            if (String.IsNullOrEmpty(ApiToken))
                await SetApiTokenAsync();  //Se o Token da api for Vazio, ele define um token.

            HttpResponseMessage? Response = await ReqConstructor(metodo: "GET", endpoint: "/ping/list?currentpage=1");

            if (Response is null)
                throw new Exception("Erro de conexão com o anotaai");

            if (!Response.IsSuccessStatusCode)
                throw new HttpRequestException(await Response.Content.ReadAsStringAsync());

            string? JsonString = await Response.Content.ReadAsStringAsync();
            PollingAnotaAI? ReturnFromApi = JsonConvert.DeserializeObject<PollingAnotaAI>(JsonString);

            if (ReturnFromApi.InfoAnotaAi.ListaDePedidos.Count() >= 100) //Entra nesse if caso venha mais de 100 pedidos, e faz a paginação 
            {
                HttpResponseMessage? ResponseSegundaPagina = await ReqConstructor(metodo: "GET", endpoint: "/ping/list?currentpage=2");

                if (ResponseSegundaPagina is null)
                    throw new Exception("Erro de conexão com o anotaai");

                if (!ResponseSegundaPagina.IsSuccessStatusCode)
                    throw new HttpRequestException(await Response.Content.ReadAsStringAsync());

                string? JsonStringSegundaPagina = await Response.Content.ReadAsStringAsync();
                PollingAnotaAI? ReturnFromApSegundaPagina = JsonConvert.DeserializeObject<PollingAnotaAI>(JsonStringSegundaPagina);

                ReturnFromApi.InfoAnotaAi.ListaDePedidos.AddRange(ReturnFromApSegundaPagina.InfoAnotaAi.ListaDePedidos);
            }

            foreach (var item in ReturnFromApi.InfoAnotaAi.ListaDePedidos)
            {
                switch (item.Check)
                {
                    case 0:
                        await SetPedido(item.IdPedido);                                    //Em análise 
                        break;
                    case 1:
                        await SetPedido(item.IdPedido);                                   //em produção 
                        break;
                    case 2:
                        await AtualizaStatusPedido(item.IdPedido, item.Check);            //pronto
                        break;
                    case 3:
                        await AtualizaStatusPedido(item.IdPedido, item.Check);            //Finalizado
                        break;
                    case 4:
                        await AtualizaStatusPedido(item.IdPedido, item.Check);            //Cancelado
                        break;
                    case 5:
                        await AtualizaStatusPedido(item.IdPedido, item.Check);           //Negado
                        break;
                    case 6:
                        await AtualizaStatusPedido(item.IdPedido, item.Check);           //Solicitação de cancelamento de pedido
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public async Task SetPedido(string? idPedido)
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {

                bool ExistePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == idPedido);

                if (!ExistePedido)
                {
                    using (HttpResponseMessage? SolicitaPedido = await ReqConstructor(metodo: "GET", endpoint: $"/ping/get/{idPedido}"))
                    {
                        ParametrosDoSistema? Configs = db.parametrosdosistema.ToList().FirstOrDefault();

                        int insertNoSysMenuConta = 0;
                        string? mesa = " ";
                        string? DataCertaEntregarEm = " ";
                        string? Complemento = " ";
                        string? EndEntrega = " ";
                        string? BairEntrega = " ";
                        string? Entregador = " ";
                        string? Status = " ";
                        float valorEntrega = 0.0f;

                        if (SolicitaPedido is null)
                            throw new NullReferenceException("Erro ao encontrar pedido");

                        if ((int)SolicitaPedido.StatusCode != 200)
                            throw new Exception($"Erro ao solicitar pedido, Stataus: {(int)SolicitaPedido.StatusCode}");

                        PedidoAnotaAi? Pedido = JsonConvert.DeserializeObject<PedidoAnotaAi>(await SolicitaPedido.Content.ReadAsStringAsync());
                        if (Configs.IntegracaoSysMenu)
                        {
                            if (Pedido.InfoDoPedido.Type == "DELIVERY")
                            {
                                mesa = "WEB";
                                DataCertaEntregarEm = DateTime.Parse(Pedido.InfoDoPedido.CreatedAt).AddMinutes(Configs.TempoEntrega).ToString();
                                Complemento = Pedido.InfoDoPedido.deliveryAddress.Complement is not null && Pedido.InfoDoPedido.deliveryAddress.Complement.Length > 0 ? Pedido.InfoDoPedido.deliveryAddress.Complement : " ";
                                EndEntrega = Pedido.InfoDoPedido.deliveryAddress.FormattedAddress;
                                BairEntrega = Pedido.InfoDoPedido.deliveryAddress.Neighborhood;
                                Status = "P";
                                valorEntrega = Convert.ToSingle(Pedido.InfoDoPedido.DeliveryFee);
                                Entregador = "00";
                            }

                            if (Pedido.InfoDoPedido.Type == "TAKE")
                            {
                                mesa = "WEBB";
                                DataCertaEntregarEm = DateTime.Parse(Pedido.InfoDoPedido.CreatedAt).AddMinutes(Configs.TempoRetirada).ToString();
                                Entregador = "RETIRADA";
                                Status = "P";
                            }

                            if (Pedido.InfoDoPedido.Type == "LOCAL")
                            {
                                mesa = await RetiraNumeroDeMesa(Pedido.InfoDoPedido.Pdv.Table);
                                DataCertaEntregarEm = DateTime.Parse(Pedido.InfoDoPedido.CreatedAt).AddMinutes(Configs.TempoRetirada).ToString();
                                Status = "A";
                                insertNoSysMenuConta = 999;
                            }

                            float ValorDescontosNum = 0.0f;
                            foreach (var item in Pedido.InfoDoPedido.Descontos)
                            {
                                ValorDescontosNum += item.Total;
                            }

                            if (Pedido.InfoDoPedido.Type != "LOCAL")
                            {
                                insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                                       mesa: mesa,
                                       cortesia: ValorDescontosNum,
                                       taxaEntrega: valorEntrega,
                                       taxaMotoboy: 0.00f,
                                       dtInicio: Pedido.InfoDoPedido.CreatedAt.ToString().Substring(0, 10),
                                        hrInicio: Pedido.InfoDoPedido.CreatedAt.ToString().Substring(11, 5),
                                        contatoNome: Pedido.InfoDoPedido.Customer.Nome,
                                        usuario: "CAIXA",
                                        dataSaida: DataCertaEntregarEm.ToString().Substring(0, 10),
                                       hrSaida: DataCertaEntregarEm.ToString().Substring(11, 5),
                                        obsConta1: " ",
                                       iFoodPedidoID: Pedido.InfoDoPedido.Id,
                                       obsConta2: " ",
                                       referencia: Complemento,
                                        endEntrega: EndEntrega,
                                        bairEntrega: BairEntrega,
                                        entregador: Entregador,
                                        eAnotaAi: true,
                                        telefone: Pedido.InfoDoPedido.Customer.Phone is not null && Pedido.InfoDoPedido.Customer.Phone.Length > 0 ? Pedido.InfoDoPedido.Customer.Phone : " "
                                        ); //fim dos parâmetros do método de integração

                                foreach (var pagamento in Pedido.InfoDoPedido.Payments)
                                {
                                    string type = Pedido.InfoDoPedido.Payments[0].Prepaid ? "ONLINE" : "OFFLINE";

                                    if (pagamento.Nome != "money")
                                        ClsDeIntegracaoSys.IntegracaoPagCartao(Pedido.InfoDoPedido.Payments[0].Nome, insertNoSysMenuConta, Pedido.InfoDoPedido.Total, type, "ANOTAAI");
                                }

                                SysIntegradorApp.ClassesAuxiliares.Payments payments = new();

                                foreach (var item in Pedido.InfoDoPedido.Payments)
                                {
                                    float changeForNovo = 0.0f;

                                    if (item.ChangeFor is not null)
                                    {
                                        changeForNovo = (float)item.ChangeFor;
                                    }

                                    float valor = float.Parse(item.value.Replace('.', ','));

                                    Cash SeForPagamentoEmDinherio = new Cash() { changeFor = changeForNovo };
                                    payments.methods.Add(new Methods() { method = item.Nome, value = valor, cash = SeForPagamentoEmDinherio });
                                }

                                ClsDeIntegracaoSys.UpdateMeiosDePagamentosSequencia(payments, insertNoSysMenuConta);
                            }
                        }

                        await db.parametrosdopedido.AddAsync(new ParametrosDoPedido()
                        {
                            Id = idPedido,
                            Json = await SolicitaPedido.Content.ReadAsStringAsync(),
                            Situacao = "CONFIRMED",
                            Conta = insertNoSysMenuConta,
                            CriadoEm = DateTimeOffset.Now.ToString(),
                            DisplayId = Pedido.InfoDoPedido.ShortReference,
                            JsonPolling = "Sem Pooling",
                            CriadoPor = "ANOTAAI",
                            PesquisaDisplayId = Pedido.InfoDoPedido.ShortReference,
                            PesquisaNome = Pedido.InfoDoPedido.Customer.Nome
                        });

                        if (Pedido.InfoDoPedido.Type == "LOCAL")
                        {
                            insertNoSysMenuConta = 0;
                        }

                        if (Configs.IntegracaoSysMenu)
                        {
                            string? NumeroCorrigido = RetornaNumeroDeTelefoneCorrigido(Pedido.InfoDoPedido.Customer.Phone); //telefone convertido para o padrão sysmenu

                            if (Pedido.InfoDoPedido.Type == "DELIVERY")
                            {
                                bool existeCliente = ClsDeIntegracaoSys.ProcuraCliente(NumeroCorrigido);
                                if (!existeCliente)
                                    ClsDeIntegracaoSys.CadastraClienteAnotaAi(Pedido.InfoDoPedido.Customer, Pedido.InfoDoPedido.deliveryAddress, TelefoneCliente: NumeroCorrigido);
                            }

                            foreach (var item in Pedido.InfoDoPedido.Items)
                            {
                                var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAnotaAi(item, eIntegracao: true);

                                ClsDeIntegracaoSys.IntegracaoContas(
                                          conta: insertNoSysMenuConta, //numero
                                          mesa: mesa, //texto curto 
                                          qtdade: item.quantity, //numero
                                          codCarda1: CaracteristicasPedido.ExternalCode1, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                          codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                          codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                          tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                          descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                          valorUnit: item.Total / item.quantity, //moeda
                                          valorTotal: item.Total, //moeda
                                          dataInicio: Pedido.InfoDoPedido.CreatedAt.Substring(0, 10).Replace("-", "/"), //data
                                          horaInicio: Pedido.InfoDoPedido.CreatedAt.Substring(11, 5), //data
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
                                          cliente: Pedido.InfoDoPedido.Customer.Phone is not null && Pedido.InfoDoPedido.Customer.Phone.Length > 0 ? Pedido.InfoDoPedido.Customer.Phone : " ", // texto curto 80 letras
                                          telefone: NumeroCorrigido,
                                          impComanda: "Não",
                                          ImpComanda2: "Não",
                                          qtdComanda: 00f,//numero duplo 
                                          status: Status
                                     );//fim dos parâmetros

                            }

                        }

                        await db.SaveChangesAsync();

                        await ConfirmaPedido(idPedido);

                        ClsSons.PlaySom();
                        ClsDeSuporteAtualizarPanel.MudouDataBase = true;

                        if (Configs.ImpressaoAut)
                            ImpressaoAnotaAi.ChamaImpressoes(idPedido);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public string? RetornaNumeroDeTelefoneCorrigido(string? telefone)
    {
        if (telefone is not null && telefone.Length > 0)
        {
            string? CodArea = telefone.Substring(0, 2); //16
            string? Primeironumero = telefone.Substring(2, 5);
            string? SegundoNumero = telefone.Substring(7);

            return $"({CodArea}){Primeironumero}-{SegundoNumero}";
        }

        return " ";
    }

    public async Task<string> RetiraNumeroDeMesa(string? place)
    {
        string? numeroMesa = " ";
        try
        {
            numeroMesa = place.Substring(4, 2).Trim();

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

    public async Task ConfirmaPedido(string? orderId)
    {
        try
        {
            HttpResponseMessage? reponse = await ReqConstructor(metodo: "POST", endpoint: $"/order/accept/{orderId}");

            if (reponse is null || !reponse.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro Ao Confirmar Pedido, Status: {(int)reponse.StatusCode}");
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task<bool> DespachaPedido(string? orderId)
    {
        try
        {
            HttpResponseMessage? reponse = await ReqConstructor(metodo: "POST", endpoint: $"/order/ready/{orderId}");

            if (reponse is null || !reponse.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro Ao Despachar Pedido, Status: {(int)reponse.StatusCode}");

            return true;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
        return false;
    }

    public async Task<bool> FinalizaPedido(string? orderId)
    {
        try
        {
            HttpResponseMessage? reponse = await ReqConstructor(metodo: "POST", endpoint: $"/order/finalize/{orderId}");

            if (reponse is null || !reponse.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro Ao Finalizar Pedido, Status: {(int)reponse.StatusCode}");

            return true;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
        return false;
    }

    public async Task<bool> CancelaPedido(string? orderId, string? motivoCancelamento)
    {
        try
        {
            HttpResponseMessage? response = await ReqConstructor(metodo: "POST", endpoint: $"/order/cancel/{orderId}", content: motivoCancelamento);

            if (response is null || !response.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro Ao Cancelar Pedido, Status: {(int)response.StatusCode}");

            MessageBox.Show(await response.Content.ReadAsStringAsync());

            return true;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
        return false;
    }

    public async Task<IEnumerable<ParametrosDoPedido?>> GetPedidosAsync(int? display_ID = null, string? pesquisaNome = null)
    {
        try
        {
            if (display_ID != null || pesquisaNome != null)
            {
                if (display_ID != null)
                {
                    using (ApplicationDbContext db = await _Context.GetContextoAsync())
                    {

                        List<ParametrosDoPedido> pedidosFromDb = db.parametrosdopedido.Where(x => x.PesquisaDisplayId == display_ID && x.CriadoPor == "ANOTAAI" || x.Conta == display_ID && x.CriadoPor == "ANOTAAI").AsNoTracking().ToList();

                        return pedidosFromDb;
                    }
                }

                if (pesquisaNome != null)
                {

                    using (ApplicationDbContext db = await _Context.GetContextoAsync())
                    {

                        List<ParametrosDoPedido> pedidosFromDb = db.parametrosdopedido.Where(x => (x.PesquisaNome.ToLower().Contains(pesquisaNome) || x.PesquisaNome.Contains(pesquisaNome) || x.PesquisaNome.ToUpper().Contains(pesquisaNome)) && x.CriadoPor == "ANOTAAI").AsNoTracking().ToList();

                        return pedidosFromDb;
                    }
                }
            }
            else
            {
                using (ApplicationDbContext db = await _Context.GetContextoAsync())
                {
                    List<ParametrosDoPedido> pedidosFromDb = db.parametrosdopedido.Where(x => x.CriadoPor == "ANOTAAI").AsNoTracking().ToList();

                    return pedidosFromDb;
                }

            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
        return null;
    }

    public async Task<PedidoCompleto?> AnotaAiPedidoCompleto(PedidoAnotaAi p)
    {
        PedidoCompleto pedidoCompleto = new PedidoCompleto();
        try
        {
            string TipoDoPedido = "";

            pedidoCompleto.CriadoPor = "ANOTAAI"; // Valor fixo de exemplo
            pedidoCompleto.JsonPolling = "{}"; // Valor fixo de exemplo
            pedidoCompleto.id = p.InfoDoPedido.IdPedido;
            pedidoCompleto.displayId = p.InfoDoPedido.ShortReference.ToString();
            pedidoCompleto.createdAt = p.InfoDoPedido.CreatedAt;
            pedidoCompleto.orderTiming = "IMMEDIATE"; // Valor fixo de exemplo

            if (p.InfoDoPedido.Schedule_Order is not null)
            {
                pedidoCompleto.orderTiming = "SCHEDULED";
            }

            string? dataLimite = "";
            string? DeliveryBy = "";

            if (p.InfoDoPedido.Type == "DELIVERY")
            {
                using (ApplicationDbContext db = await _Context.GetContextoAsync())
                {
                    ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();

                    DeliveryBy = "MERCHANT";
                    dataLimite = DateTime.Parse(p.InfoDoPedido.TimeMax).AddMinutes(Config.TempoEntrega).ToString();

                    TipoDoPedido = "DELIVERY";
                }
            }

            if (p.InfoDoPedido.Type == "TAKE")
            {
                using (ApplicationDbContext db = await _Context.GetContextoAsync())
                {
                    ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();

                    DeliveryBy = "RETIRADA";
                    dataLimite = DateTime.Parse(p.InfoDoPedido.TimeMax).AddMinutes(Config.TempoRetirada).ToString();

                    TipoDoPedido = "TAKEOUT";
                }
            }

            if (p.InfoDoPedido.Type == "LOCAL")
            {
                using (ApplicationDbContext db = await _Context.GetContextoAsync())
                {
                    ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();

                    DeliveryBy = "RETIRADA";
                    dataLimite = DateTime.Parse(p.InfoDoPedido.TimeMax).AddMinutes(Config.TempoRetirada).ToString();

                    TipoDoPedido = "PLACE";
                }
            }

            pedidoCompleto.orderType = TipoDoPedido;
            pedidoCompleto.delivery.deliveredBy = DeliveryBy;
            pedidoCompleto.delivery.deliveryDateTime = dataLimite;
            pedidoCompleto.customer.id = p.InfoDoPedido.Customer.id is not null ? p.InfoDoPedido.Customer.id.ToString() : "";
            pedidoCompleto.customer.name = p.InfoDoPedido.Customer.Nome;
            pedidoCompleto.customer.documentNumber = p.InfoDoPedido.Customer.Phone;
            pedidoCompleto.salesChannel = "ANOTAAI"; // Valor fixo de exemplo

            return pedidoCompleto;

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao Converter Pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await Logs.CriaLogDeErro(ex.ToString());

        }
        return null;

    }


    public async Task AtualizaStatusPedido(string? idPedido, int check)
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                bool ExistePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == idPedido);

                if (ExistePedido)
                {
                    bool MudouPedido = false;
                    ParametrosDoPedido? Pedido = db.parametrosdopedido.FirstOrDefault(x => x.Id == idPedido);

                    switch (check)
                    {
                        case 2:
                            if (Pedido.Situacao != "DISPATCHED")
                            {
                                Pedido.Situacao = "DISPATCHED";
                                MudouPedido = true;
                            }
                            break;
                        case 3:
                            if (Pedido.Situacao != "CONCLUDED")
                            {
                                Pedido.Situacao = "CONCLUDED";
                                MudouPedido = true;
                            }
                            break;
                        case 4:
                            if (Pedido.Situacao != "CANCELLED")
                            {
                                Pedido.Situacao = "CANCELLED";
                                MudouPedido = true;
                            }
                            break;
                        case 5:
                            if (Pedido.Situacao != "DENIED")
                            {
                                Pedido.Situacao = "DENIED";
                                MudouPedido = true;
                            }
                            break;
                        case 6:
                            if (Pedido.Situacao != "CANCELLED REQUEST")
                            {
                                Pedido.Situacao = "CANCELLED REQUEST";
                                MudouPedido = true;
                            }
                            break;
                        default:
                            MudouPedido = false;
                            break;
                    }

                    if (MudouPedido)
                    {
                        db.SaveChanges();

                        ClsDeSuporteAtualizarPanel.MudouDataBase = true;
                    }

                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public async Task SetApiTokenAsync()
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is null)
                    throw new NullReferenceException("Nenhuma instancia dos Parametros do Sistema no banco de dados foi encontrada");

                ApiToken = Config.TokenAnotaAi;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public async Task<HttpResponseMessage?> ReqConstructor(string? metodo, string? endpoint, string? content = "")
    {
        try
        {
            if (String.IsNullOrEmpty(ApiToken))
                await SetApiTokenAsync();  //Se o Token da api for Vazio, ele define um token.

            string url = UrlApi + endpoint;

            if (metodo == "GET")
            {
                using var requestClient = new HttpClient();
                requestClient.DefaultRequestHeaders.Add("Authorization", ApiToken);
                return await requestClient.GetAsync(url);
            }

            if (metodo == "POST")
            {
                using HttpClient requestClient = new HttpClient();
                requestClient.DefaultRequestHeaders.Add("Authorization", ApiToken);
                StringContent contentToReq = new StringContent(content, Encoding.UTF8, "application/json");

                return await requestClient.PostAsync(url, contentToReq);

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
        return null;
    }
}
