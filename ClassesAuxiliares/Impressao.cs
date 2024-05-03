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
namespace SysIntegradorApp.ClassesAuxiliares;


public class Impressao
{
    public static int NumContas { get; set; }
    public static List<ClsImpressaoDefinicoes>? Conteudo { get; set; } = new List<ClsImpressaoDefinicoes>();
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
    public static Font FonteEndereçoDoCliente = new Font("DejaVu sans mono", 13, FontStyle.Bold);
    public static Font FonteItens = new Font("DejaVu sans mono", 12, FontStyle.Bold);
    public static Font FonteOpcionais = new Font("DejaVu sans mono", 11, FontStyle.Regular);
    public static Font FonteObservaçõesItem = new Font("DejaVu sans mono", 11, FontStyle.Bold);
    public static Font FonteTotaisDoPedido = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteCPF = new Font("DejaVu sans mono", 8, FontStyle.Bold);

    public enum Alinhamentos
    {
        Esquerda,
        Direita,
        Centro
    }

    public static void Imprimir(List<ClsImpressaoDefinicoes> conteudo, string impressora1)
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
        printDocument.PrintPage += (sender, e) => PrintPageHandler(sender, e, conteudo);

        // Inicie o processo de impressão
        printDocument.Print();
    }

    public static void PrintPageHandler(object sender, PrintPageEventArgs e, List<ClsImpressaoDefinicoes> conteudo)
    {
        try
        {
            // Define o conteúdo a ser impresso

            int Y = 0;


            foreach (var item in conteudo)
            {
                var tamanhoFrase = e.Graphics.MeasureString(item.Texto, item.Fonte).Width;

                if (tamanhoFrase < e.PageBounds.Width)
                {
                    if (item.Alinhamento == Alinhamentos.Centro)
                    {
                        e.Graphics.DrawString(item.Texto, item.Fonte, Brushes.Black, Centro(item.Texto, item.Fonte, e), Y);
                    }
                    else
                    {
                        e.Graphics.DrawString(item.Texto, item.Fonte, Brushes.Black, 0, Y);
                        Y += 24;
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
                        if (item.Alinhamento == Alinhamentos.Centro)
                        {

                            e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, Centro(item.Texto, item.Fonte, e), Y);
                            Y += 24;
                            frase = "";
                            continue;

                        }
                        else
                        {
                            e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, 0, Y);
                            Y += 24;
                            frase = "";
                            continue;
                        }

                    }



                    if (frase != "")
                    {
                        if (item.Alinhamento == Alinhamentos.Centro)
                        {

                            e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, Centro(item.Texto, item.Fonte, e), Y);

                        }
                        else
                        {
                            e.Graphics.DrawString(frase, item.Fonte, Brushes.Black, 0, Y);

                        }

                    }

                }

                frase = "";
                Y += 24;
            }
            // Desenhe o texto na área de impressão



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
            PedidoCompleto? pedidoCompleto = JsonSerializer.Deserialize<PedidoCompleto>(pedidoPSQL.Json);
            ParametrosDoSistema? opcDoSistema = dbContext.parametrosdosistema.Where(x => x.Id == 1).FirstOrDefault();

            string banco = opcDoSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();
                string? defineEntrega = pedidoCompleto.orderType == "TAKEOUT" ? "Retirada" : "Entrega Propria";


                using (OleDbCommand comando = new OleDbCommand(sqlQuery, connection))
                using (OleDbDataReader reader = comando.ExecuteReader())
                {

                    AdicionaConteudo($"{opcDoSistema.NomeFantasia}", FonteNomeRestaurante, Alinhamentos.Centro);
                    AdicionaConteudo($"{opcDoSistema.Endereco}", FonteGeral);
                    AdicionaConteudo($"{opcDoSistema.Telefone}", FonteGeral, Alinhamentos.Centro);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Pedido: \t#{pedidoCompleto.displayId}", FonteNúmeroDoPedido);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Entrega: \t  Nº000\n", FonteNomeDoCliente);
                    AdicionaConteudo($"{defineEntrega}\n", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Código de coleta: {pedidoCompleto.delivery.pickupCode}", FonteItens);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


                    AdicionaConteudo("Origem: \t\t       Ifood", FonteGeral);
                    AdicionaConteudo("Atendente: \t      SysIntegrador", FonteGeral);
                    AdicionaConteudo($"Realizado: \t {pedidoCompleto.createdAt.Substring(0, 10)} {pedidoCompleto.createdAt.Substring(11, 5)}", FonteGeral);

                    if (defineEntrega == "Retirada")
                    {
                        AdicionaConteudo($"Terminar Até: \t {pedidoCompleto.takeout.takeoutDateTime.Substring(0, 10)} {pedidoCompleto.takeout.takeoutDateTime.Substring(11, 5)}", FonteGeral);
                    }
                    else
                    {
                        AdicionaConteudo($"Entregar Até: \t {pedidoCompleto.delivery.deliveryDateTime.Substring(0, 10)} {pedidoCompleto.delivery.deliveryDateTime.Substring(11, 5)}", FonteGeral);
                    }

                    AdicionaConteudo($"Realizado: \t {pedidoCompleto.createdAt.Substring(0, 10)} {pedidoCompleto.createdAt.Substring(11, 5)}", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Fone: {pedidoCompleto.customer.phone.number}", FonteNúmeroDoTelefone);
                    AdicionaConteudo($"Localizador: {pedidoCompleto.customer.phone.localizer}", FonteNúmeroDoTelefone);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo(pedidoCompleto.customer.name, FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.orderType == "DELIVERY")
                    {
                        AdicionaConteudo("Endereço de enntrega:", FonteCPF);
                        AdicionaConteudo($"{pedidoCompleto.delivery.deliveryAddress.formattedAddress} - {pedidoCompleto.delivery.deliveryAddress.neighborhood}", FonteEndereçoDoCliente);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }
                    else
                    {
                        AdicionaConteudo("RETIRADA NO BALCÃO", FonteEndereçoDoCliente);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    float valorDosItens = 0f;

                    foreach (var item in pedidoCompleto.items)
                    {
                        ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItem(item);

                        AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto} {item.totalPrice.ToString("c")}\n\n", FonteItens);
                        if (item.options != null)
                        {
                            foreach (var option in CaracteristicasPedido.Observações)
                            {
                                AdicionaConteudo($"{option}", FonteDetalhesDoPedido);
                            }

                            if (item.observations != null && item.observations.Length > 0)
                            {
                                AdicionaConteudo($"Obs: {item.observations}", FonteCPF);
                            }

                            valorDosItens += item.totalPrice;
                        }

                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    AdicionaConteudo($"Valor dos itens: \t   {valorDosItens.ToString("c")} ", FonteTotaisDoPedido);
                    AdicionaConteudo($"Taxa De Entrega: \t   {pedidoCompleto.total.deliveryFee.ToString("c")}", FonteTotaisDoPedido);
                    AdicionaConteudo($"Taxa Adicional:  \t ", FonteTotaisDoPedido);
                    AdicionaConteudo($"Descontos:      \t   {pedidoCompleto.total.benefits.ToString("c")}", FonteTotaisDoPedido);
                    AdicionaConteudo($"Valor Total:   \t   {pedidoCompleto.total.orderAmount.ToString("c")}", FonteTotaisDoPedido);
                    valorDosItens = 0f;
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    if (pedidoCompleto.delivery.observations != null && pedidoCompleto.delivery.observations.Length > 0)
                    {
                        AdicionaConteudo($"{pedidoCompleto.delivery.observations}", FonteObservaçõesItem);
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
                    }

                    ClsInfosDePagamentosParaImpressao infosDePagamento = DefineTipoDePagamento(pedidoCompleto.payments.methods);

                    AdicionaConteudo(infosDePagamento.FormaPagamento, FonteGeral);
                    AdicionaConteudo($"Valor: \t {infosDePagamento.valor.ToString("c")}", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


                }

                Imprimir(Conteudo, impressora1);
                Conteudo.Clear();
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }


    public static void ImprimeComanda(int numConta, int displayId, string impressora1) //comanda
    {
        try
        {
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoCompleto? pedidoCompleto = JsonSerializer.Deserialize<PedidoCompleto>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            string banco = opcSistema.CaminhodoBanco;
            string sqlQuery = $"SELECT * FROM Contas where CONTA = {numConta}";

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();
                string? defineEntrega = pedidoCompleto.delivery.deliveredBy == null ? "Retirada" : "Entrega Propria";

                using (OleDbCommand comando = new OleDbCommand(sqlQuery, connection))
                using (OleDbDataReader reader = comando.ExecuteReader())
                {
                    //nome do restaurante estatico por enquanto

                    AdicionaConteudo($"Pedido: \t#{pedidoCompleto.displayId}", FonteNúmeroDoPedido);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    AdicionaConteudo($"Entrega: \t  Nº000\n", FonteNomeDoCliente);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    int qtdItens = pedidoCompleto.items.Count();
                    int contagemItemAtual = 1;

                    foreach (var item in pedidoCompleto.items)
                    {
                        AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                        ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItem(item);

                        AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto} {item.totalPrice.ToString("c")}\n\n", FonteItens);
                        if (item.options != null)
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
                        contagemItemAtual++;
                        AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                    }
                    contagemItemAtual = 0;

                    AdicionaConteudo("Impresso por:", FonteGeral);
                    AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
                    AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

                }

                Imprimir(Conteudo, impressora1);
                Conteudo.Clear();
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
            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> ListaDeItems = new List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas>() { new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas() { Impressora1 = "Cz1" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas() { Impressora1 = "Cz2" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas() { Impressora1 = "Cz3" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas() { Impressora1 = "Cz4" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas() { Impressora1 = "Sem Impressora" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas() { Impressora1 = "Bar" } };


            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoCompleto? pedidoCompleto = JsonSerializer.Deserialize<PedidoCompleto>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();
            string? defineEntrega = pedidoCompleto.delivery.deliveredBy == null ? "Retirada" : "Entrega Propria";

            foreach (var item in pedidoCompleto.items)
            {
                bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" ? true : false;
                string externalCode = item.externalCode;

                if (ePizza)
                {
                    foreach (var option in item.options)
                    {
                        if (!option.externalCode.Contains("m"))
                        {
                            List<string> LocalDeImpressaoDasPizza = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(option.externalCode);

                            var GruposDoItemPizza = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressaoDasPizza[0] || x.Impressora1 == LocalDeImpressaoDasPizza[1]).ToList();

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

                    continue;
                }

                //-------------------------------------------------------------------------------------------------------------------------------//
                List<string> LocalDeImpressao = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(externalCode);

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

            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> ListaLimpa = ListaDeItems.Where(x => x.Itens.Count > 0).ToList();

            foreach (var item in ListaLimpa)
            {
                item.Impressora1 = DefineNomeDeImpressoraCasoEstejaSelecionadoImpSeparada(item.Impressora1);

                ImprimeComandaSeparada(item.Impressora1, displayId, item.Itens, numConta);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }


    public static void ImprimeComandaSeparada(string impressora, int displayId, List<Items> itens, int numConta)
    {
        try
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoPedido? pedidoPSQL = dbContext.parametrosdopedido.Where(x => x.DisplayId == displayId).FirstOrDefault();
            PedidoCompleto? pedidoCompleto = JsonSerializer.Deserialize<PedidoCompleto>(pedidoPSQL.Json);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();


            //List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> itemsSeparadosPorImpressao = SeparaItensParaImpressaoSeparada();
            //string? defineEntrega = pedidoCompleto.delivery.deliveredBy == null ? "Retirada" : "Entrega Propria";

            //nome do restaurante estatico por enquanto

            AdicionaConteudo($"Pedido: \t#{pedidoCompleto.displayId}", FonteNúmeroDoPedido); // aqui seria o display id Arrumar
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            AdicionaConteudo($"Entrega: \t  Nº000\n", FonteNomeDoCliente);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            int qtdItens = pedidoCompleto.items.Count();
            int contagemItemAtual = 1;


            foreach (var item in itens)
            {

                if (impressora == "Sem Impressora" || impressora == "" || impressora == null)
                {
                    throw new Exception("Uma das impressora não foi encontrada adicione ela nas configurações ou retire a impressão separada!");
                }


                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItem(item);

                AdicionaConteudo($"Item: {contagemItemAtual}/{qtdItens}", FonteItens);
                AdicionaConteudo($"{item.quantity}X {CaracteristicasPedido.NomeProduto} {item.totalPrice.ToString("c")}\n\n", FonteItens);
                if (item.options != null)
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

                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }
            contagemItemAtual = 0;

            AdicionaConteudo("Impresso por:", FonteGeral);
            AdicionaConteudo("SysMenu / SysIntegrador", FonteGeral);
            AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);


            if (impressora != "Nao")
            {
                Imprimir(Conteudo, impressora);
            }

            //Imprimir(Conteudo, impressora);
            Conteudo.Clear();

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

        DefineImpressao(numConta, displayId, opcSistema.Impressora1);
        SeparaItensParaImpressaoSeparada(numConta, displayId);
    }

    public static void ChamaImpressoes(int numConta, int displayId, string? impressora)
    {
        ApplicationDbContext db = new ApplicationDbContext();
        ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();
        int ContagemDeImpressoes = 0;

        if (impressora == opcSistema.Impressora1 || impressora == opcSistema.ImpressoraAux)
        {
            DefineImpressao(numConta, displayId, impressora);
            ContagemDeImpressoes++;
            if (opcSistema.ImprimirComandaNoCaixa)
            {
                ImprimeComanda(numConta, displayId, impressora);
            }
        }
        if (ContagemDeImpressoes == 0)
        {
            ImprimeComanda(numConta, displayId, impressora);
        }

        ContagemDeImpressoes = 0;
    }


    public static string AdicionarSeparador()
    {
        return "───────────────────────────";
    }

    public static ClsInfosDePagamentosParaImpressao DefineTipoDePagamento(List<Methods> metodos)
    {
        ClsInfosDePagamentosParaImpressao infos = new ClsInfosDePagamentosParaImpressao();
        foreach (Methods metodo in metodos)
        {
            switch (metodo.type)
            {
                case "ONLINE":
                    infos.TipoPagamento = "Pago Online";
                    break;
                case "OFFLINE":
                    infos.TipoPagamento = "VAI SER PAGO NA ENTREGA";
                    break;
            }


            switch (metodo.method)
            {
                case "CREDIT":
                    infos.FormaPagamento = "(Crédito)";
                    break;
                case "MEAL_VOUCHER":
                    infos.FormaPagamento = "(VOUCHER)";
                    break;
                case "DEBIT":
                    infos.FormaPagamento = "(Débito)";
                    break;
                case "PIX":
                    infos.FormaPagamento = "(PIX)";
                    break;
                case "CASH":
                    if (metodo.cash.changeFor > 0)
                    {
                        double totalTroco = metodo.cash.changeFor - metodo.value;
                        infos.FormaPagamento = $"(Dinheiro) Levar troco para {metodo.cash.changeFor.ToString("c")} Total Troco: {totalTroco.ToString("c")}";
                    }
                    else
                    {
                        infos.FormaPagamento = "(Dinheiro) Não precisa de troco";
                    }
                    break;
                case "BANK_PAY ":
                    infos.FormaPagamento = "(Bank Pay)";
                    break;
                case "FOOD_VOUCHER ":
                    infos.FormaPagamento = "(Vale Refeição)";
                    break;
                default:
                    infos.FormaPagamento = "(Online)";
                    break;

            }

            infos.valor = metodo.value;

        }

        return infos;
    }

    public static void AdicionaConteudo(string conteudo, Font fonte, Alinhamentos alinhamento = Alinhamentos.Esquerda)
    {
        Conteudo.Add(new ClsImpressaoDefinicoes() { Texto = conteudo, Fonte = fonte, Alinhamento = alinhamento });
    }

    public static void AdicionaConteudoParaImpSeparada(string impressora, string conteudo, Font fonte, Alinhamentos alinhamento = Alinhamentos.Esquerda)
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