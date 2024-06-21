using SysIntegradorApp.ClassesAuxiliares;
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

public partial class UCNumPagina : UserControl
{
    public UCNumPagina()
    {
        InitializeComponent();
        //ClsEstiloComponentes.SetRoundedRegion(this, 24);
    }

    public void MudaNumero(UCNumPagina instancia, int num)
    {
        instancia.num.Text = num.ToString();
    }
}
