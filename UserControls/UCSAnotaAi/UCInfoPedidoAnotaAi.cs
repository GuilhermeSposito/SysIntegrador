using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;
using System.Security.Policy;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.UserControls.UCSOnPedido;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.Forms;
using SysIntegradorApp.Forms.ANOTAAI;

namespace SysIntegradorApp.UserControls.UCSAnotaAi
{
    public partial class UCInfoPedidoAnotaAi : UserControl
    {
        public PedidoAnotaAi Pedido { get; set; }
        public string StatusPedido { get; set; }
        public AnotaAi AnotaAiClass { get; set; } = new AnotaAi(new data.InterfaceDeContexto.MeuContexto());

        public UCInfoPedidoAnotaAi()
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(this, 24); //da uma arredondada na borda do user control
            this.Resize += (sender, e) =>
            {
                ClsEstiloComponentes.SetRoundedRegion(this, 24);
            };
        }

        public async void SetLabels(PedidoAnotaAi p)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();

                    this.toolTip1.SetToolTip(this.pictureBox16, "Copiar Endereço de entrega");

                    string DefineEntrega = "";
                    string DefineLocalEntrega = "";
                    string? horarioEntrega = "";

                    if (p.InfoDoPedido.Type == "TAKE")
                    {
                        DefineEntrega = "Retirada";
                        DateTime DataCertaEntregarEmTimeStamp = DateTime.ParseExact(p.InfoDoPedido.TimeMax, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                        System.Globalization.CultureInfo.InvariantCulture,
                                                        System.Globalization.DateTimeStyles.AssumeUniversal);
                        DateTime DataCertaEntregarEm = DataCertaEntregarEmTimeStamp.ToLocalTime().AddMinutes(Config.TempoRetirada);

                        DefineLocalEntrega = "Retirada No Local Do Restaurante";

                        btnDespacharIfood.Text = "Pronto";

                        horarioEntrega = DataCertaEntregarEm.ToString();
                    }

                    if (p.InfoDoPedido.Type == "DELIVERY")
                    {
                        DefineEntrega = "Propria";
                        DateTime DataCertaEntregarEmTimeStamp = DateTime.ParseExact(p.InfoDoPedido.TimeMax, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                          System.Globalization.CultureInfo.InvariantCulture,
                                                          System.Globalization.DateTimeStyles.AssumeUniversal);
                        DateTime DataCertaEntregarEm = DataCertaEntregarEmTimeStamp.ToLocalTime().AddMinutes(Config.TempoEntrega);

                        DefineLocalEntrega = $"{p.InfoDoPedido.deliveryAddress.FormattedAddress} {p.InfoDoPedido.deliveryAddress.Neighborhood} - {p.InfoDoPedido.deliveryAddress.City}";

                        horarioEntrega = DataCertaEntregarEm.ToString();

                        if (p.InfoDoPedido.deliveryAddress.Complement is not null)
                        {
                            panel2.AutoScroll = true;
                            panel2.Height += 100;
                            var TextBoxComplemento = new System.Windows.Forms.TextBox() { Text = p.InfoDoPedido.deliveryAddress.Complement, ForeColor = Color.Black, AutoSize = true, ReadOnly = true };
                            panel2.Controls.Add(TextBoxComplemento);

                            TextBoxComplemento.BorderStyle = BorderStyle.None;
                            TextBoxComplemento.ReadOnly = true;
                            TextBoxComplemento.Multiline = false;
                            TextBoxComplemento.BackColor = Color.White; // Define a cor de fundo igual ao UserControl
                            TextBoxComplemento.TabStop = false; // Impede que o TextBox receba foco via Tab
                            TextBoxComplemento.WordWrap = true;
                            TextBoxComplemento.Font = new Font(TextBoxComplemento.Font, FontStyle.Bold);

                            for (int i = 0; i < p.InfoDoPedido.deliveryAddress.Complement.Length; i++)
                            {
                                TextBoxComplemento.Width += 10;
                            }

                            TextBoxComplemento.Location = new Point(411, 55);

                            panel2.PerformLayout();
                        }
                    }

                    if (p.InfoDoPedido.Type == "LOCAL")
                    {
                        DefineEntrega = "Propria";
                        DateTime DataCertaEntregarEmTimeStamp = DateTime.ParseExact(p.InfoDoPedido.TimeMax, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                          System.Globalization.CultureInfo.InvariantCulture,
                                                          System.Globalization.DateTimeStyles.AssumeUniversal);
                        DateTime DataCertaEntregarEm = DataCertaEntregarEmTimeStamp.ToLocalTime().AddMinutes(Config.TempoRetirada);

                        horarioEntrega = DataCertaEntregarEm.ToString();

                        btnDespacharIfood.Text = "Pronto";

                        DefineEntrega = $"{p.InfoDoPedido.Pdv.Table}";
                        DefineLocalEntrega = $"Entregar Na {p.InfoDoPedido.Pdv.Table}";
                    }


                    labelDisplayId.Text = $"#{p.InfoDoPedido.ShortReference}";

                    numId.Text = p.InfoDoPedido.Customer.Phone;

                    label1.Text = p.InfoDoPedido.Customer.Nome;

                    DateTime DataCertaDaFeitoEmTimeStamp = DateTime.ParseExact(p.InfoDoPedido.CreatedAt, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                          System.Globalization.CultureInfo.InvariantCulture,
                                                          System.Globalization.DateTimeStyles.AssumeUniversal);
                    DateTime DataCertaDaFeitoEm = DataCertaDaFeitoEmTimeStamp.ToLocalTime();

                    dateFeitoAs.Text = DataCertaDaFeitoEm.ToString().Substring(11, 5);

                    tipoEntrega.Text = DefineEntrega;

                    horarioEntregaPrevista.Text = horarioEntrega.Substring(11, 5);

                    labelEndereco.Text = DefineLocalEntrega;

                    float ValorDosItens = 0.0f;
                    foreach (var item in p.InfoDoPedido.Items)
                    {
                        ValorDosItens += item.Total;
                    }
                    ValorTotalDosItens.Text = ValorDosItens.ToString("c");


                    valorTaxaDeEntrega.Text = Convert.ToSingle(p.InfoDoPedido.DeliveryFee).ToString("c");

                    valorTaxaAdicional.Text = (0.0f).ToString("c");

                    float valorDosDescontos = 0.0f;
                    foreach (var desconto in p.InfoDoPedido.Descontos)
                    {
                        valorDosDescontos += desconto.Total;
                    }
                    valorDescontos.Text = valorDosDescontos.ToString("c");

                    valorTotal.Text = p.InfoDoPedido.Total.ToString("c");



                    var InfoPag = ClsInfosDePagamentosParaImpressaoAnotaAi.DefineTipoDePagamento(p.InfoDoPedido.Payments);

                    infoPagPedido.Text = InfoPag.TipoPagamento;
                    obsPagamentoPedido.Text = InfoPag.FormaPagamento;

                    if (p.InfoDoPedido.Customer.CPF == null)
                    {
                        labelCPF.Text = "NÃO";
                    }
                    else
                    {
                        labelCPF.Text = p.InfoDoPedido.Customer.CPF;
                    }

                    if (StatusPedido == "DISPATCHED")
                    {
                        btnDespacharIfood.Visible = false;
                    }

                    if (StatusPedido == "CONCLUDED")
                    {
                        btnDespacharIfood.Visible = false;
                        btnCancelar.Visible = false;
                        buttonReadyToPickUp.Visible = false;
                    }

                }
            }
            catch (Exception ex)
            {
                await Logs.CriaLogDeErro(ex.ToString());
            }
        }

        public void InsereItemNoPedido(List<ItemAnotaAi> items)
        {
            //primeiro instanciar um objeto do UserControl UCItem
            //Dentro do UserControl Temos que criar um método que define as labels 

            foreach (var item in items)
            {
                UCItemANotaAI uCItem = new UCItemANotaAI();
                uCItem.SetLabels(item.Name, item.quantity, item.Price, item.Total, item.SubItens, uCItem, item);
                panelDeItens.Controls.Add(uCItem);
            }

        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.labelEndereco.Text);
        }

        private async void btnDespacharIfood_Click(object sender, EventArgs e)
        {
            bool OperacaoDeuCerto = await AnotaAiClass.DespachaPedido(Pedido.InfoDoPedido.IdPedido);

            if (OperacaoDeuCerto)
            {
                string? mensagemDeSucesso = "";

                if (Pedido.InfoDoPedido.Type == "TAKE")
                    mensagemDeSucesso = "Pedido pronto pra retirada com sucesso!";

                if (Pedido.InfoDoPedido.Type == "DELIVERY")
                    mensagemDeSucesso = "Despachado Com sucesso";

                if (Pedido.InfoDoPedido.Type == "LOCAL")
                    mensagemDeSucesso = "Pedido pronto";

                MessageBox.Show(mensagemDeSucesso, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erro ao despachar pedido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonReadyToPickUp_Click(object sender, EventArgs e)
        {

            bool OperacaoDeuCerto = await AnotaAiClass.FinalizaPedido(Pedido.InfoDoPedido.IdPedido);

            if (OperacaoDeuCerto)
            {
                MessageBox.Show("Pedido Finalizado Com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erro ao finalizar pedido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            FormDeCancelamentoAnotaAi formDeCancelamentoAnotaAi = new FormDeCancelamentoAnotaAi() { IdPedido = Pedido.InfoDoPedido.IdPedido };

            formDeCancelamentoAnotaAi.ShowDialog();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            int numDaVia = pictureBoxUm.Visible == true ? 1 : 2;

            ImpressaoAnotaAi.ChamaImpressoes(Pedido.InfoDoPedido.IdPedido, numDaVia);
        }

        private void pictureBoxDois_Click(object sender, EventArgs e)
        {
            pictureBoxDois.Visible = false;
            pictureBoxUm.Visible = true;
        }

        private void pictureBoxUm_Click(object sender, EventArgs e)
        {
            pictureBoxDois.Visible = true;
            pictureBoxUm.Visible = false;
        }
    }
}
