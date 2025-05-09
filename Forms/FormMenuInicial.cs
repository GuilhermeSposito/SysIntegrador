using ExCSS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.ClassesAuxiliares.Verificacoes;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.Forms;
using SysIntegradorApp.Forms.ONPEDIDO;
using SysIntegradorApp.Forms.TaxyMachine;
using SysIntegradorApp.UserControls;
using System.Data;
using System.Drawing.Drawing2D;
using System.Text;

namespace SysIntegradorApp;

public partial class FormMenuInicial : Form
{

    public readonly ApplicationDbContext _db;
    public GarcomSysMenu garcomSysMenu = new GarcomSysMenu(new MeuContexto());
    public bool IntegraComOGarcom = false;
    public int TempoDoPolling { get; set; } = 5000;
    public int ContadorDeClickLojaOnLine { get; set; } = 0;

    private System.Threading.Timer _timer;
    private System.Threading.Timer _timer2;
    private System.Threading.Timer _timer3;
    private WebView2 webViwer = new WebView2();
    public static int ContadorPooling { get; set; }

    public AnotaAi AnotaAi { get; set; } = new AnotaAi(new MeuContexto());

    public FormMenuInicial(ApplicationDbContext DB)
    {
        _db = DB;

        InitializeComponent();
        SetRoundedRegion(panelPedidos, 20);
        SetRoundedRegion(panelDetalhePedido, 20);
        SetRoundedRegion(panelDePaginas, 20);
        SetRoundedRegion(panel1, 20);
        SetRoundedRegion(PanelDeInfosOnLine, 20);

        this.Resize += (sender, e) =>
        {
            SetRoundedRegion(panelDePaginas, 20);
            SetRoundedRegion(panelPedidos, 20);
            SetRoundedRegion(panelDetalhePedido, 20);
            SetRoundedRegion(panel1, 20);
            SetRoundedRegion(PanelDeInfosOnLine, 20);
        };

        StartChat();
        SetarPanelPedidos();
        panelDetalhePedido.Controls.Clear();
        panelDetalhePedido.Controls.Add(labelDeAvisoPedidoDetalhe);

    }

    private void panelPedidos_Paint(object sender, PaintEventArgs e) { }

    private async void FormMenuInicial_Load(object sender, EventArgs e)
    {
        await DefineTempoDoPollingDoGarcom();
        await PostgresConfigs.LimpaPedidosACada8horas();

        _timer = new System.Threading.Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(30)); //Função que chama o pulling a cada 30 segundos 
        _timer3 = new System.Threading.Timer(PollingProGarcom, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(TempoDoPolling));
        SetarPanelPedidos();
    }

    private void FormMenuInicial_FormClosed(object sender, FormClosedEventArgs e)
    {
        Application.Exit();
    }

    private void FormMenuInicial_Shown(object sender, EventArgs e) { }


    private async Task DefineTempoDoPollingDoGarcom()
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Configuracoes = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Configuracoes!.IfoodMultiEmpresa)
                {
                    labelStatusLojaNM.Text = "Status De Lojas";
                    pictureBoxOnline.Visible = false;
                    pictureBoxOfline.Visible = false;
                }

                if (Configuracoes is not null)
                    TempoDoPolling = Configuracoes.TempoPollingGarcom * 1000;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }

    public static async void SetarPanelPedidos(int? pesquisaDisplayId = null, string? pesquisaNome = null)
    {
        try
        {
            checkBoxConcluido.Enabled = false;
            checkBoxConfirmados.Enabled = false;
            checkBoxDespachados.Enabled = false;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Configuracoes = db.parametrosdosistema.ToList().FirstOrDefault();

                List<PedidoCompleto> pedidos = new List<PedidoCompleto>(); //lista para colocar os pedidos do ifood

                if (Configuracoes!.IntegraGarcom)
                {
                    GarcomSysMenu SysMenu = new GarcomSysMenu(new MeuContexto());

                    IEnumerable<ParametrosDoPedido> PedidosGarcom = await SysMenu.GetPedidoGarcom(pesquisaDisplayId, pesquisaNome);
                    foreach (var item in PedidosGarcom)
                    {
                        var pedidoJsonConvertido = JsonConvert.DeserializeObject<Pedido>(item!.Json);
                        pedidoJsonConvertido!.Id = item.Id;
                        PedidoCompleto PedidoConvertido = await SysMenu.SysMenuPedidoCompleto(pedidoJsonConvertido);
                        PedidoConvertido.Situacao = item.Situacao;
                        PedidoConvertido.NumConta = item.Conta;
                        PedidoConvertido.CriadoPor = "SYSMENU";
                        pedidos.Add(PedidoConvertido);
                    }
                }



                if (Configuracoes.IntegraOnOPedido)
                {
                    OnPedido OnPedido = new OnPedido(new MeuContexto());

                    IEnumerable<ParametrosDoPedido> PedidosONPedidos = await OnPedido.GetPedidoOnPedido(pesquisaDisplayId, pesquisaNome);
                    foreach (var item in PedidosONPedidos)
                    {
                        var pedidoJsonConvertido = JsonConvert.DeserializeObject<PedidoOnPedido>(item.Json);
                        PedidoCompleto PedidoConvertido = await OnPedido.OnPedidoPedidoCompleto(pedidoJsonConvertido);
                        PedidoConvertido.Situacao = item.Situacao;
                        PedidoConvertido.NumConta = item.Conta;
                        PedidoConvertido.CriadoPor = "ONPEDIDO";
                        pedidos.Add(PedidoConvertido);
                    }
                }

                if (Configuracoes.IntegraDelMatch)
                {
                    DelMatch Delmatch = new DelMatch(new MeuContexto());

                    IEnumerable<ParametrosDoPedido> DelMatchPedidos = await Delmatch.GetPedidoDelMatch(pesquisaDisplayId, pesquisaNome);
                    foreach (var item in DelMatchPedidos)
                    {
                        var pedidoJsonConvertido = JsonConvert.DeserializeObject<PedidoDelMatch>(item.Json);
                        PedidoCompleto PedidoConvertido = await Delmatch.DelMatchPedidoCompleto(pedidoJsonConvertido);
                        PedidoConvertido.Situacao = item.Situacao;
                        PedidoConvertido.NumConta = item.Conta;
                        PedidoConvertido.CriadoPor = "DELMATCH";
                        pedidos.Add(PedidoConvertido);
                    }
                }

                if (Configuracoes.IntegraIfood)
                {
                    Ifood Ifood = new Ifood(new MeuContexto());
                    IEnumerable<ParametrosDoPedido> IfoodPedidos = await Ifood.GetPedido(pesquisaDisplayId, pesquisaNome);
                    foreach (ParametrosDoPedido item in IfoodPedidos)
                    {
                        PedidoCompleto? pedido = JsonConvert.DeserializeObject<PedidoCompleto>(item.Json);
                        pedido.CriadoPor = "IFOOD";
                        pedido.JsonPolling = item.JsonPolling;
                        pedido.Situacao = item.Situacao;
                        pedido.NumConta = item.Conta;
                        pedidos.Add(pedido);
                    }
                }

                if (Configuracoes.IntegraCCM)
                {
                    CCM CCMNEW = new CCM(new MeuContexto());
                    IEnumerable<ParametrosDoPedido> PedidosCCM = await CCMNEW.GetPedidos(pesquisaDisplayId, pesquisaNome);
                    foreach (ParametrosDoPedido item in PedidosCCM)
                    {
                        var pedidoXMl = await CCMNEW.RetornaPedido(item);
                        if (pedidoXMl != null)
                        {
                            PedidoCompleto PedidoConvertido = await CCMNEW.CCMPedidoCompleto(pedidoXMl);
                            PedidoConvertido.Situacao = item.Situacao;
                            PedidoConvertido.NumConta = item.Conta;
                            PedidoConvertido.CriadoPor = "CCM";
                            pedidos.Add(PedidoConvertido);

                        }
                    }
                }

                if (Configuracoes.IntegraAnotaAi)
                {
                    AnotaAi AnotaAiInstancia = new AnotaAi(new MeuContexto());
                    IEnumerable<ParametrosDoPedido?> PedidosAnotaAi = await AnotaAiInstancia.GetPedidosAsync(pesquisaDisplayId, pesquisaNome);

                    if (PedidosAnotaAi != null && PedidosAnotaAi.Count() > 0)
                        foreach (var pedido in PedidosAnotaAi)
                        {
                            PedidoAnotaAi? PedidoAnotaAi = JsonConvert.DeserializeObject<PedidoAnotaAi>(pedido.Json);

                            if (PedidoAnotaAi != null)
                            {
                                PedidoCompleto? PedidoConvertido = await AnotaAiInstancia.AnotaAiPedidoCompleto(PedidoAnotaAi);
                                PedidoConvertido.Situacao = pedido.Situacao;
                                PedidoConvertido.NumConta = pedido.Conta;
                                PedidoConvertido.CriadoPor = "ANOTAAI";
                                pedidos.Add(PedidoConvertido);
                            }
                        }

                }

                panelPedidos.Controls.Clear();

                IEnumerable<PedidoCompleto> pedidosOrdenado = pedidos.OrderByDescending(p =>
                {
                    DateTime.TryParse(p.createdAt, out DateTime result);
                    return result;
                });

                //Faz um loop para adicionar os UserControls De pedido no panel
                foreach (var item in pedidosOrdenado)
                {
                    if (!checkBoxConcluido.Checked && item.Situacao == "DELIVERED" && pesquisaDisplayId is null)
                        continue;

                    if (!checkBoxConcluido.Checked && item.Situacao == "CONCLUDED" && pesquisaDisplayId is null)
                        continue;

                    if (!checkBoxConfirmados.Checked && item.Situacao == "CONFIRMED" && pesquisaDisplayId is null)
                        continue;

                    if (!checkBoxDespachados.Checked && item.Situacao == "DISPATCHED" && pesquisaDisplayId is null)
                        continue;

                    if (!checkBoxMesas.Checked && item.orderType == "INDOOR" && pesquisaDisplayId is null)
                        continue;


                    if (item.CriadoPor == "SYSMENU")
                    {
                        if (!checkBoxMesas.Checked && pesquisaDisplayId is null)
                            continue;


                        if (Configuracoes.IntegraGarcom)
                        {
                            string horarioCorrigido = "";

                            DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime!);
                            horarioCorrigido = HorarioMudado.ToString();

                            UCPedido UserControlPedido = new UCPedido()
                            {
                                Pedido = item,
                                Id_pedido = item.id,
                                OrderType = item.orderType,
                                Display_id = item.displayId,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                NomePedido = item.customer.name,
                                DeliveryBy = "RETIRADA",
                                FeitoAs = item.createdAt,
                                HorarioEntrega = horarioCorrigido,//item.delivery.deliveryDateTime,
                                LocalizadorPedido = "RETIRADA",
                                EnderecoFormatado = "RETIRADA",
                                Bairro = "RETIRADA",
                                TipoDaEntrega = "RETIRADA",
                                ValorTotalItens = item.total.subTotal,
                                ValorTaxaDeentrega = item.total.deliveryFee,
                                Valortaxaadicional = item.total.additionalFees,
                                Descontos = item.total.benefits,
                                TotalDoPedido = item.total.orderAmount,
                                // Observations = item.delivery.observations,
                                items = item.items,
                            };


                            UserControlPedido.MudaPictureBoxSysGarcom(UserControlPedido);
                            UserControlPedido.SetLabels(item.id!, item.displayId!, item.customer.name!, horarioCorrigido, item.Situacao!); // aqui muda as labels do user control para cada pedido em questão

                            panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                        }
                    }



                    if (item.CriadoPor == "ANOTAAI")
                    {
                        if (Configuracoes.IntegraAnotaAi)
                        {
                            string horarioCorrigido = "";

                            if (item.orderType == "DELIVERY")
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime);
                                horarioCorrigido = HorarioMudado.ToString();
                            }
                            else if (item.orderType == "TAKEOUT")
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime);
                                horarioCorrigido = HorarioMudado.ToString();
                            }
                            else
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime);
                                horarioCorrigido = HorarioMudado.ToString();
                            }

                            UCPedido UserControlPedido = new UCPedido()
                            {
                                Pedido = item,
                                Id_pedido = item.id,
                                OrderType = item.orderType,
                                Display_id = item.displayId,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                NomePedido = item.customer.name,
                                DeliveryBy = "RETIRADA",
                                FeitoAs = item.createdAt,
                                HorarioEntrega = horarioCorrigido,//item.delivery.deliveryDateTime,
                                LocalizadorPedido = "RETIRADA",
                                EnderecoFormatado = "RETIRADA",
                                Bairro = "RETIRADA",
                                TipoDaEntrega = "RETIRADA",
                                ValorTotalItens = item.total.subTotal,
                                ValorTaxaDeentrega = item.total.deliveryFee,
                                Valortaxaadicional = item.total.additionalFees,
                                Descontos = item.total.benefits,
                                TotalDoPedido = item.total.orderAmount,
                                // Observations = item.delivery.observations,
                                items = item.items,
                            };

                            if (item.orderType == "PLACE")
                            {
                                UserControlPedido.MudaPictureBoxMesa(UserControlPedido);
                            }

                            if (item.orderTiming == "SCHEDULED")
                            {
                                UserControlPedido.MudaPictureBoxAgendada(UserControlPedido);
                            }

                            UserControlPedido.MudaPictureBoxANOTAAI(UserControlPedido);
                            UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, horarioCorrigido, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                            panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                        }
                    }


                    if (item.CriadoPor == "CCM")
                    {
                        if (Configuracoes.IntegraCCM)
                        {
                            string horarioCorrigido = "";

                            if (item.orderType == "DELIVERY")
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime);
                                horarioCorrigido = HorarioMudado.ToString();
                            }
                            else
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime);
                                horarioCorrigido = HorarioMudado.ToString();
                            }

                            UCPedido UserControlPedido = new UCPedido()
                            {
                                Pedido = item,
                                Id_pedido = item.id,
                                OrderType = item.orderType,
                                Display_id = item.displayId,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                NomePedido = item.customer.name,
                                DeliveryBy = "RETIRADA",
                                FeitoAs = item.createdAt,
                                HorarioEntrega = horarioCorrigido,//item.delivery.deliveryDateTime,
                                LocalizadorPedido = "RETIRADA",
                                EnderecoFormatado = "RETIRADA",
                                Bairro = "RETIRADA",
                                TipoDaEntrega = "RETIRADA",
                                ValorTotalItens = item.total.subTotal,
                                ValorTaxaDeentrega = item.total.deliveryFee,
                                Valortaxaadicional = item.total.additionalFees,
                                Descontos = item.total.benefits,
                                TotalDoPedido = item.total.orderAmount,
                                // Observations = item.delivery.observations,
                                items = item.items,
                            };

                            if (item.orderTiming == "SCHEDULED")
                            {
                                UserControlPedido.MudaPictureBoxAgendada(UserControlPedido);
                            }

                            if (item.orderType == "INDOOR")
                            {
                                UserControlPedido.MudaPictureBoxMesa(UserControlPedido);
                            }


                            UserControlPedido.MudaPictureBoxCCM(UserControlPedido);
                            UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, horarioCorrigido, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                            panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                        }

                    }


                    if (item.CriadoPor == "ONPEDIDO")
                    {
                        if (Configuracoes.IntegraOnOPedido)
                        {
                            string horarioCorrigido = "";

                            if (item.orderType == "DELIVERY")
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime);
                                horarioCorrigido = HorarioMudado.ToString();
                            }
                            else
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.delivery.deliveryDateTime);
                                horarioCorrigido = HorarioMudado.ToString();
                            }

                            UCPedido UserControlPedido = new UCPedido()
                            {
                                Pedido = item,
                                Id_pedido = item.id,
                                OrderType = item.orderType,
                                Display_id = item.displayId,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                NomePedido = item.customer.name,
                                DeliveryBy = "RETIRADA",
                                FeitoAs = item.createdAt,
                                HorarioEntrega = horarioCorrigido,//item.delivery.deliveryDateTime,
                                LocalizadorPedido = "RETIRADA",
                                EnderecoFormatado = "RETIRADA",
                                Bairro = "RETIRADA",
                                TipoDaEntrega = "RETIRADA",
                                ValorTotalItens = item.total.subTotal,
                                ValorTaxaDeentrega = item.total.deliveryFee,
                                Valortaxaadicional = item.total.additionalFees,
                                Descontos = item.total.benefits,
                                TotalDoPedido = item.total.orderAmount,
                                // Observations = item.delivery.observations,
                                items = item.items,
                            };

                            if (item.orderTiming == "SCHEDULED")
                            {
                                UserControlPedido.MudaPictureBoxAgendada(UserControlPedido);
                            }

                            if (item.orderType == "INDOOR")
                            {
                                UserControlPedido.MudaPictureBoxMesa(UserControlPedido);
                            }


                            UserControlPedido.MudaParaLogoONPedido(UserControlPedido);
                            UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, horarioCorrigido, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                            panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                        }

                    }


                    if (item.CriadoPor == "DELMATCH")
                    {
                        if (Configuracoes.IntegraDelMatch)
                        {
                            string horarioCorrigido = "";

                            if (item.orderType == "DELIVERY")
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.createdAt).AddMinutes(Configuracoes.TempoEntrega);
                                horarioCorrigido = HorarioMudado.ToString();
                            }
                            else if (item.orderType == "TOGO")
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.createdAt).AddMinutes(Configuracoes.TempoRetirada);
                                horarioCorrigido = HorarioMudado.ToString();
                            }
                            else if (item.orderType == "INDOOR")
                            {
                                DateTime HorarioMudado = DateTime.Parse(item.createdAt).AddMinutes(10);
                                horarioCorrigido = HorarioMudado.ToString();
                            }

                            UCPedido UserControlPedido = new UCPedido()
                            {
                                Pedido = item,
                                Id_pedido = item.id,
                                OrderType = item.orderType,
                                Display_id = item.displayId,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                NomePedido = item.customer.name,
                                DeliveryBy = "RETIRADA",
                                FeitoAs = item.createdAt,
                                HorarioEntrega = horarioCorrigido,//item.delivery.deliveryDateTime,
                                LocalizadorPedido = "RETIRADA",
                                EnderecoFormatado = "RETIRADA",
                                Bairro = "RETIRADA",
                                TipoDaEntrega = "RETIRADA",
                                ValorTotalItens = item.total.subTotal,
                                ValorTaxaDeentrega = item.total.deliveryFee,
                                Valortaxaadicional = item.total.additionalFees,
                                Descontos = item.total.benefits,
                                TotalDoPedido = item.total.orderAmount,
                                // Observations = item.delivery.observations,
                                items = item.items,
                            };


                            UserControlPedido.MudaPicturesBoxDePedidoEnviado();
                            UserControlPedido.MudaParaLogoDelMatch(UserControlPedido);
                            UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, horarioCorrigido, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                            if (item.orderType == "INDOOR")
                            {
                                UserControlPedido.MudaCasoSejaMesaDelMatch(UserControlPedido);
                                UserControlPedido.MudaPictureBoxMesa(UserControlPedido);
                            }

                            panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                        }

                    }


                    if (item.CriadoPor == "IFOOD")
                    {
                        if (Configuracoes.IntegraIfood)
                        {

                            DateTime DataCertaDaFeitoEmTimeStamp = DateTime.ParseExact(item.createdAt, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                          System.Globalization.CultureInfo.InvariantCulture,
                                                          System.Globalization.DateTimeStyles.AssumeUniversal);
                            DateTime DataCertaDaFeitoEm = DataCertaDaFeitoEmTimeStamp.ToLocalTime();

                            if (item.takeout.mode == null) //caso Entre nesse if, é porque o pedido vai ser para delivery
                            {
                                DateTime DataCertaDaEntregaemTimeStamp = DateTime.ParseExact(item.delivery.deliveryDateTime, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                             System.Globalization.CultureInfo.InvariantCulture,
                                                             System.Globalization.DateTimeStyles.AssumeUniversal);
                                DateTime DataCertaDaEntrega = DataCertaDaEntregaemTimeStamp.ToLocalTime();

                                if (item.orderTiming != "SCHEDULED")
                                {
                                    UCPedido UserControlPedido = new UCPedido()
                                    {
                                        Pedido = item,
                                        Id_pedido = item.id,
                                        OrderType = item.orderType,
                                        Display_id = item.displayId,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                        NomePedido = item.customer.name,
                                        DeliveryBy = item.delivery.deliveredBy,
                                        FeitoAs = DataCertaDaFeitoEm.ToString(),
                                        HorarioEntrega = DataCertaDaEntrega.ToString(),//item.delivery.deliveryDateTime,
                                        LocalizadorPedido = item.delivery.pickupCode,
                                        EnderecoFormatado = item.delivery.deliveryAddress.formattedAddress,
                                        Bairro = item.delivery.deliveryAddress.neighborhood,
                                        TipoDaEntrega = item.delivery.deliveredBy,
                                        ValorTotalItens = item.total.subTotal,
                                        ValorTaxaDeentrega = item.total.deliveryFee,
                                        Valortaxaadicional = item.total.additionalFees,
                                        Descontos = item.total.benefits,
                                        TotalDoPedido = item.total.orderAmount,
                                        Observations = item.delivery.observations,
                                        items = item.items,
                                    };


                                    UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.delivery.deliveryDateTime, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                                    panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel

                                }
                                else
                                {
                                    UCPedido UserControlPedido = new UCPedido()
                                    {
                                        Pedido = item,
                                        Id_pedido = item.id,
                                        OrderType = item.orderType,
                                        Display_id = item.displayId,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                        NomePedido = item.customer.name,
                                        DeliveryBy = item.delivery.deliveredBy,
                                        FeitoAs = DataCertaDaFeitoEm.ToString(),//item.createdAt,
                                        HorarioEntrega = DataCertaDaEntrega.ToString(),//item.delivery.deliveryDateTime,
                                        LocalizadorPedido = item.delivery.pickupCode,
                                        EnderecoFormatado = item.delivery.deliveryAddress.formattedAddress,
                                        Bairro = item.delivery.deliveryAddress.neighborhood,
                                        TipoDaEntrega = item.delivery.deliveredBy,
                                        ValorTotalItens = item.total.subTotal,
                                        ValorTaxaDeentrega = item.total.deliveryFee,
                                        Valortaxaadicional = item.total.additionalFees,
                                        Descontos = item.total.benefits,
                                        TotalDoPedido = item.total.orderAmount,
                                        Observations = item.delivery.observations,
                                        items = item.items,
                                    };

                                    UserControlPedido.MudaPictureBoxAgendada(UserControlPedido);

                                    UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.schedule.deliveryDateTimeEnd, item.Situacao); // aqui muda as labels do user control para cada pedido em questão
                                    UserControlPedido.MudarLabelQuandoAgendada("Agendato até:");

                                    panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                                }
                            }

                            if (item.delivery.pickupCode == null) // se entrar nesse if é porque  vai ser para retirada
                            {
                                DateTime DataCertaDaRetiradaEmTimeStamp = DateTime.ParseExact(item.takeout.takeoutDateTime, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                             System.Globalization.CultureInfo.InvariantCulture,
                                                             System.Globalization.DateTimeStyles.AssumeUniversal);
                                DateTime DataCertaDaRetirada = DataCertaDaRetiradaEmTimeStamp.ToLocalTime();

                                if (item.orderTiming != "SCHEDULED")
                                {
                                    UCPedido UserControlPedido = new UCPedido()
                                    {
                                        Pedido = item,
                                        Id_pedido = item.id,
                                        Display_id = item.displayId,
                                        OrderType = item.orderType,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                        NomePedido = item.customer.name,
                                        FeitoAs = DataCertaDaFeitoEm.ToString(),
                                        HorarioEntrega = DataCertaDaRetirada.ToString(),//item.takeout.takeoutDateTime,
                                        LocalizadorPedido = item.delivery.pickupCode,
                                        EnderecoFormatado = "Retirada No local",
                                        Bairro = item.delivery.deliveryAddress.neighborhood,
                                        TipoDaEntrega = "Retirada",
                                        ValorTotalItens = item.total.subTotal,
                                        ValorTaxaDeentrega = item.total.deliveryFee,
                                        Valortaxaadicional = item.total.additionalFees,
                                        Descontos = item.total.benefits,
                                        TotalDoPedido = item.total.orderAmount,
                                        Observations = item.delivery.observations,
                                        items = item.items
                                    };


                                    UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.createdAt, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                                    panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                                }
                                else
                                {
                                    UCPedido UserControlPedido = new UCPedido()
                                    {
                                        Pedido = item,
                                        Id_pedido = item.id,
                                        Display_id = item.displayId,
                                        OrderType = item.orderType,//aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                                        NomePedido = item.customer.name,
                                        FeitoAs = DataCertaDaFeitoEm.ToString(),
                                        HorarioEntrega = DataCertaDaRetirada.ToString(),//item.takeout.takeoutDateTime,
                                        LocalizadorPedido = item.delivery.pickupCode,
                                        EnderecoFormatado = "Pedido Agendado Para Retirada",
                                        Bairro = item.delivery.deliveryAddress.neighborhood,
                                        TipoDaEntrega = "Retirada",
                                        ValorTotalItens = item.total.subTotal,
                                        ValorTaxaDeentrega = item.total.deliveryFee,
                                        Valortaxaadicional = item.total.additionalFees,
                                        Descontos = item.total.benefits,
                                        TotalDoPedido = item.total.orderAmount,
                                        Observations = item.delivery.observations,
                                        items = item.items
                                    };

                                    UserControlPedido.MudaPictureBoxAgendada(UserControlPedido);

                                    UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.createdAt, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                                    panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                                }

                            }

                        }

                    }
                }

                pedidos.Clear();

                panelPedidos.ResumeLayout();


                checkBoxConcluido.Enabled = true;
                checkBoxConfirmados.Enabled = true;
                checkBoxDespachados.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Ops", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public static void MudaStatusMerchant()
    {
        pictureBoxOnline.Visible = true;
        pictureBoxOfline.Visible = false;

    }

    private void SetRoundedRegion(Control control, int radius) //Método para arredondar os cantos dos paineis
    {
        GraphicsPath path = new GraphicsPath();
        int width = control.Width;
        int height = control.Height;
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(width - radius, 0, radius, radius, 270, 90);
        path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
        path.AddArc(0, height - radius, radius, radius, 90, 90);
        path.CloseFigure();

        control.Region = new Region(path);
    }

    private void pictureBoxHome_Click(object sender, EventArgs e)
    {
        SetarPanelPedidos();
        panelDetalhePedido.Controls.Clear();
        panelDetalhePedido.Controls.Add(labelDeAvisoPedidoDetalhe);
        labelDeAvisoPedidoDetalhe.Visible = true;
    }

    private async void pollingManual_Click(object sender, EventArgs e)
    {
        FormWebBrowser formChat = new FormWebBrowser();
        formChat.ShowDialog();
    }


    private async void TimerCallback(object state) // função para ser chamada a cada 30 segundos, e com isso chamando o pulling
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Configuracoes = db.parametrosdosistema.FirstOrDefault();

                bool verificaInternet = await VerificaInternet.InternetAtiva();

                if (!verificaInternet)
                {
                    throw new Exception("Por favor verifique sua conexão com a internet");
                }


                if (Configuracoes!.IntegraAiQFome)
                {
                    AiQFome AiQFome = new AiQFome(new MeuContexto());

                    await AiQFome.Polling();
                }

                if (Configuracoes.IntegraGarcom)
                {
                    IntegraComOGarcom = true;
                }

                if (Configuracoes.IntegraIfood)
                {
                    var Ifood = new Ifood(new MeuContexto());
                    await Ifood.Polling();
                }



                if (Configuracoes.IntegraDelMatch)
                {
                    DelMatch Delmatch = new DelMatch(new MeuContexto());

                    await Delmatch.PoolingDelMatch();
                    await Delmatch.FechaMesa();
                }

                if (Configuracoes.IntegraOnOPedido)
                {
                    OnPedido OnPedido = new OnPedido(new MeuContexto());

                    // await OnPedido.Pooling();
                    await OnPedido.Pooling2();
                }

                if (Configuracoes.IntegraCCM)
                {
                    CCM CCMNEW = new CCM(new MeuContexto());
                    await CCMNEW.Pooling();
                }

                if (Configuracoes.IntegraDelmatchEntregas)
                {
                    if (Configuracoes.EnviaPedidoAut)
                    {
                        ChamaEntregaAutDelMatch();
                    }
                }

                
                if (Configuracoes.IntegravariasEmpresasTaxyMachine)
                {
                    List<EmpresasEntregaTaxyMachine> empresas = new List<EmpresasEntregaTaxyMachine>();
                    if (Configuracoes.UserNameTaxyMachine.Contains("@"))
                    {
                        string CodEmpresa = "00";

                        switch (Configuracoes.EmpresadeEntrega)
                        {
                            case "JUMA":
                                CodEmpresa = "77";
                                break;
                            case "OTTO":
                                CodEmpresa = "66";
                                break;
                        }

                        empresas.Add(new EmpresasEntregaTaxyMachine()
                        {
                            NomeEmpresa = Configuracoes.EmpresadeEntrega,
                            Usuario = Configuracoes.UserNameTaxyMachine,
                            Senha = Configuracoes.PasswordTaxyMachine,
                            CodEntregador = CodEmpresa,
                            MachineId = Configuracoes.ApiKeyTaxyMachine
                        });
                    }

                    List<EmpresasEntregaTaxyMachine> EmpresasCadastrada = await db.empresastaxymachine.ToListAsync();
                    if (EmpresasCadastrada is not null)
                    {
                        empresas.AddRange(EmpresasCadastrada);
                    }

                    foreach (var empresa in empresas)
                    {
                        if (Configuracoes.EnviaPedidoAut)
                        {
                            TaxyMachine taxyMachine = new TaxyMachine(new MeuContexto()) { UsuarioEnviado = empresa.Usuario, SenhaEnviada = empresa.Senha, ApiKeyEnviado = empresa.MachineId };

                            await taxyMachine.EnviaPedidosAutomaticamente(empresa.CodEntregador);
                        }


                    }
                }


                if (Configuracoes.IntegravariasEmpresasTaxyMachine)
                {
                    if (Configuracoes.IntegraOttoEntregas)
                    {
                        OTTO Otto = new OTTO(new MeuContexto());

                        if (Configuracoes.EnviaPedidoAut)
                        {
                            await Otto.EnviaPedidosAutomaticamente(codEntregador: "66");
                        }
                    }

                    if (Configuracoes.IntegraJumaEntregas)
                    {
                        Juma Juma = new Juma(new MeuContexto());

                        if (Configuracoes.EnviaPedidoAut)
                        {
                            await Juma.EnviaPedidosAutomaticamente(codEntregador: "77");
                        }
                    }
                }

                if (Configuracoes.IntegraAnotaAi)
                    await AnotaAi.Pooling();


                if (ClsDeSuporteAtualizarPanel.MudouDataBase)
                {
                    if (ClsDeSuporteAtualizarPanel.MudouDataBasePedido) //entra aqui só se foi pedido novo
                    {
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            notifyIcon1.Text = "Novo Pedido";
                            notifyIcon1.Tag = "SyslogicaApp";
                            notifyIcon1.Visible = true;
                            notifyIcon1.BalloonTipTitle = "SysLogicaApp";
                            notifyIcon1.BalloonTipText = "SysLogicaApp";
                            notifyIcon1.ShowBalloonTip(3, "Novo Pedido", "Um novo pedido chegou para você!", ToolTipIcon.Info);

                            ClsDeSuporteAtualizarPanel.MudouDataBasePedido = false;
                        }
                    }


                    FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
                    ClsDeSuporteAtualizarPanel.MudouDataBase = false;
                }
            }

            _timer2 = new System.Threading.Timer(BarraDeCarregamento, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000)); //Função que chama o pulling a cada 30 segundos
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "OPS");
        }
    }

    private async void BarraDeCarregamento(object state)
    {
        try
        {
            ContadorPooling += 1 * 4;

            if (ContadorPooling >= 100)
            {
                _timer2.Dispose();
                ContadorPooling = 0;
            }


            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.progressBar1.Value = ContadorPooling));
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "OPS");
        }
    }

    private async void PollingProGarcom(object state)
    {
        try
        {
            if (IntegraComOGarcom)
            {
                await garcomSysMenu.Pooling();
                await garcomSysMenu.AtualizarContas();
                garcomSysMenu.AtualizaPainelDePedidos();
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "OPS");
        }
    }

    private async void ChamaEntregaAutDelMatch() //Função que vai ser chamada para chamar os pedidos aut
    {
        DelMatch Delmatch = new DelMatch(new MeuContexto());

        await Delmatch.EnviaPedidosAut();
    }

    private async void pictureBoxDelivery_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var Configs = db.parametrosdosistema.FirstOrDefault();

                if (Configs.IntegraDelmatchEntregas)
                {
                    DeliveryForm deliveryForm = new DeliveryForm();
                    deliveryForm.ShowDialog();
                }
                else if (Configs.IntegraOttoEntregas)
                {
                    EnvioDePedidos FormDePedidos = new EnvioDePedidos(new MeuContexto());
                    FormDePedidos.ShowDialog();
                }
                else if (Configs.IntegraJumaEntregas)
                {
                    EnvioDePedidos FormDePedidos = new EnvioDePedidos(new MeuContexto()) { eJuma = true };
                    FormDePedidos.ShowDialog();
                }
                else
                {
                    await SysAlerta.Alerta("Nenhuma empresa cadastrada", "Você não tem nenhuma integração com aplicativos de entrega", SysAlertaTipo.Alerta);
                }
            }


        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }

    }

    private void pictureBoxConfig_Click(object sender, EventArgs e)
    {
        //FormDeParametrosDoSistema configs = new FormDeParametrosDoSistema();
        //configs.ShowDialog();

        NewFormConfiguracoes Configs = new NewFormConfiguracoes(new MeuContexto());
        Configs.ShowDialog();
    }

    private void pictureBoxChat_Click(object sender, EventArgs e)
    {
        /*string? urlVerificacao = "https://gestordepedidos.ifood.com.br/#/home/orders/now";

        if (urlVerificacao != null && Uri.IsWellFormedUriString(urlVerificacao, UriKind.Absolute))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = urlVerificacao,
                UseShellExecute = true
            });
        }*/
        panelDetalhePedido.Controls.Clear();
        webViwer.Size = new Size(panelDetalhePedido.Size.Width - 20, panelDetalhePedido.Size.Height - 20);

        panelDetalhePedido.Controls.Add(webViwer);
        panelDetalhePedido.PerformLayout();

    }

    private void pictureBoxLupa_Click(object sender, EventArgs e)
    {
        textBoxBuscarPedido.Visible = true;
        textBoxBuscarPedido.Text = "";
        textBoxBuscarPedido.Focus();
        BtnBuscar.Visible = true;
    }

    private void BtnBuscar_Click(object sender, EventArgs e)
    {
        textBoxBuscarPedido.Visible = false;
        BtnBuscar.Visible = false;
        panelDetalhePedido.Controls.Clear();
        panelDetalhePedido.Controls.Add(labelDeAvisoPedidoDetalhe);
        labelDeAvisoPedidoDetalhe.Visible = true;

        try
        {
            int numPesquisadoDisplayId = Convert.ToInt32(textBoxBuscarPedido.Text);
            SetarPanelPedidos(numPesquisadoDisplayId);

        }
        catch (Exception ex) when (ex.Message.Contains("format"))
        {
            string? pesquisaPorNome = textBoxBuscarPedido.Text;

            SetarPanelPedidos(pesquisaNome: pesquisaPorNome);
        }
        catch (Exception exm)
        {
            MessageBox.Show(exm.Message, "Ops");
        }
    }

    private void textBoxBuscarPedido_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            BtnBuscar_Click(sender, e);
        }

        if (e.KeyChar == (char)Keys.Escape)
        {
            textBoxBuscarPedido.Visible = false;
            BtnBuscar.Visible = false;
        }
    }

    private void textBoxBuscarPedido_Leave(object sender, EventArgs e) { }

    private void FormMenuInicial_KeyPress(object sender, KeyPressEventArgs e) { }

    private void FormMenuInicial_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F2)
        {
            pictureBoxLupa_Click(sender, e);
        }

        if (e.KeyCode == Keys.Home)
        {
            pictureBoxHome_Click(sender, e);
        }
    }

    private void pictureBox2_Click(object sender, EventArgs e)
    {

        FormDeCronograma formDeCronograma = new FormDeCronograma();
        formDeCronograma.ShowDialog();
    }


    public static async void AbreModalDeAguardoDeConclPedidos(FormAguardoDeConclPedidos instanciaModal)
    {
        try
        {
            instanciaModal.ShowDialog();
        }
        catch (Exception exm)
        {
            MessageBox.Show(exm.Message, "Ops");
        }
    }

    public static async void FechaModalDeAguardoDeConclPedidos()
    {
        try
        {
            if (Application.OpenForms["FormAguardoDeConclPedidos"] != null)
            {
                Application.OpenForms["FormAguardoDeConclPedidos"].Close();
            }
        }
        catch (Exception exm)
        {
            MessageBox.Show(exm.Message, "Ops");
        }
    }

    private void checkBoxConcluido_Click(object sender, EventArgs e)
    {
        SetarPanelPedidos();
    }

    private void checkBoxDespachados_CheckedChanged(object sender, EventArgs e)
    {
        SetarPanelPedidos();
    }

    private void checkBoxConfirmados_CheckedChanged(object sender, EventArgs e)
    {
        SetarPanelPedidos();
    }

    private void notifyIcon1_Click(object sender, EventArgs e)
    {

        if (this.WindowState == FormWindowState.Minimized)
        {
            this.WindowState = FormWindowState.Normal;

            this.Activate();
        }

        this.notifyIcon1.Dispose();

    }

    private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.WindowState = FormWindowState.Maximized;

            this.Activate();
        }

        this.notifyIcon1.Dispose();
    }

    private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.WindowState = FormWindowState.Maximized;

            this.Activate();
        }

        this.notifyIcon1.Dispose();
    }

    private async void pictureBox3_Click(object sender, EventArgs e)
    {
        //FormChat formChat = new FormChat(); 
        //formChat.ShowDialog();


    }

    private async void StartChat()
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var dbConfigs = db.parametrosdosistema.FirstOrDefault();

                if (dbConfigs.IntegraIfood)
                {
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string customFolderName = "SysLogicaLogs";

                    string fullPath = Path.Combine(appDataPath, customFolderName);

                    string userDataFolder = fullPath;
                    var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, null);

                    await webViwer.EnsureCoreWebView2Async(environment);

                    string htmlContent = @"<!DOCTYPE html>
                                    <html lang='en'>

                                    <head>
                                        <meta charset='UTF-8'>
                                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                        <title>Document</title>
                                        <script async src='https://widgets.ifood.com.br/widget.js'></script>
                                        <script>
                                            window.addEventListener('load', () => {
                                                iFoodWidget.init({
                                                    widgetId: 'b1d473d7-d4b1-4ac8-a9ca-935686090095',
                                                    merchantIds: [
                                                        '9362018a-6ae2-439c-968b-a40177a085ea'
                                                    ],
                                                });
                                            });
                                        </script>
                                    </head>

                                    <body>
                                    </body>

                                    </html>";

                    // webViwer.CoreWebView2.NavigateToString(htmlContent);
                    webViwer.CoreWebView2.Navigate("https://gestordepedidos.ifood.com.br/#/home/orders/now");

                    await Task.Delay(500);

                    List<CoreWebView2Cookie> cookieList = await webViwer.CoreWebView2.CookieManager.GetCookiesAsync("https://widgets.ifood.com.br/widget.js");
                    StringBuilder cookieResult = new StringBuilder(cookieList.Count + " cookie(s) received from https://widgets.ifood.com.br/widget.js\n");

                    for (int i = 0; i < cookieList.Count; ++i)
                    {
                        CoreWebView2Cookie cookie = webViwer.CoreWebView2.CookieManager.CreateCookieWithSystemNetCookie(cookieList[i].ToSystemNetCookie());
                        cookieResult.Append($"\n{cookie.Name} {cookie.Value} {(cookie.IsSession ? "[session cookie]" : cookie.Expires.ToString("G"))}");
                    }

                }

            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }


    }

    private void checkBoxMesas_CheckedChanged(object sender, EventArgs e)
    {
        SetarPanelPedidos();
    }

    private async void FormMenuInicial_FormClosing(object sender, FormClosingEventArgs e)
    {
        try
        {

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var Configs = db.parametrosdosistema.FirstOrDefault();

                DialogResultSys opc = await SysAlerta.Alerta("Fechando", "Você tem certeza que deseja fechar o app?", SysAlertaTipo.Alerta, SysAlertaButtons.SimNao);

                if (opc == DialogResultSys.Nao)
                {
                    e.Cancel = true;
                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro", $"{ex.Message}", SysAlertaTipo.Alerta, SysAlertaButtons.Ok);
        }

    }

    private void panelStatusLoja_MouseEnter(object sender, EventArgs e)
    {
    }

    private void panelStatusLoja_MouseLeave(object sender, EventArgs e)
    {

    }

    private void labelStatusLojaNM_MouseEnter(object sender, EventArgs e)
    {

    }

    private void labelStatusLojaNM_MouseLeave(object sender, EventArgs e)
    {
    }

    private void pictureBoxOfline_MouseEnter(object sender, EventArgs e)
    {
    }

    private void pictureBoxOfline_MouseLeave(object sender, EventArgs e)
    {
    }

    private void pictureBoxOnline_MouseEnter(object sender, EventArgs e)
    {
    }

    private void pictureBoxOnline_MouseLeave(object sender, EventArgs e)
    {
    }

    private void PanelDeInfosOnLine_MouseLeave(object sender, EventArgs e)
    {
    }

    private async void panelStatusLoja_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Configuracoes = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Configuracoes!.IfoodMultiEmpresa)
                {
                    var EmpresasIfood = await db.empresasIfoods.ToListAsync();
                    PanelDeInfosOnLine.Controls.Clear();

                    foreach (var empresa in EmpresasIfood)
                    {
                        UCInfoDeLojaOnLine UCInfoDeLojaOnLine = new UCInfoDeLojaOnLine(empresa);
                        PanelDeInfosOnLine.Controls.Add(UCInfoDeLojaOnLine);
                    }

                    if (ContadorDeClickLojaOnLine % 2 == 0)
                    {
                        PanelDeInfosOnLine.Visible = true;
                        ContadorDeClickLojaOnLine++;
                    }
                    else
                    {
                        PanelDeInfosOnLine.Visible = false;
                        ContadorDeClickLojaOnLine++;
                    }
                }

            }


        }
        catch (Exception)
        {
            await SysAlerta.Alerta("Erro", "Erro ao tentar abrir informações da loja", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }

    }

    private void labelStatusLojaNM_Click(object sender, EventArgs e)
    {
        panelStatusLoja_Click(sender, e);
    }

    private void PanelDeInfosOnLine_Leave(object sender, EventArgs e)
    {
        PanelDeInfosOnLine.Visible = false;
    }
}
