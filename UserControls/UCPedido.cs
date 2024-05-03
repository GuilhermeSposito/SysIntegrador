using SysIntegradorApp.ClassesAuxiliares;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class UCPedido : UserControl
{
    public PedidoCompleto Pedido { get; set; }
    public string? Id_pedido { get; set; }
    public string? OrderType { get; set; }
    public string? Display_id { get; set; }
    public string? NomePedido { get; set; }
    public string? DeliveryBy { get; set; }
    public string? FeitoAs { get; set; }
    public string? HorarioEntrega { get; set; }
    public string? LocalizadorPedido { get; set; }
    public string? EnderecoFormatado { get; set; }
    public string? Bairro { get; set; }
    public string TipoDaEntrega { get; set; }
    public float ValorTotalItens { get; set; }
    public float ValorTaxaDeentrega { get; set; }
    public float Valortaxaadicional { get; set; }
    public float Descontos { get; set; }
    public float TotalDoPedido { get; set; }
    public string? Observations { get; set; }
    public List<Items> items { get; set; } = new List<Items>();


    public UCPedido()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 10);
    }

    public void SetLabels(string id_pedido, string numPedido, string nomePedido, string horarioPedido, string statusPedido)
    {
        Id_pedido = id_pedido;
        labelNumPedido.Text = $"#{numPedido}";
        labelNomePedido.Text = nomePedido;
        labelHorarioDeEntrega.Text = HorarioEntrega.Substring(11, 5);
        string status = TraduzStatus.TraduzStatusEnviado(statusPedido);

        if (status == "Cancelado")
        {
            labelStatus.ForeColor = Color.Red;
        }
        else
        {
            labelStatus.ForeColor = Color.Green;
        }

        labelStatus.Text = status;
    }

    public void MudarLabelQuandoAgendada(string texto)
    {
        labelEntregarAte.Text = texto;
        labelEntregarAte.ForeColor = Color.Red;

        labelHorarioDeEntrega.Location = new Point(230, 72);
    }

    private void labelStatus_Click(object sender, EventArgs e) { }

    private void UCPedido_Load(object sender, EventArgs e) { }

    public async void UCPedido_Click(object sender, EventArgs e)
    {
        FormMenuInicial.panelDetalhePedido.Controls.Clear();
        FormMenuInicial.panelDetalhePedido.PerformLayout();
        UCInfoPedido infoPedido = new UCInfoPedido() { Pedido = Pedido, Id_pedido = Id_pedido, orderType = OrderType, Display_id = Display_id };

        infoPedido.SetLabels(
                               horarioEntrega: HorarioEntrega,
                               localizadorPedido: LocalizadorPedido,
                               enderecoFormatado: EnderecoFormatado,
                               bairro: Bairro,
                               TipoEntrega: TipoDaEntrega,
                               valorTotalItens: ValorTotalItens,
                               valorTaxaDeentrega: ValorTaxaDeentrega,
                               valortaxaadicional: Valortaxaadicional,
                               descontos: Descontos,
                               total: TotalDoPedido
                          );

        int tamanhoPanel = FormMenuInicial.panelDetalhePedido.Width;

        infoPedido.Width = tamanhoPanel - 50;//1707;
        infoPedido.Height = 1200;

        infoPedido.InsereItemNoPedido(items);
        FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = false;
        FormMenuInicial.panelDetalhePedido.Controls.Add(infoPedido);

    }

    private void UCPedido_Enter(object sender, EventArgs e)
    {
        this.BackColor = Color.DarkGray;
        pictureBox1.BackColor = Color.DarkGray;
    }

    private void UCPedido_Leave(object sender, EventArgs e)
    {
        this.BackColor = Color.White;
    }

    private void labelNomePedido_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void labelNumPedido_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e); 
        this.Focus();
    }

    private void labelEntregarAte_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e); 
        this.Focus();
    }

    private void labelHorarioDeEntrega_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void labelStatus_Click_1(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e); 
        this.Focus();
    }
}
