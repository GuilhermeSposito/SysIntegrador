using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class UCPedido : UserControl
{
    public string? Id_pedido { get; set; }
    
    public UCPedido()
    {
        InitializeComponent();
    }

    public void SetLabels(string id_pedido = "Null", string numPedido = "Null", string nomePedido = "Null", string horarioPedido = "Null" , string statusPedido = "Null")
    {
        Id_pedido = id_pedido;
        labelNumPedido.Text = $"#{numPedido}";
        labelNomePedido.Text = nomePedido;
        labelHorarioDeEntrega.Text = horarioPedido;
        labelStatus.Text = statusPedido;
    }

    private void labelStatus_Click(object sender, EventArgs e) { }

    private void UCPedido_Load(object sender, EventArgs e) { }

    private void UCPedido_Click(object sender, EventArgs e)
    {
        FormMenuInicial.panelDetalhePedido.Controls.Clear();
        FormMenuInicial.panelDetalhePedido.PerformLayout();
        UCInfoPedido infoPedido = new UCInfoPedido();
        infoPedido.SetLabels(Id_pedido);
        FormMenuInicial.panelDetalhePedido.Controls.Add(infoPedido);
    }
}
