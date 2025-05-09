using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.Ifood;
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

public partial class UCInfoDeLojaOnLine : UserControl
{
    private EmpresasIfood empresaIfood { get; set; }
    public UCInfoDeLojaOnLine(EmpresasIfood empresa)
    {
        InitializeComponent();
        empresaIfood = empresa;

        ClsEstiloComponentes.SetRoundedRegion(this, 20);


        DefineInfosDeEmpresa();
    }

    private void DefineInfosDeEmpresa()
    {
        LblNomeDaEmpresa.Text = empresaIfood.NomeIdentificador;
        if (empresaIfood.Online)
        {
            labelStatus.Text = "Online";
            labelStatus.ForeColor = Color.Green;
            pictureBoxOnline.Visible = true;
            pictureBoxOfline.Visible = false;
        }
        else
        {
            labelStatus.Text = "Offline";
            labelStatus.ForeColor = Color.Black;
            pictureBoxOnline.Visible = false;
            pictureBoxOfline.Visible = true;
        }
    }
}
