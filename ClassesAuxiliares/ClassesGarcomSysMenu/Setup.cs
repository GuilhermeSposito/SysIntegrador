using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Setup
{
    [Key][Column("id")] public int Id { get; set; }
    [Column("couvertvalor")] public float CouvertValor { get; set; }
    [Column("couvertdom")] public bool CouvertDom { get; set; }
    [Column("couvertseg")] public bool CouvertSeg { get; set; }
    [Column("couvertter")] public bool CouvertTer { get; set; }
    [Column("couvertquar")] public bool CouvertQuar { get; set; }
    [Column("couvertquin")] public bool CouvertQuin { get; set; }
    [Column("couvertsex")] public bool CouvertSex { get; set; }
    [Column("couvertsab")] public bool CouvertSab { get; set; }
    [Column("CouvertHoje")] public bool CouvertHoje { get; set; }
}
