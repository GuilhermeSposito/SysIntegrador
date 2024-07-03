using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
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

namespace SysIntegradorApp.Forms.ONPEDIDO
{
    public partial class FormAguardoDeConclPedidos : Form
    {
        public FormAguardoDeConclPedidos Instancia { get; set; }
        public List<ParametrosDoPedido> Pedidos { get; set; } = new List<ParametrosDoPedido>();
        public FormAguardoDeConclPedidos()
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(this, 24);
        }

        public void mudaNumerosDePedido(FormAguardoDeConclPedidos instancia, int numPedidoAtual)
        {
        }

        public static async void AtivaConclusaoDosPedidosAut(List<ParametrosDoPedido> pedidos)
        {
            try
            {
                OnPedido OnPedido = new OnPedido(new MeuContexto());

                int numDePedidos = pedidos.Count();
                NumPedidos.Text = numDePedidos.ToString();

                foreach (var p in pedidos)
                {

                    await OnPedido.ConcluirPedido(p.Id, concluiuAut: true);
                    Thread.Sleep(20000);

                    numDePedidos--;

                    NumPedidos.Text = numDePedidos.ToString();

                }

                FormMenuInicial.FechaModalDeAguardoDeConclPedidos();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void FormAguardoDeConclPedidos_Load(object sender, EventArgs e)
        {
            // Thread.Sleep(5000);
            await Task.Run(() => AtivaConclusaoDosPedidosAut(Pedidos));
        }
    }
}
