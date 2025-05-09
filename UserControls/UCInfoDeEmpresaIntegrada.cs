using Microsoft.EntityFrameworkCore;
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

namespace SysIntegradorApp.UserControls;

public partial class UCInfoDeEmpresaIntegrada : UserControl
{
    private EmpresasIfood empresaIfood { get; set; }
    private TabControl TabControlQueChamou { get; set; }

    public UCInfoDeEmpresaIntegrada(EmpresasIfood empresa, TabControl tabControlQueChamou)
    {
        InitializeComponent();
        empresaIfood = empresa;
        TabControlQueChamou = tabControlQueChamou;

        ClsEstiloComponentes.SetRoundedRegion(panelDeIfoodNome, 24);

        DefineInfosDaEmpresa();
    }

    public void DefineInfosDaEmpresa()
    {
        LabelNomeDaEmpresa.Text = empresaIfood.NomeIdentificador;
        textBoxAcessToken.Text = empresaIfood.Token;
        textBoxRefreshToken.Text = empresaIfood.RefreshToken;
        textBoxVenceTokenIfoodEm.Text = empresaIfood.DataExpiracao;
        textBoxMerchantId.Text = empresaIfood.MerchantId;

        if (empresaIfood.IntegraEmpresa)
        {
            pictureBoxOnIntegraIfood.Visible = true;
            pictureBoxOffItegraIfood.Visible = false;
        }
        else
        {
            pictureBoxOnIntegraIfood.Visible = false;
            pictureBoxOffItegraIfood.Visible = true;
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
                    db.empresasIfoods.Remove(empresaIfood);
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

    private async void pictureBoxOffItegraIfood_Click(object sender, EventArgs e)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            EmpresasIfood Empresa = db.empresasIfoods.FirstOrDefault(x => x.Id == empresaIfood.Id)!;
            Empresa.IntegraEmpresa = true;
            await db.SaveChangesAsync();

            pictureBoxOffItegraIfood.Visible = false;
            pictureBoxOnIntegraIfood.Visible = true;
        }
    }

    private async void pictureBoxOnIntegraIfood_Click(object sender, EventArgs e)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            EmpresasIfood Empresa = db.empresasIfoods.FirstOrDefault(x => x.Id == empresaIfood.Id)!;
            Empresa.IntegraEmpresa = false;
            await db.SaveChangesAsync();

            pictureBoxOffItegraIfood.Visible = true;
            pictureBoxOnIntegraIfood.Visible = false;
        }
    }
}
