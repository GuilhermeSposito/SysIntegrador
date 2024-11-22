using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
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

public partial class NewFormConfiguracoes : Form
{
    public readonly IMeuContexto _context;
    public ApplicationDbContext _db;

    public NewFormConfiguracoes(MeuContexto context)
    {
        _context = context;

        DefineBancoDeDados();

        InitializeComponent();
        CriaStripParaOsPainel();

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


            //tela de integração ifood
            textBoxClientSecret.Text = Configuracoes.ClientSecret;
            textBoxClientId.Text = Configuracoes.ClientId;
            textBoxMerchantId.Text = Configuracoes.MerchantId;
            textBoxAcessToken.Text = AuthConfig.accessToken ?? String.Empty;
            textBoxRefreshToken.Text = AuthConfig.refreshToken ?? String.Empty;
            textBoxVenceTokenIfoodEm.Text = AuthConfig.VenceEm ?? String.Empty;
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
            MudaRadioButtonsGarcom();

            //tela de integração OTTO
            textBoxUserNameOtto.Text = Configuracoes.UserNameTaxyMachine;
            textBoxSenhaOtto.Text = Configuracoes.PasswordTaxyMachine;
            textBoxTokenDeIntegracaoOtto.Text = Configuracoes.ApiKeyTaxyMachine;
            textBoxTipoDePagamentoOtto.Text = Configuracoes.TipoPagamentoTaxyMachine;
            MudaOnOff(Configuracoes.IntegraOttoEntregas, this.pictureBoxOnOtto, this.pictureBoxOffOtto);
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

    private void pictureBoxONAgruparComanda_Click(object sender, EventArgs e)
    {
        pictureBoxONAgruparComanda.Visible = false;
        pictureBoxOFFAgruparComanda.Visible = true;
    }

    private void pictureBoxOFFAgruparComanda_Click(object sender, EventArgs e)
    {
        pictureBoxONAgruparComanda.Visible = true;
        pictureBoxOFFAgruparComanda.Visible = false;
    }

    private void pictureBoxOnImprimeCaixa_Click(object sender, EventArgs e)
    {
        pictureBoxOnImprimeCaixa.Visible = false;
        pictureBoxOffImprimeCaixa.Visible = true;
    }

    private void pictureBoxOffImprimeCaixa_Click(object sender, EventArgs e)
    {
        pictureBoxOnImprimeCaixa.Visible = true;
        pictureBoxOffImprimeCaixa.Visible = false;
    }

    private void pictureBoxOnSeparaItem_Click(object sender, EventArgs e)
    {
        pictureBoxOnSeparaItem.Visible = false;
        pictureBoxOffSeparaItem.Visible = true;
    }

    private void pictureBoxOffSeparaItem_Click(object sender, EventArgs e)
    {
        pictureBoxOnSeparaItem.Visible = true;
        pictureBoxOffSeparaItem.Visible = false;
    }

    private void pictureBoxOnImpCompacta_Click(object sender, EventArgs e)
    {
        pictureBoxOnImpCompacta.Visible = false;
        pictureBoxOffImpCompacta.Visible = true;
    }

    private void pictureBoxOffImpCompacta_Click(object sender, EventArgs e)
    {
        pictureBoxOnImpCompacta.Visible = true;
        pictureBoxOffImpCompacta.Visible = false;
    }

    private void pictureBoxOnComandaComapcat_Click(object sender, EventArgs e)
    {
        pictureBoxOnComandaComapcat.Visible = false;
        pictureBoxOffComandaComapcat.Visible = true;
    }

    private void pictureBoxOffComandaComapcat_Click(object sender, EventArgs e)
    {
        pictureBoxOnComandaComapcat.Visible = true;
        pictureBoxOffComandaComapcat.Visible = false;
    }

    private void pictureBoxOnRemoveComplementos_Click(object sender, EventArgs e)
    {
        pictureBoxOnRemoveComplementos.Visible = false;
        pictureBoxOffRemoveComplementos.Visible = true;
    }

    private void pictureBoxOffRemoveComplementos_Click(object sender, EventArgs e)
    {
        pictureBoxOnRemoveComplementos.Visible = true;
        pictureBoxOffRemoveComplementos.Visible = false;
    }

    private void pictureBoxOnImpAut_Click(object sender, EventArgs e)
    {
        pictureBoxOnImpAut.Visible = false;
        pictureBoxOffImpAut.Visible = true;
    }

    private void pictureBoxOffImpAut_Click(object sender, EventArgs e)
    {
        pictureBoxOnImpAut.Visible = true;
        pictureBoxOffImpAut.Visible = false;
    }

    private void pictureBoxOnNomeNaComanda_Click(object sender, EventArgs e)
    {
        pictureBoxOnNomeNaComanda.Visible = false;
        pictureBoxOffNomeNaComanda.Visible = true;
    }

    private void pictureBoxOffNomeNaComanda_Click(object sender, EventArgs e)
    {
        pictureBoxOnNomeNaComanda.Visible = true;
        pictureBoxOffNomeNaComanda.Visible = false;
    }

    private void pictureBoxOnDestacaObs_Click(object sender, EventArgs e)
    {
        pictureBoxOnDestacaObs.Visible = false;
        pictureBoxOffDestacaObs.Visible = true;
    }

    private void pictureBoxOffDestacaObs_Click(object sender, EventArgs e)
    {
        pictureBoxOnDestacaObs.Visible = true;
        pictureBoxOffDestacaObs.Visible = false;
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
                Config.IntegraOttoEntregas = true;

            await _db.SaveChangesAsync();

            pictureBoxOffOtto.Visible = false;
            pictureBoxOnOtto.Visible = true;
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

                MessageBox.Show("Envio de renovação concluída com sucesso!", "Sucesso");

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
        //FormLoginConfigs formLoginConfigs = new FormLoginConfigs();
        //formLoginConfigs.ShowDialog();
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
            int TempoApp = Convert.ToInt32(TextBoxTempoApp.Text) - 1;
            int TempoIntegrador = Convert.ToInt32(TextBoxTempoIntegrador.Text);

            if (TempoApp <= TempoIntegrador)
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
            MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }

    private async void TextBoxTempoApp_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            int TempoApp = Convert.ToInt32(TextBoxTempoApp.Text) - 1;
            int TempoIntegrador = Convert.ToInt32(TextBoxTempoIntegrador.Text);

            if (TempoApp <= TempoIntegrador)
            {
                TextBoxTempoApp.Text = (TempoIntegrador + 2).ToString();
                throw new Exception("O Tempo do app não pode ser menor ou igual ao do integrador");
            }

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ConfigAppGarcom? Config = await db.configappgarcom.FirstOrDefaultAsync();

                if (Config is not null)
                {
                    Config.TempoEnvioPedido = TempoApp;
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            MessageBox.Show(ex.Message, "Erro");
        }
    }
}
