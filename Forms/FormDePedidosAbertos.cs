using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.UserControls;
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
using Newtonsoft.Json;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data.InterfaceDeContexto;

namespace SysIntegradorApp.Forms;

public partial class FormDePedidosAbertos : Form
{
    public static List<Sequencia> ItensAEnviarDelMach { get; set; } = new List<Sequencia>();

    public FormDePedidosAbertos()
    {
        InitializeComponent();
    }

    public void AdicionaNoPanel(UCPedidoAbertoSys pedidoAberto)
    {
        panelDepedidosAbertos.Controls.Add(pedidoAberto);
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        ItensAEnviarDelMach.Clear();
        this.Close();
    }

    private async void btnEnviar_Click(object sender, EventArgs e)
    {
        DelMatch Delmatch = new DelMatch(new MeuContexto());

        if (ItensAEnviarDelMach.Count() > 0)
        {

            foreach (var item in ItensAEnviarDelMach)
            {
                string jsonContent = JsonConvert.SerializeObject(item);
                await Delmatch.GerarPedido(jsonContent);
                Delmatch.UpdateDelMatchId(item.numConta, item.ShortReference);
            }

            ItensAEnviarDelMach.Clear();
            this.Close();
        }
        else
        {
            MessageBox.Show("Nenhum Pedido Selecionado para enviar para delivery, selecione ao menos um!");
        }

    }
}
