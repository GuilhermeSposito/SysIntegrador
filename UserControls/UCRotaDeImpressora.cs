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

public partial class UCRotaDeImpressora : UserControl
{
    private ClsRoteamentoDeImpressao RotaDeImpressao { get; set; }
    private TabControl TabControlQueChamou { get; set; }

    public UCRotaDeImpressora(ClsRoteamentoDeImpressao rota, TabControl tabControl)
    {
        RotaDeImpressao = rota;
        TabControlQueChamou = tabControl;

        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
        ClsEstiloComponentes.SetRoundedRegion(PanelDeAtivacao, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelImps, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelNomeRota, 24);


        AlimentaComboBoxDeImpressorasEmAdicionarNovaImpressora(this);
        DefineValoresDasConfigVindaDoBanco(this);
    }

    public void DefineValoresDasConfigVindaDoBanco(UCRotaDeImpressora instancia)
    {
        instancia.comboBoxImpressoraCaixaAdicionar.Text = RotaDeImpressao.ImpressoraCaixa;
        instancia.comboBoxImpressoraAuxiliarAdicionar.Text = RotaDeImpressao.ImpressoraAuxiliar;
        instancia.comboBoxImpressoraCozinha1Adicionar.Text = RotaDeImpressao.ImpressoraCozinha1;
        instancia.comboBoxImpressoraCozinha2Adicionar.Text = RotaDeImpressao.ImpressoraCozinha2;
        instancia.comboBoxImpressoraCozinha3Adicionar.Text = RotaDeImpressao.ImpressoraCozinha3;
        instancia.comboBoxImpressoraBarAdicionar.Text = RotaDeImpressao.ImpressoraBar;

        textBoxNomeRotaImp.Text = RotaDeImpressao.NomeRota;

        if (RotaDeImpressao.Ativo)
        {
            Off.Visible = false;
            On.Visible = true;
        }
        else
        {
            Off.Visible = true;
            On.Visible = false;
        }
    }

    public void AlimentaComboBoxDeImpressorasEmAdicionarNovaImpressora(UCRotaDeImpressora instancia)
    {
        List<string> listaDeImpressoras = ParametrosDoSistema.ListaImpressoras();

        foreach (string imp in listaDeImpressoras)
        {
            instancia.comboBoxImpressoraCaixaAdicionar.Items.Add(imp);
            instancia.comboBoxImpressoraAuxiliarAdicionar.Items.Add(imp);
            instancia.comboBoxImpressoraCozinha1Adicionar.Items.Add(imp);
            instancia.comboBoxImpressoraCozinha2Adicionar.Items.Add(imp);
            instancia.comboBoxImpressoraCozinha3Adicionar.Items.Add(imp);
            instancia.comboBoxImpressoraBarAdicionar.Items.Add(imp);
        }
    }

    private async void BtnApagarImpressora_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                DialogResultSys dialogResult = await SysAlerta.Alerta("Ops", "Você tem certeza que deseja apagar essa rota ?", SysAlertaTipo.Alerta, SysAlertaButtons.SimNao);

                if (dialogResult == DialogResultSys.Sim)
                {
                    db.roteamentodeimpressoras.Remove(RotaDeImpressao);
                    db.SaveChanges();

                    TabControlQueChamou.TabPages.Remove(TabControlQueChamou.SelectedTab!);

                    await SysAlerta.Alerta("Sucesso", "Rota apagada com sucesso", SysAlertaTipo.Sucesso, SysAlertaButtons.Ok);
                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void comboBoxImpressoraCaixaAdicionar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.ImpressoraCaixa = comboBoxImpressoraCaixaAdicionar.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void comboBoxImpressoraAuxiliarAdicionar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.ImpressoraAuxiliar = comboBoxImpressoraAuxiliarAdicionar.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void comboBoxImpressoraCozinha1Adicionar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.ImpressoraCozinha1 = comboBoxImpressoraCozinha1Adicionar.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void comboBoxImpressoraCozinha2Adicionar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.ImpressoraCozinha2 = comboBoxImpressoraCozinha2Adicionar.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void comboBoxImpressoraCozinha3Adicionar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.ImpressoraCozinha3 = comboBoxImpressoraCozinha3Adicionar.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void comboBoxImpressoraBarAdicionar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.ImpressoraBar = comboBoxImpressoraBarAdicionar.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }

    }

    private async void textBoxNomeRotaImp_TextChanged(object sender, EventArgs e)
    {
        int cursorPosition = textBoxNomeRotaImp.SelectionStart;
        textBoxNomeRotaImp.Text = textBoxNomeRotaImp.Text.ToUpper();
        textBoxNomeRotaImp.SelectionStart = cursorPosition;

        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.NomeRota = textBoxNomeRotaImp.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }

    }

    private async void Off_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.Ativo = true;

                await db.SaveChangesAsync();


                Off.Visible = false;
                On.Visible = true;
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void On_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ClsRoteamentoDeImpressao? rota = await db.roteamentodeimpressoras.Where(x => x.Id == RotaDeImpressao.Id).FirstOrDefaultAsync();

                if (rota is not null)
                    rota.Ativo = false;

                await db.SaveChangesAsync();


                Off.Visible = true;
                On.Visible = false;
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }
}
