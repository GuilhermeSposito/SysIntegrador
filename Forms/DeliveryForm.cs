﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
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

    private void label2_Click(object sender, EventArgs e)
    {

        List<Sequencia> pedidosAbertos = DelMatch.ListarPedidosAbertos();
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

    private void panelDeIniciarEntrega_Click(object sender, EventArgs e)
    {

        List<Sequencia> pedidosAbertos = DelMatch.ListarPedidosAbertos();
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

    private void panelDeListarPedidos_Click(object sender, EventArgs e)
    {
        List<Sequencia> pedidosAbertos = DelMatch.ListarPedidosJaEnviados();
        int contagemdepedidos = pedidosAbertos.Count;

        if (contagemdepedidos > 0)
        {
            FormPedidosEnviados modalPedidosEnviados = new FormPedidosEnviados();

            foreach (var item in pedidosAbertos)
            {
                var userControlPedido = new UserControls.UCPedidoAbertoSys() { PedidoParaDeliveyAtual = item };
                userControlPedido.MudaVisibilidadeDaPictureBox();
                modalPedidosEnviados.AdicionaNoPanel(userControlPedido);
            }

            modalPedidosEnviados.ShowDialog();
        }
        else
        {
            MessageBox.Show("Não Tem nenhum pedido em aberto");
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


}
