using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.Ifood;
using SysIntegradorApp.data;
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

namespace SysIntegradorApp.UserControls.TaxyMachine;

public partial class UCEmpresaIntegradaTaxyMachine : UserControl
{

    public EmpresasEntregaTaxyMachine Empresa { get; set; }
    private TabControl TabControlQueChamou { get; set; }

    public UCEmpresaIntegradaTaxyMachine(EmpresasEntregaTaxyMachine empresa, TabControl tabControlQueChamou)
    {
        InitializeComponent();
        Empresa = empresa;
        TabControlQueChamou = tabControlQueChamou;

        ClsEstiloComponentes.SetRoundedRegion(this, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDeIfoodNome, 24);

        DefineInfosDaEmpresa();
    }

    public void DefineInfosDaEmpresa()
    {
        NomeEmpresa.Text = NomeEmpresa.Text += " " + Empresa.NomeEmpresa;
        textBoxUserName.Text = Empresa.Usuario;
        textBoxSenha.Text = Empresa.Senha;
        textBoxToken.Text = Empresa.MachineId;
        textBoxTipoPagamento.Text = Empresa.TipoPagamento;
        textBoxSenha.Text = Empresa.Senha;

        if (Empresa.Integra)
        {
            pictureBoxOnIntegra.Visible = true;
            pictureBoxOffItegra.Visible = false;
        }
        else
        {
            pictureBoxOnIntegra.Visible = false;
            pictureBoxOffItegra.Visible = true;
        }

        if (Empresa.CodEntregador == "66")
        {
            pictureBoxOTTO.Visible = true;
        }
        else if(Empresa.CodEntregador == "77")
        {
            pictureBoxJUMA.Visible = true;
        }
    }

    private async void AutBtn_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DialogResultSys dialogResult = await SysAlerta.Alerta("Ops", "Você tem certeza que deseja apagar essa empresa ?", SysAlertaTipo.Alerta, SysAlertaButtons.SimNao);

                if (dialogResult == DialogResultSys.Sim)
                {
                    db.empresastaxymachine.Remove(Empresa);
                    db.SaveChanges();

                    TabControlQueChamou.TabPages.Remove(TabControlQueChamou.SelectedTab!);

                    await SysAlerta.Alerta("Sucesso", "Empresa desintegrada com sucesso", SysAlertaTipo.Sucesso, SysAlertaButtons.Ok);
                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void pictureBoxOffItegra_Click(object sender, EventArgs e)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            var EmpresaAtual = db.empresastaxymachine.FirstOrDefault(x => x.Id == Empresa.Id)!;
            EmpresaAtual.Integra = true;
            await db.SaveChangesAsync();

            pictureBoxOffItegra.Visible = false;
            pictureBoxOnIntegra.Visible = true;
        }
    }

    private async void pictureBoxOnIntegra_Click(object sender, EventArgs e)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {

            var EmpresaAtual = db.empresastaxymachine.FirstOrDefault(x => x.Id == Empresa.Id)!;
            EmpresaAtual.Integra = false;
            await db.SaveChangesAsync();

            pictureBoxOffItegra.Visible = true;
            pictureBoxOnIntegra.Visible = false;
        }
    }
}
