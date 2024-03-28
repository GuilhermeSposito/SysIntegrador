using SysIntegradorApp.ClassesAuxiliares;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp
{
    public partial class FormMenuInicial : Form
    {
        //public Panel panelPedidos; 
        public FormMenuInicial()
        {
            InitializeComponent();
        }

        private void panelPedidos_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FormMenuInicial_Load(object sender, EventArgs e)
        {
            Ifood.SetTimer();
            SetarPanelPedidos();

        }

        private void FormMenuInicial_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void FormMenuInicial_Shown(object sender, EventArgs e)
        {

        }

        private async void btnTeste_Click(object sender, EventArgs e)
        {
            SetarPanelPedidos();
        }

        public static async void SetarPanelPedidos()
        {
            try
            {
                List<PedidoCompleto> pedidos = await Ifood.GetPedido();

                panelPedidos.Controls.Clear();
                panelPedidos.PerformLayout();

                //Faz um loop para adicionar os UserControls De pedido no panel
                foreach (PedidoCompleto item in pedidos)
                {
                        UCPedido UserControlPedido = new UCPedido() { Id_pedido = item.id, //aqui seta as propriedades dentro da classe para podermos usar essa informação dinamicamente no pedido
                            NomePedido = item.customer.name,
                            FeitoAs = item.createdAt,
                            HorarioEntrega =  item.orderTiming,
                            LocalizadorPedido = item.delivery.pickupCode,
                            EnderecoFormatado = item.delivery.deliveryAddress.formattedAddress,
                            Bairro = item.delivery.deliveryAddress.neighborhood,
                            TipoDaEntrega = item.delivery.deliveredBy,
                            ValorTotalItens = item.total.subTotal,
                            ValorTaxaDeentrega = item.total.deliveryFee,
                            Valortaxaadicional = item.total.additionalFees,
                            Descontos = item.total.benefits,
                            TotalDoPedido = item.total.orderAmount,
                            Observations=  item.delivery.observations };


                        UserControlPedido.SetLabels(item.id, item.displayId, item.customer.name, item.createdAt); // aqui muda as labels do user control para cada pedido em questão
                          
                        panelPedidos.Controls.Add(UserControlPedido); //Aqui adiciona o user control no panel 
                }

                panelPedidos.PerformLayout();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OK);
            }
        }
    }
}
