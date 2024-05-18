using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
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

