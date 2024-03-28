using SysIntegradorApp.ClassesAuxiliares;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class UCPedido : UserControl
{
    public string? Id_pedido { get; set; }
    public string? NomePedido { get; set; }
    public string? FeitoAs {  get; set; }
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

    public UCPedido()
    {
        InitializeComponent();
    }



    public void SetLabels(string id_pedido, string numPedido, string nomePedido, string horarioPedido, string statusPedido = "teste")
    {
        Id_pedido = id_pedido;
        labelNumPedido.Text = $"#{numPedido}";
        labelNomePedido.Text = nomePedido;
        labelHorarioDeEntrega.Text = horarioPedido;
        labelStatus.Text = statusPedido;
    }

    private void labelStatus_Click(object sender, EventArgs e) { }

    private void UCPedido_Load(object sender, EventArgs e) { }

    public void UCPedido_Click(object sender, EventArgs e)
    {
        FormMenuInicial.panelDetalhePedido.Controls.Clear();
        FormMenuInicial.panelDetalhePedido.PerformLayout();
        UCInfoPedido infoPedido = new UCInfoPedido();
        infoPedido.SetLabels(   id_Pedido: Id_pedido,
                               nomePedido: NomePedido,
                               feitoAs: FeitoAs,
                               horarioEntrega: HorarioEntrega,
                               localizadorPedido: LocalizadorPedido,
                               enderecoFormatado: EnderecoFormatado,
                               bairro: Bairro,
                               TipoEntrega: TipoDaEntrega,
                               valorTotalItens: ValorTotalItens,
                               valorTaxaDeentrega: ValorTaxaDeentrega,
                               valortaxaadicional: Valortaxaadicional,
                               descontos: Descontos,
                               total: TotalDoPedido,
                               observations: Observations);
        FormMenuInicial.panelDetalhePedido.Controls.Add(infoPedido);
    }
}
