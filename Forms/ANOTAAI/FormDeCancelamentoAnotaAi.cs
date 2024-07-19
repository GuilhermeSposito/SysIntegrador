using Newtonsoft.Json;
using SysIntegradorApp.ClassesDeConexaoComApps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp.Forms.ANOTAAI;

public partial class FormDeCancelamentoAnotaAi : Form
{
    private AnotaAi AnotaAiClass { get; set; } = new AnotaAi(new data.InterfaceDeContexto.MeuContexto());
    public string IdPedido { get; set; }

    public FormDeCancelamentoAnotaAi()
    {
        InitializeComponent();
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private async void btnEnvir_Click(object sender, EventArgs e)
    {
        MotivoDeCancelamento motivoDeCancelamento = new MotivoDeCancelamento() { justification = motivoCancelamento.Text };

        string? JsonCancelation = JsonConvert.SerializeObject(motivoDeCancelamento);

        bool DeuCertoAOperacao = await AnotaAiClass.CancelaPedido(orderId: IdPedido, JsonCancelation);

        if (DeuCertoAOperacao)
        {
            this.Close();
        }
        else
        {
            MessageBox.Show("Erro Ao Cancelar Pedido, tente mais tarde ou consulte a integradora", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

public class MotivoDeCancelamento
{
    [JsonProperty("justification")] public string? justification { get; set; }
}