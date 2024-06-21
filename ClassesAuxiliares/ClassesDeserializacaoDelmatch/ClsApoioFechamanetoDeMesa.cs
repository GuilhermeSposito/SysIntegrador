using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;

public class ClsApoioFechamanetoDeMesa
{
    public List<Mesa> Mesas {  get; set; }  = new List<Mesa>();

    public ClsApoioFechamanetoDeMesa()
    {
            
    }
}

public class Mesa
{
    public string? MESA { get; set; }
    public string? PedidoID { get; set; }

    public Mesa()
    {
            
    }
}
