using Svg;
using SysIntegradorApp.ClassesAuxiliares;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class FormMenuInicial : Form
{
    //public Panel panelPedidos; 
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

        Ifood.SetTimer();
        SetarPanelPedidos();
    }

    private void panelPedidos_Paint(object sender, PaintEventArgs e)
    {

    }

    private void FormMenuInicial_Load(object sender, EventArgs e)
    {
        Ifood.SetTimer();
        SetarPanelPedidos();

        /*// Caminho para o arquivo SVG
        string svgFilePath = "caminho/para/seu/arquivo.svg";

        // Carregar o arquivo SVG
        SvgDocument svgDocument = SvgDocument.Open(svgFilePath);

        // Converter o documento SVG em uma imagem
        Bitmap bitmap = svgDocument.Draw();

        // Exibir a imagem em um controle PictureBox
        pictureBox1.Image = bitmap;*/

    }

    private void FormMenuInicial_FormClosed(object sender, FormClosedEventArgs e)
    {
        Application.Exit();
    }

    private void FormMenuInicial_Shown(object sender, EventArgs e)
    {

    }



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
                PedidoCompleto pedido = JsonSerializer.Deserialize<PedidoCompleto>(item.Json);
                pedido.Situacao = item.Situacao;
                pedidos.Add(pedido);
            }


            var pedidosOrdenado = pedidos.OrderByDescending(p =>
            {
                DateTime.TryParse(p.createdAt, out DateTime result);
                return result;
            });


            int contador = 0;
            //Faz um loop para adicionar os UserControls De pedido no panel
            foreach (var item in pedidosOrdenado)
            {

                if (item.takeout.mode == null) //caso Entre nesse if, é porque o pedido vai ser para delivery
                {
                    UCPedido UserControlPedido = new UCPedido()
                    {
                        Id_pedido = item.id, //aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                        NomePedido = item.customer.name,
                        FeitoAs = item.createdAt,
                        HorarioEntrega = item.orderTiming,
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
                        items = item.items
                    };


                    UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.createdAt, item.Situacao); // aqui muda as labels do user control para cada pedido em questão

                    panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel
                }

                if (item.delivery.pickupCode == null) // se entrar nesse if é porque  vai ser para retirada
                {
                    UCPedido UserControlPedido = new UCPedido()
                    {
                        Id_pedido = item.id, //aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                        NomePedido = item.customer.name,
                        FeitoAs = item.createdAt,
                        HorarioEntrega = item.takeout.takeoutDateTime,
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


                    UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.createdAt, pedidoOrdenado[contador].Situacao); // aqui muda as labels do user control para cada pedido em questão

                    panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel

                }
                contador++;
            }

            contador = 0;
            panelPedidos.PerformLayout();

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OK);
        }
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
}
