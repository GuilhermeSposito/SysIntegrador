using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ImpressaoAnotaAi
{
    public static int NumContas { get; set; }
    public static List<ClsImpressaoDefinicoesAnotaAi>? Conteudo { get; set; } = new List<ClsImpressaoDefinicoesAnotaAi>();
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

    public enum AlinhamentosAnotaAi
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

    public static void Imprimir(List<ClsImpressaoDefinicoesAnotaAi> conteudo, string impressora1, int separacao)
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

    public static void PrintPageHandler(object sender, PrintPageEventArgs e, List<ClsImpressaoDefinicoesAnotaAi> conteudo, int separacao)
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
                        if (item.Alinhamento == AlinhamentosAnotaAi.Centro)
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
                            if (item.Alinhamento == AlinhamentosAnotaAi.Centro)
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
                            if (item.Alinhamento == AlinhamentosAnotaAi.Centro)
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
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoAnotaAi? pedidoCompleto = JsonConvert.DeserializeObject<PedidoAnotaAi>(pedidoPSQL.Json);
            ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.FirstOrDefault();

            string banco = opcDoSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";


            string? defineEntrega = pedidoCompleto.InfoDoPedido.Type == "TAKE" ? "R E T I R A D A" : "E N T R E G A";

            string NumContaString = numConta.ToString();


            AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo($"{defineEntrega}", FonteItens);
            AdicionaConteudo($"{opcDoSistema.NomeFantasia}", FonteItens);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo($"Pedido:                                      #{pedidoCompleto.InfoDoPedido.ShortReference}", FonteGeral);

            DateTime DataCertaCriadoEmTimeStamp = DateTime.Parse(pedidoCompleto.InfoDoPedido.CreatedAt);
            var DataCertaCriadoEm = DataCertaCriadoEmTimeStamp.ToString();

            AdicionaConteudo($"Realizado: \t {DataCertaCriadoEm.Substring(0, 10)} {DataCertaCriadoEm.Substring(11, 5)}", FonteGeral);

            if (defineEntrega == "Retirada")
            {

                DateTime DataCertaTerminarEmTimeStamp = DateTime.Parse(pedidoCompleto.InfoDoPedido.CreatedAt);
                var DataCertaTerminarEm = DataCertaTerminarEmTimeStamp;

                AdicionaConteudo($"Terminar Até: \t {DataCertaTerminarEm.ToString().Substring(0, 10)} {DataCertaTerminarEm.ToString().Substring(11, 5)}", FonteGeral);
            }
            else
            {
                DateTime DataCertaEntregarEmTimeStamp = DateTime.Parse(pedidoCompleto.InfoDoPedido.CreatedAt);
                var DataCertaEntregarEm = DataCertaEntregarEmTimeStamp;

                AdicionaConteudo($"Entregar Até: \t {DataCertaEntregarEm.ToString().Substring(0, 10)} {DataCertaEntregarEm.ToString().Substring(11, 5)}", FonteGeral);
            }

            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            if (pedidoCompleto.InfoDoPedido.Type == "LOCAL")
            {
                AdicionaConteudo($"{pedidoCompleto.InfoDoPedido.Pdv.Table}\n", FonteNúmeroDoPedido);

            }
            else
            {
                AdicionaConteudo($"Conta Nº:     {NumContaString.PadLeft(3, '0')}\n", FonteNúmeroDoPedido);
            }



            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo(pedidoCompleto.InfoDoPedido.Customer.Nome, FonteItens);
            AdicionaConteudo($"Fone: {pedidoCompleto.InfoDoPedido.Customer.Phone}", FonteNúmeroDoTelefone);


            if (pedidoCompleto.InfoDoPedido.Type == "DELIVERY")
            {
                AdicionaConteudo("Endereço de entrega:", FonteItens);
                AdicionaConteudo($"{pedidoCompleto.InfoDoPedido.deliveryAddress.FormattedAddress} {pedidoCompleto.InfoDoPedido.deliveryAddress.Neighborhood}", FonteEndereçoDoCliente);


                if (pedidoCompleto.InfoDoPedido.deliveryAddress.Complement != null && pedidoCompleto.InfoDoPedido.deliveryAddress.Complement.Length >= 1)
                {
                    AdicionaConteudo($"Complemento: {pedidoCompleto.InfoDoPedido.deliveryAddress.Complement}", FonteItens);
                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            else if (pedidoCompleto.InfoDoPedido.Type == "TAKE")
            {
                AdicionaConteudo("RETIRADA NO BALCÃO", FonteItens);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            else if (pedidoCompleto.InfoDoPedido.Type == "LOCAL")
            {
                AdicionaConteudo($"Entregar para {pedidoCompleto.InfoDoPedido.Pdv.Table}", FonteItens);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }

            float valorDosItens = 0f;

            foreach (var item in pedidoCompleto.InfoDoPedido.Items)
            {
                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAnotaAi(item);

                if (item.externalCode == "BB" || item.externalCode == "LAN")
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto} {item.Total.ToString("c")}\n\n", FonteItens);
                }
                else
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto} {item.Total.ToString("c")}\n\n", FonteItens);
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
                    if (item.SubItens.Count > 0)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.observation != null && item.observation.Length > 0)
                        {
                            AdicionaConteudo($"Obs: {item.observation}", FonteCPF, eObs: true);
                        }
                    }
                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                valorDosItens += item.Total;
            }

            float ValorTaxasAdicionais = 0.0f;

            float ValorDescontosNum = 0.0f;

            foreach (var item in pedidoCompleto.InfoDoPedido.Descontos)
            {
                ValorDescontosNum += item.Total;
            }


            AdicionaConteudo($"Valor dos itens: \t                  {valorDosItens.ToString("c")} ", FonteGeral);
            if (Convert.ToSingle(pedidoCompleto.InfoDoPedido.DeliveryFee) > 0)
                AdicionaConteudo($"Taxa De Entrega: \t   {Convert.ToSingle(pedidoCompleto.InfoDoPedido.DeliveryFee).ToString("c")}", FonteGeral);
            if (ValorTaxasAdicionais > 0)
                AdicionaConteudo($"Taxa Adicional:  \t                  {ValorTaxasAdicionais.ToString("c")} ", FonteGeral);
            if (ValorDescontosNum > 0)
                AdicionaConteudo($"Descontos:      \t                  {ValorDescontosNum.ToString("c")}", FonteGeral);
            AdicionaConteudo($"Valor Total:   \t                  {pedidoCompleto.InfoDoPedido.Total.ToString("c")}", FonteGeral);
            valorDosItens = 0f;
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


            if (pedidoCompleto.InfoDoPedido.Type == "DELIVERY")
            {
                if (pedidoCompleto.InfoDoPedido.deliveryAddress.Reference != null && pedidoCompleto.InfoDoPedido.deliveryAddress.Reference.Length > 0)
                {
                    AdicionaConteudo($"{pedidoCompleto.InfoDoPedido.deliveryAddress.Reference}", FonteObservaçõesItem);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                }

            }

            if (pedidoCompleto.InfoDoPedido.Observation is not null && pedidoCompleto.InfoDoPedido.Observation.Length > 0)
            {
                AdicionaConteudo($"{pedidoCompleto.InfoDoPedido.Observation}", FonteObservaçõesItem);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }

            var InfoPag = ClsInfosDePagamentosParaImpressaoAnotaAi.DefineTipoDePagamento(pedidoCompleto.InfoDoPedido.Payments);

            var Info1 = $"{InfoPag.TipoPagamento}, {InfoPag.FormaPagamento}";

            AdicionaConteudo(Info1, FonteGeral);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo("Impresso por:", FonteGeral);
            AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);
            AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosAnotaAi.Centro);



            Imprimir(Conteudo, impressora1, 16);
            Conteudo.Clear();


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
            PedidoAnotaAi? pedidoCompleto = JsonConvert.DeserializeObject<PedidoAnotaAi>(pedidoPSQL.Json);
            ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.FirstOrDefault();


            string banco = opcDoSistema.CaminhodoBanco;
            string NumContaString = numConta.ToString();

            string? defineEntrega = pedidoCompleto.InfoDoPedido.Type == "TAKE" ? "Retirada" : "Entrega";

            AdicionaConteudo($"Pedido:        #{pedidoCompleto.InfoDoPedido.ShortReference}", FonteNúmeroDoPedido);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            if (pedidoCompleto.InfoDoPedido.Type == "LOCAL")
            {
                AdicionaConteudo($"{pedidoCompleto.InfoDoPedido.Pdv.Table}\n", FonteNomeDoCliente);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            else
            {
                AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            }

            foreach (var item in pedidoCompleto.InfoDoPedido.Items)
            {
                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAnotaAi(item, true);

                if (item.externalCode == "BB" || item.externalCode == "LAN" || item.externalCode == "PRC")
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }
                else
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
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

                if (item.SubItens != null || item.SubItens.Count > 0)
                {
                    foreach (var option in CaracteristicasPedido.Observações)
                    {
                        AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.observation != null && item.observation.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.observation}", FonteCPF, eObs: true);
                    }

                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                //AdicionaConteudo($"", FonteItens);


            }

            AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);
            AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosAnotaAi.Centro);


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
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoAnotaAi? pedidoCompleto = JsonConvert.DeserializeObject<PedidoAnotaAi>(pedidoPSQL.Json);
            ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.FirstOrDefault();

            string banco = opcDoSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";
            string NumContaString = numConta.ToString();

            string? defineEntrega = pedidoCompleto.InfoDoPedido.Type == "TAKE" ? "Retirada" : "Entrega";

            AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);

            AdicionaConteudo($"Pedido:        #{pedidoCompleto.InfoDoPedido.ShortReference}", FonteNúmeroDoPedido);

            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            if (pedidoCompleto.InfoDoPedido.Type == "LOCAL")
            {
                AdicionaConteudo($"{pedidoCompleto.InfoDoPedido.Pdv.Table}\n", FonteNomeDoCliente);
            }
            else
            {
                AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);

            }

            int qtdItens = pedidoCompleto.InfoDoPedido.Items.Count();
            int contagemItemAtual = 1;

            foreach (var item in pedidoCompleto.InfoDoPedido.Items)
            {
                AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAnotaAi(item, true);

                if (item.externalCode == "BB" || item.externalCode == "LAN" || item.externalCode == "PRC")
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
                }
                else
                {
                    AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
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

                if (item.SubItens != null || item.SubItens.Count > 0)
                {
                    foreach (var option in CaracteristicasPedido.Observações)
                    {
                        AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.observation != null && item.observation.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.observation}", FonteCPF, eObs: true);
                    }

                }
                contagemItemAtual++;
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


                contagemItemAtual = 0;

                AdicionaConteudo("Impresso por:", FonteGeral);
                AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);
                AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosAnotaAi.Centro);
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
            PedidoAnotaAi? pedidoCompleto = JsonConvert.DeserializeObject<PedidoAnotaAi>(pedidoPSQL.Json);
            ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.FirstOrDefault();

            string banco = opcDoSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";
            string NumContaString = numConta.ToString();


            string? defineEntrega = pedidoCompleto.InfoDoPedido.Type == "TAKE" ? "Retirada" : "Entrega";
            int contagemItemAtual = 1;

            int qtdItens = 0;

            foreach (var item in pedidoCompleto.InfoDoPedido.Items)
            {
                qtdItens += 1 * item.quantity;
            }

            //nome do restaurante estatico por enquanto
            foreach (var item in pedidoCompleto.InfoDoPedido.Items)
            {
                for (var i = 0; i < item.quantity; i++)
                {

                    AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);

                    AdicionaConteudo($"Pedido:             #{pedidoCompleto.InfoDoPedido.ShortReference}", FonteNúmeroDoPedido);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.InfoDoPedido.Type == "LOCAL")
                    {
                        AdicionaConteudo($"{pedidoCompleto.InfoDoPedido.Pdv.Table}\n", FonteNomeDoCliente);
                    }
                    else
                    {
                        AdicionaConteudo($"{defineEntrega}: Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);

                    }

                    AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAnotaAi(item, true);

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
                            AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}\n\n", FonteItens);
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

                    if (item.SubItens != null || item.SubItens.Count > 0)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.observation != null && item.observation.Length > 0)
                        {
                            AdicionaConteudo($"Obs: {item.observation}", FonteCPF, eObs: true);
                        }

                    }
                    contagemItemAtual++;
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);
                    AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosAnotaAi.Centro);


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
            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi> ListaDeItems = new List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi>() { new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi() { Impressora1 = "Cz1" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi() { Impressora1 = "Cz2" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi() { Impressora1 = "Cz3" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi() { Impressora1 = "Cz4" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi() { Impressora1 = "Sem Impressora" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi() { Impressora1 = "Bar" } };


            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoAnotaAi? pedidoCompleto = JsonConvert.DeserializeObject<PedidoAnotaAi>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            string? defineEntrega = pedidoCompleto.InfoDoPedido.Type == "TAKE" ? "Retirada" : "Entrega Propria";

            foreach (var item in pedidoCompleto.InfoDoPedido.Items)
            {
                bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B" || item.externalCode == "BB" || item.externalCode == "LAN" ? true : false;
                string externalCode = item.externalCode;

                if (ePizza)
                {
                    foreach (var option in item.SubItens)
                    {
                        if (!option.externalCode.Contains("m"))
                        {
                            List<string> LocalDeImpressaoDasPizza = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(option.externalCode);

                            if (LocalDeImpressaoDasPizza.Count > 1)
                            {
                                List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi> GruposDoItemPizza = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressaoDasPizza[0] || x.Impressora1 == LocalDeImpressaoDasPizza[1]).ToList();

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

            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasAnotaAi> ListaLimpa = ListaDeItems.Where(x => x.Itens.Count > 0).ToList();

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


    public static void ImprimeComandaSeparada(string impressora, int displayId, List<ItemAnotaAi> itens, int numConta)
    {
        try
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoOnPedido? pedidoCompleto = JsonConvert.DeserializeObject<PedidoOnPedido>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();
            string NumContaString = numConta.ToString();


            //string? defineEntrega = pedidoCompleto.InfoDoPedido.Type == "TAKE" ? "Retirada" : "Entrega";


            AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);

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


                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAnotaAi(item, true);

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

                if (item.SubItens != null)
                {
                    foreach (var option in CaracteristicasPedido.Observações)
                    {
                        AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.observation != null && item.observation.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.observation}", FonteCPF, eObs: true);
                    }

                }

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            contagemItemAtual = 0;

            AdicionaConteudo("Impresso por:", FonteGeral);
            AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);
            AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosAnotaAi.Centro);

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

    public static void ImprimeComandaSeparadaTipo2(string impressora, int displayId, List<ItemAnotaAi> itens, int numConta)
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

                    AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);

                    AdicionaConteudo($"Pedido: \t#{pedidoCompleto.Return.Id}", FonteNúmeroDoPedido); // aqui seria o display id Arrumar
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Entrega: \t  Nº{NumContaString.PadLeft(3, '0')}\n", FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    int qtdItens = pedidoCompleto.Return.ItemsOn.Count();

                    if (impressora == "Sem Impressora" || impressora == "" || impressora == null)
                    {
                        throw new Exception("Uma das impressora não foi encontrada adicione ela nas configurações ou retire a impressão separada!");
                    }


                    ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemAnotaAi(item, true);

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

                    if (item.SubItens != null)
                    {
                        foreach (var option in CaracteristicasPedido.Observações)
                        {
                            AdicionaConteudo($"{option}", FonteDetalhesDoPedido, eObs: true);
                        }

                        if (item.observation != null && item.observation.Length > 0)
                        {
                            AdicionaConteudo($"Obs: {item.observation}", FonteCPF, eObs: true);
                        }

                    }

                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Anota Ai", FonteNomeDoCliente, AlinhamentosAnotaAi.Centro);
                    AdicionaConteudo("www.syslogica.com.br", FonteGeral, AlinhamentosAnotaAi.Centro);

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

    public static void ChamaImpressoes(int numConta, int displayId, string? impressora, int numDaVia)
    {
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
            int ContagemDeImpressoes = 0;

            ParametrosDoPedido? Pedido = db.parametrosdopedido.FirstOrDefault(x => x.DisplayId == displayId);

            if (Pedido is not null)
            {
                PedidoAnotaAi? PedidoOn = JsonConvert.DeserializeObject<PedidoAnotaAi>(Pedido.Json);

                if (impressora == opcSistema.Impressora1 || impressora == opcSistema.ImpressoraAux)
                {
                    if (opcSistema.ImpCompacta && PedidoOn.InfoDoPedido.Type != "LOCAL")
                    {
                        DefineImpressao2(numConta, displayId, impressora);
                    }
                    else if (PedidoOn.InfoDoPedido.Type != "LOCAL")
                    {
                        DefineImpressao2(numConta, displayId, impressora);
                    }
                    ContagemDeImpressoes++;
                    if (opcSistema.ImprimirComandaNoCaixa && numDaVia == 1)
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
                    if (opcSistema.TipoComanda == 2 && numDaVia == 1)
                    {
                        for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                        {
                            ImprimeComandaTipo2(numConta, displayId, impressora);
                        }

                    }
                    else
                    {
                        if (opcSistema.ComandaReduzida && numDaVia == 1)
                        {
                            for (int i = 0; i < opcSistema.NumDeViasDeComanda; i++)
                            {
                                ImprimeComandaReduzida(numConta, displayId, impressora);
                            }

                        }
                        else if (numDaVia == 1)
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



    public static void AdicionaConteudo(string conteudo, Font fonte, AlinhamentosAnotaAi alinhamento = AlinhamentosAnotaAi.Esquerda, bool eObs = false)
    {
        Conteudo.Add(new ClsImpressaoDefinicoesAnotaAi() { Texto = conteudo, Fonte = fonte, Alinhamento = alinhamento, eObs = eObs });
    }

    public static void AdicionaConteudoParaImpSeparada(string impressora, string conteudo, Font fonte, AlinhamentosAnotaAi alinhamento = AlinhamentosAnotaAi.Esquerda)
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

    public static void ChamaImpressoes(string? Id_pedido, int numDaVia = 1)
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
                    ImpressaoAnotaAi.ChamaImpressoes(pedido.Conta, pedido.DisplayId, imp, numDaVia);
                }
            }
        }
        else
        {
            ImpressaoAnotaAi.ChamaImpressoesCasoSejaComandaSeparada(pedido.Conta, pedido.DisplayId, impressoras);
        }



        impressoras.Clear();
    }
}
