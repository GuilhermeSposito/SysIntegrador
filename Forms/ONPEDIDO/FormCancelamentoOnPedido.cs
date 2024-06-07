using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms.ONPEDIDO
{
    public partial class FormCancelamentoOnPedido : Form
    {
        public string? IdPedido { get; set; }

        public FormCancelamentoOnPedido()
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(this, 24);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnEnvir_Click(object sender, EventArgs e)
        {
            try
            {
                string? motivo = motivoCancelamento.Text;

                bool cancelou = await OnPedido.CancelaPedido(IdPedido, motivo);

                if (cancelou)
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erro ao cancelar pedido", "Não Foi possivel");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cancelar pedido", "Ops");
            }
        }

        private void FormCancelamentoOnPedido_Load(object sender, EventArgs e)
        {
            motivoCancelamento.Focus();
        }

        private void motivoCancelamento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnEnvir_Click(sender, e);
            }
        }
    }
}
