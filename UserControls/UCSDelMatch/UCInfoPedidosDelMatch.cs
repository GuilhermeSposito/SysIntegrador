using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
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

namespace SysIntegradorApp.UserControls.UCSDelMatch;

public partial class UCInfoPedidosDelMatch : UserControl
{
    private readonly IMeuContexto _context;

    public PedidoDelMatch Pedido { get; set; }
    public string? Status { get; set; }
    public UCInfoPedidosDelMatch(IMeuContexto context)
    {
        _context = context;
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24); //da uma arredondada na borda do user control
        this.Resize += (sender, e) =>
        {
            ClsEstiloComponentes.SetRoundedRegion(this, 24);
        };
    }

    public void SetLabels()
    {
        DateTime horaDeEntregaEMDT = DateTime.Now;

        if (Pedido.Type != "INDOOR")
        {
            horaDeEntregaEMDT = DateTime.Parse(Pedido.DeliveryDateTime);
        }

        string? TipoPedido = Pedido.Type;
        string? TipoDaEntrega = "";
        string? EnderecoDaEntrega = "";
        var DataConvertida = horaDeEntregaEMDT;
        var DataCorreta = " ";

        if (TipoPedido == "DELIVERY")
        {
            TipoDaEntrega = "Propria";
            EnderecoDaEntrega = $"{Pedido.deliveryAddress.StreetName}, {Pedido.deliveryAddress.StreetNumber} - {Pedido.deliveryAddress.Neighboardhood}";
            DataCorreta = DataConvertida.AddMinutes(50).ToString();

            if (Pedido.deliveryAddress.Complement is not null)
            {
                panel2.AutoScroll = true;
                panel2.Height += 100;
                var TextBoxComplemento = new System.Windows.Forms.TextBox() { Text = Pedido.deliveryAddress.Complement, ForeColor = Color.Black, AutoSize = true, ReadOnly = true };
                panel2.Controls.Add(TextBoxComplemento);

                TextBoxComplemento.BorderStyle = BorderStyle.None;
                TextBoxComplemento.ReadOnly = true;
                TextBoxComplemento.Multiline = false;
                TextBoxComplemento.BackColor = Color.White; // Define a cor de fundo igual ao UserControl
                TextBoxComplemento.TabStop = false; // Impede que o TextBox receba foco via Tab
                TextBoxComplemento.WordWrap = true;
                TextBoxComplemento.Font = new Font(TextBoxComplemento.Font, FontStyle.Bold);

                for (int i = 0; i < Pedido.deliveryAddress.Complement.Length; i++)
                {
                    TextBoxComplemento.Width += 10;
                }

                TextBoxComplemento.Location = new Point(411, 55);

                panel2.PerformLayout();
            }
        }

        if (TipoPedido == "TOGO")
        {
            TipoDaEntrega = "Retirada";
            EnderecoDaEntrega = "RETIRADA NO LOCAL DO RESTAURANTE";
            DataCorreta = DataConvertida.AddMinutes(30).ToString();
        }

        if (TipoPedido == "INDOOR")
        {
            TipoDaEntrega = $"MESA {Pedido.Indoor.table}";
            EnderecoDaEntrega = $" Entregar na Mesa {Pedido.Indoor.table}";
            DataCorreta = DataConvertida.AddMinutes(10).ToString();
        }



        labelDisplayId.Text = $"#{Pedido.Reference.ToString()}";

        numId.Text = Pedido.Customer.Phone != null ? Pedido.Customer.Phone : "Sem Telefone Informado";

        label1.Text = Pedido.Customer.Name;


        dateFeitoAs.Text = Pedido.CreatedAt.ToString().Substring(11, 5);

        tipoEntrega.Text = TipoDaEntrega;

        horarioEntregaPrevista.Text = DataCorreta.Substring(11, 5);

        labelEndereco.Text = EnderecoDaEntrega;

        ValorTotalDosItens.Text = Pedido.SubTotal.ToString("c");

        valorTaxaDeEntrega.Text = Pedido.deliveryFee.ToString("c");

        valorTaxaAdicional.Text = Pedido.AdditionalFee.ToString("c");

        valorDescontos.Text = Pedido.Discount.ToString("c");

        valorTotal.Text = Pedido.TotalPrice.ToString("c");


        var InfoPag = ClsInfosDePagamentosParaImpressaoDelMatch.DefineTipoDePagamento(Pedido.Payments);

        var Info1 = $"{InfoPag.FormaPagamento} ({InfoPag.TipoPagamento})";
        var Info2 = InfoPag.TipoPagamento == "Não é nescessario receber do cliente na entrega" ? "Não é nescessario receber do cliente na entrega" : "Receber do Cliente na entrega";

        if (Pedido.Type == "INDOOR")
        {
            Info1 = $"O pagamento do pedido será efetuado no caixa";
            Info2 = "Pedido deverá ser cobrado no caixa";
        }

        infoPagPedido.Text = Info1;
        obsPagamentoPedido.Text = Info2;

        if (Pedido.Customer.CPF == null)
        {
            labelCPF.Text = "NÃO";
        }
        else
        {
            labelCPF.Text = Pedido.Customer.CPF;
        }
    }

    public void InsereItemNoPedido(List<items> items)
    {
        //primeiro instanciar um objeto do UserControl UCItem
        //Dentro do UserControl Temos que criar um método que define as labels 

        foreach (items item in items)
        {
            UCItemDelMatch uCItem = new UCItemDelMatch();
            uCItem.SetLabels(item.Name, item.Quantity, item.Price, item.SubItemsPrice, item.TotalPrice, item.SubItems, uCItem, item);
            panelDeItens.Controls.Add(uCItem);
        }

    }

    private async void buttonImprimir_Click(object sender, EventArgs e)
    {
        using (ApplicationDbContext db = await _context.GetContextoAsync())
        {
            ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Pedido.Id.ToString()).FirstOrDefault();
            ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

            if (!opSistema.AgruparComandas)
            {
                foreach (string imp in impressoras)
                {
                    if (imp != "Sem Impressora" && imp != null)
                    {
                        ImpressaoDelMatch.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp, true);
                    }
                }
            }
            else
            {
                ImpressaoDelMatch.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras, true);
            }
            impressoras.Clear();

        }

    }

    private async void btnDespachar_Click(object sender, EventArgs e)
    {
        try
        {
            DelMatch Delmatch = new DelMatch(new MeuContexto());

            await Delmatch.DispachaPedidoDelMatch(Pedido.Reference);

            MessageBox.Show($"Pedido {Pedido.Reference} Despachado com sucesso!", "Despachado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        catch (Exception)
        {
            MessageBox.Show("Erro ao despachar pedido");
        }
    }

    private async void buttonCancelar_Click(object sender, EventArgs e)
    {
        try
        {
            DelMatch Delmatch = new DelMatch(new MeuContexto());


            await Delmatch.CancelaPedidoDelMatch(Pedido.Reference);

            ClsDeIntegracaoSys.ExcluiPedidoCasoCancelado(Pedido.Id.ToString());

            MessageBox.Show($"Pedido {Pedido.Reference} Cancelado com sucesso!", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception)
        {
            MessageBox.Show("Erro ao despachar pedido");
        }
    }

    private async void BtnConcluirPedido_Click(object sender, EventArgs e)
    {
        try
        {
            DelMatch Delmatch = new DelMatch(new MeuContexto());


            await Delmatch.MarcaEntregePedidoDelMatch(Pedido.Reference);

            MessageBox.Show($"Pedido {Pedido.Reference} Concluido com sucesso!", "Concluido", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception)
        {
            MessageBox.Show("Erro ao despachar pedido");
        }
    }

    private async void UCInfoPedidosDelMatch_Load(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = await _context.GetContextoAsync())
            {
                var configs = db.parametrosdosistema.FirstOrDefault();

                if (Status == "Cancelado")
                {
                    buttonCancelar.Visible = false;
                    BtnConcluirPedido.Visible = false;
                    BtnConcluirPedido.Visible = false;
                    btnDespachar.Visible = false;
                }

                if (Status == "Pendente")///terminar de implementar depois
                {
                    buttonCancelar.Visible = false;
                    BtnConcluirPedido.Visible = false;
                    BtnConcluirPedido.Visible = false;
                    btnDespachar.Visible = false;

                    if (!configs.AceitaPedidoAut)
                    {
                        BtnAceitar.Visible = true;
                        BtnRejeitar.Visible = true;
                    }

                }

                if (Status == "Concluido")
                {
                    buttonCancelar.Visible = false;
                    BtnConcluirPedido.Visible = false;
                    BtnConcluirPedido.Visible = false;
                    btnDespachar.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    private async void BtnAceitar_Click(object sender, EventArgs e)
    {
        try
        {
            DelMatch Delmatch = new DelMatch(new MeuContexto());


            await Delmatch.ConfirmaPedidoDelMatch(Pedido);
            ClsSons.StopSom();

            FormMenuInicial.panelDetalhePedido.Controls.Clear();
            FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe);
            FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao confirmar pedido", "Ops");
        }
    }

    private async void BtnRejeitar_Click(object sender, EventArgs e)
    {
        try
        {
            DelMatch Delmatch = new DelMatch(new MeuContexto());


            await Delmatch.CancelaPedidoDelMatch(Pedido.Reference);

            MessageBox.Show($"Pedido {Pedido.Reference} Cancelado com sucesso!", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FormMenuInicial.panelDetalhePedido.Controls.Clear();
            FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe);
            FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true;
        }
        catch (Exception)
        {
            MessageBox.Show("Erro ao despachar pedido");
        }
    }

    private async void pictureBoxDELMATCH_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = await _context.GetContextoAsync())
            {
                ParametrosDoSistema Configs = db.parametrosdosistema.FirstOrDefault();

                DialogResult respUser = MessageBox.Show($"Você deseja chamar entregador para o pedido {Pedido.Id}?", "Chamando entregador", MessageBoxButtons.YesNo);

                if (respUser == DialogResult.Yes)
                {
                    DelMatch Delmatch = new DelMatch(new MeuContexto());

                    Sequencia ClsParaEnviarPedido = await Delmatch.CriarPedidoParaEnviar(Pedido);

                    string? JsonContent = JsonConvert.SerializeObject(ClsParaEnviarPedido);

                    HttpResponseMessage response = await Delmatch.GerarPedidoManual(JsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string? jsonContent = await response.Content.ReadAsStringAsync();

                        ClsDeserializacaoPedidoSucesso? reposta = JsonConvert.DeserializeObject<ClsDeserializacaoPedidoSucesso>(jsonContent);

                        string? Titulo = reposta.Success ? "Sucesso Ao enviar pedido" : "Erro ao enviar pedido";

                        Delmatch.UpdateDelMatchId(Pedido.NumConta, Pedido.NumConta.ToString().PadLeft(4, '0') + "-" + DateTime.Now.ToString().Substring(0, 10).Replace("-", "/"));

                        MessageBox.Show(reposta.Response, Titulo);

                        FormMenuInicial.SetarPanelPedidos();
                    }
                    else
                    {
                        string? jsonContent = await response.Content.ReadAsStringAsync();

                        ClsDeserializacaoPedidoFalho? reposta = JsonConvert.DeserializeObject<ClsDeserializacaoPedidoFalho>(jsonContent);

                        string? Titulo = reposta.Success ? "Sucesso Ao enviar pedido" : "Erro ao enviar pedido";
                        string Erro = "";

                        Erro += reposta.Response;

                        MessageBox.Show(Erro, Titulo);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
    }


}
