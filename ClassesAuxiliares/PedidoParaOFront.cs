using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class PedidoParaOFront
{
    public PedidoInfos PedidoInfos { get; set; } = new PedidoInfos();
    public List<Items> Items  { get; set; } = new List<Items>();
    public Delivery Delivery { get; set; } = new Delivery();    
    public Customer Customer { get; set; } = new Customer();
    public Payments Payments { get; set; } = new Payments();
    public Total Total { get; set; } = new Total();

    public PedidoParaOFront(){}
}

public class PedidoInfos
{
    public string? id { get; set; }
    public string? displayId { get; set; }
    public string? createdAt { get; set; }
    public string? orderTiming { get; set; }
    public string? orderType { get; set; }
    public string? preparationStartDateTime { get; set; }
    public bool isTest { get; set; }
    public string? salesChannel { get; set; }
    public string? StatusCode { get; set; }

    public PedidoInfos()
    {
            
    }

}
