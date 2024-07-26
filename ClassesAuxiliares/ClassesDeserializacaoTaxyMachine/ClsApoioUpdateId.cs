using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;

public class ClsApoioUpdateId
{
    public int NumConta { get; set; }
    public string? MachineId { get; set; }
    public bool EPedidoAgrupado { get; set; }   

    public List<int> NumerosDeConta { get; set; }
}
