using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsMotivosDeCancelamento
{
    public string? cancelCodeId { get; set; }
    public string? description { get; set; }

    public ClsMotivosDeCancelamento() { }
}

public class ClsParaEnvioDeCancelamento
{
    public string? reason {  get; set; }
    public string? cancellationCode {  get; set; }


    public ClsParaEnvioDeCancelamento()
    {
            
    }
}
