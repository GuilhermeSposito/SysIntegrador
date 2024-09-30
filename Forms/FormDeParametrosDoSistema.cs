using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SysIntegradorApp.Forms
{
    public partial class FormDeParametrosDoSistema : Form
    {
        public FormDeParametrosDoSistema()
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(panel1, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel2, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel3, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel4, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel6, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel7, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel8, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel9, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel11, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel12, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel5, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel13, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel14, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelmpressoras, 24);

            this.Resize += (sender, e) =>
            {
                ClsEstiloComponentes.SetRoundedRegion(panelmpressoras, 24);
                ClsEstiloComponentes.SetRoundedRegion(panel1, 24);
            };
        }

        private async void FormDeParametrosDoSistema_Load(object sender, EventArgs e)
        {
            FormLoginConfigs login = new FormLoginConfigs();
            login.ShowDialog();

            FormDeParametrosDoSistema instAtual = new FormDeParametrosDoSistema();
            instAtual.AlimentaComboBoxDeImpressoras(this);
            instAtual.DefineValoresDasConfigVindaDoBanco(this);

            ParametrosDoSistema Configuracoes = await ParametrosDoSistema.GetInfosSistema();

            if (Configuracoes.AgruparComandas)
            {
                pictureBoxON.Visible = true;
                pictureBoxOFF.Visible = false;
            }
            else
            {
                pictureBoxOFF.Visible = true;
                pictureBoxON.Visible = false;

            }

            if (Configuracoes.ImprimirComandaNoCaixa)
            {
                pictureBoxOn2.Visible = true;
                pictureBoxOFF2.Visible = false;
            }
            else
            {
                pictureBoxOFF2.Visible = true;
                pictureBoxOn2.Visible = false;

            }

            if (Configuracoes.TipoComanda == 1)
            {
                pictureBoxON3.Visible = false;
                pictureBoxOFF3.Visible = true;
            }
            else
            {
                pictureBoxON3.Visible = true;
                pictureBoxOFF3.Visible = false;
            }

            if (Configuracoes.EnviaPedidoAut)
            {
                pictureBoxONDELMATCH.Visible = true;
                pictureBoxOFFDELMATCH.Visible = false;
            }
            else
            {
                pictureBoxONDELMATCH.Visible = false;
                pictureBoxOFFDELMATCH.Visible = true;
            }

            if (Configuracoes.IntegraIfood)
            {
                pictureBoxONIntegracaoIfood.Visible = true;
                pictureBoxOFFIntegracaoIfood.Visible = false;
            }
            else
            {
                pictureBoxONIntegracaoIfood.Visible = false;
                pictureBoxOFFIntegracaoIfood.Visible = true;
            }

            if (Configuracoes.IntegraDelMatch)
            {
                pictureBoxOFFIntegracaoDelMatch.Visible = false;
                pictureBoxOnIntegracaoDelMatch.Visible = true;
            }
            else
            {
                pictureBoxOFFIntegracaoDelMatch.Visible = true;
                pictureBoxOnIntegracaoDelMatch.Visible = false;
            }

            if (Configuracoes.ImpCompacta)
            {
                pictureBoxOFFImpressãoCompacta.Visible = false;
                pictureBoxONImpressaoCompacta.Visible = true;
            }
            else
            {
                pictureBoxOFFImpressãoCompacta.Visible = true;
                pictureBoxONImpressaoCompacta.Visible = false;
            }

            if (Configuracoes.ComandaReduzida)
            {
                pictureBoxOFFRemoveOpcoes.Visible = false;
                pictureBoxONRemoveOpcoes.Visible = true;
            }
            else
            {
                pictureBoxOFFRemoveOpcoes.Visible = true;
                pictureBoxONRemoveOpcoes.Visible = false;
            }

            if (Configuracoes.IntegraOnOPedido)
            {
                pictureBoxOFFIntegracaoOnPedido.Visible = false;
                pictureBoxONIntegracaoOnPedido.Visible = true;
            }
            else
            {
                pictureBoxOFFIntegracaoOnPedido.Visible = true;
                pictureBoxONIntegracaoOnPedido.Visible = false;
            }

            if (Configuracoes.IntegraCCM)
            {
                pictureBoxOnCCM.Visible = true;
                pictureBoxOffCCM.Visible = false;
            }
            else
            {
                pictureBoxOnCCM.Visible = false;
                pictureBoxOffCCM.Visible = true;
            }

            if (Configuracoes.IntegraAnotaAi)
            {
                pictureBoxOffAnotaAi.Visible = false;
                pictureBoxOnAnotaAi.Visible = true;
            }
            else
            {
                pictureBoxOffAnotaAi.Visible = true;
                pictureBoxOnAnotaAi.Visible = false;
            }

        }

        public void AlimentaComboBoxDeImpressoras(FormDeParametrosDoSistema instancia)
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

        public async void DefineValoresDasConfigVindaDoBanco(FormDeParametrosDoSistema instancia)
        {
            ParametrosDoSistema Configuracoes = await ParametrosDoSistema.GetInfosSistema();

            instancia.textBoxCaminhoBanco.Text = Configuracoes.CaminhodoBanco;
            instancia.comboBoxIntegraSys.Text = Configuracoes.IntegracaoSysMenu.ToString();
            instancia.comboBoxImpAut.Text = Configuracoes.ImpressaoAut.ToString();
            instancia.comboBoxAceitaPedidoAut.Text = Configuracoes.AceitaPedidoAut.ToString();
            instancia.textBoxClientId.Text = Configuracoes.ClientId;
            instancia.textBoxClientSecret.Text = Configuracoes.ClientSecret;
            instancia.textBoxMarchantId.Text = Configuracoes.MerchantId;
            instancia.textBoxNomeFantasia.Text = Configuracoes.NomeFantasia;
            instancia.textBoxNumeroTelefone.Text = Configuracoes.Telefone;
            instancia.textBoxEndLoja.Text = Configuracoes.Endereco;
            instancia.comboBoxImpressora1.Text = Configuracoes.Impressora1;
            instancia.comboBoxImpressora2.Text = Configuracoes.Impressora2;
            instancia.comboBoxImpressora3.Text = Configuracoes.Impressora3;
            instancia.comboBoxImpressora4.Text = Configuracoes.Impressora4;
            instancia.comboBoxImpressora5.Text = Configuracoes.Impressora5;
            instancia.comboBoxImpressoraAuxiliar.Text = Configuracoes.ImpressoraAux;
            instancia.textBoxDelMatchId.Text = Configuracoes.DelMatchId;
            instancia.textBoxUserDelMatch.Text = Configuracoes.UserDelMatch;
            instancia.textBoxSenhaDelMatch.Text = Configuracoes.SenhaDelMatch;
            instancia.textBoxTokenDeIntegracaoOnPedido.Text = Configuracoes.TokenOnPedido;
            instancia.textBoxuserOnPedido.Text = Configuracoes.UserOnPedido;
            instancia.textBoxsenhaOnPedido.Text = Configuracoes.SenhaOnPedido;
            instancia.textBoxTokenCCM.Text = Configuracoes.TokenCCM;
            instancia.textBoxNumeroLoja.Text = Configuracoes.CodFilialCCM;
        }

        private void btnNao_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSim_Click(object sender, EventArgs e)
        {

            try
            {
                DialogResult opUser = MessageBox.Show("Você confirma as alterações nas configurações?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);


                if (opUser == DialogResult.Yes)
                {

                    // Obtendo os valores dos ComboBoxes e TextBoxes
                    string caminhoBanco = textBoxCaminhoBanco.Text;
                    bool integracaoSysMenu = Convert.ToBoolean(comboBoxIntegraSys.SelectedItem.ToString());
                    bool impressaoAut = Convert.ToBoolean(comboBoxImpAut.SelectedItem.ToString());
                    bool aceitaPedidoAut = Convert.ToBoolean(comboBoxAceitaPedidoAut.SelectedItem.ToString());
                    string clientId = textBoxClientId.Text;
                    string clientSecret = textBoxClientSecret.Text;
                    string merchantId = textBoxMarchantId.Text;
                    string nomeFantasia = textBoxNomeFantasia.Text;
                    string telefone = textBoxNumeroTelefone.Text;
                    string endereco = textBoxEndLoja.Text;
                    string impressora1 = comboBoxImpressora1.SelectedItem.ToString();
                    string impressora2 = comboBoxImpressora2.SelectedItem.ToString();
                    string impressora3 = comboBoxImpressora3.SelectedItem.ToString();
                    string impressora4 = comboBoxImpressora4.SelectedItem.ToString();
                    string impressora5 = comboBoxImpressora5.SelectedItem.ToString();
                    string impressoraAux = comboBoxImpressoraAuxiliar.SelectedItem.ToString();
                    bool agrupaComandas = false;
                    bool imprimirComandaNoCaixa = false;
                    int tipoComanda = 1;
                    bool enviaPedidoAut = false;
                    string delMatchId = textBoxDelMatchId.Text;
                    string UserDelMatch = textBoxUserDelMatch.Text;
                    string senhaDelMatch = textBoxSenhaDelMatch.Text;
                    bool integraIfood = false;
                    bool integraDelMatch = false;
                    bool impCompacta = false;
                    bool removeComplementos = false;
                    bool integraOnPedido = false;
                    string? tokenOnPedido = textBoxTokenDeIntegracaoOnPedido.Text;
                    string? userOnPedido = textBoxuserOnPedido.Text;
                    string? senhaOnPedido = textBoxsenhaOnPedido.Text;
                    bool integraCCM = false;
                    string tokenCCM = textBoxTokenCCM.Text;
                    bool integraAnotaAi = false;
                    string? numeroLoja = textBoxNumeroLoja.Text;

                    if (pictureBoxOFF.Visible == false)
                    {
                        agrupaComandas = false;
                    }

                    if (pictureBoxON.Visible == true)
                    {
                        agrupaComandas = true;
                    }

                    if (pictureBoxOFF2.Visible == false)
                    {
                        imprimirComandaNoCaixa = false;
                    }

                    if (pictureBoxOn2.Visible == true)
                    {
                        imprimirComandaNoCaixa = true;
                    }

                    if (pictureBoxOFF3.Visible == false)
                    {
                        tipoComanda = 1;
                    }

                    if (pictureBoxON3.Visible == true)
                    {
                        tipoComanda = 2;
                    }

                    if (pictureBoxONDELMATCH.Visible == true)
                    {
                        enviaPedidoAut = true;
                    }

                    if (pictureBoxOFFDELMATCH.Visible == true)
                    {
                        enviaPedidoAut = false;
                    }

                    if (pictureBoxONIntegracaoIfood.Visible == true) { }
                    {
                        integraIfood = true;
                    }

                    if (pictureBoxOFFIntegracaoIfood.Visible == true)
                    {
                        integraIfood = false;
                    }

                    if (pictureBoxOnIntegracaoDelMatch.Visible == true)
                    {
                        integraDelMatch = true;
                    }

                    if (pictureBoxOFFIntegracaoDelMatch.Visible == true)
                    {
                        integraDelMatch = false;
                    }

                    if (pictureBoxONImpressaoCompacta.Visible == true)
                    {
                        impCompacta = true;
                    }

                    if (pictureBoxOFFImpressãoCompacta.Visible == true)
                    {
                        impCompacta = false;
                    }

                    if (pictureBoxONRemoveOpcoes.Visible == true)
                    {
                        removeComplementos = true;
                    }

                    if (pictureBoxONRemoveOpcoes.Visible == false)
                    {
                        removeComplementos = false;
                    }

                    if (pictureBoxOFFIntegracaoOnPedido.Visible == true)
                    {
                        integraOnPedido = false;
                    }

                    if (pictureBoxONIntegracaoOnPedido.Visible == true)
                    {
                        integraOnPedido = true;
                    }

                    if (pictureBoxOnCCM.Visible == true)
                    {
                        integraCCM = true;
                    }

                    if (pictureBoxOffCCM.Visible == true)
                    {
                        integraCCM = false;
                    }

                    if (pictureBoxOffAnotaAi.Visible)
                        integraAnotaAi = false;

                    if (pictureBoxOnAnotaAi.Visible)
                        integraAnotaAi = true;

                    // Chamando o método SetInfosSistema com os valores obtidos
                    ParametrosDoSistema.SetInfosSistema(
                         nomeFantasia,
                         endereco,
                         impressaoAut,
                         aceitaPedidoAut,
                         caminhoBanco,
                         integracaoSysMenu,
                         impressora1,
                         impressora2,
                         impressora3,
                         impressora4,
                         impressora5,
                         impressoraAux,
                         telefone,
                         clientId,
                         clientSecret,
                         merchantId,
                         agrupaComandas,
                         imprimirComandaNoCaixa,
                         tipoComanda,
                         enviaPedidoAut,
                         delMatchId,
                         UserDelMatch,
                         senhaDelMatch,
                         integraIfood,
                         integraDelMatch,
                         impCompacta,
                         removeComplementos,
                         integraOnPedido,
                         tokenOnPedido,
                         userOnPedido,
                         senhaOnPedido,
                         integraCCM,
                         tokenCCM,
                         integraAnotaAi,
                         numeroLoja
                     );

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ops");
            }


        }

        private void pictureBoxOFF_Click(object sender, EventArgs e)
        {
            pictureBoxON.Visible = true;
            pictureBoxOFF.Visible = false;
        }

        private void pictureBoxON_Click(object sender, EventArgs e)
        {
            pictureBoxOFF.Visible = true;
            pictureBoxON.Visible = false;
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

        private void pictureBoxOn2_Click(object sender, EventArgs e)
        {
            pictureBoxOFF2.Visible = true;
            pictureBoxOn2.Visible = false;
        }

        private void pictureBoxOFF2_Click(object sender, EventArgs e)
        {
            pictureBoxOn2.Visible = true;
            pictureBoxOFF2.Visible = false;
        }

        private void pictureBoxOFF3_Click(object sender, EventArgs e)
        {
            pictureBoxON3.Visible = true;
            pictureBoxOFF3.Visible = false;
        }

        private void pictureBoxON3_Click(object sender, EventArgs e)
        {
            pictureBoxON3.Visible = false;
            pictureBoxOFF3.Visible = true;
        }

        private void pictureBoxOFFDELMATCH_Click(object sender, EventArgs e)
        {
            pictureBoxONDELMATCH.Visible = true;
            pictureBoxOFFDELMATCH.Visible = false;
        }

        private void pictureBoxONDELMATCH_Click(object sender, EventArgs e)
        {
            pictureBoxONDELMATCH.Visible = false;
            pictureBoxOFFDELMATCH.Visible = true;
        }

        private void pictureBoxOFFIntegracaoIfood_Click(object sender, EventArgs e)
        {
            pictureBoxOFFIntegracaoIfood.Visible = false;
            pictureBoxONIntegracaoIfood.Visible = true;
        }

        private void pictureBoxONIntegracaoIfood_Click(object sender, EventArgs e)
        {
            pictureBoxOFFIntegracaoIfood.Visible = true;
            pictureBoxONIntegracaoIfood.Visible = false;
        }

        private void pictureBoxOFFIntegracaoDelMatch_Click(object sender, EventArgs e)
        {
            pictureBoxOFFIntegracaoDelMatch.Visible = false;
            pictureBoxOnIntegracaoDelMatch.Visible = true;
        }

        private void pictureBoxOnIntegracaoDelMatch_Click(object sender, EventArgs e)
        {
            pictureBoxOFFIntegracaoDelMatch.Visible = true;
            pictureBoxOnIntegracaoDelMatch.Visible = false;
        }

        private async void LimparPedidosDelMatchBtn_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ops");
            }
        }

        private void pictureBoxOFFImpressãoCompacta_Click(object sender, EventArgs e)
        {
            pictureBoxOFFImpressãoCompacta.Visible = false;
            pictureBoxONImpressaoCompacta.Visible = true;
        }

        private void pictureBoxONImpressaoCompacta_Click(object sender, EventArgs e)
        {
            pictureBoxOFFImpressãoCompacta.Visible = true;
            pictureBoxONImpressaoCompacta.Visible = false;
        }

        private void pictureBoxONRemoveOpcoes_Click(object sender, EventArgs e)
        {
            pictureBoxOFFRemoveOpcoes.Visible = true;
            pictureBoxONRemoveOpcoes.Visible = false;
        }

        private void pictureBoxOFFRemoveOpcoes_Click(object sender, EventArgs e)
        {
            pictureBoxOFFRemoveOpcoes.Visible = false;
            pictureBoxONRemoveOpcoes.Visible = true;
        }

        private void pictureBoxOFFIntegracaoOnPedido_Click(object sender, EventArgs e)
        {
            pictureBoxOFFIntegracaoOnPedido.Visible = false;
            pictureBoxONIntegracaoOnPedido.Visible = true;

        }

        private void pictureBoxONIntegracaoOnPedido_Click(object sender, EventArgs e)
        {
            pictureBoxOFFIntegracaoOnPedido.Visible = true;
            pictureBoxONIntegracaoOnPedido.Visible = false;
        }

        private void pictureBoxOnCCM_Click(object sender, EventArgs e)
        {
            pictureBoxOnCCM.Visible = false;
            pictureBoxOffCCM.Visible = true;

        }

        private void pictureBoxOffCCM_Click(object sender, EventArgs e)
        {
            pictureBoxOnCCM.Visible = true;
            pictureBoxOffCCM.Visible = false;
        }

        private void pictureBoxOnAnotaAi_Click(object sender, EventArgs e)
        {
            pictureBoxOnAnotaAi.Visible = false;
            pictureBoxOffAnotaAi.Visible = true;
        }

        private void pictureBoxOffAnotaAi_Click(object sender, EventArgs e)
        {
            pictureBoxOnAnotaAi.Visible = true;
            pictureBoxOffAnotaAi.Visible = false;
        }
    }
}
