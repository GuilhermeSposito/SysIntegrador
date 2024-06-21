using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.data;

namespace SysIntegradorApp.Forms.ONPEDIDO;

public partial class FormDeCronograma : Form
{
    public FormDeCronograma()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24);
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    public static void AlimentaComInfosDoBancoDeDados(FormDeCronograma instancia)
    {
        try
        {

            ApplicationDbContext dbContext = new ApplicationDbContext();

            var configuracoes = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            instancia.textBoxTempoEntrega.Text = configuracoes.TempoEntrega.ToString();
            instancia.textBoxTempoDeRetirada.Text = configuracoes.TempoRetirada.ToString();
            instancia.textBoxTempoConclPedido.Text = configuracoes.TempoConclonPedido.ToString();

        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message, "Ops");
        }
    }

    private void btnEnvir_Click(object sender, EventArgs e)
    {
        try
        {
            int tempoDeEntrega = Convert.ToInt32(textBoxTempoEntrega.Text);
            int tempoDeRetirada = Convert.ToInt32(textBoxTempoDeRetirada.Text);
            int TempoConclPedido = Convert.ToInt32(textBoxTempoConclPedido.Text);


            ParametrosDoSistema.SetInfosDeCronograma(tempoDeEntrega, tempoDeRetirada, TempoConclPedido);

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
    }
}
