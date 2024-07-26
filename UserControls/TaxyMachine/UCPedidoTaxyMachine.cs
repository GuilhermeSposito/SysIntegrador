using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.logs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SysIntegradorApp.UserControls.TaxyMachine;

public partial class UCPedidoTaxyMachine : UserControl
{
    public int NumConta { get; set; }
    public Sequencia Sequencia { get; set; } = new Sequencia();

    public UCPedidoTaxyMachine()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24);

        this.labelNomeCliente.Click += (sender, e) => this.OnClick(e);
        this.labelEndereco.Click += (sender, e) => this.OnClick(e);
        this.labelComplemento.Click += (sender, e) => this.OnClick(e);
        this.LabelNumPedido.Click += (sender, e) => this.OnClick(e);

    }

    private async void UCPedidoTaxyMachine_Load(object sender, EventArgs e)
    {
        try
        {
            LabelNumPedido.Text = NumConta.ToString().PadLeft(3, '0');

            labelValor.Text = Sequencia.ValorConta.ToString("c");
            labelNomeCliente.Text = Sequencia.Customer.Name;
            labelEndereco.Text = Sequencia.DeliveryAddress.FormattedAddress + " - " +Sequencia.DeliveryAddress.Neighborhood;
            labelComplemento.Text = Sequencia.DeliveryAddress.Complement;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
        }
    }


    public void MudaLabelParaEnviado()
    {
        this.LabelEnviado.Visible = true;

        if (Sequencia.Machine_ID == "ERRO")
        {
            LabelEnviado.Text = "Não Enviado";
            LabelEnviado.ForeColor = Color.Red;
        }
    }

    public void AdicionaEvento(UCPedidoTaxyMachine instancia, EventHandler evento)
    {
        this.labelNomeCliente.Click += evento;
    }

    private void UCPedidoTaxyMachine_MouseEnter(object sender, EventArgs e)
    {
        this.BackColor = SystemColors.ControlDark;
    }

    private void UCPedidoTaxyMachine_MouseLeave(object sender, EventArgs e)
    {
        this.BackColor = SystemColors.ControlLight;
    }

    private void labelNomeCliente_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void labelNomeCliente_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void labelEndereco_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void labelEndereco_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void labelComplemento_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void labelComplemento_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void checkBoxRetorno_CheckedChanged(object sender, EventArgs e)
    {
        Sequencia.Retorno = checkBoxRetorno.Checked;
    }

    private void LabelEnviado_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void LabelEnviado_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void checkBoxRetorno_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void checkBoxRetorno_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void LabelNumPedido_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void LabelNumPedido_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);

    }

    private void labelValor_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void labelValor_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void pictureBox1_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void pictureBox1_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void pictureBox2_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void pictureBox2_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }

    private void pictureBox3_MouseEnter(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseEnter(sender, e);
    }

    private void pictureBox3_MouseLeave(object sender, EventArgs e)
    {
        this.UCPedidoTaxyMachine_MouseLeave(sender, e);
    }
}
