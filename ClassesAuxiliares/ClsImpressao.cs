using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SysIntegradorApp.ClassesAuxiliares.Impressao;
using static SysIntegradorApp.ClassesAuxiliares.ImpressaoAnotaAi;
using static SysIntegradorApp.ClassesAuxiliares.ImpressaoCCM;
using static SysIntegradorApp.ClassesAuxiliares.ImpressaoDelMatch;
using static SysIntegradorApp.ClassesAuxiliares.ImpressaoONPedido;

namespace SysIntegradorApp.ClassesAuxiliares;



public class ClsImpressaoDefinicoes
{
    public string Texto { get; set; }
    public Font Fonte { get; set; }
    public Alinhamentos Alinhamento { get; set; }
    public bool eObs { get; set; }


    public ClsImpressaoDefinicoes() { }
}


public class ClsImpressaoDefinicoesDelMatch
{
    public string Texto { get; set; }
    public Font Fonte { get; set; }
    public AlinhamentosDelMatch Alinhamento { get; set; }
    public bool eObs { get; set; }

    public ClsImpressaoDefinicoesDelMatch() { }
}

public class ClsImpressaoDefinicoesOnPedido
{
    public string Texto { get; set; }
    public Font Fonte { get; set; }
    public AlinhamentosOnPedido Alinhamento { get; set; }
    public bool eObs { get; set; }

    public ClsImpressaoDefinicoesOnPedido() { }
}

public class ClsImpressaoDefinicoesCMM
{
    public string Texto { get; set; }
    public Font Fonte { get; set; }
    public AlinhamentosCCM Alinhamento { get; set; }
    public bool eObs { get; set; }

    public ClsImpressaoDefinicoesCMM() { }
}

public class ClsImpressaoDefinicoesAnotaAi
{
    public string Texto { get; set; }
    public Font Fonte { get; set; }
    public AlinhamentosAnotaAi Alinhamento { get; set; }
    public bool eObs { get; set; }

    public ClsImpressaoDefinicoesAnotaAi() { }
}