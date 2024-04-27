using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class UCInfoPedido : UserControl
{
    public PedidoCompleto Pedido { get; set; }
    public string? Id_pedido { get; set; }
    public string? orderType { get; set; }
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

    public UCInfoPedido()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24); //da uma arredondada na borda do user control
    }

    private void labelPedidoNM_Click(object sender, EventArgs e) { }

    private void label4_Click(object sender, EventArgs e) { }

    public void SetLabels(string horarioEntrega, string localizadorPedido, string enderecoFormatado, string bairro, string TipoEntrega, float valorTotalItens, float valorTaxaDeentrega, float valortaxaadicional, float descontos, float total)
    {
        string DefineEntrega = "";
        string DefineLocalEntrega = "";

        if (TipoEntrega == "MERCHANT")
        {
            DefineEntrega = "Propria";
        }

        if (TipoEntrega == "Retirada")
        {
            DefineEntrega = "Retirada";
        }

        if (enderecoFormatado.Contains("Retirada"))
        {
            DefineLocalEntrega = "Retirada";
        }
        else
        {
            DefineLocalEntrega = $"{enderecoFormatado} - {bairro}";
        }

        labelLocalizadorPedido.Text = Pedido.delivery.pickupCode;

        labelDisplayId.Text = $"#{Pedido.displayId}";

        numId.Text = Pedido.customer.phone.localizer;

        label1.Text = Pedido.customer.name;

        dateFeitoAs.Text = Pedido.createdAt.Substring(11, 5);

        tipoEntrega.Text = DefineEntrega;

        horarioEntregaPrevista.Text = horarioEntrega.Substring(11, 5);

        labelLocalizadorPedido.Text = localizadorPedido;

        labelEndereco.Text = DefineLocalEntrega;

        ValorTotalDosItens.Text = valorTotalItens.ToString("c");

        valorTaxaDeEntrega.Text = valorTaxaDeentrega.ToString("c");

        valorTaxaAdicional.Text = valortaxaadicional.ToString("c");

        valorDescontos.Text = descontos.ToString("c");

        valorTotal.Text = total.ToString("c");



        var InfoPag = ClsInfosDePagamentosParaImpressao.DefineTipoDePagamento(metodos: Pedido.payments.methods);

        infoPagPedido.Text = $"{InfoPag.FormaPagamento} {InfoPag.TipoPagamento}";
        obsPagamentoPedido.Text = InfoPag.TipoPagamento == "Pago Online" ? "Não é nescessario receber do cliente" : "Deverá ser cobrado no local do cliente";

        if (Pedido.customer.documentNumber == null)
        {
            labelCPF.Text = "NÃO";
        }
        else
        {
            labelCPF.Text = Pedido.customer.documentNumber;
        }
    }

    public void InsereItemNoPedido(List<Items> items)
    {
        //primeiro instanciar um objeto do UserControl UCItem
        //Dentro do UserControl Temos que criar um método que define as labels 

        foreach (Items item in items)
        {
            UCItem uCItem = new UCItem();
            uCItem.SetLabels(item.name, item.quantity, item.unitPrice, item.optionsPrice, item.totalPrice, item.options, uCItem);
            panelDeItens.Controls.Add(uCItem);
        }

    }

    //Método chamado no clique da impressão, dentro dele setamos a propriedade estatica dentro da classe de impressão, para que la dentro possamos fazer um select no accsses
    private void btnImprimir_Click(object sender, EventArgs e)
    {
        using ApplicationDbContext db = new ApplicationDbContext();
        ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Id_pedido).FirstOrDefault();
        ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

        List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5 };

        if (!opSistema.AgruparComandas)
        {
            foreach (string imp in impressoras)
            {

                if (imp != "Sem Impressora" && imp != null)
                {
                    Impressao.ChamaImpressoes(pedido.Conta, imp);
                }
            }
        }
        else
        {
            Impressao.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, impressoras);
        }



        impressoras.Clear();
    }

    private void btnDespacharIfood_Click(object sender, EventArgs e)
    {
        Ifood.DespacharPedido(orderId: Id_pedido);
    }

    private void buttonReadyToPickUp_Click(object sender, EventArgs e)
    {
        Ifood.AvisoReadyToPickUp(orderId: Id_pedido);
    }

    private void UCInfoPedido_Paint(object sender, PaintEventArgs e)
    {
        if (orderType == "TAKEOUT")
        {
            btnDespacharIfood.Visible = false;
            buttonReadyToPickUp.Location = new Point(166, 13);
        }
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        FormDeCancelamento modalCancelamento = new FormDeCancelamento() { id_Pedido = Id_pedido };
        modalCancelamento.display_Id = Display_id;
        modalCancelamento.ShowDialog();
    }


}
