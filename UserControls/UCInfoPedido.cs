﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;
using SysIntegradorApp.ClassesAuxiliares.Ifood;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysIntegradorApp;

public partial class UCInfoPedido : UserControl
{
    public PedidoCompleto Pedido { get; set; }
    public string? Id_pedido { get; set; }
    public string? orderType { get; set; }
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

    public UCInfoPedido()
    {
        InitializeComponent();
        ClsEstiloComponentes.SetRoundedRegion(this, 24); //da uma arredondada na borda do user control
        this.Resize += (sender, e) =>
        {
            ClsEstiloComponentes.SetRoundedRegion(this, 24);
        };
    }

    private void labelPedidoNM_Click(object sender, EventArgs e) { }

    private void label4_Click(object sender, EventArgs e) { }

    public void SetLabels(string horarioEntrega, string localizadorPedido, string enderecoFormatado, string bairro, string TipoEntrega, float valorTotalItens, float valorTaxaDeentrega, float valortaxaadicional, float descontos, float total)
    {
        this.toolTip1.SetToolTip(this.pictureBox16, "Copiar Endereço de entrega");


        string DefineEntrega = "";
        string DefineLocalEntrega = "";

        if (TipoEntrega == "MERCHANT")
        {
            DefineEntrega = "Propria";
        }

        if (TipoEntrega == "Retirada")
        {
            DefineEntrega = "Retirada";
        }

        if (enderecoFormatado.Contains("Retirada"))
        {
            DefineLocalEntrega = "Retirada";
        }
        else
        {
            DefineLocalEntrega = $"{enderecoFormatado} - {bairro}";
        }

        labelLocalizadorPedido.Text = Pedido.delivery.pickupCode;

        labelDisplayId.Text = $"#{Pedido.displayId}";

        numId.Text = Pedido.customer.phone.localizer;

        label1.Text = Pedido.customer.name;

        DateTime DataCertaDaFeitoEmTimeStamp = DateTime.ParseExact(Pedido.createdAt, "yyyy-MM-ddTHH:mm:ss.fffZ",
                                              System.Globalization.CultureInfo.InvariantCulture,
                                              System.Globalization.DateTimeStyles.AssumeUniversal);
        DateTime DataCertaDaFeitoEm = DataCertaDaFeitoEmTimeStamp.ToLocalTime();

        dateFeitoAs.Text = DataCertaDaFeitoEm.ToString().Substring(11, 5);

        tipoEntrega.Text = DefineEntrega;

        horarioEntregaPrevista.Text = horarioEntrega.Substring(11, 5);

        labelLocalizadorPedido.Text = localizadorPedido;

        labelEndereco.Text = DefineLocalEntrega;

        ValorTotalDosItens.Text = valorTotalItens.ToString("c");

        valorTaxaDeEntrega.Text = valorTaxaDeentrega.ToString("c");

        valorTaxaAdicional.Text = valortaxaadicional.ToString("c");

        valorDescontos.Text = descontos.ToString("c");

        valorTotal.Text = total.ToString("c");



        var InfoPag = ClsInfosDePagamentosParaImpressao.DefineTipoDePagamento(metodos: Pedido.payments.methods);

        infoPagPedido.Text = $"{InfoPag.FormaPagamento} {InfoPag.TipoPagamento}";
        obsPagamentoPedido.Text = InfoPag.TipoPagamento == "Pago Online" ? "Não é nescessario receber do cliente" : "Deverá ser cobrado no local do cliente";

        if (Pedido.customer.documentNumber == null)
        {
            labelCPF.Text = "NÃO";
        }
        else
        {
            labelCPF.Text = Pedido.customer.documentNumber;
        }
    }

    public void InsereItemNoPedido(List<Items> items)
    {
        //primeiro instanciar um objeto do UserControl UCItem
        //Dentro do UserControl Temos que criar um método que define as labels 

        foreach (Items item in items)
        {
            UCItem uCItem = new UCItem();
            uCItem.SetLabels(item.name, item.quantity, item.unitPrice, item.optionsPrice, item.totalPrice, item.options, uCItem, item);
            panelDeItens.Controls.Add(uCItem);
        }

    }

    //Método chamado no clique da impressão, dentro dele setamos a propriedade estatica dentro da classe de impressão, para que la dentro possamos fazer um select no accsses
    private void btnImprimir_Click(object sender, EventArgs e)
    {
        using ApplicationDbContext db = new ApplicationDbContext();
        ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Id_pedido).FirstOrDefault();
        ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

        List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

        bool ImprimeSoCaixa = pictureBoxDois.Visible == true && pictureBoxUm.Visible == false ? true : false;


        if (!opSistema.AgruparComandas && !ImprimeSoCaixa)
        {
            foreach (string imp in impressoras)
            {
                if (imp != "Sem Impressora" && imp != null)
                {
                    Impressao.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp);
                }
            }
        }
        else if (!ImprimeSoCaixa)
        {
            Impressao.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
        }

        if (ImprimeSoCaixa)
        {
            if (opSistema.ImpCompacta)
            {
                Impressao.DefineImpressao2(pedido.Conta, pedido.DisplayId, opSistema.Impressora1);
            }
            else
            {
                Impressao.DefineImpressao(pedido.Conta, pedido.DisplayId, opSistema.Impressora1);
            }
        }


        impressoras.Clear();
    }

    private async void btnDespacharIfood_Click(object sender, EventArgs e)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (config!.IfoodMultiEmpresa)
                {
                    var empresa = await db.empresasIfoods.FirstOrDefaultAsync(x => x.MerchantId == Pedido.merchant.id && x.IntegraEmpresa);
                    Ifood Ifood = new Ifood(new MeuContexto());
                    Ifood.DespacharPedidoMultiEmpresa(orderId: Id_pedido, empresa!.Token!);
                }
                else
                {
                    Ifood Ifood = new Ifood(new MeuContexto());
                    Ifood.DespacharPedido(orderId: Id_pedido);
                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void buttonReadyToPickUp_Click(object sender, EventArgs e)
    {

        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var config = await db.parametrosdosistema.FirstOrDefaultAsync();

                if (config!.IfoodMultiEmpresa)
                {
                    var empresa = await db.empresasIfoods.FirstOrDefaultAsync(x => x.MerchantId == Pedido.merchant.id && x.IntegraEmpresa);
                    Ifood Ifood = new Ifood(new MeuContexto());
                    Ifood.AvisoReadyToPickUpMultiEmpresa(orderId: Id_pedido, empresa!.Token!);
                }
                else
                {
                    Ifood Ifood = new Ifood(new MeuContexto());
                    Ifood.AvisoReadyToPickUp(orderId: Id_pedido);
                }
            }
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private void UCInfoPedido_Paint(object sender, PaintEventArgs e)
    {
        if (orderType == "TAKEOUT")
        {
            btnDespacharIfood.Visible = false;
            buttonReadyToPickUp.Location = new Point(166, 13);
        }
    }

    private async void btnCancelar_Click(object sender, EventArgs e)
    {
        try
        {

            FormDeCancelamento modalCancelamento = new FormDeCancelamento() { id_Pedido = Id_pedido, Pedido = Pedido };
            modalCancelamento.display_Id = Display_id;
            modalCancelamento.ShowDialog();

        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private async void UCInfoPedido_Load(object sender, EventArgs e)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? ConfigsSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            if (ConfigsSistema!.IfoodMultiEmpresa)
            {
                EmpresasIfood? empresa = await db.empresasIfoods.FirstOrDefaultAsync(x => x.MerchantId == Pedido.merchant.id && x.IntegraEmpresa);
                if (empresa is not null)
                {
                    LblIdentificacaoDaLoja.Text = empresa.NomeIdentificador;
                    LblIdentificacaoDaLoja.Visible = true;
                }
            }

            if (!ConfigsSistema.AceitaPedidoAut)
            {
                if (Pedido.Situacao == "PLACED")
                {
                    BtnAceitar.Visible = true;
                    BtnRejeitar.Visible = false;
                    btnImprimir.Visible = false;
                    btnDespacharIfood.Visible = false;
                    buttonReadyToPickUp.Visible = false;


                    btnCancelar.Visible = true;
                    btnCancelar.Size = new Size(125, 51);
                    btnCancelar.Location = new Point(696, 2);
                    btnCancelar.ForeColor = Color.White;
                    btnCancelar.BackColor = Color.Red;
                    btnCancelar.ForeColor = Color.White;
                    btnCancelar.FlatAppearance.BorderColor = Color.White;
                    btnCancelar.Text = "Rejeitar";
                    btnCancelar.Font = btnCancelar.Font = new Font(btnCancelar.Font.FontFamily, 12, FontStyle.Bold);
                }
            }

            if (Pedido.Situacao == "CANCELLED" || Pedido.Situacao == "CONCLUDED")
            {
                btnDespacharIfood.Visible = false;
                buttonReadyToPickUp.Visible = false;
                btnCancelar.Visible = false;
                BtnChamaEntregador.Visible = false;
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
            Polling NovoPolling = JsonConvert.DeserializeObject<Polling>(Pedido.JsonPolling)!;
            using ApplicationDbContext db = new ApplicationDbContext();
            var opSistema = db.parametrosdosistema.ToList().FirstOrDefault();
            Ifood Ifood = new Ifood(new MeuContexto());

            if (opSistema!.IfoodMultiEmpresa)
            {
                var empresa = await db.empresasIfoods.FirstOrDefaultAsync(x => x.MerchantId == Pedido.merchant.id && x.IntegraEmpresa);

               await Ifood.ConfirmarPedidoMultiEmpresa(NovoPolling!, empresa!.Token!);
               await Ifood.AvisarAcknowledgeMultiEmpresa(NovoPolling!, empresa!.Token!);
            }
            else
            {
                await Ifood.ConfirmarPedido(NovoPolling);
                await Ifood.AvisarAcknowledge(NovoPolling);
                ClsSons.StopSom();
            }

            await SysAlerta.Alerta("Pedido Aceito", "Pedido Aceito com sucesso", SysAlertaTipo.Sucesso, SysAlertaButtons.Ok);

            List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };
            ParametrosDoPedido? pedido = db.parametrosdopedido.Where(x => x.Id == Id_pedido).FirstOrDefault();

            if (!opSistema.AgruparComandas)
            {
                foreach (string imp in impressoras)
                {
                    if (imp != "Sem Impressora" && imp != null)
                    {
                        Impressao.ChamaImpressoes(pedido!.Conta, pedido.DisplayId, imp);
                    }
                }
            }
            else
            {
                Impressao.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
            }

            impressoras.Clear();

            FormMenuInicial.panelDetalhePedido.Controls.Clear();
            FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe);
            FormMenuInicial.labelDeAvisoPedidoDetalhe.Visible = true;
        }
        catch (Exception ex)
        {
            await SysAlerta.Alerta("Erro", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }
    }

    private void pictureBoxUm_Click(object sender, EventArgs e)
    {
        pictureBoxUm.Visible = false;
        pictureBoxDois.Visible = true;

    }

    private void pictureBoxDois_Click(object sender, EventArgs e)
    {
        pictureBoxUm.Visible = true;
        pictureBoxDois.Visible = false;
    }

    private async void BtnChamaEntregador_Click(object sender, EventArgs e)
    {
        try
        {
            Ifood Ifood = new Ifood(new MeuContexto());
            await Ifood.ChamaEntregador(Pedido.id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void pictureBox16_Click(object sender, EventArgs e)
    {
        Clipboard.SetText(this.labelEndereco.Text);
    }
}
