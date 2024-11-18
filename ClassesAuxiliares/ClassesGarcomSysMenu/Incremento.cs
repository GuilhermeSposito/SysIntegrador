using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Incremento
{
    [Key][Column("codigo")] public string? Codigo { get; set; }
    [Column("descricao")] public string? Descricao { get; set; }
    [Column("valor")] public double Valor { get; set; }
    [Column("tipo")] public string? Tipo { get; set; }
    [Column("vendainternet")] public bool VendaInternet { get; set; }
    [NotMapped] public int Quantidade { get; set; } = 0;
}
