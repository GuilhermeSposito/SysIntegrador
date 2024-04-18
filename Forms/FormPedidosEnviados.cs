using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms
{
    public partial class FormPedidosEnviados : Form
    {

        public Sequencia PedidoAtual { get; set; }

        public FormPedidosEnviados()
        {
            InitializeComponent();
        }

        public void AdicionaNoPanel(UCPedidoAbertoSys pedidoAberto)
        {
            panelDePedidosJaEnviados.Controls.Add(pedidoAberto);
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            FormDePedidosAbertos.ItensAEnviarDelMach.Clear();
            this.Close();
        }
    }
}
