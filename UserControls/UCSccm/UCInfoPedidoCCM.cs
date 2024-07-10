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
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.UserControls.UCSOnPedido;
using System.Security.Cryptography.X509Certificates;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.data;
using SysIntegradorApp.Forms.CCM;

namespace SysIntegradorApp.UserControls.UCSccm;

public partial class UCInfoPedidoCCM : UserControl
{
    public Pedido Pedido { get; set; }
    public string? Status { get; set; }

    public UCInfoPedidoCCM()
    {
        InitializeComponent();
    }

    private void UCInfoPedidoCCM_Load(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var configs = db.parametrosdosistema.FirstOrDefault();

                if (Status == "Aguardando")
                {
                    if (!configs.AceitaPedidoAut)
                    {
                        btnCancelar.Visible = false;
                        btnDespacharCCM.Visible = false;
                        buttonReadyToPickUp.Visible = false;

                        BtnAceitar.Visible = true;
                        BtnRejeitar.Visible = true;
                    }
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public void SetLabels()
    {
        try
        {
            string? TipoPedido = Pedido.Retira == 1 ? "TAKEOUT" : "DELIVERY";
            string? TipoDaEntrega = "";
            string? EnderecoDaEntrega = "";
            var DataCorreta = " ";

            bool PedidoMesa = Pedido.NumeroMesa > 0 ? true : false;


            if (TipoPedido == "DELIVERY")
            {
                TipoDaEntrega = "Propria";
                EnderecoDaEntrega = $"{Pedido.Endereco.Rua}, {Pedido.Endereco.Numero} - {Pedido.Endereco.Bairro}";
                DataCorreta = Pedido.EntregarAte;
            }

            if (TipoPedido == "TAKEOUT")
            {
                if (!PedidoMesa)
                {
                    TipoDaEntrega = "Retirada";
                    EnderecoDaEntrega = "RETIRADA NO LOCAL DO RESTAURANTE";

                    DataCorreta = Pedido.EntregarAte;

                }
                else
                {
                    TipoDaEntrega = "MESA";
                    EnderecoDaEntrega = $"Entregar para a mesa: {Pedido.NumeroMesa}";

                    DataCorreta = Pedido.EntregarAte;
                }

            }

            if (Pedido.Agendamento == 1)
            {
                labelTipoEntregaNM.Text = "Agendada:";
                labelTipoEntregaNM.ForeColor = Color.Red;
                tipoEntrega.Location = new Point(155, 23);

                if (TipoPedido == "DELIVERY")
                {
                    DataCorreta = Pedido.DataHoraAgendamento;
                }

                if (TipoPedido == "TAKEOUT")
                {
                    DataCorreta = Pedido.DataHoraAgendamento;
                }

                if (TipoPedido == "INDOOR")
                {
                    //DataCorreta = Pedido.Return.Indoor.IndoorDateTime;
                }
            }

            labelDisplayId.Text = $"#{Pedido.NroPedido.ToString()}";

            numId.Text = $"{Pedido.Cliente.Telefone}";

            label1.Text = Pedido.Cliente.Nome;

            dateFeitoAs.Text = Pedido.DataHoraPedido.Substring(11, 5);

            tipoEntrega.Text = TipoDaEntrega;


            horarioEntregaPrevista.Text = DataCorreta.Substring(11, 5);

            labelEndereco.Text = EnderecoDaEntrega;

            ValorTotalDosItens.Text = Pedido.ValorBruto.ToString("c");

            float ValorEntrega = 0.0f;
            ValorEntrega = Pedido.ValorTaxa;
            valorTaxaDeEntrega.Text = ValorEntrega.ToString("c");

            float TaxaAdicional = 0.0f;

            valorTaxaAdicional.Text = TaxaAdicional.ToString("c");//Pedido.ValorTaxa.ToString("c");

            float descontos = Pedido.ValorCupom + Pedido.CreditoUtilizado;

            valorDescontos.Text = descontos.ToString("c");
            valorTotal.Text = Pedido.ValorTotal.ToString("c");

            string? defineTroco = "";

            if (!String.IsNullOrEmpty(Pedido.TrocoPara))
            {
                if (Pedido.DescricaoPagamento == "Dinheiro")
                {
                    var TrocoPara = float.Parse(Pedido.TrocoPara.Replace(".", ","));
                    var troco = TrocoPara - Pedido.ValorTotal;

                    defineTroco = $". Levar troco para {TrocoPara.ToString("c")}. Total troco: {troco.ToString("c")}";
                }
            }


            infoPagPedido.Text = Pedido.PagamentoOnline == 1 ? "Pagamento Online" : $"Pagamento será pago na entrega com {Pedido.DescricaoPagamento}{defineTroco}";
            obsPagamentoPedido.Text = Pedido.PagamentoOnline == 1 ? "Não deverá ser cobrado do cliente na entrega" : "Devera ser cobrado do cliente na entrega"; //InfoPag.FormaPagamento;

            if (TipoPedido == "INDOOR")
            {
                infoPagPedido.Text = "O pagamento do pedido será efetuado no caixa!";
                obsPagamentoPedido.Text = "Pedido deverá ser cobrado no caixa";
            }

            if (Pedido.Cliente.Email != null && !Pedido.Cliente.Email.Contains("naoinformado"))
            {
                labelCPF.Text = Pedido.Cliente.Email;
            }
            else
            {
                labelCPF.Text = "Não";
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public void InsereItemNoPedido(List<Item> items)
    {
        //primeiro instanciar um objeto do UserControl UCItem
        //Dentro do UserControl Temos que criar um método que define as labels 

        foreach (var item in items)
        {
            UCItemCCM uCItem = new UCItemCCM();
            uCItem.SetLabels(item.NomeItem, item.Quantidade, item.ValorUnit, 5.5f, item.ValorUnit, item.Adicionais, uCItem, item);
            panelDeItens.Controls.Add(uCItem);
        }

    }

    private async void btnDespacharCCM_Click(object sender, EventArgs e)
    {
        CCM ccm = new CCM(new MeuContexto());

        await ccm.AtualizaStatus(Pedido.NroPedido, status: "5", true);
    }

    private async void buttonReadyToPickUp_Click(object sender, EventArgs e)
    {
        CCM ccm = new CCM(new MeuContexto());

        await ccm.AtualizaStatus(Pedido.NroPedido, status: "6", true);
    }

    private async void btnCancelar_Click(object sender, EventArgs e)
    {
    }

    private async void BtnAceitar_Click(object sender, EventArgs e)
    {
        CCM ccm = new CCM(new MeuContexto());

        await ccm.AceitaPedido(Pedido.NroPedido);

        MessageBox.Show($"Pedido Aceito Com sucesso", "Aceito!");

        ccm.ChamaImpressaoAutomatica(Pedido);

        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.panelDetalhePedido.Controls.Clear()));
        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe)));
        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true));
    }

    private async void BtnRejeitar_Click(object sender, EventArgs e)
    {
        FormDePedidoNaoAceito recusa = new FormDePedidoNaoAceito() { NumeroPedido = Pedido.NroPedido };
        recusa.ShowDialog();
    }

    private void btnImprimir_Click(object sender, EventArgs e)
    {
        using ApplicationDbContext db = new ApplicationDbContext();
        ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Pedido.NroPedido.ToString()).FirstOrDefault();
        ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

        List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

        bool ImprimeSoCaixa = pictureBoxDois.Visible == true && pictureBoxUm.Visible == false ? true : false;


        if (!opSistema.AgruparComandas && !ImprimeSoCaixa)
        {
            foreach (string imp in impressoras)
            {
                if (imp != "Sem Impressora" && imp != null)
                {
                    ImpressaoCCM.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp);
                }
            }
        }
        else if (!ImprimeSoCaixa)
        {
            ImpressaoCCM.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
        }

        if (ImprimeSoCaixa)
        {
            if (opSistema.ImpCompacta)
            {
                ImpressaoCCM.DefineImpressao2(pedido.Conta, pedido.DisplayId, opSistema.Impressora1);
            }
            else
            {
                ImpressaoCCM.DefineImpressao2(pedido.Conta, pedido.DisplayId, opSistema.Impressora1);
            }
        }


        impressoras.Clear();
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
