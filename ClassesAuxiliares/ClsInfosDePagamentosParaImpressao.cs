using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public  class ClsInfosDePagamentosParaImpressao
{
    public string? FormaPagamento {  get; set; }
    public string? TipoPagamento { get; set; }  
    public double valor {  get; set; }

    public ClsInfosDePagamentosParaImpressao()
    {
            
    }
}
