using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.UserControls;
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

public partial class FormDeConfirmacaoDeCancelamento : Form
{

    public string? IdPedido { get; set; }
    public string? cancelCodeId { get; set; }
    public string? description { get; set; }
    public string? display_Id { get; set; }

    public FormDeConfirmacaoDeCancelamento()
    {
        InitializeComponent();
    }

    private void naoBtn_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private async void simBtn_Click(object sender, EventArgs e)
    {
        try
        {
            Ifood Ifood = new Ifood(new MeuContexto());

            int statusCode = await Ifood.CancelaPedido(orderId: IdPedido, reason: description, cancellationCode: cancelCodeId); //retorna o status code
            using ApplicationDbContext db = new ApplicationDbContext();
            var ConfigSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            if (statusCode == 202)
            {
                if (Application.OpenForms["FormDeCancelamento"] != null)
                {
                    Application.OpenForms["FormDeCancelamento"].Close();
                    this.Close();
                }

                if (!ConfigSistema.AceitaPedidoAut)
                {
                    var pedido = db.parametrosdopedido.Where(x => x.Id == IdPedido).ToList().FirstOrDefault();
                    Polling? polling = JsonConvert.DeserializeObject<Polling>(pedido.JsonPolling);

                    Ifood.AvisarAcknowledge(polling);
                }

                FormMenuInicial.panelDetalhePedido.Controls.Clear();
                FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe);
                FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true;
            }
            else
            {
                MessageBox.Show("Não foi possivel cancelar o pedido", "Ops");
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    private void FormDeConfirmacaoDeCancelamento_Load(object sender, EventArgs e)
    {

        UCMotivoCancelamento motivoCancelamento = new UCMotivoCancelamento() { cancelCodeId = cancelCodeId, description = description };
        motivoCancelamento.Size = new Size(530, 34);
        ClsEstiloComponentes.SetRoundedRegion(motivoCancelamento, 24);
        panelConfirmaCaneclamento.Controls.Add(motivoCancelamento);
    }

    private void FormDeConfirmacaoDeCancelamento_Paint(object sender, PaintEventArgs e)
    {
        labelDisplayId.Text = display_Id;
    }
}
