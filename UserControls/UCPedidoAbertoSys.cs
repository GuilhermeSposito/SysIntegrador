using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.UserControls;

public partial class UCPedidoAbertoSys : UserControl
{

    public Sequencia PedidoParaDeliveyAtual { get; set; }
    public bool ListandoPedidoAbertos { get; set; }

    public UCPedidoAbertoSys()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 18);
    }

    private async void UCPedidoAbertoSys_Paint(object sender, PaintEventArgs e)
    {
        try
        {
            DelMatch Delmatch = new DelMatch(new MeuContexto());

            ClsDeserializacaoDelMatchEntrega pedido = new ClsDeserializacaoDelMatchEntrega();

            if (ListandoPedidoAbertos)
            {
                pedido = await Delmatch.GetPedido(PedidoParaDeliveyAtual.DelMatchId);
                labelStatus.Text = ClsDeTraducaoDelMatchStatus.TraduzStatus(pedido.Status);

                picBoxCheck.Visible = false;
                picBoxCheck.Enabled = false;
            }

            labelNomeCliente.Text = PedidoParaDeliveyAtual.Customer.Name;
            labelNumConta.Text = PedidoParaDeliveyAtual.numConta.ToString().PadLeft(4, '0');
            labelEndereco.Text = PedidoParaDeliveyAtual.DeliveryAddress.StreetName.ToString();

            labelValorPedido.Text = PedidoParaDeliveyAtual.ValorConta.ToString("c");
            // panel1.BackColor = SystemColors.ControlDark;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }

    }

    private void UCPedidoAbertoSys_Load(object sender, EventArgs e) { }

    private void UCPedidoAbertoSys_Click(object sender, EventArgs e)
    {
        if (!ListandoPedidoAbertos)
        {
            if (picBoxCheck.Visible == true)
            {
                picBoxCheck.Visible = false;
                if (FormDePedidosAbertos.ItensAEnviarDelMach.Count > 0)
                {
                    int indexARemover = FormDePedidosAbertos.ItensAEnviarDelMach.FindIndex(x => x.numConta == PedidoParaDeliveyAtual.numConta);
                    if (indexARemover != -1)
                    {
                        FormDePedidosAbertos.ItensAEnviarDelMach.RemoveAt(indexARemover);
                    }
                }
            }
            else
            {
                //this.BackColor = SystemColors.ControlDarkDark;
                picBoxCheck.Visible = true;
                FormDePedidosAbertos.ItensAEnviarDelMach.Add(PedidoParaDeliveyAtual);
            }
        }
    }

    private void UCPedidoAbertoSys_Enter(object sender, EventArgs e) { }

    private void panel1_Click(object sender, EventArgs e)
    {
        UCPedidoAbertoSys_Click(sender, e);
    }

    private void picBoxCheck_Click(object sender, EventArgs e)
    {
        UCPedidoAbertoSys_Click(sender, e);
    }

    public void MudaVisibilidadeDaPictureBox()
    {
        this.picBoxCheck.Visible = true;
    }
}
