using SysIntegradorApp.ClassesAuxiliares;
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
            await ccm.RequisicaoHttp(metodo: "LIMPAPEDIDO", numPedido: NumeroPedido);
            ClsDeIntegracaoSys.ExcluiPedidoCasoCancelado(NumeroPedido.ToString());

            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.panelDetalhePedido.Controls.Clear()));
            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe)));
            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true));

            this.Dispose(true);
        }
    }
}
