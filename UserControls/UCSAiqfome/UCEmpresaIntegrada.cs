using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;
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

namespace SysIntegradorApp.UserControls.UCSAiqfome;

public partial class UCEmpresaIntegrada : UserControl
{
    private ClsEmpresasAiqFome Empresa { get; set; }
    private TabControl TabControlQueChamou { get; set; }

    public UCEmpresaIntegrada(ClsEmpresasAiqFome empresa, TabControl tabControlQueChamou)
    {
        InitializeComponent();
        Empresa = empresa;
        TabControlQueChamou = tabControlQueChamou;

        ClsEstiloComponentes.SetRoundedRegion(panelDeIfoodNome, 24);

        DefineInfosDaEmpresa();
    }

    public void DefineInfosDaEmpresa()
    {
        LabelNomeDaEmpresa.Text = Empresa.NomeIdentificador;
        textBoxAcessToken.Text = Empresa.TokenReq;
        textBoxRefreshToken.Text = Empresa.RefreshToken;
        textBoxVenceTokenIfoodEm.Text = Empresa.TokenExpiracao;
        textBoxMerchantId.Text = Empresa.ClientId;

        if (Empresa.IntegraEmpresa)
        {
            pictureBoxOnIntegraAiQueFome.Visible = true;
            pictureBoxOffItegraAiQueFome.Visible = false;
        }
        else
        {
            pictureBoxOnIntegraAiQueFome.Visible = false;
            pictureBoxOffItegraAiQueFome.Visible = true;
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
                    db.empresasaiqfome.Remove(Empresa);
                    db.SaveChanges();

                    TabControlQueChamou.TabPages.Remove(TabControlQueChamou.SelectedTab!);

                    await SysAlerta.Alerta("Sucesso", "Empresa apagada com sucesso", SysAlertaTipo.Sucesso, SysAlertaButtons.Ok);
                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void pictureBoxOnIntegraAiQueFome_Click(object sender, EventArgs e)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            ClsEmpresasAiqFome EmpresaSelecionada = db.empresasaiqfome.FirstOrDefault(x => x.Id == Empresa.Id)!;
            EmpresaSelecionada.IntegraEmpresa = false;
            await db.SaveChangesAsync();

            pictureBoxOffItegraAiQueFome.Visible = true;
            pictureBoxOnIntegraAiQueFome.Visible = false;
        }
    }

    private async void pictureBoxOffItegraAiQueFome_Click(object sender, EventArgs e)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            ClsEmpresasAiqFome EmpresaSelecionada = db.empresasaiqfome.FirstOrDefault(x => x.Id == Empresa.Id)!;
            EmpresaSelecionada.IntegraEmpresa = true;
            await db.SaveChangesAsync();

            pictureBoxOffItegraAiQueFome.Visible = false;
            pictureBoxOnIntegraAiQueFome.Visible = true;
        }
    }
}
