using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SysIntegradorApp.UserControls.UCSDelMatch;


public partial class UCPedidoDelMatch : UserControl
{
    public PedidoDelMatch? Pedido { get; set; }


    public UCPedidoDelMatch()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 10);
    }

    public void SetLabels(int numPedido, string nomePedido, string horarioPedido, string statusPedido)
    {

        labelNumPedido.Text = $"#{Pedido.Id.ToString()}";
        labelNumConta.Text = Pedido.NumConta.ToString().PadLeft(3, '0');
        labelNomePedido.Text = Pedido.Customer.Name;
        labelHorarioDeEntrega.Text = Pedido.DeliveryDateTime.Substring(11, 5);
        string status = TraduzStatus.TraduzStatusEnviado(statusPedido);

        if (status == "Cancelado" || status == "Pendente")
        {
            labelStatus.ForeColor = Color.Red;
            labelStatus.Font = new Font(labelStatus.Font.FontFamily, 9, FontStyle.Bold);
        }
        else
        {
            labelStatus.ForeColor = Color.Green;
            labelStatus.Font = new Font(labelStatus.Font.FontFamily, 9, FontStyle.Bold);
        }

        labelStatus.Text = status;
    }

    private void UCPedidoDelMatch_Click(object sender, EventArgs e)
    {
        FormMenuInicial.panelDetalhePedido.Controls.Clear();
        FormMenuInicial.panelDetalhePedido.PerformLayout();
        UCInfoPedidosDelMatch infoPedido = new UCInfoPedidosDelMatch() { Pedido = Pedido };

       // infoPedido.SetLabels(Pedido.CreatedAt, Pedido.Reference, "E", Pedido.deliveryAddress.FormattedAddress, Pedido.deliveryAddress.Neighboardhood, "MERCHANT", Pedido.SubTotal, Pedido.deliveryFee, Pedido.AdditionalFee, Pedido.Discount, Pedido.TotalPrice);

        int tamanhoPanel = FormMenuInicial.panelDetalhePedido.Width;

        infoPedido.Width = tamanhoPanel - 50;//1707;
        infoPedido.Height = 1200;

        //infoPedido.InsereItemNoPedido(items);
        FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = false;
        FormMenuInicial.panelDetalhePedido.Controls.Add(infoPedido);
    }
}
