using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Promocoes
{
    [Key][Column("id")] public int Id { get; set; }
    [Column("dia")] public string? Dia { get; set; }
    [Column("codigo")] public string? codigo { get; set; }
    [Column("pvenda1", TypeName = "decimal(10,2)")] public double pvenda1 { get; set; }
    [Column("pvenda2", TypeName = "decimal(10,2)")] public double pvenda2 { get; set; }
    [Column("pvenda3", TypeName = "decimal(10,2)")] public double pvenda3 { get; set; }
    [Column("mesa")] public bool Mesa { get; set; }
}
