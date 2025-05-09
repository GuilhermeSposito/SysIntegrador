﻿using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
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
    [Column("tipocomanda")]
    public int TipoComanda { get; set; }
    [Column("enviapedidoaut")]
    public bool EnviaPedidoAut { get; set; }
    [Column("integradelmatch")]
    public bool IntegraDelMatch { get; set; }
    [Column("integraifood")]
    public bool IntegraIfood { get; set; }
    [Column("userdelmatch")] public string? UserDelMatch { get; set; }
    [Column("senhadelmatch")] public string? SenhaDelMatch { get; set; }
    [Column("impcompacta")] public bool ImpCompacta { get; set; }
    [Column("removecomplmentos")] public bool RemoveComplementos { get; set; }
    [Column("integraonpedido")] public bool IntegraOnOPedido { get; set; }
    [Column("tokenonpedido")] public string? TokenOnPedido { get; set; }
    [Column("useronpedido")] public string? UserOnPedido { get; set; }
    [Column("senhaonpedido")] public string? SenhaOnPedido { get; set; }
    [Column("tempoentrega")] public int TempoEntrega { get; set; }
    [Column("tempoconclonpedido")] public int TempoConclonPedido { get; set; }
    [Column("temporetirada")] public int TempoRetirada { get; set; }
    [Column("dtultimaverif")] public string DtUltimaVerif { get; set; }
    [Column("integraccm")] public bool IntegraCCM { get; set; }
    [Column("tokenccm")] public string TokenCCM { get; set; }
    [Column("cardapiousando")] public string CardapioUsando { get; set; }
    [Column("empresadeentrega")] public string EmpresadeEntrega { get; set; }
    [Column("cidade")] public string Cidade { get; set; }
    [Column("comandareduzida")] public bool ComandaReduzida { get; set; }
    [Column("destacarobs")] public bool DestacarObs { get; set; }
    [Column("numviascomanda")] public int NumDeViasDeComanda { get; set; }
    [Column("apikeytaxymachine")] public string ApiKeyTaxyMachine { get; set; }
    [Column("usernametaxymachine")] public string UserNameTaxyMachine { get; set; }
    [Column("passwordtaxymachine")] public string PasswordTaxyMachine { get; set; }
    [Column("tokenanotaai")] public string TokenAnotaAi { get; set; }
    [Column("integraanotaai")] public bool IntegraAnotaAi { get; set; }
    [Column("integradelmatchentregas")] public bool IntegraDelmatchEntregas { get; set; }
    [Column("integraottoentregas")] public bool IntegraOttoEntregas { get; set; }
    [Column("tipodepagamentotaxymachine")] public string TipoPagamentoTaxyMachine { get; set; }
    [Column("usarnomenacomanda")] public bool UsarNomeNaComanda { get; set; }
    [Column("codfilialccm")] public string? CodFilialCCM { get; set; }
    [Column("integragarcom")] public bool IntegraGarcom { get; set; }
    [Column("tempopollinggarcom")] public int TempoPollingGarcom { get; set; }
    [Column("ifoodmultiempresa")] public bool IfoodMultiEmpresa { get; set; }
    [Column("retornoaut")] public bool RetornoAut { get; set; }
    [Column("client_secret_aiqfome")] public string ClientSecretAiqfome { get; set; }
    [Column("integra_aiqfome")] public bool IntegraAiQFome { get; set; } = false;
    [Column("integra_juma_entregas")] public bool IntegraJumaEntregas { get; set; } = false;
    [Column("integra_varias_empresas_taxymachine")] public bool IntegravariasEmpresasTaxyMachine { get; set; } = false;
    public ParametrosDoSistema() { }


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

    public static async Task<ParametrosDoSistema> GetInfosSistema()
    {
        ParametrosDoSistema? Configuracoes = new ParametrosDoSistema();
        try
        {
            await using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                Configuracoes = await dbContext.parametrosdosistema.FirstOrDefaultAsync();

                return Configuracoes;
            }
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
     bool imprimirComandaNoCaixa,
     int tipoComanda,
     bool enviaPedidoAut,
     string delMatchId,
     string UserDelMatch,
     string senhaDelMatch,
     bool integraIfood,
     bool integraDelMatch,
     bool impCompacta,
     bool removeComplementos,
     bool integraOnPedido,
     string tokenOnPedido,
     string userOnPedido,
     string senhaOnPedido,
     bool integraCCM,
     string tokenCCM,
     bool integraAnotaAi,
     string? numeroLoja
     )
    {
        try
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
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
                configuracoes.TipoComanda = tipoComanda;
                configuracoes.EnviaPedidoAut = enviaPedidoAut;
                configuracoes.DelMatchId = delMatchId;
                configuracoes.UserDelMatch = UserDelMatch;
                configuracoes.SenhaDelMatch = senhaDelMatch;
                configuracoes.IntegraDelMatch = integraDelMatch;
                configuracoes.IntegraIfood = integraIfood;
                configuracoes.ImpCompacta = impCompacta;
                configuracoes.ComandaReduzida = removeComplementos;
                configuracoes.IntegraOnOPedido = integraOnPedido;
                configuracoes.TokenOnPedido = tokenOnPedido;
                configuracoes.UserOnPedido = userOnPedido;
                configuracoes.SenhaOnPedido = senhaOnPedido;
                configuracoes.IntegraCCM = integraCCM;
                configuracoes.TokenCCM = tokenCCM;
                configuracoes.IntegraAnotaAi = integraAnotaAi;
                configuracoes.CodFilialCCM = numeroLoja;

                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static void SetInfosDeCronograma(int tempoDeEntrega = 50, int tempoDeRetirada = 20, int TempoConclPedido = 150, bool integraIfood = true, bool integraCardapio = false, bool integraEntregaAut = true)
    {
        try
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {

                var configuracoes = dbContext.parametrosdosistema.ToList().FirstOrDefault();

                configuracoes.TempoEntrega = tempoDeEntrega;
                configuracoes.TempoRetirada = tempoDeRetirada;
                configuracoes.TempoConclonPedido = TempoConclPedido;
                configuracoes.IntegraIfood = integraIfood;
                configuracoes.EnviaPedidoAut = integraEntregaAut;

                if (configuracoes.CardapioUsando == "DELMATCH")
                {
                    configuracoes.IntegraDelMatch = integraCardapio;
                }

                if (configuracoes.CardapioUsando == "CCM")
                {
                    configuracoes.IntegraCCM = integraCardapio;
                }

                if (configuracoes.CardapioUsando == "ONPEDIDO")
                {
                    configuracoes.IntegraOnOPedido = integraCardapio;
                }

                if (configuracoes.CardapioUsando == "ANOTAAI")
                {
                    configuracoes.IntegraAnotaAi = integraCardapio;
                }


                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());

            throw;
        }
    }

}
