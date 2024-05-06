using Svg;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SysIntegradorApp;

public partial class FormMenuInicial : Form
{
    //public Panel panelPedidos; 

    private System.Threading.Timer _timer;

    public FormMenuInicial()
    {
        InitializeComponent();

        SetRoundedRegion(panelPedidos, 20);
        SetRoundedRegion(panelDetalhePedido, 20);
        SetRoundedRegion(panel1, 20);

        this.Resize += (sender, e) =>
        {
            SetRoundedRegion(panelPedidos, 20);
            SetRoundedRegion(panelDetalhePedido, 20);
            SetRoundedRegion(panel1, 20);
        };

        SetarPanelPedidos();
        panelDetalhePedido.Controls.Clear();
        panelDetalhePedido.Controls.Add(labelDeAvisoPedidoDetalhe);
    }

    private void panelPedidos_Paint(object sender, PaintEventArgs e) { }

    private void FormMenuInicial_Load(object sender, EventArgs e)
    {
        _timer = new System.Threading.Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(30)); //Função que chama o pulling a cada 30 segundos 
        SetarPanelPedidos();
    }

    private void FormMenuInicial_FormClosed(object sender, FormClosedEventArgs e)
    {
        Application.Exit();
    }

    private void FormMenuInicial_Shown(object sender, EventArgs e) { }



    public static async void SetarPanelPedidos()
    {
        try
        {
            List<PedidoCompleto> pedidos = new List<PedidoCompleto>();

            List<ParametrosDoPedido> pedidosFromDb = await Ifood.GetPedido();

            var pedidoOrdenado = pedidosFromDb.ToList();

            panelPedidos.Controls.Clear();
            panelPedidos.PerformLayout();

            foreach (ParametrosDoPedido item in pedidoOrdenado)
            {
                PedidoCompleto? pedido = JsonSerializer.Deserialize<PedidoCompleto>(item.Json);
                pedido.JsonPolling = item.JsonPolling;
                pedido.Situacao = item.Situacao;
                pedidos.Add(pedido);
            }


            var pedidosOrdenado = pedidos.OrderByDescending(p =>
            {
                DateTime.TryParse(p.createdAt, out DateTime result);
                return result;
            });

            //Faz um loop para adicionar os UserControls De pedido no panel
            foreach (var item in pedidosOrdenado)
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


                        UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.createdAt, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                        panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                    }

                }

            }

            pedidos.Clear();

            panelPedidos.PerformLayout();

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OK);
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

    private void pollingManual_Click(object sender, EventArgs e)
    {
        Ifood.Polling();
        SetarPanelPedidos();
    }


    private async void TimerCallback(object state) // função para ser chamada a cada 30 segundos, e com isso chamando o pulling
    {
        await Ifood.Polling();
    }

    private void pictureBoxDelivery_Click(object sender, EventArgs e)
    {
        DeliveryForm deliveryForm = new DeliveryForm();
        deliveryForm.ShowDialog();
    }

    private void pictureBoxConfig_Click(object sender, EventArgs e)
    {
        FormDeParametrosDoSistema configs = new FormDeParametrosDoSistema();
        configs.ShowDialog();
    }

    private void pictureBoxChat_Click(object sender, EventArgs e)
    {
        string? urlVerificacao = "https://gestordepedidos.ifood.com.br/#/home/orders/now";

        if (urlVerificacao != null && Uri.IsWellFormedUriString(urlVerificacao, UriKind.Absolute))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = urlVerificacao,
                UseShellExecute = true
            });
        }
    }
}
