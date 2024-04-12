using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesDeConexaoComApps;

namespace SysIntegradorApp;

public partial class DeliveryForm : Form
{
    public DeliveryForm()
    {
        InitializeComponent();
        SetRoundedRegion(panelDeIniciarEntrega, 24);
        SetRoundedRegion(panelDeListarPedidos, 24);
        SetRoundedRegion(pictureBoxEmpresaDelivery, 50);
    }

    private void SetRoundedRegion(Control control, int radius) //Método para arredondar os cantos dos paineis
    {
        GraphicsPath path = new GraphicsPath();
        int width = control.Width;
        int height = control.Height;
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(width - radius, 0, radius, radius, 270, 90);
        path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
        path.AddArc(0, height - radius, radius, radius, 90, 90);
        path.CloseFigure();

        control.Region = new Region(path);
    }

    private void pictureBoxDeInciarPedido_Click(object sender, EventArgs e)
    {
        DelMatch.GerarPedido();
    }
}
