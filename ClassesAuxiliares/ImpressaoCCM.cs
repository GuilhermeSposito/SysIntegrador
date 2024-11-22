using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ImpressaoCCM
{
    public static int NumContas { get; set; }
    public static List<ClsImpressaoDefinicoesCMM>? Conteudo { get; set; } = new List<ClsImpressaoDefinicoesCMM>();
    public static List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> ConteudoParaImpSeparada { get; set; } = new List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas>();


    public static Font FonteGeral = new Font("DejaVu sans mono mono", 11, FontStyle.Bold);
    public static Font FonteSeparadores = new Font("DejaVu sans mono", 11, FontStyle.Bold);
    public static Font FonteCódigoDeBarras = new Font("3 of 9 Barcode", 35, FontStyle.Regular);
    public static Font FonteNomeRestaurante = new Font("DejaVu sans mono", 15, FontStyle.Bold);
    public static Font FonteEndereçoDoRestaurante = new Font("DejaVu sans mono", 9, FontStyle.Bold);
    public static Font FonteNúmeroDoPedido = new Font("DejaVu sans mono", 17, FontStyle.Bold);
    public static Font FonteDetalhesDoPedido = new Font("DejaVu sans mono", 9, FontStyle.Bold);
    public static Font FonteNúmeroDoTelefone = new Font("DejaVu sans mono", 11, FontStyle.Bold);
    public static Font FonteNomeDoCliente = new Font("DejaVu sans mono", 15, FontStyle.Bold);
    public static Font FonteEndereçoDoCliente = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteItens = new Font("DejaVu sans mono", 12, FontStyle.Bold);
    public static Font FonteOpcionais = new Font("DejaVu sans mono", 11, FontStyle.Regular);
    public static Font FonteObservaçõesItem = new Font("DejaVu sans mono", 11, FontStyle.Bold);
    public static Font FonteTotaisDoPedido = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteCPF = new Font("DejaVu sans mono", 8, FontStyle.Bold);

    public enum AlinhamentosCCM
    {
        Esquerda,
        Direita,
        Centro
    }
    public enum TamanhoPizza
    {
        PEQUENA,
        MÉDIA,
        GRANDE,
        BROTINHO
    }

    public static void Imprimir(List<ClsImpressaoDefinicoesCMM> conteudo, string impressora1, int separacao)
    {
        // Defina o nome da impressora específica que você deseja usar
        string printerName = impressora1;
        string texto = "";
        // Crie uma instância de PrintDocument
        PrintDocument printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;

        printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", 280, 500000);
        printDocument.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);



        // Atribua um manipulador de evento para o evento PrintPage
        printDocument.PrintPage += (sender, e) => PrintPageHandler(sender, e, conteudo, separacao);

        // Inicie o processo de impressão
        printDocument.Print();
    }

    public static void PrintPageHandler(object sender, PrintPageEventArgs e, List<ClsImpressaoDefinicoesCMM> conteudo, int separacao)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                bool DestacaObs = db.parametrosdosistema.FirstOrDefault().DestacarObs;


                // Define o conteúdo a ser impresso

                int Y = 0;


                foreach (var item in conteudo)
                {
                    var tamanhoFrase = e.Graphics.MeasureString(item.Texto, item.Fonte).Width;

                    if (tamanhoFrase < e.PageBounds.Width)
                    {
                        if (item.Alinhamento == AlinhamentosCCM.Centro)
                        {
                            e.Graphics.DrawString(item.Texto, item.Fonte, Brushes.Black, Centro(item.Texto, item.Fonte, e), Y);
                        }
                        else if (!item.eObs || !DestacaObs)
                        {
                            e.Graphics.DrawString(item.Texto, item.Fonte, Brushes.Black, 0, Y);
                            Y += separacao;
                            continue;
                        }
                        else if (item.eObs && DestacaObs)
                        {
                            PointF ponto = new PointF(0, Y);

                            SizeF tamanhoTexto = e.Graphics.MeasureString(item.Texto, item.Fonte);
                            RectangleF retanguloTexto = new RectangleF(ponto, new SizeF(e.PageBounds.Width, tamanhoTexto.Height));

                            e.Graphics.FillRectangle(Brushes.LightSlateGray, retanguloTexto);
                            e.Graphics.DrawString(item.Texto, item.Fonte, Brushes.Black, 0, Y);

                            Y += separacao;

                            continue;
                        }
                    }


                    var listPalavras = item.Texto.Split(" ").ToList();
                    string frase = "";

                    foreach (var palavra in listPalavras)
                    {

                        frase += palavra + " ";

                        tamanhoFrase = e.Graphics.MeasureString(frase, item.Fonte).Width;

                        if (tamanhoFrase > e.PageBounds.Width - 70 && frase != "")
                        {
                            if (item.Alinhamento == AlinhamentosCCM.Centro)
                            {

                                e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, Centro(item.Texto, item.Fonte, e), Y);
                                Y += separacao;
                                frase = "";
                                continue;

                            }
                            else if (!item.eObs || !DestacaObs)
                            {
                                e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, 0, Y);
                                Y += separacao;
                                frase = "";
                                continue;
                            }
                            else if (item.eObs && DestacaObs)
                            {
                                PointF ponto = new PointF(0, Y);

                                SizeF tamanhoTexto = e.Graphics.MeasureString(frase, item.Fonte);
                                RectangleF retanguloTexto = new RectangleF(ponto, new SizeF(e.PageBounds.Width, tamanhoTexto.Height));

                                e.Graphics.FillRectangle(Brushes.LightSlateGray, retanguloTexto);
                                e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, 0, Y);

                                Y += separacao;
                                frase = "";

                                continue;
                            }

                        }

                        if (frase != "")
                        {
                            if (item.Alinhamento == AlinhamentosCCM.Centro)
                            {

                                e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, Centro(item.Texto, item.Fonte, e), Y);

                            }
                            else if (!item.eObs || !DestacaObs)
                            {
                                e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, 0, Y);

                            }
                            else if (item.eObs && DestacaObs)
                            {
                                PointF ponto = new PointF(0, Y);

                                SizeF tamanhoTexto = e.Graphics.MeasureString(frase, item.Fonte);
                                RectangleF retanguloTexto = new RectangleF(ponto, new SizeF(e.PageBounds.Width, tamanhoTexto.Height));


                                e.Graphics.FillRectangle(Brushes.LightSlateGray, retanguloTexto);
                                e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, 0, Y);

                                //continue;
                            }

                        }

                    }

                    frase = "";
                    Y += separacao;
                }

            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        conteudo.Clear();
    }

    public static float Centro(string Texto, Font Fonte, System.Drawing.Printing.PrintPageEventArgs e)
    {
        SizeF Tamanho = e.Graphics.MeasureString(Texto, Fonte);

        float Meio = e.PageBounds.Width / 2 - Tamanho.Width / 2;

        return Meio;
    }


    public static void DefineImpressao2(int numConta, int displayId, string impressora1) //impressão caixa
    {
        try
        {
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
                Pedido? pedidoCompleto = JsonConvert.DeserializeObject<Pedido>(pedidoPSQL.Json);
                ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.FirstOrDefault();

                string? TipoPedido = pedidoCompleto.Retira == 1 ? "TAKEOUT" : "DELIVERY";
                string? defineEntrega = TipoPedido == "TAKEOUT" ? "R E T I R A D A" : "E N T R E G A";

                string NumContaString = numConta.ToString();


                AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo($"{defineEntrega}", FonteItens);
                AdicionaConteudo($"{opcDoSistema.NomeFantasia}", FonteItens);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo($"Pedido:                                      #{pedidoCompleto.NroPedido}", FonteGeral);

                DateTime DataCertaCriadoEmTimeStamp = DateTime.Parse(pedidoCompleto.DataHoraPedido);
                var DataCertaCriadoEm = DataCertaCriadoEmTimeStamp.ToString();

                AdicionaConteudo($"Realizado: \t {DataCertaCriadoEm.Substring(0, 10)} {DataCertaCriadoEm.Substring(11, 5)}", FonteGeral);

                if (TipoPedido == "TAKEOUT")
                {
                    if (pedidoCompleto.Retira == 1)
                    {
                        DateTime DataCertaTerminarEmTimeStamp = DateTime.Parse(pedidoCompleto.DataHoraPedido).AddMinutes(dbContext.parametrosdosistema.FirstOrDefault().TempoRetirada);
                        var DataCertaTerminarEm = DataCertaTerminarEmTimeStamp;

                        AdicionaConteudo($"Terminar Até: \t {DataCertaTerminarEm.ToString().Substring(0, 10)} {DataCertaTerminarEm.ToString().Substring(11, 5)}", FonteGeral);
                    }
                    else
                    {
                        DateTime DataCertaTerminarEmTimeStamp = DateTime.Parse(pedidoCompleto.EntregarAte);
                        var DataCertaTerminarEm = DataCertaTerminarEmTimeStamp;

                        AdicionaConteudo($"Terminar Até: \t {DataCertaTerminarEm.ToString().Substring(0, 10)} {DataCertaTerminarEm.ToString().Substring(11, 5)}", FonteGeral);
                    }

                }
                else
                {
                    if (pedidoCompleto.Retira == 1)
                    {
                        DateTime DataCertaEntregarEmTimeStamp = DateTime.Parse(pedidoCompleto.DataHoraAgendamento);
                        var DataCertaTerminarEm = DataCertaEntregarEmTimeStamp;

                        AdicionaConteudo($"Entregar Até: \t {DataCertaTerminarEm.ToString().Substring(0, 10)} {DataCertaTerminarEm.ToString().Substring(11, 5)}", FonteGeral);
                    }
                    else
                    {
                        DateTime DataCertaEntregarEmTimeStamp = DateTime.Parse(pedidoCompleto.EntregarAte);
                        var DataCertaEntregarEm = DataCertaEntregarEmTimeStamp;

                        AdicionaConteudo($"Entregar Até: \t {DataCertaEntregarEm.ToString().Substring(0, 10)} {DataCertaEntregarEm.ToString().Substring(11, 5)}", FonteGeral);

                    }
                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo($"Conta Nº:     {NumContaString.PadLeft(3, '0')}\n", FonteNúmeroDoPedido);

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo(pedidoCompleto.Cliente.Nome, FonteItens);
                AdicionaConteudo($"Fone: {pedidoCompleto.Cliente.Telefone}", FonteNúmeroDoTelefone);

                if (TipoPedido == "DELIVERY")
                {
                    AdicionaConteudo("Endereço de entrega:", FonteItens);
                    AdicionaConteudo($"{pedidoCompleto.Endereco.Rua}, {pedidoCompleto.Endereco.Numero} - {pedidoCompleto.Endereco.Bairro}", FonteEndereçoDoCliente);


                    if (pedidoCompleto.Endereco.Complemento != null && pedidoCompleto.Endereco.Complemento.Length >= 1)
                    {
                        AdicionaConteudo($"Complemento: {pedidoCompleto.Endereco.Complemento}", FonteItens);
                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                }
                else if (TipoPedido == "TAKEOUT")
                {
                    AdicionaConteudo("RETIRADA NO BALCÃO", FonteItens);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                }
                else if (TipoPedido == "INDOOR")
                {
                    //AdicionaConteudo($"Entregar para {pedidoCompleto.Return.Indoor.Place}", FonteItens);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                }

                float valorDosItens = 0f;

                foreach (var item in pedidoCompleto.Itens)
                {
                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemCCM(item);


                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto} {item.ValorUnit.ToString("c")}\n\n", FonteItens);


                    if (item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B")
                    {
                        if (item.CodPdvGrupo == "G")
                        {
                            AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "M")
                        {
                            AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "P")
                        {
                            AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "B")
                        {
                            AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                        }

                    }

                    if (!opcDoSistema.RemoveComplementos)
                    {
                        if (CaracteristicasPedido.Observações.Count > 0)
                        {
                            foreach (var option in CaracteristicasPedido.Observações)
                            {
                                AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                            }
                        }

                        if (item.ObsItem != null && item.ObsItem.Length > 0)
                        {
                            AdicionaConteudo($"Obs: {item.ObsItem}", FonteCPF, eObs: true);
                        }


                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                }

                float ValorEntrega = 0.0f;
                float ValorDeDescontos = 0.0f;

                ValorDeDescontos = pedidoCompleto.ValorCupom + pedidoCompleto.CreditoUtilizado;

                AdicionaConteudo($"Valor dos itens: \t                  {pedidoCompleto.ValorBruto.ToString("c")} ", FonteGeral);

                if (pedidoCompleto.ValorTaxa > 0)
                    AdicionaConteudo($"Taxa De Entrega: \t   {pedidoCompleto.ValorTaxa.ToString("c")}", FonteGeral);

                //AdicionaConteudo($"Taxa Adicional:  \t                  {(0.0).ToString("c")} ", FonteGeral);
                if (ValorDeDescontos > 0)
                    AdicionaConteudo($"Descontos:      \t                  {pedidoCompleto.CreditoUtilizado.ToString("c")}", FonteGeral);

                AdicionaConteudo($"Valor Total:   \t                  {pedidoCompleto.ValorTotal.ToString("c")}", FonteGeral);
                valorDosItens = 0f;
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


                if (TipoPedido == "DELIVERY")
                {
                    if (pedidoCompleto.Endereco.Referencia != null && pedidoCompleto.Endereco.Referencia.Length > 0)
                    {
                        AdicionaConteudo($"{pedidoCompleto.Endereco.Referencia}", FonteObservaçõesItem);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                }

                if (!String.IsNullOrEmpty(pedidoCompleto.ObsGeraisPedido))
                {
                    AdicionaConteudo($"{pedidoCompleto.ObsGeraisPedido}", FonteObservaçõesItem, eObs: true);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                }


                var InfoPag = ClsInfosDePagamentosParaImpressaoCCM.DefineTipoDePagamento(pedidoCompleto.PagamentoOnline, pedidoCompleto.DescricaoPagamento, pedidoCompleto.ValorTotal, pedidoCompleto.TrocoPara);

                var Info1 = $"{InfoPag.FormaPagamento} ({InfoPag.TipoPagamento})";

                AdicionaConteudo(Info1, FonteGeral);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo("Impresso por:", FonteGeral);
                AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);
                AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosCCM.Centro);



                Imprimir(Conteudo, impressora1, 16);
                Conteudo.Clear();

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public static async void ImprimeComandaReduzida(int numConta, int displayId, string impressora1)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ParametrosDoPedido? pedidoPSQL = db.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
                Pedido? pedidoCompleto = JsonConvert.DeserializeObject<Pedido>(pedidoPSQL.Json);
                ParametrosDoSistema? opcDoSistema = db.parametrosdosistema.FirstOrDefault();

                bool eMesa = pedidoCompleto.NumeroMesa > 0 ? true : false;
                string? TipoPedido = pedidoCompleto.Retira == 1 ? "TAKEOUT" : "DELIVERY";
                string? defineEntrega = TipoPedido == "TAKEOUT" ? "Retirada" : "Entrega";

                string NumContaString = numConta.ToString();
                AdicionaConteudo($"Pedido:   #{pedidoCompleto.NroPedido}", FonteNúmeroDoPedido);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                if (eMesa)
                {
                    AdicionaConteudo($"MESA: \t\t {pedidoCompleto.NumeroMesa}\n", FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                }
                else
                {

                    AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (opcDoSistema.UsarNomeNaComanda)
                    {
                        AdicionaConteudo($"Cliente: {pedidoCompleto.Cliente.Nome}", FonteItens);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }
                }

                foreach (var item in pedidoCompleto.Itens)
                {
                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemCCM(item, true);


                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);


                    if (item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B")
                    {
                        if (item.CodPdvGrupo == "G")
                        {
                            AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "M")
                        {
                            AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "P")
                        {
                            AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "B")
                        {
                            AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                        }

                    }

                    if (item.Adicionais != null || item.Adicionais.Count > 0)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.ObsItem != null && item.ObsItem.Length > 0)
                        {
                            if (CaracteristicasPedido.ObsDoItem != item.ObsItem)
                            {
                                AdicionaConteudo($"Obs: {item.ObsItem}", FonteCPF, eObs: true);
                            }
                        }

                        if (CaracteristicasPedido.ObsDoItem != " ")
                        {
                            AdicionaConteudo($"Obs: {CaracteristicasPedido.ObsDoItem}", FonteCPF, eObs: true);
                        }

                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                }

                AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);
                AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosCCM.Centro);


                Imprimir(Conteudo, impressora1, 18);
                Conteudo.Clear();
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.Message);
        }
    }


    public static void ImprimeComanda(int numConta, int displayId, string impressora1) //comanda
    {
        try
        {
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                ParametrosDoPedido? pedidoPSQL = db.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
                Pedido? pedidoCompleto = JsonConvert.DeserializeObject<Pedido>(pedidoPSQL.Json);
                ParametrosDoSistema? opcDoSistema = db.parametrosdosistema.FirstOrDefault();

                string NumContaString = numConta.ToString();

                string? TipoPedido = pedidoCompleto.Retira == 1 ? "TAKEOUT" : "DELIVERY";
                string? defineEntrega = TipoPedido == "TAKEOUT" ? "Retirada" : "Entrega";


                AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);

                AdicionaConteudo($"Pedido:   #{pedidoCompleto.NroPedido}", FonteNúmeroDoPedido);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                if (TipoPedido == "INDOOR")
                {
                    //AdicionaConteudo($"{pedidoCompleto.Return.Indoor.Place}\n", FonteNomeDoCliente);
                }
                else
                {
                    AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);

                    if (opcDoSistema.UsarNomeNaComanda)
                    {
                        AdicionaConteudo($"Cliente: {pedidoCompleto.Cliente.Nome}", FonteItens);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                }

                int qtdItens = pedidoCompleto.Itens.Count();
                int contagemItemAtual = 1;

                foreach (var item in pedidoCompleto.Itens)
                {
                    AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemCCM(item, true);

                    if (item.CodPdvGrupo == "BB" || item.CodPdvGrupo == "LAN" || item.CodPdvGrupo == "PRC")
                    {
                        AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                    }
                    else
                    {
                        AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                    }

                    if (item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B")
                    {
                        if (item.CodPdvGrupo == "G")
                        {
                            AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "M")
                        {
                            AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "P")
                        {
                            AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "B")
                        {
                            AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                        }

                    }

                    if (item.Adicionais != null || item.Adicionais.Count > 0)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.ObsItem != null && item.ObsItem.Length > 0)
                        {
                            if (CaracteristicasPedido.ObsDoItem != item.ObsItem)
                            {
                                AdicionaConteudo($"Obs: {item.ObsItem}", FonteCPF, eObs: true);
                            }
                        }

                        if (CaracteristicasPedido.ObsDoItem != " ")
                        {
                            AdicionaConteudo($"Obs: {CaracteristicasPedido.ObsDoItem}", FonteCPF, eObs: true);
                        }

                    }
                    contagemItemAtual++;
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                }
                contagemItemAtual = 0;

                AdicionaConteudo("Impresso por:", FonteGeral);
                AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);
                AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosCCM.Centro);


                Imprimir(Conteudo, impressora1, 24);
                Conteudo.Clear();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static void ImprimeComandaTipo2(int numConta, int displayId, string impressora1) //comanda
    {

        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                ParametrosDoPedido? pedidoPSQL = db.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
                Pedido? pedidoCompleto = JsonConvert.DeserializeObject<Pedido>(pedidoPSQL.Json);
                ParametrosDoSistema? opcDoSistema = db.parametrosdosistema.FirstOrDefault();

                string NumContaString = numConta.ToString();

                string? TipoPedido = pedidoCompleto.Retira == 1 ? "TAKEOUT" : "DELIVERY";
                string? defineEntrega = TipoPedido == "TAKEOUT" ? "Retirada" : "Entrega";

                int contagemItemAtual = 1;

                int qtdItens = 0;

                foreach (var item in pedidoCompleto.Itens)
                {
                    qtdItens += 1 * item.Quantidade;
                }

                //nome do restaurante estatico por enquanto
                foreach (var item in pedidoCompleto.Itens)
                {
                    for (var i = 0; i < item.Quantidade; i++)
                    {

                        AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);

                        AdicionaConteudo($"Pedido:          #{pedidoCompleto.NroPedido}", FonteNúmeroDoPedido);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                        if (TipoPedido == "INDOOR")
                        {
                            //AdicionaConteudo($"{pedidoCompleto.Return.Indoor.Place}\n", FonteNomeDoCliente);
                        }
                        else
                        {
                            AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);

                            if (opcDoSistema.UsarNomeNaComanda)
                            {
                                AdicionaConteudo($"Cliente: {pedidoCompleto.Cliente.Nome}", FonteItens);
                                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                            }

                        }

                        AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                        ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemCCM(item, true);

                        if (item.Quantidade > 1)
                        {
                            AdicionaConteudo($"1X {item.NomeItem}\n\n", FonteItens);
                        }
                        else
                        {
                            if (item.CodPdvGrupo == "BB" || item.CodPdvGrupo == "LAN")
                            {
                                AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                            }
                            else
                            {
                                AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                            }
                        }

                        if (item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B")
                        {
                            if (item.CodPdvGrupo == "G")
                            {
                                AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                            }

                            if (item.CodPdvGrupo == "M")
                            {
                                AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                            }

                            if (item.CodPdvGrupo == "P")
                            {
                                AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                            }

                            if (item.CodPdvGrupo == "B")
                            {
                                AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                            }

                        }


                        if (item.Adicionais != null || item.Adicionais.Count > 0)
                        {
                            foreach (var option in CaracteristicasPedido.Observações)
                            {
                                AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                            }

                            if (item.ObsItem != null && item.ObsItem.Length > 0)
                            {
                                if (CaracteristicasPedido.ObsDoItem != item.ObsItem)
                                {
                                    AdicionaConteudo($"Obs: {item.ObsItem}", FonteCPF, eObs: true);
                                }
                            }

                            if (CaracteristicasPedido.ObsDoItem != " ")
                            {
                                AdicionaConteudo($"Obs: {CaracteristicasPedido.ObsDoItem}", FonteCPF, eObs: true);
                            }

                        }
                        contagemItemAtual++;
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                        AdicionaConteudo("Impresso por:", FonteGeral);
                        AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                        AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);
                        AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosCCM.Centro);

                        Imprimir(Conteudo, impressora1, 24);
                        Conteudo.Clear();


                    }
                }

                contagemItemAtual = 1;
                qtdItens = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static void SeparaItensParaImpressaoSeparada(int numConta, int displayId)
    {
        try
        {
            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM> ListaDeItems = new List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM>() { new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM() { Impressora1 = "Cz1" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM() { Impressora1 = "Cz2" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM() { Impressora1 = "Cz3" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM() { Impressora1 = "Cz4" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM() { Impressora1 = "Sem Impressora" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM() { Impressora1 = "Bar" } };


            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            Pedido? pedidoCompleto = JsonConvert.DeserializeObject<Pedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            string? TipoPedido = pedidoCompleto.Retira == 1 ? "TAKEOUT" : "DELIVERY";
            string? defineEntrega = TipoPedido == "TAKEOUT" ? "Retirada" : "Entrega Propria";

            foreach (var item in pedidoCompleto.Itens)
            {
                bool ePizza = item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B" || item.CodPdvGrupo == "BB" || item.CodPdvGrupo == "LAN" ? true : false;
                string externalCode = item.CodPdvGrupo;
                int tamanhoPartesDaPizza = item.Parte.Count();

                if (ePizza && tamanhoPartesDaPizza > 0)
                {
                    foreach (var option in item.Parte)
                    {
                        if (!option.CodPdvItem.Contains("m"))
                        {
                            List<string> LocalDeImpressaoDasPizza = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(option.CodPdvItem);

                            if (LocalDeImpressaoDasPizza.Count > 1)
                            {
                                List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM> GruposDoItemPizza = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressaoDasPizza[0] || x.Impressora1 == LocalDeImpressaoDasPizza[1]).ToList();

                                if (LocalDeImpressaoDasPizza.Count() > 1)
                                {
                                    foreach (var grupo in GruposDoItemPizza)
                                    {
                                        var verifSejaExisteAPizzaDEntroDosItens = grupo.Itens.Any(x => x == item);

                                        if (!verifSejaExisteAPizzaDEntroDosItens)
                                        {
                                            grupo.Itens.Add(item);
                                        }
                                    }
                                }
                                else
                                {
                                    var GrupoDoItem = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressaoDasPizza[0] || x.Impressora1 == LocalDeImpressaoDasPizza[1]).FirstOrDefault();

                                    if (GrupoDoItem != null)
                                    {
                                        var verifSejaExisteAPizzaDEntroDosItens = GrupoDoItem.Itens.Any(x => x == item);

                                        if (!verifSejaExisteAPizzaDEntroDosItens)
                                        {
                                            GrupoDoItem.Itens.Add(item);
                                        }

                                    }
                                }
                            }
                        }
                    }

                    continue;
                }

                //-------------------------------------------------------------------------------------------------------------------------------//
                List<string> LocalDeImpressao = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(externalCode);

                if (LocalDeImpressao.Count > 1)
                {
                    var GruposDoItem = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressao[0] || x.Impressora1 == LocalDeImpressao[1]).ToList();

                    if (GruposDoItem.Count() > 1)
                    {
                        foreach (var grupo in GruposDoItem)
                        {
                            grupo.Itens.Add(item);
                        }
                    }
                    else
                    {
                        var GrupoDoItem = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressao[0] || x.Impressora1 == LocalDeImpressao[1]).FirstOrDefault();

                        if (GrupoDoItem != null)
                        {
                            GrupoDoItem.Itens.Add(item);
                        }
                    }
                }

            }

            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM> ListaLimpa = ListaDeItems.Where(x => x.Itens.Count > 0).ToList();

            foreach (var item in ListaLimpa)
            {
                item.Impressora1 = DefineNomeDeImpressoraCasoEstejaSelecionadoImpSeparada(item.Impressora1);
                if (opcSistema.TipoComanda == 2)
                {
                    ImprimeComandaSeparadaTipo2(item.Impressora1, displayId, item.Itens, numConta);
                }
                else
                {
                    ImprimeComandaSeparada(item.Impressora1, displayId, item.Itens, numConta);
                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }


    public static void ImprimeComandaSeparada(string impressora, int displayId, List<SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Item> itens, int numConta)
    {
        try
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            Pedido? pedidoCompleto = JsonConvert.DeserializeObject<Pedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();
            string NumContaString = numConta.ToString();

            //List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> itemsSeparadosPorImpressao = SeparaItensParaImpressaoSeparada();
            //string? defineEntrega = pedidoCompleto.delivery.deliveredBy == null ? "Retirada" : "Entrega Propria";

            //nome do restaurante estatico por enquanto


            AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);

            AdicionaConteudo($"Pedido: \t#{pedidoCompleto.NroPedido}", FonteNúmeroDoPedido); // aqui seria o display id Arrumar
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo($"Entrega: \t  Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            int qtdItens = pedidoCompleto.Itens.Count();
            int contagemItemAtual = 1;


            foreach (var item in itens)
            {

                if (impressora == "Sem Impressora" || impressora == "" || impressora == null)
                {
                    throw new Exception("Uma das impressora não foi encontrada adicione ela nas configurações ou retire a impressão separada!");
                }


                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemCCM(item, true);

                AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);

                if (item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B")
                {
                    if (item.CodPdvGrupo == "G")
                    {
                        AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                    }

                    if (item.CodPdvGrupo == "M")
                    {
                        AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                    }

                    if (item.CodPdvGrupo == "P")
                    {
                        AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                    }

                    if (item.CodPdvGrupo == "B")
                    {
                        AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                    }

                }

                if (item.CodPdvGrupo == "BB" || item.CodPdvGrupo == "LAN")
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }
                else
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }

                if (item.Adicionais != null || item.Adicionais.Count > 0)
                {
                    foreach (var option in CaracteristicasPedido.Observações)
                    {
                        AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.ObsItem != null && item.ObsItem.Length > 0)
                    {
                        if (CaracteristicasPedido.ObsDoItem != item.ObsItem)
                        {
                            AdicionaConteudo($"Obs: {item.ObsItem}", FonteCPF, eObs: true);
                        }
                    }

                    if (CaracteristicasPedido.ObsDoItem != " ")
                    {
                        AdicionaConteudo($"Obs: {CaracteristicasPedido.ObsDoItem}", FonteCPF, eObs: true);
                    }

                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            contagemItemAtual = 0;

            AdicionaConteudo("Impresso por:", FonteGeral);
            AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);
            AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosCCM.Centro);

            if (impressora != "Nao")
            {
                Imprimir(Conteudo, impressora, 24);
            }

            //Imprimir(Conteudo, impressora);
            Conteudo.Clear();

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static void ImprimeComandaSeparadaTipo2(string impressora, int displayId, List<SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Item> itens, int numConta)
    {
        try
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            Pedido? pedidoCompleto = JsonConvert.DeserializeObject<Pedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();
            string NumContaString = numConta.ToString();

            //List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> itemsSeparadosPorImpressao = SeparaItensParaImpressaoSeparada();
            //string? defineEntrega = pedidoCompleto.delivery.deliveredBy == null ? "Retirada" : "Entrega Propria";
            int contagemItemAtual = 1;

            //nome do restaurante estatico por enquanto
            foreach (var item in itens)
            {
                int quantidadeDoItem = Convert.ToInt32(item.Quantidade);

                for (int i = 0; i < quantidadeDoItem; i++)
                {

                    AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);

                    AdicionaConteudo($"Pedido: \t#{pedidoCompleto.NroPedido}", FonteNúmeroDoPedido); // aqui seria o display id Arrumar
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Entrega: \t  Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    int qtdItens = pedidoCompleto.Itens.Count();

                    if (impressora == "Sem Impressora" || impressora == "" || impressora == null)
                    {
                        throw new Exception("Uma das impressora não foi encontrada adicione ela nas configurações ou retire a impressão separada!");
                    }


                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemCCM(item, true);

                    AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);

                    if (item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B")
                    {
                        if (item.CodPdvGrupo == "G")
                        {
                            AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "M")
                        {
                            AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "P")
                        {
                            AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                        }

                        if (item.CodPdvGrupo == "B")
                        {
                            AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                        }

                    }

                    if (item.CodPdvGrupo == "BB" || item.CodPdvGrupo == "LAN")
                    {
                        AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                    }
                    else
                    {
                        AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                    }

                    if (item.Adicionais != null || item.Adicionais.Count > 0)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.ObsItem != null && item.ObsItem.Length > 0)
                        {
                            if (CaracteristicasPedido.ObsDoItem != item.ObsItem)
                            {
                                AdicionaConteudo($"Obs: {item.ObsItem}", FonteCPF, eObs: true);
                            }
                        }

                        if (CaracteristicasPedido.ObsDoItem != " ")
                        {
                            AdicionaConteudo($"Obs: {CaracteristicasPedido.ObsDoItem}", FonteCPF, eObs: true);
                        }

                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("CCM", FonteNomeDoCliente, AlinhamentosCCM.Centro);
                    AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosCCM.Centro);

                    if (impressora != "Nao")
                    {
                        Imprimir(Conteudo, impressora, 24);
                    }
                    contagemItemAtual++;
                }

            }
            //Imprimir(Conteudo, impressora);
            Conteudo.Clear();
            contagemItemAtual = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }


    public static void ChamaImpressoesCasoSejaComandaSeparada(int numConta, int displayId, List<string> impressoras)
    {
        ApplicationDbContext db = new ApplicationDbContext();
        ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();

        if (opcSistema.ImpCompacta)
        {
            DefineImpressao2(numConta, displayId, opcSistema.Impressora1);
        }
        else
        {
            DefineImpressao2(numConta, displayId, opcSistema.Impressora1);
        }

        SeparaItensParaImpressaoSeparada(numConta, displayId);
    }

    public static void ChamaImpressoes(int numConta, int displayId, string? impressora)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
            int ContagemDeImpressoes = 0;

            ParametrosDoPedido? Pedido = db.parametrosdopedido.FirstOrDefault(x => x.DisplayId == displayId);

            if (Pedido is not null)
            {
                Pedido? PedidoCCM = JsonConvert.DeserializeObject<Pedido>(Pedido.Json);

                bool eMesa = PedidoCCM.NumeroMesa > 0 ? true : false;

                if (impressora == opcSistema.Impressora1 || impressora == opcSistema.ImpressoraAux)
                {
                    if (opcSistema.ImpCompacta && !eMesa)
                    {
                        DefineImpressao2(numConta, displayId, impressora);
                    }
                    else if (!eMesa)
                    {
                        DefineImpressao2(numConta, displayId, impressora);
                    }
                    ContagemDeImpressoes++;
                    if (opcSistema.ImprimirComandaNoCaixa)
                    {
                        if (opcSistema.TipoComanda == 2)
                        {
                            for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                            {
                                ImprimeComandaTipo2(numConta, displayId, impressora);
                            }
                        }
                        else
                        {
                            if (opcSistema.ComandaReduzida)
                            {
                                for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                                {
                                    ImprimeComandaReduzida(numConta, displayId, impressora);
                                }

                            }
                            else
                            {
                                for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                                {
                                    ImprimeComanda(numConta, displayId, impressora);
                                }
                            }

                        }
                    }
                }
                if (ContagemDeImpressoes == 0)
                {
                    if (opcSistema.TipoComanda == 2)
                    {
                        for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                        {
                            ImprimeComandaTipo2(numConta, displayId, impressora);
                        }
                    }
                    else
                    {
                        if (opcSistema.ComandaReduzida)
                        {
                            for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                            {
                                ImprimeComandaReduzida(numConta, displayId, impressora);
                            }

                        }
                        else
                        {
                            for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                            {
                                ImprimeComanda(numConta, displayId, impressora);
                            }
                        }
                    }
                }

                ContagemDeImpressoes = 0;
            }
        }
    }


    public static string AdicionarSeparador()
    {
        return "───────────────────────────";
    }

    public static void AdicionaConteudo(string conteudo, Font fonte, AlinhamentosCCM alinhamento = AlinhamentosCCM.Esquerda, bool eObs = false)
    {
        Conteudo.Add(new ClsImpressaoDefinicoesCMM() { Texto = conteudo, Fonte = fonte, Alinhamento = alinhamento, eObs = eObs });
    }

    public static void AdicionaConteudoParaImpSeparada(string impressora, string conteudo, Font fonte, AlinhamentosCCM alinhamento = AlinhamentosCCM.Esquerda)
    {
        //ConteudoParaImpSeparada.Add(new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas() { Impressora = impressora, conteudo = new ClsImpressaoDefinicoes() { Texto = conteudo, Fonte = fonte, Alinhamento = alinhamento } });
    }

    public static string DefineNomeDeImpressoraCasoEstejaSelecionadoImpSeparada(string LocalImpressao)
    {
        string NomeImpressora = "";
        try
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            switch (LocalImpressao)
            {
                case "Cz1":
                    NomeImpressora = opcSistema.Impressora2;
                    break;
                case "Cz2":
                    NomeImpressora = opcSistema.Impressora3;
                    break;
                case "Cz3":
                    NomeImpressora = opcSistema.Impressora4;
                    break;
                case "Bar":
                    NomeImpressora = opcSistema.Impressora5;
                    break;
                case "Nao":
                    NomeImpressora = "Nao";
                    break;
                default:
                    NomeImpressora = "Sem Impressora";
                    break;
            }

        }
        catch (Exception ex)
        {

            MessageBox.Show("Erro ao Definir nome da impresora para impressão");
        }


        return NomeImpressora;
    }
}
