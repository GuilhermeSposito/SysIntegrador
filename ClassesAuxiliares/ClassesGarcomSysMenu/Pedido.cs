using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Pedido
{
    [NotMapped] public string? Id { get; set; }
    public string? Mesa { get; set; }
    public string? Comanda { get; set; }
    public string? HorarioFeito { get; set; } 
    public string? GarcomResponsavel { get; set; }
    public List<Produto> produtos { get; set; } = new List<Produto>();
}
