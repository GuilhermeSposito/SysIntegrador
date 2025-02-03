using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.data;
using System.Drawing.Printing;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ImpressaoGarcom
{
    public static int NumContas { get; set; }
    public static List<ClsImpressaoDefinicoeGarcom>? Conteudo { get; set; } = new List<ClsImpressaoDefinicoeGarcom>();
    public static List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas> ConteudoParaImpSeparada { get; set; } = new List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas>();


    public static Font FonteGeral = new Font("DejaVu sans mono mono", 11, FontStyle.Bold);
    public static Font FonteSeparadores = new Font("DejaVu sans mono", 11, FontStyle.Bold);
    public static Font FonteSeparadoresSimples = new Font("DejaVu sans mono", 8, FontStyle.Regular);
    public static Font FonteCódigoDeBarras = new Font("3 of 9 Barcode", 35, FontStyle.Regular);
    public static Font FonteNomeRestaurante = new Font("DejaVu sans mono", 15, FontStyle.Bold);
    public static Font FonteEndereçoDoRestaurante = new Font("DejaVu sans mono", 9, FontStyle.Bold);
    public static Font FonteNúmeroDoPedido = new Font("DejaVu sans mono", 17, FontStyle.Bold);
    public static Font FonteDetalhesDoPedido = new Font("DejaVu sans mono", 12, FontStyle.Bold);
    public static Font FonteNúmeroDoTelefone = new Font("DejaVu sans mono", 11, FontStyle.Bold);
    public static Font FonteNomeDoCliente = new Font("DejaVu sans mono", 15, FontStyle.Bold);
    public static Font FonteEndereçoDoCliente = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteItens = new Font("DejaVu sans mono", 15, FontStyle.Bold);
    public static Font FonteValorTotal = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteOpcionais = new Font("DejaVu sans mono", 11, FontStyle.Regular);
    public static Font FonteObservaçõesItem = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteTotaisDoPedido = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteCPF = new Font("DejaVu sans mono", 10, FontStyle.Bold);
    public static Font FonteSubTotal = new Font("DejaVu sans mono", 8, FontStyle.Regular);
    public static Font FonteMarcaDAgua = new Font("DejaVu sans mono", 7, FontStyle.Regular);

    public enum AlinhamentosGarcom
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

    public static void Imprimir(List<ClsImpressaoDefinicoeGarcom> conteudo, string impressora1, int espacamento)
    {

        string printerName = impressora1;

        PrintDocument printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;

        printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", 280, 500000);
        printDocument.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

        printDocument.PrintPage += (sender, e) => PrintPageHandler(sender, e, conteudo, espacamento);

        printDocument.Print();
    }

    public static void PrintPageHandler(object sender, PrintPageEventArgs e, List<ClsImpressaoDefinicoeGarcom> conteudo, int separacao)
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                bool DestacaObs = db.parametrosdosistema.FirstOrDefault()!.DestacarObs;


                // Define o conteúdo a ser impresso

                int Y = 0;


                foreach (var item in conteudo)
                {
                    var tamanhoFrase = e.Graphics.MeasureString(item.Texto, item.Fonte).Width;

                    if (tamanhoFrase < e.PageBounds.Width)
                    {
                        if (item.Alinhamento == AlinhamentosGarcom.Centro)
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
                            if (item.Alinhamento == AlinhamentosGarcom.Centro)
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
                            if (item.Alinhamento == AlinhamentosGarcom.Centro)
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

    public static void ImprimeComanda(string? PedidoJson, int displayId, string impressora1) //comanda
    {

        try
        {
            //fazer select no banco de dados de parâmetros do pedido aonde o num contas sejá relacionado com ele
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();
            string banco = opcSistema!.CaminhodoBanco!;

            var pedido = JsonConvert.DeserializeObject<ClassesAuxiliares.ClassesGarcomSysMenu.Pedido>(PedidoJson!);
            string? mesa = pedido!.Mesa is not null && pedido!.Mesa != "0000" ? pedido!.Mesa : pedido.Comanda;
            string? DataInicio = pedido.HorarioFeito!.ToString()!.Substring(0, 10).Replace("-", "/");
            string? HoraInicio = pedido.HorarioFeito!.ToString()!.Substring(11, 5);
            string? NomeGarcom = "PADRÃO";

            //Testar Parte do nome do garçom
            if (!String.IsNullOrEmpty(pedido.GarcomResponsavel))
            {
                bool garcomPesquisa = dbContext.garcons.Any(x => x.Codigo == pedido.GarcomResponsavel);
                if (garcomPesquisa)
                {
                    var garcom = dbContext.garcons.FirstOrDefault(x => x.Codigo == pedido.GarcomResponsavel);

                    NomeGarcom = garcom!.Nome;
                }
            }


            if (mesa!.Length > 4)
                AdicionaConteudo($"Comanda: {Convert.ToInt32(mesa)}     Garcom: {NomeGarcom} ", FonteGeral);
            else
                AdicionaConteudo($"Mesa: {Convert.ToInt32(pedido.Mesa).ToString()}     Garcom: {NomeGarcom} ", FonteGeral);

            AdicionaConteudo(AdicionarSeparadorDuplo(), FonteSeparadores);
            AdicionaConteudo($"Emitido em {DataInicio}  -  {HoraInicio}", FonteGeral);
            AdicionaConteudo(AdicionarSeparadorDuplo(), FonteSeparadores);


            foreach (var item in pedido.produtos)
            {
                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemGarcomSys(item, true);


                AdicionaConteudo($"   {item.Quantidade}\n", FonteItens);
                AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}", FonteItens);


                if (CaracteristicasPedido.Tamanho == "G" || CaracteristicasPedido.Tamanho == "M" || CaracteristicasPedido.Tamanho == "P" || CaracteristicasPedido.Tamanho == "B")
                {
                    if (CaracteristicasPedido.Tamanho == "G")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                    }

                    if (CaracteristicasPedido.Tamanho == "M")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                    }

                    if (CaracteristicasPedido.Tamanho == "P")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                    }

                    if (CaracteristicasPedido.Tamanho == "B")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                    }

                }

                if (item.incrementos != null)
                {
                    foreach (var option in item.incrementos)
                    {
                        AdicionaConteudo($"{option.Quantidade}X {option.Descricao}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.Observacao != null && item.Observacao.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.Observacao}", FonteCPF, eObs: true);
                    }

                }
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);

            }

            AdicionaConteudo("SysMenu ....... www.syslogica.com.br", FonteMarcaDAgua, AlinhamentosGarcom.Centro);

            Imprimir(Conteudo, impressora1, 19);
            Conteudo.Clear();

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static void ImprimeFechamentoDeConta(string? PedidoJson, int NumConta, string impressora1) //comanda
    {

        try
        {
            SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu.ClsSuporteDeFechamentoDeMesa? clsApoioFechamanetoDeMesa = JsonConvert.DeserializeObject<ClsSuporteDeFechamentoDeMesa>(PedidoJson);
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.FirstOrDefault();
            string banco = opcSistema!.CaminhodoBanco!;
            var Contas = dbContext.contas.Where(x => x.Conta == NumConta.ToString()).ToList();


            string? mesa = Contas.FirstOrDefault()!.Mesa;
            string? DataInicio = Contas.FirstOrDefault()!.DataInicio!.Substring(0, 10).Replace("-", "/");
            string? HoraInicio = Contas.FirstOrDefault()!.HoraInicio;


            AdicionaConteudo($"{opcSistema.NomeFantasia}", FonteItens);
            AdicionaConteudo(AdicionarSeparadorDuplo(), FonteSeparadores);


            AdicionaConteudo($"CONTA Nro: {NumConta.ToString().PadLeft(3, '0')}", FonteCPF);
            if (dbContext.configappgarcom.FirstOrDefault()!.Comanda)
            {
                var COdMesa = dbContext.mesas.FirstOrDefault(x => x.Codigo == mesa)!.Cartao;

                AdicionaConteudo($"Comanda: {Convert.ToInt32(COdMesa)}", FonteCPF);
            }
            else
                AdicionaConteudo($"Mesa: {Convert.ToInt32(mesa).ToString()}", FonteCPF);

            AdicionaConteudo($"Emitido em {DataInicio}  -  {HoraInicio}", FonteCPF);
            AdicionaConteudo($"Controle Interno \t Sem valor fiscal", FonteCPF);

            float ValorTotal = 0f;

            AdicionaConteudo(AdicionarSeparadorSimples(), FonteSeparadoresSimples);
            AdicionaConteudo($"Qtdade.  Descrição.   V.Unit.   Total.", FonteCPF);
            AdicionaConteudo(AdicionarSeparadorSimples(), FonteSeparadoresSimples);
            foreach (var item in Contas)
            {
                var NomeItem = item.Descarda;
                if (NomeItem.Length > 16)
                    NomeItem = item.Descarda!.Substring(0, 16);

                AdicionaConteudo($"     {item.Qtdade}  {NomeItem}\t  {item.ValorUnit}  {item.ValorTotal}", FonteCPF);

                ValorTotal += Convert.ToSingle(item.ValorTotal);

                AdicionaConteudo(AdicionarSeparadorSimples(), FonteSeparadoresSimples);
            }

            float TaxaDeServico = ClsDeIntegracaoSys.TaxaDeGarcom(ValorTotal, mesa);

            AdicionaConteudo($"Sub Total ..............: {ValorTotal.ToString("c")}", FonteSubTotal);
            if (TaxaDeServico > 0)
            {
                ValorTotal += TaxaDeServico;
                AdicionaConteudo($"Taxa de Serviço ........: {TaxaDeServico.ToString("c")}", FonteSubTotal);
            }

            if (clsApoioFechamanetoDeMesa!.ValorCouvert > 0)
            {
                ValorTotal += clsApoioFechamanetoDeMesa!.ValorCouvert;
                AdicionaConteudo($"Couvert ................: {clsApoioFechamanetoDeMesa!.ValorCouvert.ToString("c")}", FonteSubTotal);
            }

            AdicionaConteudo(AdicionarSeparadorSimples(), FonteSeparadoresSimples);

            AdicionaConteudo($"TOTAL DA CONTA ....: {ValorTotal.ToString("c")}", FonteValorTotal);
            AdicionaConteudo(AdicionarSeparadorSimples(), FonteSeparadores);

            AdicionaConteudo("SysMenu ....... www.syslogica.com.br", FonteMarcaDAgua, AlinhamentosGarcom.Centro);


            Imprimir(Conteudo!, impressora1, 14);
            Conteudo.Clear();

        }
        catch (Exception ex)
        {
            throw;
        }
    }


    public static string AdicionarSeparador()
    {
        return "───────────────────────────";
    }

    public static string AdicionarSeparadorDuplo()
    {
        return "===========================";
    }

    public static string AdicionarSeparadorSimples()
    {
        return "---------------------------------------";
    }

    public static void AdicionaConteudo(string conteudo, Font fonte, AlinhamentosGarcom alinhamento = AlinhamentosGarcom.Esquerda, bool eObs = false)
    {
        Conteudo!.Add(new ClsImpressaoDefinicoeGarcom() { Texto = conteudo, Fonte = fonte, Alinhamento = alinhamento, eObs = eObs });
    }

    public static void ChamaImpessoes(string? PedidoJson)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

            if (!opSistema.AgruparComandas)
            {
                foreach (string imp in impressoras)
                {
                    if (imp != "Sem Impressora" && imp != null)
                    {
                        ImprimeComanda(PedidoJson, 1, imp);
                    }
                }
            }
            else
            {
                SeparaItensParaImpressaoSeparada(PedidoJson);
            }


            impressoras.Clear();
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    public static void ChamaImpessaoDeFechamento(string? PedidoJson, int numConta)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? opSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            List<string> impressoras = new List<string>() { opSistema.Impressora1, opSistema.Impressora2, opSistema.Impressora3, opSistema.Impressora4, opSistema.Impressora5, opSistema.ImpressoraAux };

            foreach (string imp in impressoras)
            {
                if (imp != "Sem Impressora" && imp != null)
                {
                    ImprimeFechamentoDeConta(PedidoJson, numConta, imp);
                }

            }

            impressoras.Clear();
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    public static void SeparaItensParaImpressaoSeparada(string? PedidoJson)
    {
        try
        {
            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom> ListaDeItems = new List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom>() { new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom() { Impressora1 = "Cz1" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom() { Impressora1 = "Cz2" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom() { Impressora1 = "Cz3" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom() { Impressora1 = "Cz4" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom() { Impressora1 = "Sem Impressora" }, new ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom() { Impressora1 = "Bar" } };


            using ApplicationDbContext dbContext = new ApplicationDbContext();
            var pedido = JsonConvert.DeserializeObject<ClassesAuxiliares.ClassesGarcomSysMenu.Pedido>(PedidoJson!);
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();


            foreach (var item in pedido!.produtos)
            {
                bool ePizza = item.Codigo != null && (item.Codigo2 != null || item.Codigo3 != null) ? true : false;

                if (ePizza)
                {
                    var ListaDeCodigo = new List<string>() { item.Codigo!, item.Codigo2!, item.Codigo3! };

                    foreach (var cod in ListaDeCodigo)
                    {
                        if (!String.IsNullOrEmpty(cod) && cod != " ")
                        {
                            List<string> LocalDeImpressaoDasPizza = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(cod);

                            if (LocalDeImpressaoDasPizza.Count() > 1)
                            {
                                List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom> GruposDoItemPizza = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressaoDasPizza[0] || x.Impressora1 == LocalDeImpressaoDasPizza[1]).ToList();

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
                                    ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom GrupoDoItem = ListaDeItems.Where(x => x.Impressora1 == LocalDeImpressaoDasPizza[0] || x.Impressora1 == LocalDeImpressaoDasPizza[1]).FirstOrDefault();

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
                List<string> LocalDeImpressao = ClsDeIntegracaoSys.DefineNomeImpressoraPorProduto(item.Codigo!);

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

            List<ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasGarcom> ListaLimpa = ListaDeItems.Where(x => x.Itens.Count > 0).ToList();

            foreach (var item in ListaLimpa)
            {
                item.Impressora1 = DefineNomeDeImpressoraCasoEstejaSelecionadoImpSeparada(item.Impressora1);
                if (opcSistema.TipoComanda == 2)
                {
                    //ImprimeComandaSeparadaTipo2(item.Impressora1, displayId, item.Itens, numConta);
                }
                else
                {
                    ImprimeComandaSeparada(item.Impressora1, item.Itens, PedidoJson);
                }
            }

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static void ImprimeComandaSeparada(string impressora, List<Produto> itens, string? PedidoJson)
    {
        try
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            var pedido = JsonConvert.DeserializeObject<ClassesAuxiliares.ClassesGarcomSysMenu.Pedido>(PedidoJson!);
            string? mesa = pedido!.Mesa is not null && pedido!.Mesa != "0000" ? pedido!.Mesa : pedido.Comanda;
            string? DataInicio = pedido.HorarioFeito!.ToString()!.Substring(0, 10).Replace("-", "/");
            string? HoraInicio = pedido.HorarioFeito!.ToString()!.Substring(11, 5);
            string? NomeGarcom = "PADRÃO";

            //Testar Parte do nome do garçom
            if (!String.IsNullOrEmpty(pedido.GarcomResponsavel))
            {
                bool garcomPesquisa = dbContext.garcons.Any(x => x.Codigo == pedido.GarcomResponsavel);
                if (garcomPesquisa)
                {
                    var garcom = dbContext.garcons.FirstOrDefault(x => x.Codigo == pedido.GarcomResponsavel);

                    NomeGarcom = garcom!.Nome;
                }
            }

            if (mesa!.Length > 4)
                AdicionaConteudo($"Comanda: {Convert.ToInt32(mesa)}     Garcom: {NomeGarcom} ", FonteGeral);
            else
                AdicionaConteudo($"Mesa: {Convert.ToInt32(pedido.Mesa).ToString()}     Garcom: {NomeGarcom} ", FonteGeral);


            AdicionaConteudo(AdicionarSeparadorDuplo(), FonteSeparadores);
            AdicionaConteudo($"Emitido em {DataInicio}  -  {HoraInicio}", FonteGeral);
            AdicionaConteudo(AdicionarSeparadorDuplo(), FonteSeparadores);


            foreach (var item in itens)
            {

                if (impressora == "Sem Impressora" || impressora == "" || impressora == null)
                {
                    throw new Exception("Uma das impressora não foi encontrada adicione ela nas configurações ou retire a impressão separada!");
                }


                ClsDeSuporteParaImpressaoDosItens CaracteristicasPedido = ClsDeIntegracaoSys.DefineCaracteristicasDoItemGarcomSys(item, true);


                AdicionaConteudo($"   {item.Quantidade}\n", FonteItens);
                AdicionaConteudo($"{CaracteristicasPedido.NomeProduto}", FonteItens);


                if (CaracteristicasPedido.Tamanho == "G" || CaracteristicasPedido.Tamanho == "M" || CaracteristicasPedido.Tamanho == "P" || CaracteristicasPedido.Tamanho == "B")
                {
                    if (CaracteristicasPedido.Tamanho == "G")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.GRANDE.ToString(), FonteSeparadores);
                    }

                    if (CaracteristicasPedido.Tamanho == "M")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.MÉDIA.ToString(), FonteSeparadores);
                    }

                    if (CaracteristicasPedido.Tamanho == "P")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.PEQUENA.ToString(), FonteSeparadores);
                    }

                    if (CaracteristicasPedido.Tamanho == "B")
                    {
                        AdicionaConteudo(" " + TamanhoPizza.BROTINHO.ToString(), FonteSeparadores);
                    }

                }

                if (item.incrementos != null)
                {
                    foreach (var option in item.incrementos)
                    {
                        AdicionaConteudo($"{option.Quantidade}X {option.Descricao}", FonteDetalhesDoPedido, eObs: true);
                    }

                    if (item.Observacao != null && item.Observacao.Length > 0)
                    {
                        AdicionaConteudo($"Obs: {item.Observacao}", FonteCPF, eObs: true);
                    }

                }
                AdicionaConteudo(AdicionarSeparador(), FonteSeparadores);
            }


            AdicionaConteudo("SysMenu ....... www.syslogica.com.br", FonteMarcaDAgua, AlinhamentosGarcom.Centro);

            if (impressora != "Nao")
            {
                Imprimir(Conteudo!, impressora, 24);
            }

            //Imprimir(Conteudo, impressora);
            Conteudo!.Clear();

        }
        catch (Exception ex)
        {
            throw;
        }
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
