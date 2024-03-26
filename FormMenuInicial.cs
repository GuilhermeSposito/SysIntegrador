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

        private  void FormMenuInicial_Load(object sender, EventArgs e)
        {
            Ifood.SetTimer();
            this.Invoke((MethodInvoker)async delegate
            { 
               await Ifood.GetPedido();
            });

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
          await Ifood.GetPedido();
        }
    }
}
