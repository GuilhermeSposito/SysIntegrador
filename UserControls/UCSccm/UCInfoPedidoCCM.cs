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

            if (TipoPedido == "DELIVERY")
            {
                TipoDaEntrega = "Propria";
                EnderecoDaEntrega = $"{Pedido.Endereco.Rua}, {Pedido.Endereco.Numero} - {Pedido.Endereco.Bairro}";
                DataCorreta = Pedido.EntregarAte;
            }

            if (TipoPedido == "TAKEOUT")
            {
                TipoDaEntrega = "Retirada";
                EnderecoDaEntrega = "RETIRADA NO LOCAL DO RESTAURANTE";

                DataCorreta = Pedido.EntregarAte;

                // btnDespachar.Text = "Pronto";

            }

            /*if (TipoPedido == "INDOOR")
            {
                TipoDaEntrega = Pedido.Return.Indoor.Place;
                EnderecoDaEntrega = $"Entregar pedido na {Pedido.Return.Indoor.Place}";

                labelTipoEntregaNM.Text = "Entregar para:";
                tipoEntrega.Location = new Point(183, 23);

                DataCorreta = Pedido.Return.Indoor.IndoorDateTime;


            }*/

            if (Pedido.Agendamento == 1)
            {
                labelTipoEntregaNM.Text = "Agendada:";
                labelTipoEntregaNM.ForeColor = Color.Red;
                tipoEntrega.Location = new Point(155, 23);

                if (TipoPedido == "DELIVERY")
                {
                    DataCorreta = Pedido.HoraAgendamento;
                }

                if (TipoPedido == "TAKEOUT")
                {
                    DataCorreta = Pedido.HorarioRetirada;
                }

                if (TipoPedido == "INDOOR")
                {
                    //DataCorreta = Pedido.Return.Indoor.IndoorDateTime;
                }
            }

            labelDisplayId.Text = $"#{Pedido.NroPedido.ToString()}";

            numId.Text = $"{Pedido.Cliente.Telefone}";

            label1.Text = Pedido.Cliente.Nome;

            dateFeitoAs.Text = Pedido.DataHoraPedido;

            tipoEntrega.Text = TipoDaEntrega;


            horarioEntregaPrevista.Text = DataCorreta.Substring(11, 5);

            labelEndereco.Text = EnderecoDaEntrega;

            ValorTotalDosItens.Text = Pedido.ValorTotal.ToString("c");

            float ValorEntrega = 0.0f;
            ValorEntrega = Pedido.ValorTaxa;
            valorTaxaDeEntrega.Text = ValorEntrega.ToString("c");

            float TaxaAdicional = 0.0f;

            valorTaxaAdicional.Text = TaxaAdicional.ToString("c");//Pedido.ValorTaxa.ToString("c");

            valorDescontos.Text = Pedido.ValorCupom.ToString("c");
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
    }

    private async void BtnRejeitar_Click(object sender, EventArgs e)
    {
        FormDePedidoNaoAceito recusa = new FormDePedidoNaoAceito() { NumeroPedido = Pedido.NroPedido};
        recusa.ShowDialog();    
    }
}
