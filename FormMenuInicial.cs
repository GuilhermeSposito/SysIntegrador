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
            this.Invoke((MethodInvoker)delegate
            {
                
                Ifood.GetPedido("fefcb2b0-f56f-4080-aa1d-80406ef0ecde");
                //UCInfoPedido infoPedido = new UCInfoPedido();
                //panelDetalhePedido.Controls.Add(infoPedido);
            });

        }

        private void FormMenuInicial_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void FormMenuInicial_Shown(object sender, EventArgs e)
        {

        }

        private void btnTeste_Click(object sender, EventArgs e)
        {
            Ifood.GetPedido("fefcb2b0-f56f-4080-aa1d-80406ef0ecde");
        }
    }
}
