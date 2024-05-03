using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ParametrosDoSistema
{
    [Column("id")]
    public int Id { get; set; }  
    [Column("nomefantasia")]
    public string? NomeFantasia { get; set; }
    [Column("endereco")]
    public string? Endereco { get; set; }
    [Column("impressaoaut")]
    public bool ImpressaoAut { get; set; }
    [Column("aceitapedidoaut")]
    public bool AceitaPedidoAut { get; set; }
    [Column("caminhodobanco")]
    public string? CaminhodoBanco { get; set; }
    [Column("caminhoservidor")]
    public string? CaminhoServidor { get; set; }
    [Column("integracaosysmenu")]
    public bool IntegracaoSysMenu { get; set; }
    [Column("impressora1")]
    public string? Impressora1 { get; set; }
    [Column("impressora2")]
    public string? Impressora2 { get; set; }
    [Column("impressora3")]
    public string? Impressora3 { get; set; }
    [Column("impressora4")]
    public string? Impressora4 { get; set; }
    [Column("impressora5")]
    public string? Impressora5 { get; set; }
    [Column("impressoraaux")]
    public string? ImpressoraAux { get; set; }
    [Column("telefone")]
    public string? Telefone { get; set; }
    [Column("clientid")]
    public string? ClientId { get; set; }
    [Column("clientsecret")]

    public string? ClientSecret { get; set; }
    [Column("merchantid")]

    public string? MerchantId { get; set; }
    [Column("delmatchid")]
    public string? DelMatchId { get; set; }
    [Column("agruparcomandas")]
    public bool AgruparComandas { get; set; }
    [Column("imprimircomandacaixa")]
    public bool ImprimirComandaNoCaixa { get; set; }
    public ParametrosDoSistema() {}


    public static List<string> ListaImpressoras()
    {
        var impressoras = PrinterSettings.InstalledPrinters;
        List<string> listaImpressoras = new List<string>();

        foreach (var item in impressoras)
        {
            listaImpressoras.Add(item.ToString());
        }

        return listaImpressoras;    
    }

    public static ParametrosDoSistema GetInfosSistema()
    {
        ParametrosDoSistema Configuracoes = new ParametrosDoSistema();  
        try
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            Configuracoes = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            return Configuracoes;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao listar informações do sistema");
        }
        return Configuracoes;

    }

    public static void SetInfosSistema(
     string? nomeFantasia,
     string? endereco,
     bool impressaoAut,
     bool aceitaPedidoAut,
     string? caminhoDoBanco,
     bool integracaoSysMenu,
     string? impressora1,
     string? impressora2,
     string? impressora3,
     string? impressora4,
     string? impressora5,
     string? impressoraAux,
     string? telefone,
     string? clientId,
     string? clientSecret,
     string? merchantId,
     bool agrupaComandas,
     bool imprimirComandaNoCaixa)
    {
        try
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            var configuracoes = dbContext.parametrosdosistema.ToList().FirstOrDefault();

            configuracoes.NomeFantasia = nomeFantasia;
            configuracoes.Endereco = endereco;
            configuracoes.ImpressaoAut = impressaoAut;
            configuracoes.AceitaPedidoAut = aceitaPedidoAut;
            configuracoes.CaminhodoBanco = caminhoDoBanco;
            configuracoes.IntegracaoSysMenu = integracaoSysMenu;
            configuracoes.Impressora1 = impressora1;
            configuracoes.Impressora2 = impressora2;
            configuracoes.Impressora3 = impressora3;
            configuracoes.Impressora4 = impressora4;
            configuracoes.Impressora5 = impressora5;
            configuracoes.ImpressoraAux = impressoraAux;
            configuracoes.Telefone = telefone;
            configuracoes.ClientId = clientId;
            configuracoes.ClientSecret = clientSecret;
            configuracoes.MerchantId = merchantId;
            configuracoes.AgruparComandas = agrupaComandas;
            configuracoes.ImprimirComandaNoCaixa = imprimirComandaNoCaixa;

            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }


}
