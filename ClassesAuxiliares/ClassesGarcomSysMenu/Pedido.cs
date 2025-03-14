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
    public string? IdPedido { get; set; }
    public string? NomeClienteNaMesa { get; set; } = String.Empty;
    public string? HorarioFeito { get; set; }
    public string? GarcomResponsavel { get; set; }
    public List<Produto> produtos { get; set; } = new List<Produto>();
    public bool EBalcao { get; set; }
    public Balcao? BalcaoInfos { get; set; } = null;
}

public class Balcao
{
    public bool Repetido = false;
    public string? CodBalcao { get; set; } = String.Empty;
    public string? NomeCliente { get; set; } = String.Empty;
}