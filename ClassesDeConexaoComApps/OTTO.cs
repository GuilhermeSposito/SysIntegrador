using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class OTTO : TaxyMachine
{
    public OTTO(MeuContexto context) : base(context)
    {
    }
}
