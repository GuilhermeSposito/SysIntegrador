using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class IncrementoCardapio
{
    [Key][Column("id")] public int Id { get; set; }
    [Column("incremento")] public string? Incremento { get; set; }
    [Column("codcarda")] public string? CodCardapio { get; set; }
}
