using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Mesa
{
    [Key][Column("codigo")] public string? Codigo { get; set; }
    [Column("praca")] public string? Praca { get; set; }
    [Column("tipo")] public string? Tipo { get; set; }
    [Column("status")] public string? status { get; set; }
    [Column("bloqueado")] public bool Bloqueado { get; set; }
    [Column("cartao")] public string? Cartao { get; set; }
    [Column("consumacao")] public double  Consumacao { get; set; }
    [Column("vip")] public bool Vip { get; set; }
}
