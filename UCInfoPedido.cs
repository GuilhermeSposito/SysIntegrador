using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp
{
    public partial class UCInfoPedido : UserControl
    {
        public UCInfoPedido()
        {
            InitializeComponent();
        }

        private void labelPedidoNM_Click(object sender, EventArgs e){ }

        private void label4_Click(object sender, EventArgs e){ }

        public void SetLabels(string id_Pedido,
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
            numId.Text = id_Pedido;
            label1.Text = nomePedido;
            dateFeitoAs.Text = feitoAs;
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

    }
}
