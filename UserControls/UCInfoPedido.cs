using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class UCInfoPedido : UserControl
{
    public string? Id_pedido { get; set; }
    public string? Display_id {  get; set; }
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

    public UCInfoPedido()
    {
        InitializeComponent();
        SetRoundedRegion(this, 24); //da uma arredondada na borda do user control
        this.btnDespacharDelMacth.FlatAppearance.BorderColor = Color.FromArgb(237, 121, 12, 93);
        this.btnDespacharDelMacth.ForeColor = Color.FromArgb(237, 121, 12, 93);
    }

    private void labelPedidoNM_Click(object sender, EventArgs e) { }

    private void label4_Click(object sender, EventArgs e) { }

    public void SetLabels(string id_Pedido,
                          string display_id,
                          string nomePedido,
                          string feitoAs,
                          string horarioEntrega,
                          string localizadorPedido,
                          string enderecoFormatado,
                          string bairro,
                          string TipoEntrega,
                          float valorTotalItens,
                          float valorTaxaDeentrega,
                          float valortaxaadicional,
                          float descontos,
                          float total,
                          string observations)
    {
        labelLocalizadorPedido.Text = "Localizador teste do pedido";
        labelDisplayId.Text = display_id;
        numId.Text = id_Pedido;
        label1.Text = nomePedido;
        dateFeitoAs.Text = feitoAs.Substring(11, 5);
        tipoEntrega.Text = TipoEntrega;
        horarioEntregaPrevista.Text = horarioEntrega;
        labelLocalizadorPedido.Text = localizadorPedido;
        labelEndereco.Text = $"{enderecoFormatado} - {bairro}";
        ValorTotalDosItens.Text = valorTotalItens.ToString("c");
        valorTaxaDeEntrega.Text = valorTaxaDeentrega.ToString("c");
        valorTaxaAdicional.Text = valortaxaadicional.ToString("c");
        valorDescontos.Text = descontos.ToString("c");
        valorTotal.Text = total.ToString("c");
        infoPagPedido.Text = observations;

    }

    public void InsereItemNoPedido(List<Items> items)
    {
        //primeiro instanciar um objeto do UserControl UCItem
        //Dentro do UserControl Temos que criar um método que define as labels 

        foreach (Items item in items)
        {
            UCItem uCItem = new UCItem();
            uCItem.SetLabels(item.name, item.quantity, item.unitPrice, item.optionsPrice, item.totalPrice, item.options);
            panelDeItens.Controls.Add(uCItem);
        }

    }

    private void SetRoundedRegion(Control control, int radius) //Método para arredondar os cantos dos UserCntrol
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

    //Método chamado no clique da impressão, dentro dele setamos a propriedade estatica dentro da classe de impressão, para que la dentro possamos fazer um select no accsses
    private void btnImprimir_Click(object sender, EventArgs e)
    {
        using ApplicationDbContext db = new ApplicationDbContext();
        ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Id_pedido).FirstOrDefault();
        ParametrosDoSistema? opSistema = db.parametrosdosistema.Where(x => x.Id == 1).FirstOrDefault();

        Impressao.ChamaImpressoes(pedido.Conta, opSistema.Impressora1);
    }
}
