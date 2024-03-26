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

        public void SetLabels(string id_Pedido)
        {
            labelLocalizadorPedido.Text = "Localizador teste do pedido";
            label1.Text = id_Pedido; 
        }

    }
}
