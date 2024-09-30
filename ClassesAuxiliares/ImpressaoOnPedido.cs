using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Windows.Forms.LinkLabel;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ImpressaoONPedido
{
    public static int NumContas { get; set; }
    public static List<ClsImpressaoDefinicoesOnPedido>? Conteudo { get; set; } = new List<ClsImpressaoDefinicoesOnPedido>();
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

    public enum AlinhamentosOnPedido
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

    public static void Imprimir(List<ClsImpressaoDefinicoesOnPedido> conteudo, string impressora1, int separacao)
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

    public static void PrintPageHandler(object sender, PrintPageEventArgs e, List<ClsImpressaoDefinicoesOnPedido> conteudo, int separacao)
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
                        if (item.Alinhamento == AlinhamentosOnPedido.Centro)
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
                            if (item.Alinhamento == AlinhamentosOnPedido.Centro)
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
                            if (item.Alinhamento == AlinhamentosOnPedido.Centro)
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


    public static void DefineImpressao(int numConta, int displayId, string impressora1) //impressão caixa
    {
        try
        {
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.Where(x => x.Id == 1).FirstOrDefault();



            string banco = opcDoSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();
                string? defineEntrega = pedidoCompleto.Return.Type == "TAKEOUT" ? "Retirada" : "Entrega Propria";

                string NumContaString = numConta.ToString();

                using (OleDbCommand comando = new OleDbCommand(sqlQuery, connection))
                using (OleDbDataReader reader = comando.ExecuteReader())
                {

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);

                    AdicionaConteudo($"{opcDoSistema.NomeFantasia}", FonteNomeRestaurante, AlinhamentosOnPedido.Centro);
                    AdicionaConteudo($"{opcDoSistema.Endereco}", FonteGeral);
                    AdicionaConteudo($"{opcDoSistema.Telefone}", FonteGeral, AlinhamentosOnPedido.Centro);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Pedido:  #{pedidoCompleto.Return.Id}", FonteNúmeroDoPedido);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.Return.Type == "INDOOR")
                    {
                        AdicionaConteudo($"{pedidoCompleto.Return.Indoor.Place}\n", FonteNomeDoCliente);
                    }
                    else
                    {
                        AdicionaConteudo($"Entrega: \t  Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);

                    }

                    AdicionaConteudo($"{defineEntrega}\n", FonteGeral);


                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Origem: \t            ONPEDIDO", FonteGeral);
                    AdicionaConteudo("Atendente: \t       SysIntegrador", FonteGeral);


                    DateTime DataCertaCriadoEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.CreatedAt);
                    var DataCertaCriadoEm = DataCertaCriadoEmTimeStamp.ToString();

                    AdicionaConteudo($"Realizado: \t {DataCertaCriadoEm.Substring(0, 10)} {DataCertaCriadoEm.Substring(11, 5)}", FonteGeral);

                    if (defineEntrega == "Retirada")
                    {

                        DateTime DataCertaTerminarEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.TakeOut.TakeoutDateTime);
                        var DataCertaTerminarEm = DataCertaTerminarEmTimeStamp;

                        AdicionaConteudo($"Terminar Até: \t {DataCertaTerminarEm.ToString().Substring(0, 10)} {DataCertaTerminarEm.ToString().Substring(11, 5)}", FonteGeral);
                    }
                    else
                    {
                        DateTime DataCertaEntregarEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.Delivery.DeliveryDateTime);
                        var DataCertaEntregarEm = DataCertaEntregarEmTimeStamp.AddMinutes(50);

                        AdicionaConteudo($"Entregar Até: \t {DataCertaEntregarEm.ToString().Substring(0, 10)} {DataCertaEntregarEm.ToString().Substring(11, 5)}", FonteGeral);
                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Fone: ({pedidoCompleto.Return.Customer.PhoneOn.Extension}) {pedidoCompleto.Return.Customer.PhoneOn.Number}", FonteNúmeroDoTelefone);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo(pedidoCompleto.Return.Customer.Name, FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.Return.Type == "DELIVERY")
                    {
                        AdicionaConteudo("Endereço de entrega:", FonteCPF);
                        AdicionaConteudo($"{pedidoCompleto.Return.Delivery.DeliveryAddressON.Street}, {pedidoCompleto.Return.Delivery.DeliveryAddressON.Number} - {pedidoCompleto.Return.Delivery.DeliveryAddressON.District}", FonteEndereçoDoCliente);


                        if (pedidoCompleto.Return.Delivery.DeliveryAddressON.Complement != null && pedidoCompleto.Return.Delivery.DeliveryAddressON.Complement.Length >= 1)
                        {
                            AdicionaConteudo($"Complemento: {pedidoCompleto.Return.Delivery.DeliveryAddressON.Complement}", FonteEndereçoDoCliente);
                        }

                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }
                    else if (pedidoCompleto.Return.Type == "TAKEOUT")
                    {
                        AdicionaConteudo("RETIRADA NO BALCÃO", FonteEndereçoDoCliente);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }
                    else if (pedidoCompleto.Return.Type == "INDOOR")
                    {
                        AdicionaConteudo($"Entregar para {pedidoCompleto.Return.Indoor.Place}", FonteEndereçoDoCliente);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    float valorDosItens = 0f;

                    foreach (var item in pedidoCompleto.Return.ItemsOn)
                    {
                        ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item);

                        if (item.externalCode == "BB" || item.externalCode == "LAN")
                        {
                            AdicionaConteudo($"{CaracteristicasPedido.NomeProduto} {item.TotalPrice.Value.ToString("c")}\n\n", FonteItens);
                        }
                        else
                        {
                            AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto} {item.TotalPrice.Value.ToString("c")}\n\n", FonteItens);
                        }

                        if (item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B")
                        {
                            if (item.externalCode == "G")
                            {
                                AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                            }

                            if (item.externalCode == "M")
                            {
                                AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                            }

                            if (item.externalCode == "P")
                            {
                                AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                            }

                            if (item.externalCode == "B")
                            {
                                AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                            }

                        }

                        if (!opcDoSistema.RemoveComplementos)
                        {
                            if (item.Options.Count > 0)
                            {
                                foreach (var option in CaracteristicasPedido.Observações)
                                {
                                    AdicionaConteudo($"{option}", FonteDetalhesDoPedido);
                                }

                                if (item.observations != null && item.observations.Length > 0)
                                {
                                    AdicionaConteudo($"Obs: {item.observations}", FonteCPF);
                                }
                            }
                        }

                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    float ValorEntrega = 0.0f;

                    var EntregaObj = pedidoCompleto.Return.OtherFees.Where(x => x.Type == "DELIVERY_FEE").FirstOrDefault();
                    ValorEntrega = EntregaObj.Price.Value;

                    float ValorTaxasAdicionais = 0.0f;

                    foreach (var item in pedidoCompleto.Return.OtherFees)
                    {
                        if (item.Type != "DELIVERY_FEE")
                        {
                            ValorTaxasAdicionais += item.Price.Value;
                        }
                    }

                    float ValorDescontosNum = 0.0f;

                    foreach (var item in pedidoCompleto.Return.Discounts)
                    {
                        ValorDescontosNum += item.Amount.value;
                    }


                    AdicionaConteudo($"Valor dos itens: \t   {pedidoCompleto.Return.Total.ItemsPrice.value.ToString("c")} ", FonteTotaisDoPedido);
                    AdicionaConteudo($"Taxa De Entrega: \t   {ValorEntrega.ToString("c")}", FonteTotaisDoPedido);
                    AdicionaConteudo($"Taxa Adicional:  \t   {ValorTaxasAdicionais.ToString("c")} ", FonteTotaisDoPedido);
                    AdicionaConteudo($"Descontos:      \t   {ValorDescontosNum.ToString("c")}", FonteTotaisDoPedido);
                    AdicionaConteudo($"Valor Total:   \t   {pedidoCompleto.Return.Total.OrderAmount.value.ToString("c")}", FonteTotaisDoPedido);
                    valorDosItens = 0f;
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.Return.Type == "DELIVERY")
                    {
                        if (pedidoCompleto.Return.Delivery.DeliveryAddressON.Reference != null && pedidoCompleto.Return.Delivery.DeliveryAddressON.Reference.Length > 0)
                        {
                            AdicionaConteudo($"{pedidoCompleto.Return.Delivery.DeliveryAddressON.Reference}", FonteObservaçõesItem);
                            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                        }

                    }

                    var InfoPag = ClsInfosDePagamentosParaImpressaoONPedido.DefineTipoDePagamento(pedidoCompleto.Return.Payments);

                    var Info1 = $"{InfoPag.FormaPagamento} ({InfoPag.TipoPagamento})";

                    AdicionaConteudo(Info1, FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
                    AdicionaConteudo("www.syslogica.com.br", FonteCPF, AlinhamentosOnPedido.Centro);

                }

                Imprimir(Conteudo, impressora1, 24);
                Conteudo.Clear();
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }

    public static void DefineImpressao2(int numConta, int displayId, string impressora1) //impressão caixa
    {
        try
        {
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.Where(x => x.Id == 1).FirstOrDefault();

            string banco = opcDoSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();
                string? defineEntrega = pedidoCompleto.Return.Type == "TAKEOUT" ? "R E T I R A D A" : "E N T R E G A";

                string NumContaString = numConta.ToString();

                using (OleDbCommand comando = new OleDbCommand(sqlQuery, connection))
                using (OleDbDataReader reader = comando.ExecuteReader())
                {

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"{defineEntrega}", FonteItens);
                    AdicionaConteudo($"{opcDoSistema.NomeFantasia}", FonteItens);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.Return.OrderTiming == "SCHEDULED")
                    {
                        AdicionaConteudo("*** PEDIDO AGENDADO ***", FonteGeral, AlinhamentosOnPedido.Centro);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    AdicionaConteudo($"Pedido:                                     #{pedidoCompleto.Return.Id}", FonteGeral);

                    DateTime DataCertaCriadoEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.CreatedAt);
                    var DataCertaCriadoEm = DataCertaCriadoEmTimeStamp.ToString();

                    AdicionaConteudo($"Realizado: \t {DataCertaCriadoEm.Substring(0, 10)} {DataCertaCriadoEm.Substring(11, 5)}", FonteGeral);

                    if (pedidoCompleto.Return.Type == "TAKEOUT")
                    {
                        if (pedidoCompleto.Return.OrderTiming != "SCHEDULED")
                        {
                            DateTime DataCertaTerminarEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.TakeOut.TakeoutDateTime);
                            var DataCertaTerminarEm = DataCertaTerminarEmTimeStamp;

                            AdicionaConteudo($"Terminar Até: \t {DataCertaTerminarEm.ToString().Substring(0, 10)} {DataCertaTerminarEm.ToString().Substring(11, 5)}", FonteGeral);
                        }
                        else
                        {
                            DateTime DataCertaTerminarEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.Schedule.ScheduledDateTimeEnd);
                            var DataCertaTerminarEm = DataCertaTerminarEmTimeStamp;

                            AdicionaConteudo($"Terminar Até: \t {DataCertaTerminarEm.ToString().Substring(0, 10)} {DataCertaTerminarEm.ToString().Substring(11, 5)}", FonteGeral);
                        }
                    }
                    else
                    {
                        if (pedidoCompleto.Return.OrderTiming != "SCHEDULED")
                        {
                            DateTime DataCertaEntregarEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.Delivery.DeliveryDateTime);
                            var DataCertaEntregarEm = DataCertaEntregarEmTimeStamp;
                            AdicionaConteudo($"Entregar Até: \t {DataCertaEntregarEm.ToString().Substring(0, 10)} {DataCertaEntregarEm.ToString().Substring(11, 5)}", FonteGeral);

                        }
                        else
                        {
                            DateTime DataCertaEntregarEmTimeStamp = DateTime.Parse(pedidoCompleto.Return.Schedule.ScheduledDateTimeEnd);
                            var DataCertaEntregarEm = DataCertaEntregarEmTimeStamp;
                            AdicionaConteudo($"Entregar Até: \t {DataCertaEntregarEm.ToString().Substring(0, 10)} {DataCertaEntregarEm.ToString().Substring(11, 5)}", FonteGeral);
                        }


                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.Return.Type == "INDOOR")
                    {
                        AdicionaConteudo($"{pedidoCompleto.Return.Indoor.Place}\n", FonteNúmeroDoPedido);

                    }
                    else
                    {
                        AdicionaConteudo($"Conta Nº:     {NumContaString.PadLeft(3, '0')}\n", FonteNúmeroDoPedido);
                    }



                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo(pedidoCompleto.Return.Customer.Name, FonteItens);
                    AdicionaConteudo($"Fone: ({pedidoCompleto.Return.Customer.PhoneOn.Extension}) {pedidoCompleto.Return.Customer.PhoneOn.Number}", FonteNúmeroDoTelefone);


                    if (pedidoCompleto.Return.Type == "DELIVERY")
                    {
                        AdicionaConteudo("Endereço de entrega:", FonteItens);
                        AdicionaConteudo($"{pedidoCompleto.Return.Delivery.DeliveryAddressON.Street}, {pedidoCompleto.Return.Delivery.DeliveryAddressON.Number} - {pedidoCompleto.Return.Delivery.DeliveryAddressON.District}", FonteEndereçoDoCliente);


                        if (pedidoCompleto.Return.Delivery.DeliveryAddressON.Complement != null && pedidoCompleto.Return.Delivery.DeliveryAddressON.Complement.Length >= 1)
                        {
                            AdicionaConteudo($"Complemento: {pedidoCompleto.Return.Delivery.DeliveryAddressON.Complement}", FonteItens);
                        }

                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }
                    else if (pedidoCompleto.Return.Type == "TAKEOUT")
                    {
                        AdicionaConteudo("RETIRADA NO BALCÃO", FonteItens);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }
                    else if (pedidoCompleto.Return.Type == "INDOOR")
                    {
                        AdicionaConteudo($"Entregar para {pedidoCompleto.Return.Indoor.Place}", FonteItens);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    float valorDosItens = 0f;

                    foreach (var item in pedidoCompleto.Return.ItemsOn)
                    {
                        ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item);

                        if (item.externalCode == "BB" || item.externalCode == "LAN")
                        {
                            AdicionaConteudo($"{CaracteristicasPedido.NomeProduto} {item.TotalPrice.Value.ToString("c")}\n\n", FonteItens);
                        }
                        else
                        {
                            AdicionaConteudo($"{CaracteristicasPedido.NomeProduto} {item.TotalPrice.Value.ToString("c")}\n\n", FonteItens);
                        }



                        if (item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B")
                        {
                            if (item.externalCode == "G")
                            {
                                AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                            }

                            if (item.externalCode == "M")
                            {
                                AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                            }

                            if (item.externalCode == "P")
                            {
                                AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                            }

                            if (item.externalCode == "B")
                            {
                                AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                            }

                        }

                        if (!opcDoSistema.RemoveComplementos)
                        {
                            if (item.Options.Count > 0)
                            {
                                foreach (var option in CaracteristicasPedido.Observações)
                                {
                                    AdicionaConteudo($"{option}", FonteDetalhesDoPedido);
                                }

                                if (item.observations != null && item.observations.Length > 0)
                                {
                                    AdicionaConteudo($"Obs: {item.observations}", FonteCPF);
                                }
                            }
                        }

                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    float ValorEntrega = 0.0f;

                    var EntregaObj = pedidoCompleto.Return.OtherFees.Where(x => x.Type == "DELIVERY_FEE").FirstOrDefault();
                    ValorEntrega = EntregaObj.Price.Value;

                    float ValorTaxasAdicionais = 0.0f;

                    foreach (var item in pedidoCompleto.Return.OtherFees)
                    {
                        if (item.Type != "DELIVERY_FEE")
                        {
                            ValorTaxasAdicionais += item.Price.Value;
                        }
                    }

                    float ValorDescontosNum = 0.0f;

                    foreach (var item in pedidoCompleto.Return.Discounts)
                    {
                        ValorDescontosNum += item.Amount.value;
                    }


                    AdicionaConteudo($"Valor dos itens: \t                  {pedidoCompleto.Return.Total.ItemsPrice.value.ToString("c")} ", FonteGeral);
                    AdicionaConteudo($"Taxa De Entrega: \t   {ValorEntrega.ToString("c")}", FonteGeral);
                    AdicionaConteudo($"Taxa Adicional:  \t                  {ValorTaxasAdicionais.ToString("c")} ", FonteGeral);
                    AdicionaConteudo($"Descontos:      \t                  {ValorDescontosNum.ToString("c")}", FonteGeral);
                    AdicionaConteudo($"Valor Total:   \t                  {pedidoCompleto.Return.Total.OrderAmount.value.ToString("c")}", FonteGeral);
                    valorDosItens = 0f;
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


                    if (pedidoCompleto.Return.Type == "DELIVERY")
                    {
                        if (pedidoCompleto.Return.Delivery.DeliveryAddressON.Reference != null && pedidoCompleto.Return.Delivery.DeliveryAddressON.Reference.Length > 0)
                        {
                            AdicionaConteudo($"{pedidoCompleto.Return.Delivery.DeliveryAddressON.Reference}", FonteObservaçõesItem);
                            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                        }

                    }

                    var InfoPag = ClsInfosDePagamentosParaImpressaoONPedido.DefineTipoDePagamento(pedidoCompleto.Return.Payments);

                    var Info1 = $"{InfoPag.FormaPagamento} ({InfoPag.TipoPagamento})";

                    AdicionaConteudo(Info1, FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
                    AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosOnPedido.Centro);

                }

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
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            string banco = opcSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";
            string NumContaString = numConta.ToString();

            string? defineEntrega = pedidoCompleto.Return.Type == "TAKEOUT" ? "Retirada" : "Entrega";

            AdicionaConteudo($"Pedido:   #{pedidoCompleto.Return.Id}", FonteNúmeroDoPedido);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            if (pedidoCompleto.Return.Type == "INDOOR")
            {
                AdicionaConteudo($"{pedidoCompleto.Return.Indoor.Place}\n", FonteNomeDoCliente);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            else
            {
                AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            }

            foreach (var item in pedidoCompleto.Return.ItemsOn)
            {
                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item, true);

                if (item.externalCode == "BB" || item.externalCode == "LAN" || item.externalCode == "PRC")
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }
                else
                {
                    AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }

                if (item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B")
                {
                    if (item.externalCode == "G")
                    {
                        AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "M")
                    {
                        AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "P")
                    {
                        AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "B")
                    {
                        AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                    }

                }

                if (item.Options != null || item.Options.Count > 0)
                {
                    foreach (var option in CaracteristicasPedido.Observações)
                    {
                        AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.observations != null && item.observations.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.observations}", FonteCPF, eObs: true);
                    }

                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                //AdicionaConteudo($"", FonteItens);


            }

            AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
            AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosOnPedido.Centro);


            Imprimir(Conteudo, impressora1, 18);
            Conteudo.Clear();

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
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            string banco = opcSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";
            string NumContaString = numConta.ToString();


            string? defineEntrega = pedidoCompleto.Return.Type == "TAKEOUT" ? "Retirada" : "Entrega";


            AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);

            AdicionaConteudo($"Pedido:   #{pedidoCompleto.Return.Id}", FonteNúmeroDoPedido);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            if (pedidoCompleto.Return.Type == "INDOOR")
            {
                AdicionaConteudo($"{pedidoCompleto.Return.Indoor.Place}\n", FonteNomeDoCliente);
            }
            else
            {
                AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);

            }

            int qtdItens = pedidoCompleto.Return.ItemsOn.Count();
            int contagemItemAtual = 1;

            foreach (var item in pedidoCompleto.Return.ItemsOn)
            {
                AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item, true);

                if (item.externalCode == "BB" || item.externalCode == "LAN" || item.externalCode == "PRC")
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }
                else
                {
                    AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }

                if (item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B")
                {
                    if (item.externalCode == "G")
                    {
                        AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "M")
                    {
                        AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "P")
                    {
                        AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "B")
                    {
                        AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                    }

                }

                if (item.Options != null || item.Options.Count > 0)
                {
                    foreach (var option in CaracteristicasPedido.Observações)
                    {
                        AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.observations != null && item.observations.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.observations}", FonteCPF, eObs: true);
                    }

                }
                contagemItemAtual++;
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


                contagemItemAtual = 0;

                AdicionaConteudo("Impresso por:", FonteGeral);
                AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
                AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosOnPedido.Centro);
            }

            Imprimir(Conteudo, impressora1, 24);
            Conteudo.Clear();

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
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            string banco = opcSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";
            string NumContaString = numConta.ToString();


            string? defineEntrega = pedidoCompleto.Return.Type == "TAKEOUT" ? "Retirada" : "Entrega";
            int contagemItemAtual = 1;

            int qtdItens = 0;

            foreach (var item in pedidoCompleto.Return.ItemsOn)
            {
                qtdItens += 1 * item.quantity;
            }

            //nome do restaurante estatico por enquanto
            foreach (var item in pedidoCompleto.Return.ItemsOn)
            {
                for (var i = 0; i < item.quantity; i++)
                {

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);

                    AdicionaConteudo($"Pedido:          #{pedidoCompleto.Return.Id}", FonteNúmeroDoPedido);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.Return.Type == "INDOOR")
                    {
                        AdicionaConteudo($"{pedidoCompleto.Return.Indoor.Place}\n", FonteNomeDoCliente);
                    }
                    else
                    {
                        AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);

                    }

                    AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item, true);

                    if (item.quantity > 1)
                    {
                        AdicionaConteudo($"1X {item.Name}\n\n", FonteItens);
                    }
                    else
                    {
                        if (item.externalCode == "BB" || item.externalCode == "LAN")
                        {
                            AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                        }
                        else
                        {
                            AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                        }
                    }

                    if (item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B")
                    {
                        if (item.externalCode == "G")
                        {
                            AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                        }

                        if (item.externalCode == "M")
                        {
                            AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                        }

                        if (item.externalCode == "P")
                        {
                            AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                        }

                        if (item.externalCode == "B")
                        {
                            AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                        }

                    }

                    if (item.Options != null || item.Options.Count > 0)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.observations != null && item.observations.Length > 0)
                        {
                            AdicionaConteudo($"Obs: {item.observations}", FonteCPF, eObs: true);
                        }

                    }
                    contagemItemAtual++;
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
                    AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosOnPedido.Centro);


                    Imprimir(Conteudo, impressora1, 24);
                    Conteudo.Clear();

                }
            }

            contagemItemAtual = 1;
            qtdItens = 0;
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
            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido> ListaDeItems = new List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido>() { new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido() { Impressora1 = "Cz1" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido() { Impressora1 = "Cz2" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido() { Impressora1 = "Cz3" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido() { Impressora1 = "Cz4" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido() { Impressora1 = "Sem Impressora" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido() { Impressora1 = "Bar" } };


            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            string? defineEntrega = pedidoCompleto.Return.Type == "TAKEOUT" ? "Retirada" : "Entrega Propria";

            foreach (var item in pedidoCompleto.Return.ItemsOn)
            {
                bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B" || item.externalCode == "BB" || item.externalCode == "LAN" ? true : false;
                string externalCode = item.externalCode;

                if (ePizza)
                {
                    foreach (var option in item.Options)
                    {
                        if (!option.externalCode.Contains("m"))
                        {
                            List<string> LocalDeImpressaoDasPizza = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(option.externalCode);

                            if (LocalDeImpressaoDasPizza.Count > 1)
                            {
                                List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido> GruposDoItemPizza = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressaoDasPizza[0] || x.Impressora1 == LocalDeImpressaoDasPizza[1]).ToList();

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

            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido> ListaLimpa = ListaDeItems.Where(x => x.Itens.Count > 0).ToList();

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


    public static void ImprimeComandaSeparada(string impressora, int displayId, List<itemsOn> itens, int numConta)
    {
        try
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();
            string NumContaString = numConta.ToString();

            //List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> itemsSeparadosPorImpressao = SeparaItensParaImpressaoSeparada();
            //string? defineEntrega = pedidoCompleto.delivery.deliveredBy == null ? "Retirada" : "Entrega Propria";

            //nome do restaurante estatico por enquanto


            AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);

            AdicionaConteudo($"Pedido: \t#{pedidoCompleto.Return.Id}", FonteNúmeroDoPedido); // aqui seria o display id Arrumar
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo($"Entrega: \t  Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            int qtdItens = pedidoCompleto.Return.ItemsOn.Count();
            int contagemItemAtual = 1;


            foreach (var item in itens)
            {

                if (impressora == "Sem Impressora" || impressora == "" || impressora == null)
                {
                    throw new Exception("Uma das impressora não foi encontrada adicione ela nas configurações ou retire a impressão separada!");
                }


                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item, true);

                AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);

                if (item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B")
                {
                    if (item.externalCode == "G")
                    {
                        AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "M")
                    {
                        AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "P")
                    {
                        AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                    }

                    if (item.externalCode == "B")
                    {
                        AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                    }

                }

                if (item.externalCode == "BB" || item.externalCode == "LAN")
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }
                else
                {
                    AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }

                if (item.Options != null)
                {
                    foreach (var option in CaracteristicasPedido.Observações)
                    {
                        AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.observations != null && item.observations.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.observations}", FonteCPF, eObs: true);
                    }

                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            contagemItemAtual = 0;

            AdicionaConteudo("Impresso por:", FonteGeral);
            AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
            AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosOnPedido.Centro);

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

    public static void ImprimeComandaSeparadaTipo2(string impressora, int displayId, List<itemsOn> itens, int numConta)
    {
        try
        {

            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();
            string NumContaString = numConta.ToString();

            //List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> itemsSeparadosPorImpressao = SeparaItensParaImpressaoSeparada();
            //string? defineEntrega = pedidoCompleto.delivery.deliveredBy == null ? "Retirada" : "Entrega Propria";
            int contagemItemAtual = 1;

            //nome do restaurante estatico por enquanto
            foreach (var item in itens)
            {
                int quantidadeDoItem = Convert.ToInt32(item.quantity);

                for (int i = 0; i < quantidadeDoItem; i++)
                {

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);

                    AdicionaConteudo($"Pedido: \t#{pedidoCompleto.Return.Id}", FonteNúmeroDoPedido); // aqui seria o display id Arrumar
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Entrega: \t  Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    int qtdItens = pedidoCompleto.Return.ItemsOn.Count();

                    if (impressora == "Sem Impressora" || impressora == "" || impressora == null)
                    {
                        throw new Exception("Uma das impressora não foi encontrada adicione ela nas configurações ou retire a impressão separada!");
                    }


                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemOnPedido(item, true);

                    AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);

                    if (item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B")
                    {
                        if (item.externalCode == "G")
                        {
                            AdicionaConteudo(TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                        }

                        if (item.externalCode == "M")
                        {
                            AdicionaConteudo(TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                        }

                        if (item.externalCode == "P")
                        {
                            AdicionaConteudo(TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                        }

                        if (item.externalCode == "B")
                        {
                            AdicionaConteudo(TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                        }

                    }

                    if (item.externalCode == "BB" || item.externalCode == "LAN")
                    {
                        AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                    }
                    else
                    {
                        AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                    }

                    if (item.Options != null)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.observations != null && item.observations.Length > 0)
                        {
                            AdicionaConteudo($"Obs: {item.observations}", FonteCPF, eObs: true);
                        }

                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("ONPEDIDO", FonteNomeDoCliente, AlinhamentosOnPedido.Centro);
                    AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosOnPedido.Centro);

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
            DefineImpressao(numConta, displayId, opcSistema.Impressora1);
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
                PedidoOnPedido? PedidoOn = JsonConvert.DeserializeObject<PedidoOnPedido>(Pedido.Json);

                if (impressora == opcSistema.Impressora1 || impressora == opcSistema.ImpressoraAux)
                {
                    if (opcSistema.ImpCompacta && PedidoOn.Return.Type != "INDOOR")
                    {
                        DefineImpressao2(numConta, displayId, impressora);
                    }
                    else if (PedidoOn.Return.Type != "INDOOR")
                    {
                        DefineImpressao(numConta, displayId, impressora);
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



    public static void AdicionaConteudo(string conteudo, Font fonte, AlinhamentosOnPedido alinhamento = AlinhamentosOnPedido.Esquerda, bool eObs = false)
    {
        Conteudo.Add(new ClsImpressaoDefinicoesOnPedido() { Texto = conteudo, Fonte = fonte, Alinhamento = alinhamento, eObs = eObs });
    }

    public static void AdicionaConteudoParaImpSeparada(string impressora, string conteudo, Font fonte, AlinhamentosOnPedido alinhamento = AlinhamentosOnPedido.Esquerda)
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


