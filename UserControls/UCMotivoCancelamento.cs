using SysIntegradorApp.ClassesAuxiliares;
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

namespace SysIntegradorApp.UserControls
{
    public partial class UCMotivoCancelamento : UserControl
    {
        
        public string? IdPedido { get; set; }
        public string? cancelCodeId { get; set; }
        public string? description { get; set; }
        public string? display_Id { get; set; }

        public UCMotivoCancelamento()
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(this, 24);
        }
        private void labelDeMotivo_Click(object sender, EventArgs e)
        {
            FormDeConfirmacaoDeCancelamento formCancelamento = new FormDeConfirmacaoDeCancelamento();
            formCancelamento.ShowDialog();
        }

        private void UCMotivoCancelamento_Load(object sender, EventArgs e)
        {
            labelDeMotivo.Text = description;
        }

        private void UCMotivoCancelamento_Click(object sender, EventArgs e)
        {
            FormDeConfirmacaoDeCancelamento formCancelamento = new FormDeConfirmacaoDeCancelamento() { IdPedido = IdPedido, cancelCodeId = cancelCodeId, description = description, display_Id = display_Id };
            formCancelamento.ShowDialog();
        }
    }
}
