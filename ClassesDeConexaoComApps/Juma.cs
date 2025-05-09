using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class Juma : TaxyMachine
{
    public Juma(MeuContexto context) : base(context)
    {
    }
}

