using SysIntegradorApp.ClassesAuxiliares;
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

    public UCPedidoAbertoSys()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
    }

    private void UCPedidoAbertoSys_Paint(object sender, PaintEventArgs e)
    {
        labelNomeCliente.Text = PedidoParaDeliveyAtual.Customer.Name;
        labelNumConta.Text = PedidoParaDeliveyAtual.ShortReference;  //.numConta.ToString();
        labelEndereco.Text = PedidoParaDeliveyAtual.DeliveryAddress.StreetName.ToString();
        labelValorPedido.Text = PedidoParaDeliveyAtual.ValorConta.ToString("c");
       // panel1.BackColor = SystemColors.ControlDark;
    }

    private void UCPedidoAbertoSys_Load(object sender, EventArgs e) { }

    private void UCPedidoAbertoSys_Click(object sender, EventArgs e)
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
