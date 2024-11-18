using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;

namespace SysIntegradorApp.Forms.ONPEDIDO;

public partial class FormDeCronograma : Form
{
    public FormDeCronograma()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
        ClsEstiloComponentes.CustomizePanelBorder(panel1);
        ClsEstiloComponentes.CustomizePanelBorder(panel2);
        ClsEstiloComponentes.CustomizePanelBorder(panel3);
        ClsEstiloComponentes.CustomizePanelBorder(panel4);
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    public static async void AlimentaComInfosDoBancoDeDados(FormDeCronograma instancia)
    {
        try
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                var configuracoes = dbContext.parametrosdosistema.FirstOrDefault();

                instancia.textBoxTempoEntrega.Text = configuracoes.TempoEntrega.ToString();
                instancia.textBoxTempoDeRetirada.Text = configuracoes.TempoRetirada.ToString();
                instancia.textBoxTempoConclPedido.Text = configuracoes.TempoConclonPedido.ToString();
                instancia.OnIfood.Visible = configuracoes.IntegraIfood;

                if (configuracoes.CardapioUsando == "DELMATCH")
                {
                    instancia.OnCardapio.Visible = configuracoes.IntegraDelMatch;
                }

                if (configuracoes.CardapioUsando == "CCM")
                {
                    instancia.OnCardapio.Visible = configuracoes.IntegraCCM;
                }

                if (configuracoes.CardapioUsando == "ONPEDIDO")
                {
                    instancia.OnCardapio.Visible = configuracoes.IntegraOnOPedido;

                    Button BtnDeRenovarToken = new Button();
                    BtnDeRenovarToken.Text = "Renovar conexão ONPEDIDO";
                    BtnDeRenovarToken.Size = new Size(300, 59);
                    BtnDeRenovarToken.Location = new Point(80, 480);
                    BtnDeRenovarToken.BackColor = Color.Red;
                    BtnDeRenovarToken.ForeColor = Color.White;
                    BtnDeRenovarToken.Cursor = Cursors.Hand;
                    BtnDeRenovarToken.Font = new Font("Segoe UI", 12, FontStyle.Bold);

                    instancia.Controls.Add(BtnDeRenovarToken);
                    BtnDeRenovarToken.Click += async (sender, e) =>
                    {
                        try
                        {
                            OnPedido onPedido = new OnPedido(new MeuContexto());
                            await onPedido.GetToken();

                            MessageBox.Show("Envio de renovação concluída com sucesso!", "Sucesso");

                            instancia.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ops");
                        }
                    };

                   // AjustarTamanhoForm(instancia);

                }

                if (configuracoes.CardapioUsando == "ANOTAAI")
                {
                    instancia.OnCardapio.Visible = configuracoes.IntegraAnotaAi;
                }

                if (String.IsNullOrEmpty(configuracoes.CardapioUsando))
                {
                    instancia.OnCardapio.Visible = false;
                }

                instancia.OnEntrega.Visible = configuracoes.EnviaPedidoAut;


                instancia.labelCardapio.Text = configuracoes.CardapioUsando;

            }
        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message, "Ops");
        }
    }

    static void AjustarTamanhoForm(Form form)
    {
        int alturaMaxima = 0;

        // Percorrer todos os controles e encontrar a posição mais baixa (inferior)
        foreach (Control ctrl in form.Controls)
        {
            int bottom = ctrl.Bottom;
            if (bottom > alturaMaxima)
            {
                alturaMaxima = bottom;
            }
        }

        // Ajustar o tamanho do formulário para acomodar todos os controles
        form.Height = alturaMaxima + 20; // Adicionar uma margem extra
        form.PerformLayout();
    }

    private void btnEnvir_Click(object sender, EventArgs e)
    {
        try
        {
            int tempoDeEntrega = Convert.ToInt32(textBoxTempoEntrega.Text);
            int tempoDeRetirada = Convert.ToInt32(textBoxTempoDeRetirada.Text);
            int TempoConclPedido = Convert.ToInt32(textBoxTempoConclPedido.Text);
            bool IntegraIfood = OnIfood.Visible == true ? true : false;
            bool IntegraCardapio = OnCardapio.Visible == true ? true : false;
            bool entregaAut = OnEntrega.Visible == true ? true : false;


            ParametrosDoSistema.SetInfosDeCronograma(tempoDeEntrega, tempoDeRetirada, TempoConclPedido, IntegraIfood, IntegraCardapio, entregaAut);

            this.Close();
        }
        catch (Exception ex) when (ex.Message.Contains("format"))
        {
            MessageBox.Show("Formato de dado errado, pode ser apenas inserido números!", "Ops");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    private void FormDeCronograma_Load(object sender, EventArgs e)
    {

        AlimentaComInfosDoBancoDeDados(this);

        if (OnIfood.Visible)
        {
            OffIfood.Visible = false;
        }
        else
        {
            OffIfood.Visible = true;
            OnIfood.Visible = false;
        }

        if (OnCardapio.Visible)
        {
            OffCardapio.Visible = false;
        }
        else
        {
            OffCardapio.Visible = true;
            OnCardapio.Visible = false;
        }

        if (OnEntrega.Visible)
        {
            OffEntrega.Visible = false;
        }
        else
        {
            OffEntrega.Visible = true;
            OnEntrega.Visible = false;
        }

    }

    private void OffIfood_Click(object sender, EventArgs e)
    {
        OffIfood.Visible = false;
        OnIfood.Visible = true;
    }

    private void OnIfood_Click(object sender, EventArgs e)
    {

        OffIfood.Visible = true;
        OnIfood.Visible = false;
    }

    private void OffCardapio_Click(object sender, EventArgs e)
    {
        OffCardapio.Visible = false;
        OnCardapio.Visible = true;
    }

    private void OnCardapio_Click(object sender, EventArgs e)
    {
        OffCardapio.Visible = true;
        OnCardapio.Visible = false;
    }

    private void OffEntrega_Click(object sender, EventArgs e)
    {
        OffEntrega.Visible = false;
        OnEntrega.Visible = true;
    }

    private void OnEntrega_Click(object sender, EventArgs e)
    {
        OffEntrega.Visible = true;
        OnEntrega.Visible = false;
    }
}
