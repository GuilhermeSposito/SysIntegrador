using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms.CCM
{
    public partial class FormDePedidoNaoAceito : Form
    {
        public int NumeroPedido {  get; set; }   

        public FormDePedidoNaoAceito()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private async void btnEnvir_Click(object sender, EventArgs e)
        {
            SysIntegradorApp.ClassesDeConexaoComApps.CCM ccm = new ClassesDeConexaoComApps.CCM(new MeuContexto());

            await ccm.RecusaPedido(NumeroPedido, msg: motivoCancelamento.Text);

            this.Dispose(true);
        }
    }
}
