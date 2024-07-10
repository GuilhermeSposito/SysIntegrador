using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsDeSuporteParaImpressaoDosItens
{
    public string? NomeProduto { get; set; }
    public List<string>? Observações { get; set; } = new List<string>();
    public string? ExternalCode1 {  get; set; }   
    public string? ExternalCode2 {  get; set; }   
    public string? ExternalCode3 {  get; set; }   
    public string? Tamanho { get; set; }    
    public string? Obs1 {  get; set; }   
    public string? Obs2 {  get; set; }   
    public string? Obs3 {  get; set; }   
    public string? Obs4 {  get; set; }   
    public string? Obs5 {  get; set; }   
    public string? Obs6 {  get; set; }   
    public string? Obs7 {  get; set; }   
    public string? Obs8 {  get; set; }   
    public string? Obs9 {  get; set; }   
    public string? Obs10 {  get; set; }   
    public string? Obs11 {  get; set; }   
    public string? Obs12 {  get; set; }   
    public string? Obs13 {  get; set; }   
    public string? Obs14 {  get; set; }   
    public string? ObsDoItem {  get; set; }   
    public ClsDeSuporteParaImpressaoDosItens()
    {
            
    }
}

public class ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas
{
    public string? Impressora1 { get; set; }
    public string? Impressora2 { get; set; }
    public List<Items> Itens { get; set; } = new List<Items>();
    
    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas(string? impressora)
    {
        Impressora1 = impressora;
        Itens = new List<Items>();
    }
    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadas()
    {
            
    }
}
public class ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasDelMatch
{
    public string? Impressora1 { get; set; }
    public string? Impressora2 { get; set; }
    public List<items> Itens { get; set; } = new List<items>();

    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasDelMatch(string? impressora)
    {
        Impressora1 = impressora;
        Itens = new List<items>();
    }
    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasDelMatch()
    {

    }
}

public class ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido
{
    public string? Impressora1 { get; set; }
    public string? Impressora2 { get; set; }
    public List<itemsOn> Itens { get; set; } = new List<itemsOn>();

    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido(string? impressora)
    {
        Impressora1 = impressora;
        Itens = new List<itemsOn>();
    }
    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasOnPedido()
    {

    }
}

public class ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM
{
    public string? Impressora1 { get; set; }
    public string? Impressora2 { get; set; }
    public List<SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Item> Itens { get; set; } = new List<SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Item>();

    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM(string? impressora)
    {
        Impressora1 = impressora;
        Itens = new List<SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Item>();
    }
    public ClsDeSuporteParaImpressaoDosItensEmComandasSeparadasCCM()
    {

    }
}
