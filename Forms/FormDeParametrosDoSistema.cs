using SysIntegradorApp.ClassesAuxiliares;
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
            ClsEstiloComponentes.SetRoundedRegion(panel5, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel6, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel7, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel8, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel9, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel10, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel11, 24);
            ClsEstiloComponentes.SetRoundedRegion(panel12, 24);
            ClsEstiloComponentes.SetRoundedRegion(panelmpressoras, 24);

            this.Resize += (sender, e) =>
            {
                ClsEstiloComponentes.SetRoundedRegion(panelmpressoras, 24);
                ClsEstiloComponentes.SetRoundedRegion(panel1, 24);
            };
        }

        private void FormDeParametrosDoSistema_Load(object sender, EventArgs e)
        {
            FormLoginConfigs login = new FormLoginConfigs();
            login.ShowDialog();

            FormDeParametrosDoSistema instAtual = new FormDeParametrosDoSistema();
            instAtual.AlimentaComboBoxDeImpressoras(this);
            instAtual.DefineValoresDasConfigVindaDoBanco(this);

            ParametrosDoSistema Configuracoes = ParametrosDoSistema.GetInfosSistema();

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

        public void DefineValoresDasConfigVindaDoBanco(FormDeParametrosDoSistema instancia)
        {
            ParametrosDoSistema Configuracoes = ParametrosDoSistema.GetInfosSistema();

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
                         imprimirComandaNoCaixa
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
    }
}
