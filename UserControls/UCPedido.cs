using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.data;
using SysIntegradorApp.UserControls.UCSDelMatch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class UCPedido : UserControl
{
    public PedidoCompleto Pedido { get; set; } = new PedidoCompleto();
    public string? Id_pedido { get; set; }
    public string? OrderType { get; set; }
    public string? Display_id { get; set; }
    public string? NomePedido { get; set; }
    public string? DeliveryBy { get; set; }
    public string? FeitoAs { get; set; }
    public string? HorarioEntrega { get; set; }
    public string? LocalizadorPedido { get; set; }
    public string? EnderecoFormatado { get; set; }
    public string? Bairro { get; set; }
    public string TipoDaEntrega { get; set; }
    public float ValorTotalItens { get; set; }
    public float ValorTaxaDeentrega { get; set; }
    public float Valortaxaadicional { get; set; }
    public float Descontos { get; set; }
    public float TotalDoPedido { get; set; }
    public string? Observations { get; set; }
    public List<Items> items { get; set; } = new List<Items>();


    public UCPedido()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 10);
    }

    public void SetLabels(string id_pedido, string numPedido, string nomePedido, string horarioPedido, string statusPedido)
    {
        Id_pedido = id_pedido;
        labelNumPedido.Text = $"#{numPedido}";
        labelNumConta.Text = Pedido.NumConta.ToString().PadLeft(3, '0');
        labelNomePedido.Text = nomePedido;
        labelHorarioDeEntrega.Text = HorarioEntrega.Substring(11, 5);
        string status = TraduzStatus.TraduzStatusEnviado(statusPedido);

        if (status == "Cancelado" || status == "Pendente")
        {
            labelStatus.ForeColor = Color.Red;
            labelStatus.Font = new Font(labelStatus.Font.FontFamily, 9, FontStyle.Bold);
        }
        else
        {
            labelStatus.ForeColor = Color.Green;
            labelStatus.Font = new Font(labelStatus.Font.FontFamily, 9, FontStyle.Bold);
        }

        labelStatus.Text = status;
    }

    public void MudarLabelQuandoAgendada(string texto)
    {
        labelEntregarAte.Text = texto;
        labelEntregarAte.ForeColor = Color.Red;

        labelHorarioDeEntrega.Location = new Point(230, 72);
    }

    private void labelStatus_Click(object sender, EventArgs e) { }

    private void UCPedido_Load(object sender, EventArgs e) { }

    public async void UCPedido_Click(object sender, EventArgs e)
    {
        try
        {
            if (Pedido.CriadoPor == "DELMATCH")
            {
                try
                {

                    ApplicationDbContext db = new ApplicationDbContext();
                    ParametrosDoPedido? Pedido = db.parametrosdopedido.Where(x => x.Id == Id_pedido).ToList().FirstOrDefault();

                    PedidoDelMatch? PedidoDeserializado = JsonConvert.DeserializeObject<PedidoDelMatch>(Pedido.Json);

                    FormMenuInicial.panelDetalhePedido.Controls.Clear();
                    FormMenuInicial.panelDetalhePedido.PerformLayout();
                    UCInfoPedidosDelMatch infoPedido = new UCInfoPedidosDelMatch() { Pedido = PedidoDeserializado, Status = Pedido.Situacao};
                    infoPedido.SetLabels();

                    int tamanhoPanel = FormMenuInicial.panelDetalhePedido.Width;

                    infoPedido.Width = tamanhoPanel - 50;//1707;
                    infoPedido.Height = 1200;

                    infoPedido.InsereItemNoPedido(PedidoDeserializado.Items);
                    FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = false;
                    FormMenuInicial.panelDetalhePedido.Controls.Add(infoPedido);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }


            if (Pedido.CriadoPor == "IFOOD")
            {

                FormMenuInicial.panelDetalhePedido.Controls.Clear();
                FormMenuInicial.panelDetalhePedido.PerformLayout();
                UCInfoPedido infoPedido = new UCInfoPedido() { Pedido = Pedido, Id_pedido = Id_pedido, orderType = OrderType, Display_id = Display_id };

                infoPedido.SetLabels(
                                       horarioEntrega: HorarioEntrega,
                                       localizadorPedido: LocalizadorPedido,
                                       enderecoFormatado: EnderecoFormatado,
                                       bairro: Bairro,
                                       TipoEntrega: TipoDaEntrega,
                                       valorTotalItens: ValorTotalItens,
                                       valorTaxaDeentrega: ValorTaxaDeentrega,
                                       valortaxaadicional: Valortaxaadicional,
                                       descontos: Descontos,
                                       total: TotalDoPedido
                                  );

                int tamanhoPanel = FormMenuInicial.panelDetalhePedido.Width;

                infoPedido.Width = tamanhoPanel - 50;//1707;
                infoPedido.Height = 1200;

                infoPedido.InsereItemNoPedido(items);
                FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = false;
                FormMenuInicial.panelDetalhePedido.Controls.Add(infoPedido);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro");
        }

    }

    public void MudaParaLogoDelMatch(UCPedido instancia)
    {
        instancia.pictureBox1.Visible = false;
        instancia.pictureBoxDELMATCH.Visible = true;
    }

    private void UCPedido_Enter(object sender, EventArgs e)
    {
        this.BackColor = Color.DarkGray;
        pictureBox1.BackColor = Color.DarkGray;
    }

    private void UCPedido_Leave(object sender, EventArgs e)
    {
        this.BackColor = Color.White;
    }

    private void labelNomePedido_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void labelNumPedido_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void labelEntregarAte_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void labelHorarioDeEntrega_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void labelStatus_Click_1(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }

    private void UCPedido_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            UCPedido_Click(sender, e);
        }

        if (e.KeyChar == (char)Keys.F4)
        {
            pictureBoxImp_Click(sender, e);
        }

    }

    private void pictureBoxImp_Click(object sender, EventArgs e)
    {
        try
        {
            if (Pedido.CriadoPor == "DELMATCH")
            {
                using ApplicationDbContext db = new ApplicationDbContext();
                ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Id_pedido).FirstOrDefault();
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

                if (!opSistema.AgruparComandas)
                {
                    foreach (string imp in impressoras)
                    {
                        if (imp != "Sem Impressora" && imp != null)
                        {
                            ImpressaoDelMatch.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp);
                        }
                    }
                }
                else
                {
                    ImpressaoDelMatch.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
                }



                impressoras.Clear();
            }

            if (Pedido.CriadoPor == "IFOOD")
            {

                using ApplicationDbContext db = new ApplicationDbContext();
                ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Id_pedido).FirstOrDefault();
                ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

                if (!opSistema.AgruparComandas)
                {
                    foreach (string imp in impressoras)
                    {
                        if (imp != "Sem Impressora" && imp != null)
                        {
                            Impressao.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp);
                        }
                    }
                }
                else
                {
                    Impressao.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
                }



                impressoras.Clear();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro Ao imprimir pelo UCPEDIDO");
        }
    }

    private void UCPedido_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.I)
        {
            pictureBoxImp_Click(sender, e);
        }
    }

    private void pictureBoxDELMATCH_Click(object sender, EventArgs e)
    {
        UCPedido_Click(sender, e);
        UCPedido_Enter(sender, e);
        this.Focus();
    }
}
