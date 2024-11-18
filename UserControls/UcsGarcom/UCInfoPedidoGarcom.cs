using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.UserControls.UcsGarcom
{
    public partial class UCInfoPedidoGarcom : UserControl
    {
        public UCInfoPedidoGarcom()
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(this, 24); //da uma arredondada na borda do user control
            this.Resize += (sender, e) =>
            {
                ClsEstiloComponentes.SetRoundedRegion(this, 24);
            };
        }

        public Pedido? Pedido { get; set; }
        public string? StatusPedido { get; set; }
        public List<Produto> items { get; set; } = new List<Produto>();


        public void SetLabels()
        {
            //  this.toolTip1.SetToolTip(this.pictureBox16, "Copiar Endereço de entrega");

            string? DefineMesaOuComanda = Pedido!.Mesa == null ? Pedido.Comanda : Pedido.Mesa;
            string? DefineMesaOuComandaString = Pedido!.Mesa == null ? "Comanda" : "Mesa";


            string? DefineEntrega = DefineMesaOuComanda;
            string? DefineLocalEntrega = DefineMesaOuComanda;


            labelDisplayId.Text = $"#{DefineMesaOuComanda}";


            label1.Text = DefineMesaOuComanda;

            dateFeitoAs.Text = Pedido!.HorarioFeito!.ToString()!.Substring(11, 5);
            tipoEntrega.Text = $"{DefineMesaOuComandaString} {DefineEntrega}";
            horarioEntregaPrevista.Text = "Teste";//Pedido.HorarioFeito.Value.AddMinutes(15).ToString()!.Substring(11, 5);
            labelEndereco.Text = $"Entregar para a {DefineMesaOuComandaString} {DefineLocalEntrega}";

            float ValorTotal = 0f;
            float ValorTotalComComplemento = 0f;
            foreach (var item in Pedido.produtos)
            {
                ClsDeSuporteParaImpressaoDosItens DoSuporte = ClsDeIntegracaoSys.DefineCaracteristicasDoItemGarcomSys(item);

                ValorTotal += DoSuporte.valorDoItem;
                ValorTotalComComplemento += DoSuporte.valorTotalDoItem;
            }

            ValorTotalDosItens.Text = ValorTotal.ToString("c");
            valorTotal.Text = ValorTotalComComplemento.ToString("c");

        }

        private void pictureBoxUm_Click(object sender, EventArgs e)
        {
            pictureBoxUm.Visible = false;
            pictureBoxDois.Visible = true;

        }

        private void pictureBoxDois_Click(object sender, EventArgs e)
        {
            pictureBoxUm.Visible = true;
            pictureBoxDois.Visible = false;
        }

        public void InsereItemNoPedido(List<Produto> items)
        {
            //primeiro instanciar um objeto do UserControl UCItem
            //Dentro do UserControl Temos que criar um método que define as labels 

            foreach (var item in items)
            {
                UCItemGarcom uCItem = new UCItemGarcom();
                uCItem.SetLabels(item);
                panelDeItens.Controls.Add(uCItem);
            }

        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            string? PedidoJson = JsonConvert.SerializeObject(Pedido);

            ImpressaoGarcom.ChamaImpessoes(PedidoJson);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult OpcUser = MessageBox.Show("Você deseja excluir este pedido ?\n (Lembrando que o pedido não sera excluido do sysmenu, apenas da tela do integrador)", "Excluindo Pedido", MessageBoxButtons.OKCancel);

            if (OpcUser == DialogResult.OK)
            {
                try
                {
                    var pedido = Pedido;

                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        ParametrosDoPedido? Pedido = db.parametrosdopedido.Where(x => x.Id == pedido.Id).FirstOrDefault();

                        db.parametrosdopedido.Remove(Pedido);
                        db.SaveChanges();

                        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
                        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.panelDetalhePedido.Controls.Clear()));
                        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe)));
                        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir pedido: " + ex.Message);
                }
            }
        }
    }
}
