using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesDeConexaoComApps;
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

public partial class FormDeCancelamento : Form
{
    public string? id_Pedido { get; set; }
    public string? display_Id { get; set; }
    public FormDeCancelamento()
    {
        InitializeComponent();
    }


    private async void FormDeCancelamento_Load(object sender, EventArgs e)
    {
        List<ClsMotivosDeCancelamento> motivos = await Ifood.CancelaPedidoOpcoes(id_Pedido);

        if (motivos.Count() == 0)
        {
           var OkUser =  MessageBox.Show("Não é possivel cancelar este pedido", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);

           this.Close();
        }
        else
        {
            foreach (ClsMotivosDeCancelamento motivo in motivos)
            {
                UCMotivoCancelamento UserControlDeMotivos = new UCMotivoCancelamento() { IdPedido = id_Pedido, cancelCodeId = motivo.cancelCodeId, description = motivo.description, display_Id = display_Id };
                panelDeMotivos.Controls.Add(UserControlDeMotivos);
            }
        }
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
