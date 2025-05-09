using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.ClassesAuxiliares.Ifood;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.UserControls;
using SysIntegradorApp.UserControls.TaxyMachine;
using SysIntegradorApp.UserControls.UCSAiqfome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms;

public partial class NewFormConfiguracoes : Form
{
    public readonly IMeuContexto _context;
    public ApplicationDbContext _db;
    private string? CodeFromUser { get; set; } = String.Empty;
    public NewFormConfiguracoes(MeuContexto context)
    {
        _context = context;

        DefineBancoDeDados();

        InitializeComponent();
        CriaStripParaOsPainel();

        ClsEstiloComponentes.SetRoundedRegion(panelmpressoras, 24);
        ClsEstiloComponentes.SetRoundedRegion(panel15, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDeConfigDeimpressao, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelLogomarca, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDeGeral, 24);
        ClsEstiloComponentes.SetRoundedRegion(button1, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDoIfood, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDeIfoodNome, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelOnPedido, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelOnPedidoNome, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelCCMNome, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelCCM, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDelMatch, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDelmatchNome, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDelmatchEntrega, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelAnotaAiNome, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelAnotaAi, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelGarcomNome, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelGarcom, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelOttoNome, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelOttoEntegas, 24);
        ClsEstiloComponentes.SetRoundedRegion(panel2, 24);
        ClsEstiloComponentes.SetRoundedRegion(panel10, 24);
        ClsEstiloComponentes.SetRoundedRegion(panel13, 24);
        ClsEstiloComponentes.SetRoundedRegion(panel1, 24);
        ClsEstiloComponentes.SetRoundedRegion(panel9, 24);
        ClsEstiloComponentes.SetRoundedRegion(panelDeMultiEmpresa, 24);
        this.Resize += (sender, e) =>
        {
            ClsEstiloComponentes.SetRoundedRegion(panelmpressoras, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDeConfigDeimpressao, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelLogomarca, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDeGeral, 24);
            ClsEstiloComponentes.SetRoundedRegion(button1, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDoIfood, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDeIfoodNome, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelOnPedido, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelOnPedidoNome, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelCCMNome, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelCCM, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDelMatch, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDelmatchNome, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDelmatchEntrega, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelAnotaAiNome, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelAnotaAi, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelGarcomNome, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelGarcom, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelOttoNome, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelOttoEntegas, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel2, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel10, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel13, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel1, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel9, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelDeMultiEmpresa, 24);
        };


        AlimentaInformacoes();
    }

    public async void DefineBancoDeDados()
    {
        try
        {

            _db = await _context.GetContextoAsync();

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }


    public async void AlimentaInformacoes()
    {
        try
        {
            if (_db is null)
                throw new Exception("Erro ao se comunicar com o banco de dados");

            ParametrosDoSistema? Configuracoes = await _db.parametrosdosistema.FirstOrDefaultAsync();
            Token? AuthConfig = await _db.parametrosdeautenticacao.FirstOrDefaultAsync();

            if (AuthConfig is null)
                AuthConfig = new Token();

            //tela de configurações de impressoras
            MudaOnOff(Configuracoes.AgruparComandas, pictureBoxONAgruparComanda, pictureBoxOFFAgruparComanda);
            MudaOnOff(Configuracoes.ImprimirComandaNoCaixa, pictureBoxOnImprimeCaixa, pictureBoxOffImprimeCaixa);
            MudaOnOff((Configuracoes.TipoComanda == 1 ? false : true), pictureBoxOnSeparaItem, pictureBoxOffSeparaItem);
            MudaOnOff(Configuracoes.ImpCompacta, pictureBoxOnImpCompacta, pictureBoxOffImpCompacta);
            MudaOnOff(Configuracoes.ComandaReduzida, pictureBoxOnComandaComapcat, pictureBoxOffComandaComapcat);
            MudaOnOff(Configuracoes.RemoveComplementos, pictureBoxOnRemoveComplementos, pictureBoxOffRemoveComplementos);
            MudaOnOff(Configuracoes.ImpressaoAut, pictureBoxOnImpAut, pictureBoxOffImpAut);
            MudaOnOff(Configuracoes.UsarNomeNaComanda, pictureBoxOnNomeNaComanda, pictureBoxOffNomeNaComanda);
            MudaOnOff(Configuracoes.DestacarObs, pictureBoxOnDestacaObs, pictureBoxOffDestacaObs);
            TextBoxNumeroDeViasComanda.Text = Convert.ToString(Configuracoes.NumDeViasDeComanda);


            //tela de configurações geral
            textBoxCaminhoBanco.Text = Configuracoes.CaminhodoBanco;
            textBoxCardapioUsado.Text = Configuracoes.CardapioUsando;
            textBoxEmpresaDeEntrega.Text = Configuracoes.EmpresadeEntrega;
            textBoxNomeFantasia.Text = Configuracoes.NomeFantasia;
            MudaOnOff(Configuracoes.AceitaPedidoAut, this.pictureBoxOnAceitaPedidoAut, this.pictureBoxOffAceitaPedidoAut);
            MudaOnOff(Configuracoes.EnviaPedidoAut, this.pictureBoxOnEnviaPedidoAut, this.pictureBoxOffEnviaPedidoAut);
            MudaOnOff(Configuracoes.RetornoAut, this.pictureBoxONRetornoAut, this.pictureBoxOFFRetornoAut);


            //tela de integração ifood
            textBoxClientSecret.Text = Configuracoes.ClientSecret;
            textBoxClientId.Text = Configuracoes.ClientId;
            textBoxMerchantId.Text = Configuracoes.MerchantId;
            textBoxAcessToken.Text = AuthConfig.accessToken ?? String.Empty;
            textBoxRefreshToken.Text = AuthConfig.refreshToken ?? String.Empty;
            textBoxVenceTokenIfoodEm.Text = AuthConfig.VenceEm ?? String.Empty;
            AdicionaEmpresasNoPageControlIfood();
            MudaOnOff(Configuracoes.IfoodMultiEmpresa, this.pictureBoxONMultiEmpresas, this.pictureBoxOFFMultiEmpresas);
            MudaOnOff(Configuracoes.IntegraIfood, this.pictureBoxOnIntegraIfood, this.pictureBoxOffItegraIfood);

            //tela de integração ONPEDIDO
            textBoxUserNameOnPedido.Text = Configuracoes.UserOnPedido;
            textBoxSenhaOnPedido.Text = Configuracoes.SenhaOnPedido;
            textBoxTokenOnPedido.Text = Configuracoes.TokenOnPedido;
            textBoxAcessTokenOnPedido.Text = AuthConfig.TokenOnPedido ?? String.Empty;
            textBoxVenceEmOnPedido.Text = AuthConfig.VenceEmOnPedido ?? String.Empty;
            MudaOnOff(Configuracoes.IntegraOnOPedido, this.pictureBoxOnPedidoIntegra, this.pictureBoxOffOnPedidoIntegra);

            //tela de integração CCM
            textBoxTokenCCM.Text = Configuracoes.TokenCCM;
            TextBoxNumeroLojaCCM.Text = Configuracoes.CodFilialCCM;
            MudaOnOff(Configuracoes.IntegraCCM, this.pictureBoxOnCCMIntegracao, this.pictureBoxOffCCMIntegracao);

            //tela de integração DELMATCH
            textBoxUserNameDelmatch.Text = Configuracoes.UserDelMatch;
            textBoxSenhaDelmatch.Text = Configuracoes.SenhaDelMatch;
            textBoxAccessTokenDelmatch.Text = AuthConfig.TokenDelMatch ?? String.Empty;
            textBoxVenceEmDelmatchToken.Text = AuthConfig.VenceEmDelMatch ?? String.Empty;
            textBoxTokenDeEntregaDelmatch.Text = Configuracoes.DelMatchId;
            MudaOnOff(Configuracoes.IntegraDelMatch, this.pictureBoxOnDelmatchCardapio, this.pictureBoxOffDelmatchCardapio);
            MudaOnOff(Configuracoes.IntegraDelmatchEntregas, this.pictureBoxOnDelmatchEntrega, this.pictureBoxOffDelmatchEntrega);

            //tela de integração ANOTA AI
            textBoxTokenAnotaAi.Text = Configuracoes.TokenAnotaAi;
            MudaOnOff(Configuracoes.IntegraAnotaAi, this.pictureBoxOnAnotaAiIntegra, this.pictureBoxOffAnotaAiIntegra);

            //tela de integração GARCOM
            MudaOnOff(Configuracoes.IntegraGarcom, this.pictureBoxOnGarcom, this.pictureBoxOffGarcom);
            MudaRadioButtonsGarcom();

            //tela de integração OTTO
            textBoxUserNameOtto.Text = Configuracoes.UserNameTaxyMachine;
            textBoxSenhaOtto.Text = Configuracoes.PasswordTaxyMachine;
            textBoxTokenDeIntegracaoOtto.Text = Configuracoes.ApiKeyTaxyMachine;
            textBoxTipoDePagamentoOtto.Text = Configuracoes.TipoPagamentoTaxyMachine;
            MudaOnOff(Configuracoes.IntegraOttoEntregas, this.pictureBoxOnOtto, this.pictureBoxOffOtto);

            //tela de integração AIQFOME
            AdicionaEmpresasNoPageControlAiQueFome();


            //tela TaxyMachine
            AdicionaEmpresasNoPageControlTaxyMachine();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public void MudaOnOff(bool OffOuOn, PictureBox BoxOn, PictureBox BoxOff)
    {
        if (OffOuOn)
        {
            BoxOn.Visible = true;
            BoxOff.Visible = false;
        }
        else
        {
            BoxOn.Visible = false;
            BoxOff.Visible = true;
        }
    }

    public async void MudaRadioButtonsGarcom()
    {
        try
        {
            ParametrosDoSistema? Configuracoes = await _db.parametrosdosistema.FirstOrDefaultAsync();
            ConfigAppGarcom? ConfigAppGarcom = await _db.configappgarcom.FirstOrDefaultAsync();

            if (ConfigAppGarcom is null)
                ConfigAppGarcom = new ConfigAppGarcom();

            //if para definir requisição
            if (ConfigAppGarcom.RequisicaoAlfaNumerica)
            {
                radioButtonReqAlfaNumerica.Checked = true;
                radioButtonReqNum.Checked = false;
                radioButtonSemReq.Checked = false;
            }
            else if (ConfigAppGarcom.RequisicaoNumerica)
            {
                radioButtonReqAlfaNumerica.Checked = false;
                radioButtonReqNum.Checked = true;
                radioButtonSemReq.Checked = false;

            }
            else
            {
                radioButtonReqAlfaNumerica.Checked = false;
                radioButtonReqNum.Checked = false;
                radioButtonSemReq.Checked = true;
            }

            //if para definir mesa ou comanda
            if (ConfigAppGarcom.Mesa)
            {
                radioButtonMesa.Checked = true;
                radioButtonComanda.Checked = false;
            }
            else if (ConfigAppGarcom.Comanda)
            {
                radioButtonMesa.Checked = false;
                radioButtonComanda.Checked = true;
            }

            //if para definir formas de lançamento
            if (ConfigAppGarcom.ListaDeItens)
            {
                radioButtonListaDeItens.Checked = true;
                radioButtonBuscaDeItens.Checked = false;
                radioButtonListaDeGrupos.Checked = false;
            }
            else if (ConfigAppGarcom.BuscaDeItens)
            {

                radioButtonListaDeItens.Checked = false;
                radioButtonBuscaDeItens.Checked = true;
                radioButtonListaDeGrupos.Checked = false;
            }
            else if (ConfigAppGarcom.ListaPorGrupo)
            {
                radioButtonListaDeItens.Checked = false;
                radioButtonBuscaDeItens.Checked = false;
                radioButtonListaDeGrupos.Checked = true;
            }

            //definir tempo dos apps 
            TextBoxTempoApp.Text = Convert.ToString(ConfigAppGarcom.TempoEnvioPedido);
            TextBoxTempoIntegrador.Text = Convert.ToString(Configuracoes.TempoPollingGarcom);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }
    private void CriaStripParaOsPainel()
    {
        ClsEstiloComponentes.CustomizePanelBorder(PanelDeAgruparComanda);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeImpComandaNoCaixa);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeSepararItem);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeImpCompacta);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeComandaCompacta);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeRemovComple);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeImpAut);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeNomeComanda);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeNumComanda);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeDestacObs);
        ClsEstiloComponentes.CustomizePanelBorder(panelCaminhobanco);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeAceitaAut);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeCardapioUsado);
        ClsEstiloComponentes.CustomizePanelBorder(panelDeEmpresaDeEntrega);
        //ClsEstiloComponentes.CustomizePanelBorder(panelDeClientSecret);
        //ClsEstiloComponentes.CustomizePanelBorder(panelDeTokensIfood);
    }
    private void NewFormConfiguracoes_Load(object sender, EventArgs e)
    {
        NewFormConfiguracoes instAtual = new NewFormConfiguracoes(new MeuContexto());
        instAtual.AlimentaComboBoxDeImpressoras(this);
        instAtual.DefineValoresDasConfigVindaDoBanco(this);

        //FormLoginConfigs formLoginConfigs = new FormLoginConfigs();
        //formLoginConfigs.ShowDialog();
    }


    public void AlimentaComboBoxDeImpressoras(NewFormConfiguracoes instancia)
    {
        List<string> listaDeImpressoras = ParametrosDoSistema.ListaImpressoras();

        foreach (string imp in listaDeImpressoras)
        {
            instancia.comboBoxImpressora1.Items.Add(imp);
            instancia.comboBoxImpressora2.Items.Add(imp);
            instancia.comboBoxImpressora3.Items.Add(imp);
            instancia.comboBoxImpressora4.Items.Add(imp);
            instancia.comboBoxImpressora5.Items.Add(imp);
            instancia.comboBoxImpressoraAuxiliar.Items.Add(imp);
        }
    }

    public async void DefineValoresDasConfigVindaDoBanco(NewFormConfiguracoes instancia)
    {
        ParametrosDoSistema Configuracoes = await ParametrosDoSistema.GetInfosSistema();

        instancia.comboBoxImpressora1.Text = Configuracoes.Impressora1;
        instancia.comboBoxImpressora2.Text = Configuracoes.Impressora2;
        instancia.comboBoxImpressora3.Text = Configuracoes.Impressora3;
        instancia.comboBoxImpressora4.Text = Configuracoes.Impressora4;
        instancia.comboBoxImpressora5.Text = Configuracoes.Impressora5;
        instancia.comboBoxImpressoraAuxiliar.Text = Configuracoes.ImpressoraAux;

    }

    private async void pictureBoxONAgruparComanda_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.AgruparComandas = false;

                await db.SaveChangesAsync();

                pictureBoxONAgruparComanda.Visible = false;
                pictureBoxOFFAgruparComanda.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOFFAgruparComanda_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.AgruparComandas = true;

                await db.SaveChangesAsync();

                pictureBoxONAgruparComanda.Visible = true;
                pictureBoxOFFAgruparComanda.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void pictureBoxOnImprimeCaixa_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ImprimirComandaNoCaixa = false;

                await db.SaveChangesAsync();

                pictureBoxOnImprimeCaixa.Visible = false;
                pictureBoxOffImprimeCaixa.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffImprimeCaixa_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ImprimirComandaNoCaixa = true;

                await db.SaveChangesAsync();

                pictureBoxOnImprimeCaixa.Visible = true;
                pictureBoxOffImprimeCaixa.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnSeparaItem_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.TipoComanda = 1;

                await db.SaveChangesAsync();

                pictureBoxOnSeparaItem.Visible = false;
                pictureBoxOffSeparaItem.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffSeparaItem_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.TipoComanda = 2;

                await db.SaveChangesAsync();

                pictureBoxOnSeparaItem.Visible = true;
                pictureBoxOffSeparaItem.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnImpCompacta_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ImpCompacta = false;

                await db.SaveChangesAsync();

                pictureBoxOnImpCompacta.Visible = false;
                pictureBoxOffImpCompacta.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffImpCompacta_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ImpCompacta = true;

                await db.SaveChangesAsync();

                pictureBoxOnImpCompacta.Visible = true;
                pictureBoxOffImpCompacta.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnComandaComapcat_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ComandaReduzida = false;

                await db.SaveChangesAsync();

                pictureBoxOnComandaComapcat.Visible = false;
                pictureBoxOffComandaComapcat.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOffComandaComapcat_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ComandaReduzida = true;

                await db.SaveChangesAsync();

                pictureBoxOnComandaComapcat.Visible = true;
                pictureBoxOffComandaComapcat.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnRemoveComplementos_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.RemoveComplementos = false;

                await db.SaveChangesAsync();

                pictureBoxOnRemoveComplementos.Visible = false;
                pictureBoxOffRemoveComplementos.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOffRemoveComplementos_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.RemoveComplementos = true;

                await db.SaveChangesAsync();

                pictureBoxOnRemoveComplementos.Visible = true;
                pictureBoxOffRemoveComplementos.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOnImpAut_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ImpressaoAut = false;

                await db.SaveChangesAsync();

                pictureBoxOnImpAut.Visible = false;
                pictureBoxOffImpAut.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOffImpAut_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ImpressaoAut = true;

                await db.SaveChangesAsync();

                pictureBoxOnImpAut.Visible = true;
                pictureBoxOffImpAut.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOnNomeNaComanda_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.UsarNomeNaComanda = false;

                await db.SaveChangesAsync();

                pictureBoxOnNomeNaComanda.Visible = false;
                pictureBoxOffNomeNaComanda.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffNomeNaComanda_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.UsarNomeNaComanda = true;

                await db.SaveChangesAsync();

                pictureBoxOnNomeNaComanda.Visible = true;
                pictureBoxOffNomeNaComanda.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnDestacaObs_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.DestacarObs = false;

                await db.SaveChangesAsync();


                pictureBoxOnDestacaObs.Visible = false;
                pictureBoxOffDestacaObs.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void pictureBoxOffDestacaObs_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.DestacarObs = true;

                await db.SaveChangesAsync();

                pictureBoxOnDestacaObs.Visible = true;
                pictureBoxOffDestacaObs.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void pictureBoxOnAceitaPedidoAut_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.AceitaPedidoAut = false;

            await _db.SaveChangesAsync();


            pictureBoxOnAceitaPedidoAut.Visible = false;
            pictureBoxOffAceitaPedidoAut.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffAceitaPedidoAut_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.AceitaPedidoAut = true;

            await _db.SaveChangesAsync();

            pictureBoxOnAceitaPedidoAut.Visible = true;
            pictureBoxOffAceitaPedidoAut.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private void button1_Click(object sender, EventArgs e)
    {
        DialogResult opcUser = MessageBox.Show("Você deseja apagar todos os pedidos do Banco De dados? Esses pedidos não serão apagados do caixa do SysMenu, mas não será possivel mais interagir com eles e imprimi-los.", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (opcUser == DialogResult.Yes)
        {
            PostgresConfigs.LimparPedidos();
            FormMenuInicial.panelPedidos.Controls.Clear();
        }
    }

    private async void pictureBoxOffEnviaPedidoAut_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.EnviaPedidoAut = true;

            await _db.SaveChangesAsync();

            pictureBoxOffEnviaPedidoAut.Visible = false;
            pictureBoxOnEnviaPedidoAut.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void pictureBoxOnEnviaPedidoAut_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.EnviaPedidoAut = false;

            await _db.SaveChangesAsync();

            pictureBoxOffEnviaPedidoAut.Visible = true;
            pictureBoxOnEnviaPedidoAut.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffItegraIfood_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraIfood = true;

            await _db.SaveChangesAsync();

            pictureBoxOffItegraIfood.Visible = false;
            pictureBoxOnIntegraIfood.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnIntegraIfood_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraIfood = false;

            await _db.SaveChangesAsync();

            pictureBoxOffItegraIfood.Visible = true;
            pictureBoxOnIntegraIfood.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOnPedidoIntegra_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraOnOPedido = false;

            await _db.SaveChangesAsync();

            pictureBoxOnPedidoIntegra.Visible = false;
            pictureBoxOffOnPedidoIntegra.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffOnPedidoIntegra_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraOnOPedido = true;

            await _db.SaveChangesAsync();

            pictureBoxOnPedidoIntegra.Visible = true;
            pictureBoxOffOnPedidoIntegra.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOffCCMIntegracao_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraCCM = true;

            await _db.SaveChangesAsync();


            pictureBoxOffCCMIntegracao.Visible = false;
            pictureBoxOnCCMIntegracao.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOnCCMIntegracao_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraCCM = false;

            await _db.SaveChangesAsync();


            pictureBoxOffCCMIntegracao.Visible = true;
            pictureBoxOnCCMIntegracao.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOffDelmatchCardapio_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraDelMatch = true;

            await _db.SaveChangesAsync();

            pictureBoxOffDelmatchCardapio.Visible = false;
            pictureBoxOnDelmatchCardapio.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnDelmatchCardapio_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraDelMatch = false;

            await _db.SaveChangesAsync();

            pictureBoxOffDelmatchCardapio.Visible = true;
            pictureBoxOnDelmatchCardapio.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOffDelmatchEntrega_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraDelmatchEntregas = true;

            await _db.SaveChangesAsync();

            pictureBoxOffDelmatchEntrega.Visible = false;
            pictureBoxOnDelmatchEntrega.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOnDelmatchEntrega_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraDelmatchEntregas = false;

            await _db.SaveChangesAsync();

            pictureBoxOffDelmatchEntrega.Visible = true;
            pictureBoxOnDelmatchEntrega.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOffAnotaAiIntegra_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraAnotaAi = true;

            await _db.SaveChangesAsync();

            pictureBoxOffAnotaAiIntegra.Visible = false;
            pictureBoxOnAnotaAiIntegra.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnAnotaAiIntegra_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraAnotaAi = false;

            await _db.SaveChangesAsync();

            pictureBoxOffAnotaAiIntegra.Visible = true;
            pictureBoxOnAnotaAiIntegra.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOffGarcom_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraGarcom = true;

            await _db.SaveChangesAsync();

            pictureBoxOffGarcom.Visible = false;
            pictureBoxOnGarcom.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void pictureBoxOnGarcom_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraGarcom = false;

            await _db.SaveChangesAsync();

            pictureBoxOffGarcom.Visible = true;
            pictureBoxOnGarcom.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private void pictureBox8_Click(object sender, EventArgs e)
    {
        Clipboard.SetText("mch_api_XBMzNUddP6GK4MjziuItGMuz");
    }

    private async void pictureBoxOffOtto_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
            {
                Config.IntegraOttoEntregas = true;
                Config.IntegraJumaEntregas = false;
            }


            await _db.SaveChangesAsync();

            pictureBoxOffOtto.Visible = false;
            pictureBoxOnOtto.Visible = true;

            //trabalha com a juma
            pictureBoxOFFJUMA.Visible = true;
            pictureBoxOnJuma.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void pictureBoxOnOtto_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraOttoEntregas = false;

            await _db.SaveChangesAsync();

            pictureBoxOffOtto.Visible = true;
            pictureBoxOnOtto.Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void textBoxTipoDePagamentoOtto_TextChanged(object sender, EventArgs e)
    {
        try
        {
            int cursorPosition = textBoxTipoDePagamentoOtto.SelectionStart;
            textBoxTipoDePagamentoOtto.Text = textBoxTipoDePagamentoOtto.Text.ToUpper();
            textBoxTipoDePagamentoOtto.SelectionStart = cursorPosition;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.TipoPagamentoTaxyMachine = textBoxTipoDePagamentoOtto.Text.ToUpper();

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }


    private async void textBoxCardapioUsado_TextChanged_1(object sender, EventArgs e)
    {
        try
        {
            int cursorPosition = textBoxCardapioUsado.SelectionStart;
            textBoxCardapioUsado.Text = textBoxCardapioUsado.Text.ToUpper();
            textBoxCardapioUsado.SelectionStart = cursorPosition;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.CardapioUsando = textBoxCardapioUsado.Text.ToUpper();

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }


    }

    private async void textBoxEmpresaDeEntrega_TextChanged(object sender, EventArgs e)
    {
        try
        {
            int cursorPosition = textBoxEmpresaDeEntrega.SelectionStart;
            textBoxEmpresaDeEntrega.Text = textBoxEmpresaDeEntrega.Text.ToUpper();
            textBoxEmpresaDeEntrega.SelectionStart = cursorPosition;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.EmpresadeEntrega = textBoxEmpresaDeEntrega.Text.ToUpper();

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void textBoxCaminhoBanco_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.CaminhodoBanco = textBoxCaminhoBanco.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxMerchantId_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.MerchantId = textBoxMerchantId.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxUserNameOnPedido_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.UserOnPedido = textBoxUserNameOnPedido.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxSenhaOnPedido_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.SenhaOnPedido = textBoxSenhaOnPedido.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxTokenOnPedido_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.TokenOnPedido = textBoxTokenOnPedido.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void BtnRenovarTokenOnPedido_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                OnPedido onPedido = new OnPedido(new MeuContexto());
                await onPedido.GetToken();

                //   MessageBox.Show("Envio de renovação concluída com sucesso!", "Sucesso");
                await SysAlerta.Alerta("Sucesso", "Envio de renovação concluída com sucesso!", SysAlertaTipo.Sucesso, SysAlertaButtons.Ok);


                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();
                Token token = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                if (Config is not null && token is not null)
                {
                    textBoxAcessTokenOnPedido.Text = token.TokenOnPedido;
                    textBoxVenceEmOnPedido.Text = token.VenceEmOnPedido;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void textBoxTokenCCM_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.TokenCCM = textBoxTokenCCM.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void TextBoxNumeroLojaCCM_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.CodFilialCCM = TextBoxNumeroLojaCCM.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxUserNameDelmatch_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.UserDelMatch = textBoxUserNameDelmatch.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxSenhaDelmatch_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.SenhaDelMatch = textBoxSenhaDelmatch.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }

    }

    private async void textBoxTokenDeEntregaDelmatch_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.DelMatchId = textBoxTokenDeEntregaDelmatch.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void BtnRenovaTokenDelmatch_Click(object sender, EventArgs e)
    {
        try
        {
            DelMatch delMatch = new DelMatch(new MeuContexto());
            await delMatch.GetToken();

            MessageBox.Show("Envio de renovação concluída com sucesso!", "Sucesso");

            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();
                Token token = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                if (Config is not null && token is not null)
                {
                    textBoxAccessTokenDelmatch.Text = token.TokenDelMatch;
                    textBoxVenceEmDelmatchToken.Text = token.VenceEmDelMatch;
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxTokenAnotaAi_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.TokenAnotaAi = textBoxTokenAnotaAi.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxUserNameOtto_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.UserNameTaxyMachine = textBoxUserNameOtto.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxSenhaOtto_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.PasswordTaxyMachine = textBoxSenhaOtto.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void textBoxTokenDeIntegracaoOtto_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ApiKeyTaxyMachine = textBoxTokenDeIntegracaoOtto.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void comboBoxImpressora1_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.Impressora1 = comboBoxImpressora1.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void comboBoxImpressora2_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.Impressora2 = comboBoxImpressora2.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void comboBoxImpressora3_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.Impressora3 = comboBoxImpressora3.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void comboBoxImpressora4_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.Impressora4 = comboBoxImpressora4.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void comboBoxImpressora5_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.Impressora5 = comboBoxImpressora5.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void comboBoxImpressoraAuxiliar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.ImpressoraAux = comboBoxImpressoraAuxiliar.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private void NewFormConfiguracoes_Shown(object sender, EventArgs e)
    {
        FormLoginConfigs formLoginConfigs = new FormLoginConfigs();
        formLoginConfigs.ShowDialog();
    }

    private async void TextBoxNumeroDeViasComanda_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.NumDeViasDeComanda = Convert.ToInt32(TextBoxNumeroDeViasComanda.Text);

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private void panelGarcom_Paint(object sender, PaintEventArgs e)
    {

    }
    private async void radioButtonReqAlfaNumerica_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.RequisicaoAlfaNumerica = true;
                    Config.RequisicaoNumerica = false;
                    Config.SemRequisicao = false;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void radioButtonReqNum_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.RequisicaoAlfaNumerica = false;
                    Config.RequisicaoNumerica = true;
                    Config.SemRequisicao = false;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }


    private async void radioButtonSemReq_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.RequisicaoAlfaNumerica = false;
                    Config.RequisicaoNumerica = false;
                    Config.SemRequisicao = true;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void radioButtonMesa_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.Mesa = true;
                    Config.Comanda = false;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void radioButtonComanda_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.Mesa = false;
                    Config.Comanda = true;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void radioButtonListaDeItens_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.ListaDeItens = true;
                    Config.BuscaDeItens = false;
                    Config.ListaPorGrupo = false;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void radioButtonBuscaDeItens_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.ListaDeItens = false;
                    Config.BuscaDeItens = true;
                    Config.ListaPorGrupo = false;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void radioButtonListaDeGrupos_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.ListaDeItens = false;
                    Config.BuscaDeItens = false;
                    Config.ListaPorGrupo = true;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro");
        }
    }

    private async void TextBoxTempoIntegrador_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            await Task.Delay(100);

            int TempoApp = Convert.ToInt32(TextBoxTempoApp.Text);
            int TempoIntegrador = Convert.ToInt32(TextBoxTempoIntegrador.Text);

            if (TempoIntegrador >= TempoApp)
            {
                TextBoxTempoIntegrador.Text = (TempoIntegrador - 1).ToString();
                throw new Exception("O Tempo do app não pode ser menor ou igual ao do integrador");
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.TempoPollingGarcom = Convert.ToInt32(TextBoxTempoIntegrador.Text);

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }

    }

    private async void TextBoxTempoApp_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            await Task.Delay(100);

            int TempoApp = Convert.ToInt32(TextBoxTempoApp.Text);
            int TempoIntegrador = Convert.ToInt32(TextBoxTempoIntegrador.Text);

            if (TempoApp <= TempoIntegrador)
            {
                TextBoxTempoApp.Text = (TempoApp + 2).ToString();
                throw new Exception("O Tempo do app não pode ser menor ou igual ao do integrador");
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();
                if (Config is not null)
                {
                    await db.SaveChangesAsync();
                    Config.TempoEnvioPedido = TempoApp;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void textBoxNomeFantasia_TextChanged(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (Config is not null)
                    Config.NomeFantasia = textBoxNomeFantasia.Text;

                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void GerarCodigoDeAutBtn_Click(object sender, EventArgs e)
    {
        try
        {
            GerarCodigoDeAutBtn.Visible = false;
            AutBtn.Visible = true;
            panelDeColar.Visible = true;
            textBoxCodAutorizacao.Visible = true;

            using (ApplicationDbContext db = await _context.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                string url = "https://merchant-api.ifood.com.br/authentication/v1.0/oauth/";

                using (HttpClient client = new HttpClient())
                {
                    FormUrlEncodedContent formData = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("clientId", opcSistema.ClientId)
                         });

                    HttpResponseMessage response = await client.PostAsync($"{url}userCode", formData);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException("\nErro ao acessar o user code\n");
                    }

                    string jsonContent = await response.Content.ReadAsStringAsync();
                    UserCodeReturnFromAPI codesOfVerif = JsonSerializer.Deserialize<UserCodeReturnFromAPI>(jsonContent);
                    UserCodeReturnFromAPI.CodeVerifier = codesOfVerif.authorizationCodeVerifier; //seta o valor em uma propriedade static para podermos pegar no proximo método 

                    string? urlVerificacao = codesOfVerif.verificationUrlComplete;

                    if (urlVerificacao != null && Uri.IsWellFormedUriString(urlVerificacao, UriKind.Absolute))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = urlVerificacao,
                            UseShellExecute = true
                        });
                    }

                    Clipboard.SetText(codesOfVerif.userCode);

                }
            }

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void AdicionaEmpresasNoPageControlIfood()
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();
                List<EmpresasIfood> Empresas = await db.empresasIfoods.ToListAsync();

                if (Empresas is not null)
                {
                    foreach (EmpresasIfood Empresa in Empresas)
                    {
                        AdicionaNovaEmpresaAoTabControlDeEmpresasIFood(Empresa);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void AdicionaEmpresasNoPageControlAiQueFome()
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();
                List<ClsEmpresasAiqFome> Empresas = await db.empresasaiqfome.ToListAsync();

                if (Empresas is not null)
                {
                    foreach (ClsEmpresasAiqFome Empresa in Empresas)
                    {
                        AdicionaEmpresaNoTabControlDeEmpresasDoAiQFome(Empresa);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }


    private async void AdicionaEmpresasNoPageControlTaxyMachine()
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoSistema? Config = await db.parametrosdosistema.FirstOrDefaultAsync();
                List<EmpresasEntregaTaxyMachine> Empresas = await db.empresastaxymachine.ToListAsync();

                if (Empresas is not null)
                {
                    foreach (EmpresasEntregaTaxyMachine Empresa in Empresas)
                    {
                        AdicionaEmpresaNoTabControlDeEmpresaDaTaxyMachine(Empresa);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }





    private async void AutBtn_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (textBoxCodAutorizacao.Text is null || textBoxCodAutorizacao.Text!.Length < 1)
                {
                    throw new Exception("O campo de código de autorização não pode estar vazio");
                }

                if (String.IsNullOrEmpty(textBoxMerchantIdNovaEmp.Text) || String.IsNullOrEmpty(NomeIdentificadorNovaEmp.Text))
                {
                    throw new Exception("Os campos de MerchantId e Nome Identificador não podem estar vazios");
                }

                string url = "https://merchant-api.ifood.com.br/authentication/v1.0/oauth/";
                ParametrosDoSistema? opcSistema = await db.parametrosdosistema.FirstOrDefaultAsync();
                List<EmpresasIfood> Empresas = await db.empresasIfoods.ToListAsync();
                string? codeFromMenu = CodeFromUser;

                FormUrlEncodedContent formDataToGetTheToken = new FormUrlEncodedContent(new[]
                     {
                        new KeyValuePair<string, string>("grantType", "authorization_code"),
                        new KeyValuePair<string, string>("clientId", opcSistema.ClientId),
                        new KeyValuePair<string, string>("clientSecret", opcSistema.ClientSecret),
                        new KeyValuePair<string, string>("authorizationCode", codeFromMenu),
                        new KeyValuePair<string, string>("authorizationCodeVerifier", UserCodeReturnFromAPI.CodeVerifier)
                    });

                using HttpClient client = new HttpClient();

                HttpResponseMessage responseWithToken = await client.PostAsync($"{url}token", formDataToGetTheToken);
                if (!responseWithToken.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("\nErro ao acessar o token de acesso\n");
                }

                string? jsonObjTokenFromAPI = await responseWithToken.Content.ReadAsStringAsync();
                Token propriedadesAPIWithToken = JsonSerializer.Deserialize<Token>(jsonObjTokenFromAPI)!;

                DateTime horaAtual = DateTime.Now;
                double milissegundosAdicionais = 21600;
                DateTime horaFutura = horaAtual.AddSeconds(propriedadesAPIWithToken.expiresIn);
                string HoraFormatada = horaFutura.ToString();

                propriedadesAPIWithToken.VenceEm = HoraFormatada;

                EmpresasIfood NovaEmpresa = new EmpresasIfood()
                {
                    NomeIdentificador = NomeIdentificadorNovaEmp.Text,
                    MerchantId = textBoxMerchantIdNovaEmp.Text,
                    Token = propriedadesAPIWithToken.accessToken,
                    RefreshToken = propriedadesAPIWithToken.refreshToken,
                    DataExpiracao = propriedadesAPIWithToken.VenceEm
                };

                await db.empresasIfoods.AddAsync(NovaEmpresa);
                await db.SaveChangesAsync();
                AdicionaNovaEmpresaAoTabControlDeEmpresasIFood(NovaEmpresa, true);
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private void AdicionaNovaEmpresaAoTabControlDeEmpresasIFood(EmpresasIfood NovaEmpresa, bool eNovaEmpresa = false)
    {
        int indexPenultimo = TabControlEmpresas.TabPages.Count - 1;
        TabPage AbaDeNovaEmpresa = new TabPage($"{NovaEmpresa.NomeIdentificador}");
        UCInfoDeEmpresaIntegrada uCInfoDeEmpresaIntegrada = new UCInfoDeEmpresaIntegrada(NovaEmpresa, TabControlEmpresas);

        AbaDeNovaEmpresa.Controls.Add(uCInfoDeEmpresaIntegrada);
        TabControlEmpresas.TabPages.Insert(indexPenultimo, AbaDeNovaEmpresa);

        if (eNovaEmpresa)
        {
            TabControlEmpresas.SelectedTab = AbaDeNovaEmpresa;
            textBoxMerchantIdNovaEmp.Text = "";
            NomeIdentificadorNovaEmp.Text = "";
            AutBtn.Visible = false;
            GerarCodigoDeAutBtn.Visible = true;
            panelDeColar.Visible = false;
            textBoxCodAutorizacao.Visible = false;
        }
    }

    private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            CodeFromUser = Clipboard.GetText();
            textBoxCodAutorizacao.Text = CodeFromUser;
        }
        else
        {
            await SysAlerta.Alerta("Ops", "Area de transferencia Vazia", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void pictureBoxOFFMultiEmpresas_Click(object sender, EventArgs e)
    {
        try
        {
            DialogResultSys opcUser = await SysAlerta.Alerta("Atenção", "Atenção, Você esta ligando o modo multiempresas do ifood, tem certeza?", SysAlertaTipo.Alerta, SysAlertaButtons.SimNao);

            if (opcUser == DialogResultSys.Sim)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var Config = await db.parametrosdosistema.FirstOrDefaultAsync();
                    Config!.IfoodMultiEmpresa = true;
                    await db.SaveChangesAsync();

                    pictureBoxOFFMultiEmpresas.Visible = false;
                    pictureBoxONMultiEmpresas.Visible = true;
                }
            }

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void pictureBoxONMultiEmpresas_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var Config = await db.parametrosdosistema.FirstOrDefaultAsync();
                Config!.IfoodMultiEmpresa = false;
                await db.SaveChangesAsync();

                pictureBoxOFFMultiEmpresas.Visible = true;
                pictureBoxONMultiEmpresas.Visible = false;
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void pictureBoxOFFRetornoAut_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema? Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.RetornoAut = true;

            await _db.SaveChangesAsync();

            pictureBoxOFFRetornoAut.Visible = false;
            pictureBoxONRetornoAut.Visible = true;
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void pictureBoxONRetornoAut_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema? Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.RetornoAut = false;

            await _db.SaveChangesAsync();

            pictureBoxOFFRetornoAut.Visible = true;
            pictureBoxONRetornoAut.Visible = false;
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void AutBntAiQFome_Click(object sender, EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(textBoxClientIdAiQueFome.Text))
            {
                throw new Exception("O campo ClientId não pode estar vazio");
            }

            if (String.IsNullOrEmpty(textBoxNomeIdentificadorAiQFome.Text))
            {
                throw new Exception("O campo Nome identificador não pode estar vazio");
            }

            if (String.IsNullOrEmpty(textBoxURIAiQueFOme.Text))
            {
                throw new Exception("O campo de URI não pode estar vazio");
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string url = "https://id.magalu.com/oauth/token";
                ParametrosDoSistema? opcSistema = await db.parametrosdosistema.FirstOrDefaultAsync();
                List<ClsEmpresasAiqFome> Empresas = await db.empresasaiqfome.ToListAsync();
                string? codeFromMenu = textBoxURIAiQueFOme.Text.Substring(textBoxURIAiQueFOme.Text.IndexOf("=") + 1);

                FormUrlEncodedContent formDataToGetTheToken = new FormUrlEncodedContent(new[]
                     {
                        new KeyValuePair<string, string>("client_id", textBoxClientIdAiQueFome.Text),
                        new KeyValuePair<string, string>("client_secret", opcSistema!.ClientSecretAiqfome!),
                        new KeyValuePair<string, string>("redirect_uri", textBoxURIAiQueFOme.Text),
                        new KeyValuePair<string, string>("code", codeFromMenu),
                        new KeyValuePair<string, string>("grant_type", "authorization_code")
                    });

                using HttpClient client = new HttpClient();

                HttpResponseMessage responseWithToken = await client.PostAsync(url, formDataToGetTheToken);
                if (!responseWithToken.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("\nErro ao acessar o token de acesso\n");
                }

                string RetornoDaApi = await responseWithToken.Content.ReadAsStringAsync();
                ClsDeSuporteTokenDeAcesso propriedadesAPIWithToken = JsonSerializer.Deserialize<ClsDeSuporteTokenDeAcesso>(RetornoDaApi)!;
                DateTime DataAtual = DateTime.Now.AddSeconds(propriedadesAPIWithToken.ExpiresIn);

                ClsEmpresasAiqFome NovaEmpresa = new ClsEmpresasAiqFome()
                {
                    NomeIdentificador = textBoxNomeIdentificadorAiQFome.Text,
                    ClientId = textBoxClientIdAiQueFome.Text,
                    TokenReq = propriedadesAPIWithToken.AccessToken,
                    RefreshToken = propriedadesAPIWithToken.RefreshToken,
                    TokenExpiracao = DataAtual.ToString()
                };

                await db.empresasaiqfome.AddAsync(NovaEmpresa);
                await db.SaveChangesAsync();

                AdicionaEmpresaNoTabControlDeEmpresasDoAiQFome(NovaEmpresa, true);

                await SysAlerta.Alerta("Sucesso", "Empresa adicionada!", SysAlertaTipo.Sucesso, SysAlertaButtons.Ok);
            }

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private void AdicionaEmpresaNoTabControlDeEmpresasDoAiQFome(ClsEmpresasAiqFome NovaEmpresa, bool eNovaEmpresa = false)
    {
        int indexPenultimo = tabControlDeEmpresasAiQueFome.TabPages.Count - 1;
        TabPage AbaDeNovaEmpresa = new TabPage($"{NovaEmpresa.NomeIdentificador}");
        UCEmpresaIntegrada uCInfoDeEmpresaIntegrada = new UCEmpresaIntegrada(NovaEmpresa, TabControlEmpresas);

        AbaDeNovaEmpresa.Controls.Add(uCInfoDeEmpresaIntegrada);
        tabControlDeEmpresasAiQueFome.TabPages.Insert(indexPenultimo, AbaDeNovaEmpresa);

        if (eNovaEmpresa)
        {
            tabControlDeEmpresasAiQueFome.SelectedTab = AbaDeNovaEmpresa;
            textBoxClientIdAiQueFome.Text = "";
            textBoxNomeIdentificadorAiQFome.Text = "";
        }
    }

    private async void pictureBoxOFFJUMA_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema? Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
            {
                Config.IntegraJumaEntregas = true;
                Config.IntegraOttoEntregas = false;
            }

            await _db.SaveChangesAsync();


            //trabalha com a juma
            pictureBoxOnJuma.Visible = true;
            pictureBoxOFFJUMA.Visible = false;

            //trabalha com a atto
            pictureBoxOffOtto.Visible = true;
            pictureBoxOnOtto.Visible = false;

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void pictureBoxOnJuma_Click(object sender, EventArgs e)
    {
        try
        {
            ParametrosDoSistema? Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

            if (Config is not null)
                Config.IntegraJumaEntregas = false;

            await _db.SaveChangesAsync();

            pictureBoxOnJuma.Visible = false;
            pictureBoxOFFJUMA.Visible = true;

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void BtnAdicionarEmpresaTaxyMachine_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (String.IsNullOrEmpty(textBoxNomeEmpresaTaxyMachine.Text) || String.IsNullOrEmpty(textBoxTipoPagamento.Text) || String.IsNullOrEmpty(textBoxToken.Text) || String.IsNullOrEmpty(textBoxUserName.Text) || String.IsNullOrEmpty(textBoxSenha.Text))
                {
                    throw new Exception("Confira se algum campo esta em vazio e tente novamente");
                }

                string CodEntregador = String.Empty;

                if (!textBoxNomeEmpresaTaxyMachine.Text.Contains("OTTO", StringComparison.OrdinalIgnoreCase) && !textBoxNomeEmpresaTaxyMachine.Text.Contains("JUMA", StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Adicione uma empresa Valida. Ainda não temos integração com a empresa {textBoxNomeEmpresaTaxyMachine.Text}. \n Empresas que temos integração: \n - OTTO \n - JUMA");

                if (textBoxNomeEmpresaTaxyMachine.Text.Contains("OTTO", StringComparison.OrdinalIgnoreCase))
                {
                    CodEntregador = "66";
                }
                else if (textBoxNomeEmpresaTaxyMachine.Text.Contains("JUMA", StringComparison.OrdinalIgnoreCase))
                {
                    CodEntregador = "77";
                }

                EmpresasEntregaTaxyMachine Empresa = new EmpresasEntregaTaxyMachine()
                {
                    NomeEmpresa = textBoxNomeEmpresaTaxyMachine.Text.ToUpper(),
                    Usuario = textBoxUserName.Text,
                    Senha = textBoxSenha.Text,
                    MachineId = textBoxToken.Text,
                    TipoPagamento = textBoxTipoPagamento.Text,
                    CodEntregador = CodEntregador
                };

                await db.empresastaxymachine.AddAsync(Empresa);
                await db.SaveChangesAsync();

                AdicionaEmpresaNoTabControlDeEmpresaDaTaxyMachine(Empresa, true);

            }

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Ops", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private void AdicionaEmpresaNoTabControlDeEmpresaDaTaxyMachine(EmpresasEntregaTaxyMachine NovaEmpresa, bool eNovaEmpresa = false)
    {
        int indexPenultimo = tabControlEmpresasTaxyMachine.TabPages.Count - 1;
        TabPage AbaDeNovaEmpresa = new TabPage($"{NovaEmpresa.NomeEmpresa}");
        UCEmpresaIntegradaTaxyMachine uCInfoDeEmpresaIntegrada = new UCEmpresaIntegradaTaxyMachine(NovaEmpresa, tabControlEmpresasTaxyMachine);

        AbaDeNovaEmpresa.Controls.Add(uCInfoDeEmpresaIntegrada);
        tabControlEmpresasTaxyMachine.TabPages.Insert(indexPenultimo, AbaDeNovaEmpresa);

        if (eNovaEmpresa)
        {
            tabControlEmpresasTaxyMachine.SelectedTab = AbaDeNovaEmpresa;
            textBoxUserName.Text = "";
            textBoxSenha.Text = "";
        }
    }

    private void textBoxTipoPagamento_TextChanged(object sender, EventArgs e)
    {
        textBoxTipoPagamento.Text = textBoxTipoPagamento.Text.ToUpper();
    }

    private async void pictureBoxOnIntegraVariasEmpresasTaxy_Click(object sender, EventArgs e)
    {
        ParametrosDoSistema? Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

        if (Config is not null)
            Config.IntegravariasEmpresasTaxyMachine = true;

        await _db.SaveChangesAsync();

        pictureBoxoffIntegraVariasEmpresasTaxy.Visible = false;
        pictureBoxIntegraVariasEmpresasTaxyMachine.Visible = true;

    }

    private async void pictureBoxIntegraVariasEmpresasTaxyMachine_Click(object sender, EventArgs e)
    {
        ParametrosDoSistema? Config = await _db.parametrosdosistema.FirstOrDefaultAsync();

        if (Config is not null)
            Config.IntegravariasEmpresasTaxyMachine = false;

        await _db.SaveChangesAsync();

        pictureBoxoffIntegraVariasEmpresasTaxy.Visible = true;
        pictureBoxIntegraVariasEmpresasTaxyMachine.Visible = false;
    }
}
