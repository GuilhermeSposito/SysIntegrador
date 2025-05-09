using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using System.Net.Http.Json;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;
using Microsoft.VisualBasic.Devices;
using System.Globalization;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using SysIntegradorApp.Forms;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class GarcomSysMenu
{
    private readonly IMeuContexto _Context;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    public GarcomSysMenu(MeuContexto contexto)
    {
        _Context = contexto;
    }

    public async Task Pooling()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                List<ApoioAppGarcom> apoioAppGarcoms = dbPostgres.apoioappgarcom.Where(x => x.Processado == false && x.Obs == null).ToList();

                foreach (var apoio in apoioAppGarcoms)
                {
                    if (apoio.Tipo == "pedido")
                    {
                        bool PedidoProcessado = await SetPedido(apoio.PedidoJson, apoio.Id);
                    }

                    if (apoio.Tipo == "FECHAMENTO")
                    {
                        await FechaMesa(apoio.PedidoJson, apoio.Id);
                    }

                    if (apoio.Tipo == "AVISO")
                    {
                        await GeraAviso(apoio.PedidoJson, apoio.Id);
                    }

                }

            }

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", $"{ex.Message}", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }
    private async Task GeraAviso(string? aviso, int IdDoApoio)
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                var ApoioTabela = db.apoioappgarcom.FirstOrDefault(x => x.Id == IdDoApoio);
                if (ApoioTabela is not null)
                {
                    ApoioTabela.Processado = true;
                    await db.SaveChangesAsync();

                }

                ClsSons.PlaySom2();
                await SysAlerta.Alerta("Aviso", aviso, SysAlertaTipo.Alerta, SysAlertaButtons.Ok);
                ClsSons.StopSom();

            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", $"{ex.Message}", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async Task FechaMesa(string? JsonDeFechamento, int idDoAPoio)
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu.ClsSuporteDeFechamentoDeMesa? clsApoioFechamanetoDeMesa = JsonConvert.DeserializeObject<ClsSuporteDeFechamentoDeMesa>(JsonDeFechamento);

                string? NumeroMesa = clsApoioFechamanetoDeMesa!.NumeroMesaOuComanda;

                var Contas = db.contas.Where(x => x.Mesa == NumeroMesa && x.Status == "A").ToList();

                if (Contas is not null && Contas.Count > 0)
                {
                    var ApoioTabela = db.apoioappgarcom.FirstOrDefault(x => x.Id == idDoAPoio);
                    if (ApoioTabela is not null)
                    {
                        ApoioTabela.Processado = true;
                        await db.SaveChangesAsync();

                    }

                    var Conta = Contas.FirstOrDefault();

                    DateTime HorarioAtual = DateTime.Now.ToUniversalTime();
                    string? DataSaida = HorarioAtual.ToString()!.Substring(0, 10).Replace("-", "/");
                    string? HoraSaida = HorarioAtual.ToString()!.Substring(11, 5);


                    int insertNoSysMenuConta = await ClsDeIntegracaoSys.IntegracaoSequencia(
                                 mesa: NumeroMesa,
                                 cortesia: 0f,
                                 taxaEntrega: 0f,
                                 taxaMotoboy: 0f,
                                 dtInicio: Conta!.DataInicio!.Substring(0, 10),
                                 hrInicio: Conta.HoraInicio,
                                 contatoNome: " ",
                                 usuario: "ADMIN",
                                 dataSaida: DataSaida,
                                 hrSaida: HoraSaida,
                                 obsConta1: " ",
                                 iFoodPedidoID: Conta!.Id.ToString(),
                                 obsConta2: " ",
                                 referencia: " ",
                                 endEntrega: " ",
                                 bairEntrega: " ",
                                 entregador: " ",
                                 eIfood: false,
                                 telefone: " ",
                                 status: "F",
                                 Couvert: clsApoioFechamanetoDeMesa.ValorCouvert
                                 ); //fim dos parâmetros do método de integração

                    string? mesa = Contas.FirstOrDefault()!.Mesa;

                    float ValorTotal = 0f;
                    foreach (var item in Contas)
                    {
                        ValorTotal += Convert.ToSingle(item.ValorTotal);
                    }
                    float TaxaDeServico = ClsDeIntegracaoSys.TaxaDeGarcom(ValorTotal, mesa);

                    await AtualizaTaxaDeServicoNoSequencia(insertNoSysMenuConta, TaxaDeServico);


                    await AtualizaNumeroContaNoSequencia(insertNoSysMenuConta, NumeroMesa!);
                    await AtualizarContas();

                    ImpressaoGarcom.ChamaImpessaoDeFechamento(JsonDeFechamento, insertNoSysMenuConta);

                }

            }

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", $"{ex.Message}", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async Task AtualizaTaxaDeServicoNoSequencia(int NumConta, float ValorTaxa)
    {
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = await dbPostgres.parametrosdosistema.FirstOrDefaultAsync();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;


            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {

                connection.Open();
                string updateQuery = $"UPDATE Sequencia SET SERVICO = @NovoValor WHERE CONTA = @CONDICAO";

                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    // Definindo os parâmetros para a instrução SQL
                    command.Parameters.AddWithValue($"@NovoValor", ValorTaxa);
                    command.Parameters.AddWithValue("@CONDICAO", NumConta);

                    // Executando o comando UPDATE
                    command.ExecuteNonQuery();
                }

            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private async Task AtualizaNumeroContaNoSequencia(int NumConta, string Mesa)
    {
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = await dbPostgres.parametrosdosistema.FirstOrDefaultAsync();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;


            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {

                connection.Open();
                string updateQuery = $"UPDATE Contas SET CONTA = @NovoValor WHERE MESA = @CONDICAO AND STATUS = 'A';";

                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    // Definindo os parâmetros para a instrução SQL
                    command.Parameters.AddWithValue($"@NovoValor", NumConta);
                    command.Parameters.AddWithValue("@CONDICAO", Mesa);

                    // Executando o comando UPDATE
                    command.ExecuteNonQuery();
                }

                await Task.Delay(1000);

                await AtualizaStatusDaContaNoSequencia(NumConta, null);
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private async Task AtualizaStatusDaContaNoSequencia(int NumConta, string? Mesa)
    {
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = await dbPostgres.parametrosdosistema.FirstOrDefaultAsync();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;


            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {

                connection.Open();
                string updateQuery = $"UPDATE Contas SET STATUS = @NovoValor WHERE CONTA = @CONDICAO;";

                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    // Definindo os parâmetros para a instrução SQL
                    command.Parameters.AddWithValue($"@NovoValor", "F");
                    command.Parameters.AddWithValue("@CONDICAO", NumConta.ToString());

                    // Executando o comando UPDATE
                    command.ExecuteNonQuery();
                }

            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }


    private async Task<bool> SetPedido(string? PedidoJson, int IdDoPedidoNoDB)
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                int insertNoSysMenuConta = 0;
                var pedido = JsonConvert.DeserializeObject<ClassesAuxiliares.ClassesGarcomSysMenu.Pedido>(PedidoJson!);
                string? NomeCliente = " ";
                string? mesa = pedido!.Mesa is not null && pedido!.Mesa != "0000" ? pedido!.Mesa : pedido.Comanda;
                string? GarcomResponsavel = pedido.GarcomResponsavel;
                string? IdDoPedidoGuid = Guid.NewGuid().ToString();

                bool ExistePedido = true;

                if (pedido.IdPedido is not null)
                {
                    ExistePedido = await db.parametrosdopedido.AnyAsync(x => x.Id == pedido.IdPedido);
                    IdDoPedidoGuid = pedido.IdPedido;
                }
                else if (pedido.IdPedido is null)
                {
                    ExistePedido = false;
                }

                if (!ExistePedido)
                {
                    if (!String.IsNullOrEmpty(pedido.NomeClienteNaMesa))
                        NomeCliente = pedido.NomeClienteNaMesa;

                    if (mesa!.Length > 4 && !pedido.EBalcao && pedido.Comanda != "000000")
                    {
                        var COdMesa = db.mesas.FirstOrDefault(x => x.Cartao == mesa);

                        if (COdMesa is not null)
                            mesa = COdMesa.Codigo;
                        else
                            throw new Exception("Comanda não encontrada no sistema");

                    }
                    else if (pedido.EBalcao)
                    {
                        if (pedido.BalcaoInfos != null && pedido.BalcaoInfos.Repetido)
                        {
                            if (pedido.BalcaoInfos.Repetido)
                            {
                                mesa = pedido.BalcaoInfos.CodBalcao;
                                NomeCliente = pedido.BalcaoInfos.NomeCliente;
                            }
                        }
                        else
                        {
                            string? CodigoBalcao = "B";
                            int ContagemDeControles = 1;
                            List<Contas> contas = db.contas.Where(x => x.Status == "A" || x.Status == "F" && x.Mesa!.Contains(CodigoBalcao)).ToList();

                            foreach (var conta in contas)
                            {
                                var CodigoBalcaoCompletoInicial = $"{CodigoBalcao}{ContagemDeControles}";

                                if (contas.Any(x => x.Mesa == CodigoBalcaoCompletoInicial))
                                    ContagemDeControles++;

                            }

                            if (pedido.BalcaoInfos != null)
                            {
                                if (pedido.BalcaoInfos.NomeCliente is not null)
                                {
                                    NomeCliente = pedido.BalcaoInfos.NomeCliente;
                                }

                                pedido.BalcaoInfos.CodBalcao = $"{CodigoBalcao}{ContagemDeControles}";

                                var PedidoNATabela = db.apoioappgarcom.FirstOrDefault(x => x.Id == IdDoPedidoNoDB);
                                if (PedidoNATabela is not null)
                                {
                                    string? Json = JsonConvert.SerializeObject(pedido);
                                    PedidoNATabela.PedidoJson = Json;
                                    PedidoJson = Json;
                                    await db.SaveChangesAsync();
                                }
                            }

                            mesa = $"{CodigoBalcao}{ContagemDeControles}";
                        }

                    }

                    var Garcom = db.garcons.FirstOrDefault(x => x.Codigo == GarcomResponsavel);  // Se não encontrar o garçom, coloca o nome de Garcom
                    if (Garcom is not null)
                        GarcomResponsavel = Garcom.Nome;

                    int NumeroDeDisplayId = 0;
                    bool ConversaoNumMesa = int.TryParse(mesa, out int result);
                    if (ConversaoNumMesa)
                    {
                        NumeroDeDisplayId = result;
                    }

                    if (pedido.IdPedido is not null)
                    {
                        IdDoPedidoGuid = pedido.IdPedido;
                    }

                    var pedidoInserido = db.parametrosdopedido.Add(new ParametrosDoPedido()
                    {
                        Id = IdDoPedidoGuid,
                        Json = PedidoJson,
                        Situacao = "CONFIRMED",
                        Conta = insertNoSysMenuConta,
                        CriadoEm = DateTime.Now.ToString(),
                        DisplayId = NumeroDeDisplayId,
                        JsonPolling = "Sem Polling ID",
                        CriadoPor = "SYSMENU",
                        PesquisaDisplayId = NumeroDeDisplayId,
                        PesquisaNome = GarcomResponsavel
                    });
                    await db.SaveChangesAsync();

                    ClsDeSuporteAtualizarPanel.MudouDataBasePedido = true;
                    ClsDeSuporteAtualizarPanel.MudouDataBase = true;

                    foreach (var item in pedido.produtos)
                    {
                        var CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemGarcomSys(item, eIntegracao: true);
                        string? DataInicio = pedido.HorarioFeito!.ToString()!.Substring(0, 10).Replace("-", "/");
                        string? HoraInicio = pedido.HorarioFeito!.ToString()!.Substring(11, 5);

                        if (pedido.HorarioFeito.Contains("+00:00"))
                        {
                            DataInicio = "01/01/2000";
                            HoraInicio = "01:01";
                        }


                        ClsDeIntegracaoSys.IntegracaoContas(
                                   conta: insertNoSysMenuConta, //numero
                                   mesa: mesa, //texto curto 
                                   qtdade: item.Quantidade, //numero
                                   codCarda1: CaracteristicasPedido.ExternalCode1, //item.externalCode != null && item.options.Count() > 0 ? item.options[0].externalCode : "Test" , //texto curto 4 letras
                                   codCarda2: CaracteristicasPedido.ExternalCode2, //texto curto 4 letras
                                   codCarda3: CaracteristicasPedido.ExternalCode3, //texto curto 4 letras
                                   tamanho: CaracteristicasPedido.Tamanho, ////texto curto 1 letra
                                   descarda: CaracteristicasPedido.NomeProduto, // texto curto 31 letras
                                   valorUnit: CaracteristicasPedido.valorDoItem, //moeda
                                   valorTotal: CaracteristicasPedido.valorTotalDoItem, //moeda
                                   dataInicio: DataInicio, //data
                                   horaInicio: HoraInicio, //data
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
                                   cliente: NomeCliente, // texto curto 80 letras
                                   telefone: " ", // texto curto 14 letras
                                   impComanda: "Não",
                                   ImpComanda2: "Não",
                                   qtdComanda: 00f,
                                   status: "A",
                                   Requisicao: String.IsNullOrEmpty(item.Requisicao) ? " " : item.Requisicao.ToUpper(),
                                   HoraDeLancamentoDoItem: $"{item.Quantidade} Item-{HoraInicio}",
                                   garcom: pedido.GarcomResponsavel
                              );//fim dos parâmetros

                    }

                    var ApoioTabela = db.apoioappgarcom.FirstOrDefault(x => x.Id == IdDoPedidoNoDB);
                    if (ApoioTabela is not null)
                    {
                        ApoioTabela.Processado = true;
                        await db.SaveChangesAsync();
                    }

                    ImpressaoGarcom.ChamaImpessoes(PedidoJson);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
            if (!IsMessageBoxOpen("Erro Garçom"))
            {
                await using (ApplicationDbContext db = await _Context.GetContextoAsync())
                {
                    var PedidoComErro = db.apoioappgarcom.FirstOrDefault(x => x.Id == IdDoPedidoNoDB);

                    if (PedidoComErro is not null)
                    {
                        PedidoComErro.Obs = ex.Message;
                        db.SaveChanges();
                    }

                    MessageBox.Show(ex.ToString(), "Erro Garçom");
                }
            }

        }
        return false;
    }

    public void AtualizaPainelDePedidos()
    {
        if (ClsDeSuporteAtualizarPanel.MudouDataBase)
        {
            if (ClsDeSuporteAtualizarPanel.MudouDataBasePedido) //entra aqui só se foi pedido novo
            {
                ClsDeSuporteAtualizarPanel.MudouDataBasePedido = false;
            }

            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
            ClsDeSuporteAtualizarPanel.MudouDataBase = false;
        }
    }

    public async Task<PedidoCompleto> SysMenuPedidoCompleto(ClassesAuxiliares.ClassesGarcomSysMenu.Pedido p)
    {
        PedidoCompleto PedidoCompletoConvertido = new PedidoCompleto();
        try
        {
            using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                string GarcomResponsavel = p.GarcomResponsavel!;

                var Garcom = db.garcons.FirstOrDefault(x => x.Codigo == GarcomResponsavel);  // Se não encontrar o garçom, coloca o nome de Garcom
                if (Garcom is not null)
                    GarcomResponsavel = Garcom.Nome!;

                string? DisplayId = p!.Mesa is not null && p!.Mesa != "0000" ? p!.Mesa : p.Comanda;

                if (p.EBalcao)
                {
                    DisplayId = "BALCÃO";

                    if (p.BalcaoInfos is not null)
                    {
                        if (p.BalcaoInfos.NomeCliente is not null)
                        {
                            if (!String.IsNullOrEmpty(p.BalcaoInfos.NomeCliente.Trim()))
                                GarcomResponsavel = p.BalcaoInfos.NomeCliente;
                            else
                                GarcomResponsavel = p.BalcaoInfos.CodBalcao!;
                        }
                        else
                        {
                            GarcomResponsavel = p.BalcaoInfos.CodBalcao!;
                        }
                    }
                }

                PedidoCompletoConvertido.CriadoPor = "SYSMENU";
                PedidoCompletoConvertido.JsonPolling = "{}";
                PedidoCompletoConvertido.id = p.Id;
                PedidoCompletoConvertido.displayId = DisplayId;
                PedidoCompletoConvertido.createdAt = p.HorarioFeito.ToString();
                PedidoCompletoConvertido.orderTiming = "IMEDIATE";
                PedidoCompletoConvertido.orderType = "MESA";

                string? dataLimite = DateTime.TryParse(p.HorarioFeito.ToString()!, out DateTime result) ? result.AddMinutes(30).ToString() : DateTime.Now.AddMinutes(30).ToString();
                string? DeliveryBy = "MESA";

                PedidoCompletoConvertido.delivery.deliveredBy = DeliveryBy;
                PedidoCompletoConvertido.delivery.deliveryDateTime = dataLimite;
                PedidoCompletoConvertido.customer.id = Guid.NewGuid().ToString();
                PedidoCompletoConvertido.customer.name = GarcomResponsavel;
                PedidoCompletoConvertido.customer.documentNumber = " ";
                PedidoCompletoConvertido.salesChannel = "SYSMENU";

                return PedidoCompletoConvertido;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
        return PedidoCompletoConvertido;
    }

    public async Task<List<ParametrosDoPedido>> GetPedidoGarcom(int? display_ID = null, string? pesquisaNome = null)
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

                        pedidosFromDb = db.parametrosdopedido.Where(x => x.DisplayId == display_ID && x.CriadoPor == "SYSMENU" || x.Conta == display_ID && x.CriadoPor == "SYSMENU").AsNoTracking().ToList();


                        return pedidosFromDb;
                    }
                }

                if (pesquisaNome != null)
                {
                    using (ApplicationDbContext db = await _Context.GetContextoAsync())
                    {

                        pedidosFromDb = db.parametrosdopedido.Where(x => (x.PesquisaNome.ToLower().Contains(pesquisaNome) || x.PesquisaNome.Contains(pesquisaNome) || x.PesquisaNome.ToUpper().Contains(pesquisaNome)) && x.CriadoPor == "SYSMENU").AsNoTracking().ToList();


                        return pedidosFromDb;
                    }
                }
            }
            else
            {
                using (ApplicationDbContext db = await _Context.GetContextoAsync())
                {

                    pedidosFromDb = db.parametrosdopedido.Where(x => x.CriadoPor == "SYSMENU").AsNoTracking().ToList();

                    return pedidosFromDb;
                }
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }

        return pedidosFromDb;
    }


    public async Task AtualizarListaDeGarcom()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteGarconNoPostgres = dbPostgres.garcons.Any();

                if (ExisteGarconNoPostgres)
                {
                    dbPostgres.garcons.RemoveRange(dbPostgres.garcons);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Garcon";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Garcom garcom = new Garcom();

                                garcom.Codigo = reader["CODIGO"].ToString();
                                garcom.Nome = reader["NOME"].ToString();
                                garcom.Senha = reader["SENHA"].ToString();
                                garcom.Valor = Convert.ToSingle(reader["VALOR"].ToString());

                                dbPostgres.garcons.Add(garcom);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarListaDePromocoes()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExistePromocoesNoPostgres = dbPostgres.promocoes.Any();

                if (ExistePromocoesNoPostgres)
                {
                    dbPostgres.promocoes.RemoveRange(dbPostgres.promocoes);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema!.CaminhodoBanco!.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Promocao";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Promocoes promocao = new Promocoes();

                                promocao.Dia = reader["DIA"].ToString();
                                promocao.codigo = reader["CODIGO"].ToString();
                                promocao.pvenda1 = Convert.ToSingle(reader["PVENDA1"].ToString());
                                promocao.pvenda2 = Convert.ToSingle(reader["PVENDA2"].ToString());
                                promocao.pvenda3 = Convert.ToSingle(reader["PVENDA3"].ToString());
                                promocao.Mesa = Convert.ToBoolean(reader["MESA"].ToString());

                                dbPostgres.promocoes.Add(promocao);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }

                await AtualizaValoresSeTiverPromocao();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarListaDeIncrementos()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteIncrementos = dbPostgres.incrementos.Any();

                if (ExisteIncrementos)
                {
                    dbPostgres.incrementos.RemoveRange(dbPostgres.incrementos);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Incrementos";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Incremento incremento = new Incremento();

                                incremento.Codigo = reader["CODIGO"].ToString();
                                incremento.Descricao = reader["DESCRICAO"].ToString();
                                incremento.Valor = Convert.ToDouble(reader["VALOR"].ToString());
                                incremento.Tipo = reader["TIPO"].ToString();
                                incremento.VendaInternet = true;

                                dbPostgres.incrementos.Add(incremento);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarListaDeIncrementosCardapio()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteIncrementosCardapio = dbPostgres.incrementocardapio.Any();

                if (ExisteIncrementosCardapio)
                {
                    dbPostgres.incrementocardapio.RemoveRange(dbPostgres.incrementocardapio);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM IncrementoCardapio";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IncrementoCardapio incrementoCard = new IncrementoCardapio();

                                incrementoCard.Incremento = reader["INCREMENTO"].ToString();
                                incrementoCard.CodCardapio = reader["CODCARDA"].ToString();


                                dbPostgres.incrementocardapio.Add(incrementoCard);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarProdutos()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteProdutosNoPostgres = dbPostgres.cardapio.Any();

                if (ExisteProdutosNoPostgres)
                {
                    dbPostgres.cardapio.RemoveRange(dbPostgres.cardapio);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Cardapio";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Produto produto = new Produto();

                                produto.Codigo = reader["CODIGO"].ToString();
                                produto.Descricao = reader["DESCRICAO"].ToString();
                                produto.Grupo = reader["GRUPO"].ToString();
                                produto.Fracionado = reader["FRACIONADO"].ToString();
                                produto.TamanhoUnico = reader["TAMUNICO"].ToString();
                                produto.Preco1 = Convert.ToSingle(reader["PVENDA1"].ToString());
                                produto.Preco2 = Convert.ToSingle(reader["PVENDA2"].ToString());
                                produto.Preco3 = Convert.ToSingle(reader["PVENDA3"].ToString());
                                produto.OcultaTablet = Convert.ToBoolean(reader["OCULTATABLET"].ToString());

                                dbPostgres.cardapio.Add(produto);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarMesas()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteProdutosNoPostgres = dbPostgres.mesas.Any();

                if (ExisteProdutosNoPostgres)
                {
                    dbPostgres.mesas.RemoveRange(dbPostgres.mesas);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Mesas WHERE TIPO = 'M' OR CODIGO LIKE '%B%'"; // OR TIPO = 'B'

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ClassesAuxiliares.ClassesGarcomSysMenu.Mesa mesa = new();

                                mesa.Codigo = reader["CODIGO"].ToString();
                                mesa.Praca = reader["PRACA"].ToString();
                                mesa.Tipo = reader["TIPO"].ToString();
                                mesa.status = reader["STATUS"].ToString();
                                mesa.Cartao = reader["CARTAO"].ToString();
                                mesa.Taxa = Convert.ToBoolean(reader["TAXA"].ToString());
                                mesa.Bloqueado = Convert.ToBoolean(reader["BLOQUEADO"].ToString());
                                mesa.Consumacao = Convert.ToSingle(reader["CONSUMACAO"].ToString());
                                mesa.Vip = Convert.ToBoolean(reader["VIP"].ToString());

                                dbPostgres.mesas.Add(mesa);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarContas()
    {
        try
        {

            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteDadosNoContaDoPostgres = dbPostgres.contas.Any();

                if (ExisteDadosNoContaDoPostgres)
                {
                    dbPostgres.contas.RemoveRange(dbPostgres.contas);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema!.CaminhodoBanco;

                string SqlSelectIntoContas = $"SELECT * FROM Contas";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoContas, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contas Conta = new Contas();

                                Contas conta = new Contas();

                                conta.Conta = reader["CONTA"].ToString();
                                conta.Mesa = reader["MESA"].ToString();
                                conta.Qtdade = float.Parse(reader["QTDADE"].ToString());
                                conta.CodCarda1 = reader["CODCARDA1"].ToString();
                                conta.CodCarda2 = reader["CODCARDA2"].ToString();
                                conta.CodCarda3 = reader["CODCARDA3"].ToString();
                                conta.Tamanho = reader["TAMANHO"].ToString();
                                conta.Descarda = reader["DESCARDA"].ToString();
                                conta.ValorUnit = reader["VALORUNIT"].ToString();
                                conta.ValorTotal = reader["VALORTOTAL"].ToString();
                                conta.DataInicio = reader["DATAINICIO"].ToString();
                                conta.HoraInicio = reader["HORAINICIO"].ToString();
                                conta.Obs1 = reader["OBS1"].ToString();
                                conta.Obs2 = reader["OBS2"].ToString();
                                conta.Obs3 = reader["OBS3"].ToString();
                                conta.Obs4 = reader["OBS4"].ToString();
                                conta.Obs5 = reader["OBS5"].ToString();
                                conta.Obs6 = reader["OBS6"].ToString();
                                conta.Obs7 = reader["OBS7"].ToString();
                                conta.Obs8 = reader["OBS8"].ToString();
                                conta.Obs9 = reader["OBS9"].ToString();
                                conta.Obs10 = reader["OBS10"].ToString();
                                conta.Obs11 = reader["OBS11"].ToString();
                                conta.Obs12 = reader["OBS12"].ToString();
                                conta.Obs13 = reader["OBS13"].ToString();
                                conta.Obs14 = reader["OBS14"].ToString();
                                conta.Obs15 = reader["OBS15"].ToString();
                                conta.Cliente = reader["CLIENTE"].ToString();
                                conta.Requisicao = reader["REQUISICAO"].ToString();
                                conta.Status = reader["STATUS"].ToString();
                                conta.Telefone = reader["TELEFONE"].ToString();
                                conta.ImpComanda = reader["IMPCOMANDA"].ToString();
                                conta.ImpComanda2 = reader["IMPCOMANDA2"].ToString();
                                conta.QtdComanda = Convert.ToSingle(reader["QTDCOMANDA"].ToString());
                                conta.Usuario = reader["USUARIO"].ToString();

                                dbPostgres.contas.Add(conta);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", $"{ex.Message}", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizaGrupos()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteProdutosNoPostgres = dbPostgres.grupos.Any();

                if (ExisteProdutosNoPostgres)
                {
                    dbPostgres.grupos.RemoveRange(dbPostgres.grupos);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM GruCard";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Grupo grupo = new Grupo();

                                grupo.Codigo = reader["CODIGO"].ToString();
                                grupo.Descricao = reader["DESCRICAO"].ToString();
                                grupo.Familia = reader["FAMILIA"].ToString();
                                grupo.Oculta = Convert.ToBoolean(reader["OCULTATABLET"].ToString());
                                grupo.TOTGRUPO = Convert.ToDouble(reader["TOTGRUPO"].ToString());


                                dbPostgres.grupos.Add(grupo);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizaValoresSeTiverPromocao()
    {
        try
        {
            string? DiaDaSemana = RetornaDiaDaSemana();
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                var Promocoes = await dbPostgres.promocoes.ToListAsync();
                if (Promocoes is not null)
                {
                    bool ExistePromocaoNoDIa = Promocoes.Any(x => DiaDaSemana.Contains(x.Dia, StringComparison.OrdinalIgnoreCase));
                    if (ExistePromocaoNoDIa)
                    {
                        var PromocoesValidas = Promocoes.Where(x => DiaDaSemana.Contains(x.Dia, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (PromocoesValidas is not null)
                        {
                            foreach (var promocao in PromocoesValidas)
                            {
                                if (promocao.Mesa)
                                {
                                    Produto? produto = await dbPostgres.cardapio.FirstOrDefaultAsync(x => x.Codigo == promocao.codigo);
                                    if (produto is not null)
                                    {
                                        produto.Preco1 = promocao.pvenda1;
                                        produto.Preco2 = promocao.pvenda2;
                                        produto.Preco3 = promocao.pvenda3;

                                        await dbPostgres.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizaSetup()
    {
        try
        {
            string? DiaDaSemana = RetornaDiaDaSemana();

            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteSetupNoPostgres = dbPostgres.setup.Any();

                if (ExisteSetupNoPostgres)
                {
                    dbPostgres.setup.RemoveRange(dbPostgres.setup);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "SETUP");

                string SqlSelectIntoCadastros = $"SELECT * FROM Config";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Setup setup = new Setup();

                                setup.CouvertValor = Convert.ToSingle(reader["COUVERT"].ToString());
                                setup.CouvertDom = Convert.ToBoolean(reader["COUVERTDOM"].ToString());
                                setup.CouvertSeg = Convert.ToBoolean(reader["COUVERTSEG"].ToString());
                                setup.CouvertTer = Convert.ToBoolean(reader["COUVERTTER"].ToString());
                                setup.CouvertQuar = Convert.ToBoolean(reader["COUVERTQUA"].ToString());
                                setup.CouvertQuin = Convert.ToBoolean(reader["COUVERTQUI"].ToString());
                                setup.CouvertSex = Convert.ToBoolean(reader["COUVERTSEX"].ToString());
                                setup.CouvertSab = Convert.ToBoolean(reader["COUVERTSAB"].ToString());

                                if (DiaDaSemana.Contains("dom"))
                                {
                                    if (setup.CouvertDom)
                                        setup.CouvertHoje = true;
                                }

                                if (DiaDaSemana.Contains("seg"))
                                {
                                    if (setup.CouvertSeg)
                                        setup.CouvertHoje = true;
                                }

                                if (DiaDaSemana.Contains("ter"))
                                {
                                    if (setup.CouvertTer)
                                        setup.CouvertHoje = true;
                                }

                                if (DiaDaSemana.Contains("quar"))
                                {
                                    if (setup.CouvertQuar)
                                        setup.CouvertHoje = true;
                                }

                                if (DiaDaSemana.Contains("quin"))
                                {
                                    if (setup.CouvertQuin)
                                        setup.CouvertHoje = true;
                                }

                                if (DiaDaSemana.Contains("sex"))
                                {
                                    if (setup.CouvertSex)
                                        setup.CouvertHoje = true;
                                }

                                if (DiaDaSemana.Contains("sab"))
                                {
                                    if (setup.CouvertSab)
                                        setup.CouvertHoje = true;
                                }

                                dbPostgres.setup.Add(setup);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }


    public string RetornaDiaDaSemana()
    {
        DateTime dataAtual = DateTime.Now;
        string diaDaSemanaPtBr = dataAtual.ToString("dddd", new CultureInfo("pt-BR"));

        return diaDaSemanaPtBr;
    }

    public bool IsMessageBoxOpen(string title)
    {
        IntPtr hWnd = FindWindow(null, title);
        return hWnd != IntPtr.Zero;
    }

    public async Task AtualizarBancoDeDadosParaOGarcon()
    {
        await AtualizarListaDeGarcom();
        await AtualizaGrupos();
        await AtualizarMesas();
        await AtualizarProdutos();
        await AtualizarContas();
        await AtualizarListaDeIncrementos();
        await AtualizarListaDeIncrementosCardapio();
        await AtualizarListaDePromocoes();
        await AtualizaSetup();
    }

}

