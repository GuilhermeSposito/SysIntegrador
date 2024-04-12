using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SysIntegradorApp.ClassesAuxiliares.Impressao;

namespace SysIntegradorApp.ClassesAuxiliares;



public class ClsImpressaoDefinicoes
{
    public string Texto { get; set; }
    public Font Fonte { get; set; }
    public Alinhamentos Alinhamento { get; set; }

    public ClsImpressaoDefinicoes() { }
}
