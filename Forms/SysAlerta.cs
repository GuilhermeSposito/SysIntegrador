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

namespace SysIntegradorApp.Forms;

public enum DialogResultSys
{
    Ok,
    Cancelar,
    Sim,
    Nao
}

public enum SysAlertaTipo
{
    Alerta,
    Erro,
    Sucesso
}

public enum SysAlertaButtons
{
    Ok,
    SimNao
}

public partial class SysAlerta : Form
{
    public static DialogResultSys OpcDoUser { get; set; } = DialogResultSys.Ok;

    public SysAlerta()
    {
        InitializeComponent();

        ClsEstiloComponentes.SetRoundedRegion(panelDeTitulo, 24);
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDeMensagem, 24);
    }

    public static async Task<DialogResultSys> Alerta(string Titulo, string message, SysAlertaTipo TipoDoAlerta = SysAlertaTipo.Alerta, SysAlertaButtons Buttons = SysAlertaButtons.Ok)
    {
        try
        {

            var alerta = new SysAlerta();
            if (Buttons == SysAlertaButtons.SimNao)
            {
                alerta.BtnOk.Visible = false;
            }
            else
            {
                alerta.BtnSim.Visible = false;
                alerta.BtnNao.Visible = false;
            }

            if (TipoDoAlerta == SysAlertaTipo.Alerta)
            {
                ClsSons.PlaySomErroDb();

                alerta.pictureBoxError.Visible = false;
                alerta.pictureBoxSucesso.Visible = false;
                alerta.pictureBoxAviso.Visible = true;
            }
            else if (TipoDoAlerta == SysAlertaTipo.Erro)
            {
                ClsSons.PlaySom2();

                alerta.panelDeTitulo.BackColor = Color.FromArgb(255, 0, 0);
                alerta.pictureBoxError.Visible = true;
                alerta.pictureBoxAviso.Visible = false;
                alerta.pictureBoxSucesso.Visible = false;

            }
            else if (TipoDoAlerta == SysAlertaTipo.Sucesso)
            {
                alerta.panelDeTitulo.BackColor = Color.FromArgb(136, 201, 65);
                alerta.pictureBoxSucesso.Visible = true;
                alerta.pictureBoxError.Visible = false;
                alerta.pictureBoxAviso.Visible = false;
            }


            alerta.lblTitulo.Text = Titulo;
            alerta.LblMensagem.Text = message;
            alerta.ShowDialog();

            return OpcDoUser;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        return OpcDoUser;
    }

    private void BtnSim_Click(object sender, EventArgs e)
    {

        var teste = (Button)sender;

        OpcDoUser = DialogResultSys.Sim;


        this.Close();
    }

    private void BtnNao_Click(object sender, EventArgs e)
    {

        OpcDoUser = DialogResultSys.Nao;
        this.Close();
    }

    private void BtnOk_Click(object sender, EventArgs e)
    {
        OpcDoUser = DialogResultSys.Ok;
        this.Close();
    }
}
