using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.Forms;

namespace SysIntegradorApp;

public partial class DeliveryForm : Form
{
    public DeliveryForm()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(panelDeIniciarEntrega, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDeListarPedidos, 24);
        ClsEstiloComponentes.SetRoundedRegion(pictureBoxEmpresaDelivery, 50);
        ClsEstiloComponentes.SetRoundedRegion(pictureBox2, 50);
    }



    private void DeliveryForm_Paint(object sender, PaintEventArgs e)
    {
        FormMenuInicial.panelDetalhePedido.Controls.Clear();
        FormMenuInicial.panelPedidos.Controls.Clear();
    }

    private void DeliveryForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe);
        FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true;
        FormMenuInicial.SetarPanelPedidos();

    }

    private async void label2_Click(object sender, EventArgs e)
    {
        DelMatch Delmatch = new DelMatch(new MeuContexto());

        List<Sequencia> pedidosAbertos = await Delmatch.ListarPedidosAbertos();
        int contagemdepedidos = pedidosAbertos.Count;


        if (contagemdepedidos > 0)
        {
            FormDePedidosAbertos modalPedidosAbertos = new FormDePedidosAbertos();

            foreach (var item in pedidosAbertos)
            {
                modalPedidosAbertos.AdicionaNoPanel(new UserControls.UCPedidoAbertoSys() { PedidoParaDeliveyAtual = item });
            }

            modalPedidosAbertos.ShowDialog();
        }
        else
        {
            MessageBox.Show("Não Tem nenhum pedido em aberto");
        }


    }

    private void panelDeIniciarEntrega_Paint(object sender, PaintEventArgs e)
    { }

    private async void panelDeIniciarEntrega_Click(object sender, EventArgs e)
    {
        DelMatch Delmatch = new DelMatch(new MeuContexto());


        List<Sequencia> pedidosAbertos = await Delmatch.ListarPedidosAbertos();
        int contagemdepedidos = pedidosAbertos.Count;


        if (contagemdepedidos > 0)
        {
            FormDePedidosAbertos modalPedidosAbertos = new FormDePedidosAbertos();

            foreach (var item in pedidosAbertos)
            {
                modalPedidosAbertos.AdicionaNoPanel(new UserControls.UCPedidoAbertoSys() { PedidoParaDeliveyAtual = item, ListandoPedidoAbertos = false });
            }

            modalPedidosAbertos.ShowDialog();
        }
        else
        {
            MessageBox.Show("Não Tem nenhum pedido em aberto");
        }

    }

    private async void panelDeListarPedidos_Click(object sender, EventArgs e)
    {
        DelMatch Delmatch = new DelMatch(new MeuContexto());


        List<Sequencia> pedidosAbertos = await Delmatch.ListarPedidosJaEnviados();
        int contagemdepedidos = pedidosAbertos.Count;

        if (contagemdepedidos > 0)
        {
            FormPedidosEnviados modalPedidosEnviados = new FormPedidosEnviados();

            foreach (var item in pedidosAbertos)
            {
                var userControlPedido = new UserControls.UCPedidoAbertoSys() { PedidoParaDeliveyAtual = item, ListandoPedidoAbertos = true };
                userControlPedido.MudaVisibilidadeDaPictureBox();
                modalPedidosEnviados.AdicionaNoPanel(userControlPedido);
            }

            modalPedidosEnviados.ShowDialog();
        }
        else
        {
            MessageBox.Show("Não temos nenhum pedido enviado para ser listado.");
        }


    }

    private void panelDeIniciarEntrega_MouseEnter(object sender, EventArgs e)
    {
        panelDeIniciarEntrega.BackColor = Color.FromArgb(197, 85, 6);
    }

    private void panelDeIniciarEntrega_MouseLeave(object sender, EventArgs e)
    {
        panelDeIniciarEntrega.BackColor = Color.FromArgb(219, 95, 7);
    }

    private void label2_MouseEnter(object sender, EventArgs e)
    {
        panelDeIniciarEntrega.BackColor = Color.FromArgb(197, 85, 6);
    }

    private void label2_MouseLeave(object sender, EventArgs e)
    {
        panelDeIniciarEntrega.BackColor = Color.FromArgb(197, 85, 6);
    }

    private void panelDeListarPedidos_MouseEnter(object sender, EventArgs e)
    {
        panelDeListarPedidos.BackColor = Color.FromArgb(197, 85, 6);
    }

    private void panelDeListarPedidos_MouseLeave(object sender, EventArgs e)
    {
        panelDeListarPedidos.BackColor = Color.FromArgb(219, 95, 7);
    }

    private void label1_MouseEnter(object sender, EventArgs e)
    {
        panelDeListarPedidos.BackColor = Color.FromArgb(197, 85, 6);
    }

    private void label1_Click(object sender, EventArgs e)
    {
        panelDeListarPedidos_Click(sender, e);
    }

    private void pictureBoxEmpresaDelivery_Click(object sender, EventArgs e)
    {
        if (Uri.IsWellFormedUriString("https://delmatchdelivery.com/site/login", UriKind.Absolute))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://delmatchdelivery.com/site/login",
                UseShellExecute = true
            });
        }
    }

    private async void pictureBox2_Click(object sender, EventArgs e)
    {
        FormMapaDelmatch mapa = new FormMapaDelmatch();
        await mapa.StartMapa();
        mapa.Show();
    }
}

